using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class PrimaryTherapistViewModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public TherapistRepository _therapistRepository;
        private ObservableCollection<Therapist> _therapistList;

        string _therapist;

        public InterfaceSetup mInt;

        public string TherapistSelection
        {
            get { return _therapist; }

            set
            {
                _therapist = value;
                OnPropertyChanged("TherapistSelection");

                if (value != null)
                    mInt.TherapistChangeInterfaceMethod(value);
            }
        }

        public ObservableCollection<Therapist> AllTherapists
        {
            get { return _therapistList; }
            set
            {
                _therapistList = value;
                OnPropertyChanged("AllTherapists");
            }
        }

        public PrimaryTherapistViewModel()
        {
            if (_therapistRepository == null)
            {
                _therapistRepository = new TherapistRepository();
            }

            this._therapistList = _therapistRepository.GetTherapists();
            this.AllTherapists = _therapistRepository.GetTherapists();
        }

        protected override void OnDispose()
        {
            this.AllTherapists.Clear();
        }

        public void RefreshRepository(string groupName, string indivName)
        {

            var target = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\Therapists.json";

            if (File.Exists(@target))
            {
                string mInput = File.ReadAllText(@target);
                Therapists items = JsonConvert.DeserializeObject<Therapists>(mInput);

                AllTherapists.Clear();

                foreach (Therapist item in items.PrimaryTherapists)
                {
                    AllTherapists.Add(item);
                }
            }

        }

        public ICommand InvasionCommand
        {
            get
            {
                if (_invasionCommand == null)
                {
                    _invasionCommand = new RelayCommand(param => this.InvasionCommandExecute(), param => this.InvasionCommandCanExecute);
                }
                return _invasionCommand;
            }
        }

        void InvasionCommandExecute()
        {
            //bool isInvasion = true;

            foreach (Therapist emp in this.AllTherapists)
            {
                /*
                System.Console.WriteLine(emp.GroupName.Trim().ToLower());

                if (emp.LastName.Trim().ToLower() == "smith")
                {
                    isInvasion = false;
                }
                */
            }

            /*
            if (isInvasion)
            {
                System.Console.WriteLine("True out");
                BackgroundBrush = new SolidColorBrush(Colors.Green);
            }
            else
            {
                System.Console.WriteLine("False out");
                BackgroundBrush = new SolidColorBrush(Colors.White);
            }
            */

        }

        bool InvasionCommandCanExecute
        {
            get
            {
                if (this.AllTherapists.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
