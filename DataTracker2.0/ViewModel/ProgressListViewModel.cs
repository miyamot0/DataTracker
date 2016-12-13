using DataTracker.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    public class ProgressListViewModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        private ObservableCollection<ProgressMonitor> _list;

        public ObservableCollection<ProgressMonitor> AllListItems
        {
            get { return _list; }
            set
            {
                _list = value;
                OnPropertyChanged("AllListItems");
            }
        }

        public ProgressListViewModel()
        {
            this.AllListItems = new ObservableCollection<ProgressMonitor>();
        }

        public void AddLatest(ProgressMonitor mProg)
        {
            _list.Add(mProg); 
        }

        protected override void OnDispose()
        {
            this.AllListItems.Clear();
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

            foreach (ProgressMonitor emp in this.AllListItems)
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
                if (this.AllListItems.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
