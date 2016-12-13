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

using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.ViewModel
{
    class KeyboardListViewModel : ViewModelBase
    {
        public InterfaceSetup mInt;

        public KeyboardRepository _keyboardRepository;

        private KeyboardStorage _keyboard;
        public KeyboardStorage keyboardSelection
        {
            get { return _keyboard; }

            set
            {
                _keyboard = value;
                OnPropertyChanged("ConditionSelection");

                if (value != null)
                    mInt.KeyboardChangeInterfaceMethod(_keyboard);
            }
        }

        private ObservableCollection<KeyboardStorage> _keyboardList;
        public ObservableCollection<KeyboardStorage> AllKeyboards
        {
            get { return _keyboardList; }
            set
            {
                _keyboardList = value;
                OnPropertyChanged("AllKeyboards");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyboardListViewModel()
        {
            if (_keyboardRepository == null)
            {
                _keyboardRepository = new KeyboardRepository();
            }

            AllKeyboards = new ObservableCollection<KeyboardStorage>(_keyboardRepository.GetKeyboards());
            _keyboardList = new ObservableCollection<KeyboardStorage>(_keyboardRepository.GetKeyboards());

        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            AllKeyboards.Clear();
        }

        /// <summary>
        /// Interface update
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void KeyboardListModelUpdate(string groupName, string indivName)
        {
            _keyboardRepository.UpdateRepository(AllKeyboards, groupName, indivName);
        }

        /// <summary>
        /// Update contents of directory
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void RefreshRepository(string groupName, string indivName)
        {            
            var targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;
            AllKeyboards.Clear();

            string[] subdirectoryEntries = Directory.GetFiles(targetDirectory, "*.json");

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] splitter = subdirectory.Split('\\');

                if (splitter[splitter.Length-1].ToLower().Equals("therapists.json") || splitter[splitter.Length - 1].ToLower().Equals("datacollectors.json"))
                {

                }
                else
                {
                    string text = File.ReadAllText(@subdirectory);

                    KeyboardStorage deSer = JsonConvert.DeserializeObject<KeyboardStorage>(text);
                    _keyboardList.Add(deSer);
                }
            }
        }
    }
}
