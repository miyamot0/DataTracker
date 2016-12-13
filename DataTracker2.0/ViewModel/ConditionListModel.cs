using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class ConditionListModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public ConditionRepository _conditionRepository;
        private ObservableCollection<Condition> _conditionList;

        Condition _condition;

        public InterfaceSetup mInt;

        public Condition ConditionSelection
        {
            get { return _condition; }

            set
            {
                _condition = value;
                OnPropertyChanged("ConditionSelection");

                if (value != null)
                    mInt.ConditionChangeInterfaceMethod(_condition.ConditionName);

            }
        }

        public ObservableCollection<Condition> AllConditions
        {
            get { return _conditionList; }
            set
            {
                _conditionList = value;
                OnPropertyChanged("AllConditions");
            }
        }

        public ConditionListModel()
        {
            if (_conditionRepository == null)
            {
                _conditionRepository = new ConditionRepository();
            }

            this.AllConditions = new ObservableCollection<Condition>(_conditionRepository.GetConditions());
            this._conditionList = new ObservableCollection<Condition>(_conditionRepository.GetConditions());

        }

        public void ConditionListModelUpdate(string groupName, string indivName, string evalName)
        {
            _conditionRepository.UpdateRepository(AllConditions, groupName, indivName, evalName);
        }

        protected override void OnDispose()
        {
            this.AllConditions.Clear();
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

        public void RefreshRepository(string groupName, string indivName, string evalName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + groupName + "\\" + indivName + "\\" + evalName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                AllConditions.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllConditions.Add(Condition.CreateCondition(group[group.Length - 1]));
                }

            }
            catch
            {

            }
        }

        void InvasionCommandExecute()
        {
            //bool isInvasion = true;

            foreach (Condition emp in this.AllConditions)
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
                if (this.AllConditions.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
