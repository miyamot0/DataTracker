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

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            EnterSettingsCommand = new RelayCommand(param => GoToSettings(), param=>true);
            EnterSessionBuilderCommand = new RelayCommand(param => GoToSessionBuilder(), param => true);
            EnterReliabilityCommand = new RelayCommand(param => GoToReliability(), param => true);

            // Assign a save location if blank or inaccessible
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

        /// <summary>
        /// Settings command
        /// </summary>
        public void GoToSettings()
        {
            var window = new SettingsWindow();
            var mModel = new SettingsViewModel();
            window.DataContext = mModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        /// <summary>
        /// Session command
        /// </summary>
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

        /// <summary>
        /// Reliability command
        /// </summary>
        public void GoToReliability()
        {
            var kbWindow = new ReliabilityWindow();
            var mModel = new ReliabilityViewModel();

            kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            kbWindow.DataContext = mModel;
            kbWindow.ShowDialog();
        }
    }
}
