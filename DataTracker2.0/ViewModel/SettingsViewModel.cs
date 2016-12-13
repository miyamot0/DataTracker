using DataTracker.Dialog;
using System.IO;
using System.Windows;

namespace DataTracker.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        public RelayCommand SettingsSaveLocationDialog { get; set; }
        public RelayCommand SettingsCloseWindow { get; set; }

        private string mMessage = Properties.Settings.Default.SaveLocation;

        public string SaveLocation {
            get { return mMessage; }
            set
            {
                mMessage = value;
                OnPropertyChanged("SaveLocation");
            }
        }

        public SettingsViewModel()
        {
            SettingsSaveLocationDialog = new RelayCommand(param => OpenSaveLocationDialog(), param => true);
            SettingsCloseWindow = new RelayCommand(param => CloseSettingsWindow(), param => true);
        }

        public void OpenSaveLocationDialog()
        {
            var dialog = new MetroDialog();
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

        public void CloseSettingsWindow()
        {
            var mainWindow = Utilities.WindowTools.GetWindowRef("SettingsWindowTag");
            mainWindow.Close();
        }
    }
}
