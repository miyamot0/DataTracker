using DataTracker.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class KeyboardRepository
    {
        public ObservableCollection<KeyboardStorage> _keyboards;

        public KeyboardRepository()
        {
            if (_keyboards == null)
            {
                _keyboards = new ObservableCollection<KeyboardStorage>();
            }
        }

        public KeyboardRepository(string groupName, string patientName)
        {
            if (_keyboards == null)
            {
                _keyboards = new ObservableCollection<KeyboardStorage>();
            }
        }

        public void UpdateRepository(ObservableCollection<KeyboardStorage> coll1, string groupName, string indivName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            try
            {
                string[] subdirectoryEntries = Directory.GetFiles(targetDirectory, "*.json");

                foreach (string subdirectory in subdirectoryEntries)
                {
                    var deSer = JsonConvert.DeserializeObject<KeyboardStorage>(subdirectory);
                    _keyboards.Add(deSer);
                }
            }
            catch
            {

            }

        }

        public ObservableCollection<KeyboardStorage> GetKeyboards()
        {
            return new ObservableCollection<KeyboardStorage>(_keyboards);
        }
    }
}
