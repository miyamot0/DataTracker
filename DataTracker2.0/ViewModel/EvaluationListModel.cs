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
    class EvaluationListModel : ViewModelBase
    {
        public InterfaceSetup mInt;

        public EvaluationRepository _evaluationRepository;
        
        private Evaluation _evaluation;
        public Evaluation EvaluationSelection
        {
            get { return _evaluation; }

            set
            {
                _evaluation = value;
                OnPropertyChanged("EvaluationSelection");

                if (value != null)
                    mInt.EvaluationChangeInterfaceMethod(_evaluation.EvaluationName);

            }
        }

        private ObservableCollection<Evaluation> _evaluationList;
        public ObservableCollection<Evaluation> AllEvaluations
        {
            get { return _evaluationList; }
            set
            {
                _evaluationList = value;
                OnPropertyChanged("AllEvaluations");
            }
        }
        
        /// <summary>
        /// Public constructor
        /// </summary>
        public EvaluationListModel()
        {
            if (_evaluationRepository == null)
            {
                _evaluationRepository = new EvaluationRepository();
            }

            AllEvaluations = new ObservableCollection<Evaluation>(_evaluationRepository.GetEvaluations());
            _evaluationList = new ObservableCollection<Evaluation>(_evaluationRepository.GetEvaluations());
        }

        /// <summary>
        /// Interface update
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void EvaluationListModelUpdate(string groupName, string indivName)
        {
            _evaluationRepository.UpdateRepository(AllEvaluations, groupName, indivName);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            AllEvaluations.Clear();
        }

        /// <summary>
        /// Reparse directory
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void RefreshRepository(string groupName, string indivName)
        {
            var targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                AllEvaluations.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllEvaluations.Add(Evaluation.CreateEvaluation(group[group.Length - 1]));
                }
            }
            catch
            {

            }
        }
    }
}
