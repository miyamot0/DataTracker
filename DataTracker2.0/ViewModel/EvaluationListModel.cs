using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class EvaluationListModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public EvaluationRepository _evaluationRepository;
        private ObservableCollection<Evaluation> _evaluationList;

        Evaluation _evaluation;

        public InterfaceSetup mInt;

        public Evaluation EvaluationSelection
        {
            get { return _evaluation; }

            set
            {
                _evaluation = value;
                OnPropertyChanged("EvaluationSelection");

                if (value != null)
                    mInt.EvaluationChangeInterfaceMethod(_evaluation.EvaluationName);

            }
        }

        public ObservableCollection<Evaluation> AllEvaluations
        {
            get { return _evaluationList; }
            set
            {
                _evaluationList = value;
                OnPropertyChanged("AllEvaluations");
            }
        }
        
        public EvaluationListModel()
        {
            if (_evaluationRepository == null)
            {
                _evaluationRepository = new EvaluationRepository();
            }

            this.AllEvaluations = new ObservableCollection<Evaluation>(_evaluationRepository.GetEvaluations());
            this._evaluationList = new ObservableCollection<Evaluation>(_evaluationRepository.GetEvaluations());
        }

        public void EvaluationListModelUpdate(string groupName, string indivName)
        {
            _evaluationRepository.UpdateRepository(AllEvaluations, groupName, indivName);
        }

        protected override void OnDispose()
        {
            this.AllEvaluations.Clear();
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

        public void RefreshRepository(string groupName, string indivName)
        {
            var targetDirectory = DataTracker.Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName;

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

                AllEvaluations.Clear();

                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllEvaluations.Add(Evaluation.CreateEvaluation(group[group.Length - 1]));
                }

            }
            catch
            {

            }
        }

        void InvasionCommandExecute()
        {
            //bool isInvasion = true;

            foreach (Evaluation emp in this.AllEvaluations)
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
                if (this.AllEvaluations.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
