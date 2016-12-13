using DataTracker.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class IndividualRepository
    {
        public ObservableCollection<Individual> _individuals;

        public IndividualRepository()
        {
            if (_individuals == null)
            {
                _individuals = new ObservableCollection<Individual>();
            }

        }

        public IndividualRepository(string groupName)
        {
            if (_individuals == null)
            {
                _individuals = new ObservableCollection<Individual>();
            }

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName;

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] group = subdirectory.Split('\\');

                _individuals.Add(Individual.CreateIndividual(group[group.Length - 1]));
            }

        }

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

        public ObservableCollection<Individual> GetIndividuals()
        {
            return new ObservableCollection<Individual>(_individuals);
        }
    }
}