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
    class IndividualRepository
    {
        public ObservableCollection<Individual> _individuals;

        /// <summary>
        /// Default constructor
        /// </summary>
        public IndividualRepository()
        {
            if (_individuals == null)
            {
                _individuals = new ObservableCollection<Individual>();
            }

        }

        /// <summary>
        /// Build collection
        /// </summary>
        /// <param name="groupName"></param>
        public IndividualRepository(string groupName)
        {
            if (_individuals == null)
            {
                _individuals = new ObservableCollection<Individual>();
            }

            var targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + groupName;

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] group = subdirectory.Split('\\');

                _individuals.Add(Individual.CreateIndividual(group[group.Length - 1]));
            }

        }

        /// <summary>
        /// Update repository
        /// </summary>
        /// <param name="coll1"></param>
        /// <param name="groupName"></param>
        public void UpdateRepository(ObservableCollection<Individual> coll1, string groupName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                coll1.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    coll1.Add(Individual.CreateIndividual(group[group.Length - 1]));
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
        public ObservableCollection<Individual> GetIndividuals()
        {
            return new ObservableCollection<Individual>(_individuals);
        }
    }
}