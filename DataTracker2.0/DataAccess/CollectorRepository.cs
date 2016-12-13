using DataTracker.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class CollectorRepository
    {
        public Collectors _mCollObj;

        public CollectorRepository()
        {
            if (_mCollObj == null)
            {
                _mCollObj = new Collectors();
                _mCollObj.DataCollectors = new ObservableCollection<Collector>();
            }
        }

        public ObservableCollection<Collector> GetCollectors(string groupName, string indivName)
        {
            var target = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\DataCollectors.json";

            if (File.Exists(@target))
            {
                _mCollObj = JsonConvert.DeserializeObject<Collectors>(target);
            }

            return new ObservableCollection<Collector>(_mCollObj.DataCollectors);
        }

        public ObservableCollection<Collector> GetCollectors()
        {
            return new ObservableCollection<Collector>(_mCollObj.DataCollectors);
        }
    }
}
