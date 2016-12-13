using DataTracker.Dialog;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public KeyDefinitions SelectedDurationString
        {
            get { return _selectedDurationString; }
            set
            {
                _selectedDurationString = value;
                OnPropertyChanged("SelectedDurationString");
            }
        }

        public ObservableCollection<KeyDefinitions> FrequencyKeys
        {
            get { return _frequencyKeys; }
            set
            {
                _frequencyKeys = value;
                OnPropertyChanged("FrequencyKeys");
            }
        }

        public ObservableCollection<KeyDefinitions> DurationKeys
        {
            get { return _durationKeys; }
            set
            {
                _durationKeys = value;
                OnPropertyChanged("DurationKeys");
            }
        }

        public class KeyboardStorage
        {
            public List<KeyDefinitions> frequencyKeys { get; set; }
            public List<KeyDefinitions> durationKeys { get; set; }

            public string name { get; set; }
        }

        public KeyboardStorage mReturnedKeys { get; set; }

        public class KeyDefinitions
        {
            public string KeyName { get; set; }
            public string KeyDescription { get; set; }
            public Key KeyCode { get; set; }
        }

        public KeyboardScreenViewModel()
        {
            this.FrequencyKeyCommand = new RelayCommand(param => FrequencyButton(), param => true);
            this.FrequencyKeyRemove = new RelayCommand(param => FrequencyRemoveButton(), param => true);

            this.DurationKeyCommand = new RelayCommand(param => DurationButton(), param => true);
            this.DurationKeyRemove = new RelayCommand(param => DurationRemoveButton(), param => true);

            this.SaveCommand = new RelayCommand(param => SaveButton(), param => true);
            this.CloseWindow = new RelayCommand(param => SaveButton(), param => true);

        }

        public void SetupKeysEditing(bool editing)
        {
            CurrentlyEditing = editing;

            if (CurrentlyEditing)
            {
                string targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + GroupName + "\\" + PatientName + "\\" + FileName + ".json";
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

        void SaveButton()
        {
            var mainWindow = Utilities.WindowTools.GetWindowRef("KeyboardWindowTag");

            KeyboardStorage mStore = new KeyboardStorage();
            mStore.frequencyKeys = _frequencyKeys.ToList<KeyDefinitions>();
            mStore.durationKeys = _durationKeys.ToList<KeyDefinitions>();
            mStore.name = FileName;

            mReturnedKeys = mStore;

            mainWindow.DialogResult = true;
            mainWindow.Close();
        }
    }
}
