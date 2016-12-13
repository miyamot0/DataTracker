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

using DataTracker.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class KeyboardRepository
    {
        public ObservableCollection<KeyboardStorage> _keyboards;

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyboardRepository()
        {
            if (_keyboards == null)
            {
                _keyboards = new ObservableCollection<KeyboardStorage>();
            }
        }

        /// <summary>
        /// Build collection
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="patientName"></param>
        public KeyboardRepository(string groupName, string patientName)
        {
            if (_keyboards == null)
            {
                _keyboards = new ObservableCollection<KeyboardStorage>();
            }
        }

        /// <summary>
        /// Update repository
        /// </summary>
        /// <param name="coll1"></param>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void UpdateRepository(ObservableCollection<KeyboardStorage> coll1, string groupName, string indivName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            try
            {
                string[] subdirectoryEntries = Directory.GetFiles(targetDirectory, "*.json");

                foreach (string subdirectory in subdirectoryEntries)
                {
                    var deSer = JsonConvert.DeserializeObject<KeyboardStorage>(subdirectory);
                    _keyboards.Add(deSer);
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// Access Collection
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<KeyboardStorage> GetKeyboards()
        {
            return new ObservableCollection<KeyboardStorage>(_keyboards);
        }
    }
}
