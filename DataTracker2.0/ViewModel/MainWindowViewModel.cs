using System;
using DataTracker.View;
using System.Windows;
using System.Threading.Tasks;
using System.IO;

namespace DataTracker.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand EnterSettingsCommand { get; set; }
        public RelayCommand EnterSessionBuilderCommand { get; set; }
        public RelayCommand EnterReliabilityCommand { get; set; }
        public RelayCommand TestCommand { get; set; }

        public MainWindowViewModel()
        {
            EnterSettingsCommand = new RelayCommand(param => GoToSettings(), param=>true);
            EnterSessionBuilderCommand = new RelayCommand(param => GoToSessionBuilder(), param => true);
            EnterReliabilityCommand = new RelayCommand(param => GoToReliability(), param => true);
            TestCommand = new RelayCommand(param => GoToTest(), param => true);

            if (Properties.Settings.Default.SaveLocation == "")
            {
                string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DataTracker\\";
                Properties.Settings.Default.SaveLocation = defaultPath;
                Properties.Settings.Default.Save();

                DirectoryInfo di1 = Directory.CreateDirectory(defaultPath);
                DirectoryInfo di2 = Directory.CreateDirectory(defaultPath + "\\Patients\\");
            }
            else
            {
                Func<bool> func = () => Directory.Exists(Properties.Settings.Default.SaveLocation);
                Task<bool> task = new Task<bool>(func);

                task.Start();

                if (!task.Wait(1000))
                {
                    MessageBox.Show("Hmm, save location is available right now.  If on a network drive, consider a local location. Otherwise, double check your save location again.");
                }
            }
        }

        public void GoToSettings()
        {
            var window = new SettingsWindow();
            var mModel = new SettingsViewModel();
            window.DataContext = mModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        public void GoToSessionBuilder()
        {
            var window = new SetupWindow();
            var mModel = new SetupViewModel();
            window.DataContext = mModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            try
            {
                window.ShowDialog();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void GoToReliability()
        {
            var kbWindow = new ReliabilityWindow();
            var mModel = new ReliabilityViewModel();

            kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            kbWindow.DataContext = mModel;
            kbWindow.ShowDialog();
        }

        public void GoToTest()
        {


        }
    }
}
