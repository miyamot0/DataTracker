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
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace DataTracker.ViewModel
{
    class GroupListViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public InterfaceSetup mInt;

        GroupRepository _groupRepository;

        private Group _group;
        public Group GroupSelection
        {
            get { return _group; }

            set
            {
                _group = value;
                OnPropertyChanged("GroupSelection");
                if (value != null)
                    mInt.GroupChangeInterfaceMethod(_group.GroupName);
            }
        }

        private ObservableCollection<Group> _groupList;
        public ObservableCollection<Group> AllGroups
        {
            get { return _groupList; }
            set
            {
                OnPropertyChanged("AllGroups");
                _groupList = value;
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="groupRepository"></param>
        public GroupListViewModel(GroupRepository groupRepository)
        {
            if (groupRepository == null)
            {
                throw new ArgumentNullException("groupRepository");
            }

            _groupRepository = groupRepository;
            AllGroups = new ObservableCollection<Group>(_groupRepository.GetGroups());
            _groupList = new ObservableCollection<Group>(_groupRepository.GetGroups());
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            AllGroups.Clear();
        }

        /// <summary>
        /// Reparse directory
        /// </summary>
        public void RefreshRepository()
        {
            AllGroups.Clear();

            var targetDirectory = Properties.Settings.Default.SaveLocation;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllGroups.Add(Group.CreateGroup(group[group.Length - 1]));
                }
            }
            catch
            {

            }
        }
    }
}
