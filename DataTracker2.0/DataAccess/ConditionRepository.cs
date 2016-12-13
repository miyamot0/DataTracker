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
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class ConditionRepository
    {
        public ObservableCollection<Condition> _conditions;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConditionRepository()
        {
            if (_conditions == null)
            {
                _conditions = new ObservableCollection<Condition>();
            }

            _conditions.Add(Condition.CreateCondition("John"));
        }

        /// <summary>
        /// Build collection
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        /// <param name="evalName"></param>
        public ConditionRepository(string groupName, string indivName, string evalName)
        {
            if (_conditions == null)
            {
                _conditions = new ObservableCollection<Condition>();
            }

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] group = subdirectory.Split('\\');

                _conditions.Add(Condition.CreateCondition(group[group.Length - 1]));
            }
        }

        /// <summary>
        /// Update repository
        /// </summary>
        /// <param name="coll1"></param>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        /// <param name="evalName"></param>
        public void UpdateRepository(ObservableCollection<Condition> coll1, string groupName, string indivName, string evalName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                coll1.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    coll1.Add(Condition.CreateCondition(group[group.Length - 1]));
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
        public ObservableCollection<Condition> GetConditions()
        {
            return new ObservableCollection<Condition>(_conditions);
        }
    }
}
