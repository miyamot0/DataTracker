using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class KeyboardListViewModel : ViewModelBase
    {
        RelayCommand _invasionCommand;

        public KeyboardRepository _keyboardRepository;
        private ObservableCollection<KeyboardStorage> _keyboardList;

        KeyboardStorage _keyboard;

        public InterfaceSetup mInt;

        public KeyboardStorage keyboardSelection
        {
            get { return _keyboard; }

            set
            {
                _keyboard = value;
                OnPropertyChanged("ConditionSelection");

                if (value != null)
                    mInt.KeyboardChangeInterfaceMethod(_keyboard);

            }
        }

        public ObservableCollection<KeyboardStorage> AllKeyboards
        {
            get { return _keyboardList; }
            set
            {
                _keyboardList = value;
                OnPropertyChanged("AllKeyboards");
            }
        }

        public KeyboardListViewModel()
        {
            if (_keyboardRepository == null)
            {
                _keyboardRepository = new KeyboardRepository();
            }

            this.AllKeyboards = new ObservableCollection<KeyboardStorage>(_keyboardRepository.GetKeyboards());
            this._keyboardList = new ObservableCollection<KeyboardStorage>(_keyboardRepository.GetKeyboards());

        }

        protected override void OnDispose()
        {
            this.AllKeyboards.Clear();
        }

        public void KeyboardListModelUpdate(string groupName, string indivName)
        {
            _keyboardRepository.UpdateRepository(AllKeyboards, groupName, indivName);
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
            AllKeyboards.Clear();

            string[] subdirectoryEntries = Directory.GetFiles(targetDirectory, "*.json");

            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] splitter = subdirectory.Split('\\');

                if (splitter[splitter.Length-1].ToLower().Equals("therapists.json") || splitter[splitter.Length - 1].ToLower().Equals("datacollectors.json"))
                {

                }
                else
                {
                    string text = File.ReadAllText(@subdirectory);

                    KeyboardStorage deSer = JsonConvert.DeserializeObject<KeyboardStorage>(text);
                    _keyboardList.Add(deSer);
                }
            }
        }

        void InvasionCommandExecute()
        {
            //bool isInvasion = true;

            foreach (KeyboardStorage emp in this.AllKeyboards)
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
                if (this.AllKeyboards.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
