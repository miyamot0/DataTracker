using DataTracker.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class ConditionRepository
    {
        public ObservableCollection<Condition> _conditions;

        public ConditionRepository()
        {
            if (_conditions == null)
            {
                _conditions = new ObservableCollection<Condition>();
            }

            _conditions.Add(Condition.CreateCondition("John"));
        }

        public ConditionRepository(string groupName, string indivName, string evalName)
        {
            if (_conditions == null)
            {
                _conditions = new ObservableCollection<Condition>();
            }

            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] group = subdirectory.Split('\\');

                _conditions.Add(Condition.CreateCondition(group[group.Length - 1]));
            }

        }

        public void UpdateRepository(ObservableCollection<Condition> coll1, string groupName, string indivName, string evalName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                coll1.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    coll1.Add(Condition.CreateCondition(group[group.Length - 1]));
                }

            }
            catch
            {

            }

        }

        public ObservableCollection<Condition> GetConditions()
        {
            return new ObservableCollection<Condition>(_conditions);
        }
    }
}
