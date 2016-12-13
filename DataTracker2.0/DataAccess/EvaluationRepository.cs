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
    class EvaluationRepository
    {
        public ObservableCollection<Evaluation> _evaluations;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EvaluationRepository()
        {
            if (_evaluations == null)
            {
                _evaluations = new ObservableCollection<Evaluation>();
            }
        }

        /// <summary>
        /// Build collection
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public EvaluationRepository(string groupName, string indivName)
        {
            if (_evaluations == null)
            {
                _evaluations = new ObservableCollection<Evaluation>();
            }

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] group = subdirectory.Split('\\');

                _evaluations.Add(Evaluation.CreateEvaluation(group[group.Length - 1]));
            }

        }

        /// <summary>
        /// Update repository
        /// </summary>
        /// <param name="coll1"></param>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void UpdateRepository(ObservableCollection<Evaluation> coll1, string groupName, string indivName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                coll1.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    coll1.Add(Evaluation.CreateEvaluation(group[group.Length - 1]));
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
        public ObservableCollection<Evaluation> GetEvaluations()
        {
            return new ObservableCollection<Evaluation>(_evaluations);
        }
    }
}
