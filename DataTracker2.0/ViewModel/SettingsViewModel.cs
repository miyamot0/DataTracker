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

using System.IO;
using System.Windows;

namespace DataTracker.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        public RelayCommand SettingsSaveLocationDialog { get; set; }
        public RelayCommand SettingsCloseWindow { get; set; }

        private bool mRestore = Properties.Settings.Default.RestoreSelection;
        public bool RestoreSettings
        {
            get { return mRestore; }
            set
            {
                mRestore = value;
                OnPropertyChanged("RestoreSettings");
                Properties.Settings.Default.RestoreSelection = mRestore;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Save location
        /// </summary>
        private string mMessage = Properties.Settings.Default.SaveLocation;
        public string SaveLocation {
            get { return mMessage; }
            set
            {
                mMessage = value;
                OnPropertyChanged("SaveLocation");
            }
        }

        /// <summary>
        /// Commands
        /// </summary>
        public SettingsViewModel()
        {
            SettingsSaveLocationDialog = new RelayCommand(param => OpenSaveLocationDialog(), param => true);
            SettingsCloseWindow = new RelayCommand(param => CloseSettingsWindow(), param => true);
        }

        /// <summary>
        /// Open save dialog
        /// </summary>
        public void OpenSaveLocationDialog()
        {
            var dialog = new Dialog.Dialog();
            dialog.Title = "Set Default Save Location";
            dialog.QuestionText = "Please set the default location for save files.";
            dialog.ResponseText = Properties.Settings.Default.SaveLocation;
            dialog.ShowDialog();

            if (Directory.Exists(dialog.ResponseText))
            {
                Properties.Settings.Default.SaveLocation = dialog.ResponseText;
                Properties.Settings.Default.Save();
                SaveLocation = Properties.Settings.Default.SaveLocation;
            }
            else
            {
                MessageBox.Show("This location doesn't seem to exist or isn't available.  Are you sure this is the correct location?");
            }
        }

        /// <summary>
        /// Close window command
        /// </summary>
        public void CloseSettingsWindow()
        {
            var mainWindow = Utilities.WindowTools.GetWindowRef("SettingsWindowTag");
            mainWindow.Close();
        }
    }
}
