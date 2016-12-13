using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class GroupListViewModel : ViewModelBase, INotifyPropertyChanged
    {
        RelayCommand _invasionCommand;

        GroupRepository _groupRepository;
        ObservableCollection<Group> _groupList;

        Group _group;

        public InterfaceSetup mInt;

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

        public ObservableCollection<Group> AllGroups
        {
            get { return _groupList; }
            set
            {
                OnPropertyChanged("AllGroups");
                _groupList = value;
            }
        }

        public GroupListViewModel(GroupRepository groupRepository)
        {
            if (groupRepository == null)
            {
                throw new ArgumentNullException("groupRepository");
            }

            _groupRepository = groupRepository;
            this.AllGroups = new ObservableCollection<Group>(_groupRepository.GetGroups());
            this._groupList = new ObservableCollection<Group>(_groupRepository.GetGroups());
        }

        protected override void OnDispose()
        {
            this.AllGroups.Clear();
        }

        public void RefreshRepository()
        {
            AllGroups.Clear();

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation;

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

        public ICommand InvasionCommand
        {
            get
            {
                if (_invasionCommand == null)
                {
                    _invasionCommand = new RelayCommand(param => this.InvasionCommandExecute(), param => this.InvasionCommandCanExecute);
                }
                return _invasionCommand;
            }
        }

        void InvasionCommandExecute()
        {
            //bool isInvasion = true;

            foreach (Model.Group emp in this.AllGroups)
            {
                /*
                System.Console.WriteLine(emp.GroupName.Trim().ToLower());

                if (emp.LastName.Trim().ToLower() == "smith")
                {
                    isInvasion = false;
                }
                */
            }

            /*
            if (isInvasion)
            {
                System.Console.WriteLine("True out");
                BackgroundBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                System.Console.WriteLine("False out");
                BackgroundBrush = new SolidColorBrush(Colors.White);
            }
            */

        }

        bool InvasionCommandCanExecute
        {
            get
            {
                if (this.AllGroups.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
