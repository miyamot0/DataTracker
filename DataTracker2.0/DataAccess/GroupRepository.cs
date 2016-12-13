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
    class GroupRepository
    {
        public ObservableCollection<Group> _groups;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupRepository()
        {
            if (_groups == null)
            {
                _groups = new ObservableCollection<Group>();
            }

            var targetDirectory = Properties.Settings.Default.SaveLocation;

            try
            {
                var textFiles = Directory.EnumerateDirectories(targetDirectory);
                foreach (string currentDir in textFiles)
                {
                    string[] group = currentDir.Split('\\');
                    _groups.Add(Group.CreateGroup(group[group.Length - 1]));
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
        public ObservableCollection<Group> GetGroups()
        {
            return new ObservableCollection<Group>(_groups);
        }
    }
}
