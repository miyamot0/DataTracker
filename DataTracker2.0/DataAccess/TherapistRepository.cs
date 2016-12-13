using DataTracker.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.DataAccess
{
    class TherapistRepository
    {
        public Therapists _mCollObj;

        public TherapistRepository()
        {
            if (_mCollObj == null)
            {
                _mCollObj = new Therapists();
                _mCollObj.PrimaryTherapists = new ObservableCollection<Therapist>();
            }
        }

        public ObservableCollection<Therapist> GetCollectors(string groupName, string indivName)
        {
            var target = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\PrimaryTherapists.json";

            if (File.Exists(@target))
            {
                _mCollObj = JsonConvert.DeserializeObject<Therapists>(target);
            }

            return new ObservableCollection<Therapist>(_mCollObj.PrimaryTherapists);
        }

        public ObservableCollection<Therapist> GetTherapists()
        {
            return new ObservableCollection<Therapist>(_mCollObj.PrimaryTherapists);
        }
    }
}
