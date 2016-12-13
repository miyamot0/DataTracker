/*
    Copyright 2016 Shawn Gilroy

    This file is part of DataTracker.

    Discounting Model Selector is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.

    DataTracker is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with DataTracker.  If not, see <http://www.gnu.org/licenses/gpl-3.0.html>.
*/

using DataTracker.Dialog;
using DataTracker.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DataTracker.ViewModel
{
    class KeyboardScreenViewModel : ViewModelBase
    {
        public RelayCommand FrequencyKeyCommand { get; set; }
        public RelayCommand FrequencyKeyRemove { get; set; }

        public RelayCommand DurationKeyCommand { get; set; }
        public RelayCommand DurationKeyRemove { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CloseWindow { get; set; }

        public string PatientName { get; set; }
        public string GroupName { get; set; }
        public string FileName { get; set; }

        public bool CurrentlyEditing { get; set; }

        ObservableCollection<KeyDefinitions> _frequencyKeys = new ObservableCollection<KeyDefinitions>();
        ObservableCollection<KeyDefinitions> _durationKeys = new ObservableCollection<KeyDefinitions>();

        KeyDefinitions _selectedFrequencyString;
        /// <summary>
        /// Selected string, frequency
        /// </summary>
        public KeyDefinitions SelectedFrequencyString
        {
            get { return _selectedFrequencyString; }
            set
            {
                _selectedFrequencyString = value;
                OnPropertyChanged("SelectedFrequencyString");
            }
        }

        KeyDefinitions _selectedDurationString;
        /// <summary>
        /// Selected string, duration
        /// </summary>
        public KeyDefinitions SelectedDurationString
        {
            get { return _selectedDurationString; }
            set
            {
                _selectedDurationString = value;
                OnPropertyChanged("SelectedDurationString");
            }
        }

        /// <summary>
        /// Frequency keys
        /// </summary>
        public ObservableCollection<KeyDefinitions> FrequencyKeys
        {
            get { return _frequencyKeys; }
            set
            {
                _frequencyKeys = value;
                OnPropertyChanged("FrequencyKeys");
            }
        }

        /// <summary>
        /// Duration keys
        /// </summary>
        public ObservableCollection<KeyDefinitions> DurationKeys
        {
            get { return _durationKeys; }
            set
            {
                _durationKeys = value;
                OnPropertyChanged("DurationKeys");
            }
        }

        /// <summary>
        /// Container object
        /// </summary>
        public KeyboardStorage mReturnedKeys { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyboardScreenViewModel()
        {
            this.FrequencyKeyCommand = new RelayCommand(param => FrequencyButton(), param => true);
            this.FrequencyKeyRemove = new RelayCommand(param => FrequencyRemoveButton(), param => true);

            this.DurationKeyCommand = new RelayCommand(param => DurationButton(), param => true);
            this.DurationKeyRemove = new RelayCommand(param => DurationRemoveButton(), param => true);

            this.SaveCommand = new RelayCommand(param => SaveButton(), param => true);
            this.CloseWindow = new RelayCommand(param => SaveButton(), param => true);
        }

        /// <summary>
        /// Check if keys being edited
        /// </summary>
        /// <param name="editing"></param>
        public void SetupKeysEditing(bool editing)
        {
            CurrentlyEditing = editing;

            if (CurrentlyEditing)
            {
                string targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + GroupName + "\\" + PatientName + "\\" + FileName + ".json";
                string mObjString = File.ReadAllText(targetDirectory);

                KeyboardStorage deSer = JsonConvert.DeserializeObject<KeyboardStorage>(mObjString);
                foreach (KeyDefinitions key in deSer.frequencyKeys)
                {
                    _frequencyKeys.Add(key);
                }

                foreach (KeyDefinitions key in deSer.durationKeys)
                {
                    _durationKeys.Add(key);
                }

            }
        }

        /// <summary>
        /// Add frequency button
        /// </summary>
        void FrequencyButton()
        {
            var dialog = new AddKeyDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (dialog.ShowDialog() == true)
            {
                if (dialog.ResponseText.Length == 0 || ((Button)dialog.buttonKey).Content.ToString().Contains("PRESS") || ((Button)dialog.buttonKey).Content.ToString().Contains("Press") || ((Button)dialog.buttonKey).Content.ToString().Length == 0)
                {
                    MessageBox.Show("The data you submitted was missing or incomplete!");
                }
                else
                {
                    FrequencyKeys.Add(new KeyDefinitions
                    {
                        KeyName = ((Button)dialog.buttonKey).Content.ToString(),
                        KeyDescription = dialog.ResponseText,
                        KeyCode = dialog.KeyValue
                    });
                }
            }
        }

        /// <summary>
        /// Remove frequency button
        /// </summary>
        void FrequencyRemoveButton()
        {
            if (SelectedFrequencyString == null || SelectedFrequencyString.KeyDescription.Length < 1)
                return;

            var mCopy = new ObservableCollection<KeyDefinitions>(FrequencyKeys);
            foreach (var item in mCopy)
            {
                if (item.KeyName == SelectedFrequencyString.KeyName)
                {
                    FrequencyKeys.Remove(item);
                }
            }
        }
        
        /// <summary>
        /// Add duration button
        /// </summary>
        void DurationButton()
        {
            var dialog = new AddKeyDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (dialog.ShowDialog() == true)
            {
                if (dialog.ResponseText.Length == 0 || ((Button)dialog.buttonKey).Content.ToString().Contains("PRESS") || ((Button)dialog.buttonKey).Content.ToString().Contains("Press") || ((Button)dialog.buttonKey).Content.ToString().Length == 0)
                {
                    MessageBox.Show("The data you submitted was missing or incomplete!");
                }
                else
                {
                    DurationKeys.Add(new KeyDefinitions
                    {
                        KeyName = ((Button)dialog.buttonKey).Content.ToString(),
                        KeyDescription = dialog.ResponseText,
                        KeyCode = dialog.KeyValue
                    });
                }
            }
        }

        /// <summary>
        /// Remove duration button
        /// </summary>
        void DurationRemoveButton()
        {
            if (SelectedDurationString == null || SelectedDurationString.KeyDescription.Length < 1)
                return;

            var mCopy = new ObservableCollection<KeyDefinitions>(DurationKeys);
            foreach (var item in mCopy)
            {
                if (item.KeyName == SelectedDurationString.KeyName)
                {
                    DurationKeys.Remove(item);
                }
            }
        }

        /// <summary>
        /// Save file button
        /// </summary>
        void SaveButton()
        {
            var mainWindow = Utilities.WindowTools.GetWindowRef("KeyboardWindowTag");

            KeyboardStorage mStore = new KeyboardStorage();
            mStore.frequencyKeys = new ObservableCollection<KeyDefinitions>(_frequencyKeys);
            mStore.durationKeys = new ObservableCollection<KeyDefinitions>(_durationKeys);
            mStore.name = FileName;

            mReturnedKeys = mStore;

            mainWindow.DialogResult = true;
            mainWindow.Close();
        }
    }
}
