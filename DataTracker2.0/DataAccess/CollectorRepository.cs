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
    class CollectorRepository
    {
        public Collectors _mCollObj;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CollectorRepository()
        {
            if (_mCollObj == null)
            {
                _mCollObj = new Collectors();
                _mCollObj.DataCollectors = new ObservableCollection<Collector>();
            }
        }

        /// <summary>
        /// Build collection
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        /// <returns></returns>
        public ObservableCollection<Collector> GetCollectors(string groupName, string indivName)
        {
            var target = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\DataCollectors.json";

            if (File.Exists(@target))
            {
                _mCollObj = JsonConvert.DeserializeObject<Collectors>(target);
            }

            return new ObservableCollection<Collector>(_mCollObj.DataCollectors);
        }

        /// <summary>
        /// Access Collection
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Collector> GetCollectors()
        {
            return new ObservableCollection<Collector>(_mCollObj.DataCollectors);
        }
    }
}
