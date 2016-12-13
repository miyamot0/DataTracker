using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class IndividualListModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public IndividualRepository _individualRepository;
        private ObservableCollection<Individual> _individualList;

        Individual _individual;

        public InterfaceSetup mInt;

        public Individual IndividualSelection
        {
            get { return _individual; }

            set
            {
                _individual = value;
                OnPropertyChanged("IndividualSelection");

                if (value != null)
                    mInt.IndividualChangeInterfaceMethod(_individual.IndividualName);

            }
        }

        public ObservableCollection<Individual> AllIndividuals
        {
            get { return _individualList; }
            set
            {
                _individualList = value;
                OnPropertyChanged("AllIndividuals");
            }
        }

        public IndividualListModel()
        {
            if (_individualRepository == null)
            {
                _individualRepository = new IndividualRepository();
            }

            this.AllIndividuals = new ObservableCollection<Individual>(_individualRepository.GetIndividuals());
            this._individualList = new ObservableCollection<Individual>(_individualRepository.GetIndividuals());
        }

        public void IndividualListModelUpdate(string groupName)
        {
            _individualRepository.UpdateRepository(AllIndividuals, groupName);
        }

        protected override void OnDispose()
        {
            this.AllIndividuals.Clear();
        }

        public void RefreshRepository(string groupName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                AllIndividuals.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllIndividuals.Add(Individual.CreateIndividual(group[group.Length - 1]));
                }
            }
            catch
            {

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

            foreach (Individual emp in this.AllIndividuals)
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
                if (this.AllIndividuals.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
