using DataTracker.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class GroupRepository
    {
        public ObservableCollection<Group> _groups;

        public GroupRepository()
        {
            if (_groups == null)
            {
                _groups = new ObservableCollection<Group>();
            }

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation;

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

        public ObservableCollection<Group> GetGroups()
        {
            return new ObservableCollection<Group>(_groups);
        }
    }
}
