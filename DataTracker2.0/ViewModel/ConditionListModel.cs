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
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.ViewModel
{
    class ConditionListModel : ViewModelBase
    {
        public InterfaceSetup mInt;

        public ConditionRepository _conditionRepository;

        private Condition _condition;
        public Condition ConditionSelection
        {
            get { return _condition; }

            set
            {
                _condition = value;
                OnPropertyChanged("ConditionSelection");

                if (value != null)
                    mInt.ConditionChangeInterfaceMethod(_condition.ConditionName);

            }
        }

        private ObservableCollection<Condition> _conditionList;
        public ObservableCollection<Condition> AllConditions
        {
            get { return _conditionList; }
            set
            {
                _conditionList = value;
                OnPropertyChanged("AllConditions");
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConditionListModel()
        {
            if (_conditionRepository == null)
            {
                _conditionRepository = new ConditionRepository();
            }

            AllConditions = new ObservableCollection<Condition>(_conditionRepository.GetConditions());
            _conditionList = new ObservableCollection<Condition>(_conditionRepository.GetConditions());
        }

        /// <summary>
        /// Interface
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        /// <param name="evalName"></param>
        public void ConditionListModelUpdate(string groupName, string indivName, string evalName)
        {
            _conditionRepository.UpdateRepository(AllConditions, groupName, indivName, evalName);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            this.AllConditions.Clear();
        }
        
        /// <summary>
        /// Reparse directory
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        /// <param name="evalName"></param>
        public void RefreshRepository(string groupName, string indivName, string evalName)
        {
            var targetDirectory = Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                AllConditions.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllConditions.Add(Condition.CreateCondition(group[group.Length - 1]));
                }
            }
            catch
            {

            }
        }
    }
}
