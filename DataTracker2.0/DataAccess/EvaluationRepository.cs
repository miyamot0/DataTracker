using DataTracker.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class EvaluationRepository
    {
        public ObservableCollection<Evaluation> _evaluations;

        public EvaluationRepository()
        {
            if (_evaluations == null)
            {
                _evaluations = new ObservableCollection<Evaluation>();
            }
        }

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

        public ObservableCollection<Evaluation> GetEvaluations()
        {
            return new ObservableCollection<Evaluation>(_evaluations);
        }

    }
}
