using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class CollectorListViewModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public CollectorRepository _collectorRepository;
        private ObservableCollection<Collector> _collectorList;

        string _collector;

        public InterfaceSetup mInt;

        public string CollectorSelection
        {
            get { return _collector; }

            set
            {
                _collector = value;
                OnPropertyChanged("CollectorSelection");

                if (value != null)
                    mInt.CollectorChangeInterfaceMethod(value);
            }
        }

        public ObservableCollection<Collector> AllCollectors
        {
            get { return _collectorList; }
            set
            {
                _collectorList = value;
                OnPropertyChanged("AllCollectors");
            }
        }

        public CollectorListViewModel()
        {
            if (_collectorRepository == null)
            {
                _collectorRepository = new CollectorRepository();
            }

            this._collectorList = _collectorRepository.GetCollectors();
            this.AllCollectors = _collectorRepository.GetCollectors();
        }

        protected override void OnDispose()
        {
            this.AllCollectors.Clear();
        }

        public void RefreshRepository(string groupName, string indivName)
        {
            
            var target = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\DataCollectors.json";

            if (File.Exists(@target))
            {
                string mInput = File.ReadAllText(@target);
                var items = JsonConvert.DeserializeObject<Collectors>(mInput);

                AllCollectors.Clear();

                foreach (Collector item in items.DataCollectors)
                {
                    AllCollectors.Add(item);
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

            foreach (Collector emp in this.AllCollectors)
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
                if (this.AllCollectors.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
