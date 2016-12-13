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
    class IndividualListModel : ViewModelBase
    {
        public InterfaceSetup mInt;

        public IndividualRepository _individualRepository;

        private Individual _individual;
        public Individual IndividualSelection
        {
            get { return _individual; }

            set
            {
                _individual = value;
                OnPropertyChanged("IndividualSelection");

                if (value != null)
                    mInt.IndividualChangeInterfaceMethod(_individual.IndividualName);

            }
        }

        private ObservableCollection<Individual> _individualList;
        public ObservableCollection<Individual> AllIndividuals
        {
            get { return _individualList; }
            set
            {
                _individualList = value;
                OnPropertyChanged("AllIndividuals");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public IndividualListModel()
        {
            if (_individualRepository == null)
            {
                _individualRepository = new IndividualRepository();
            }

            AllIndividuals = new ObservableCollection<Individual>(_individualRepository.GetIndividuals());
            _individualList = new ObservableCollection<Individual>(_individualRepository.GetIndividuals());
        }

        /// <summary>
        /// Interface update
        /// </summary>
        /// <param name="groupName"></param>
        public void IndividualListModelUpdate(string groupName)
        {
            _individualRepository.UpdateRepository(AllIndividuals, groupName);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            AllIndividuals.Clear();
        }

        /// <summary>
        /// Reparse directory
        /// </summary>
        /// <param name="groupName"></param>
        public void RefreshRepository(string groupName)
        {
            var targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + groupName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                AllIndividuals.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllIndividuals.Add(Individual.CreateIndividual(group[group.Length - 1]));
                }
            }
            catch
            {

            }
        }
    }
}
