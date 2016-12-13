/*
    Copyright 2016 Shawn Gilroy

    This file is part of DataTracker.

    Discounting Model Selector is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.

    DataTracker is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with DataTracker.  If not, see <http://www.gnu.org/licenses/gpl-3.0.html>.
*/

using DataTracker.DataAccess;
using DataTracker.Dialog;
using DataTracker.Model;
using DataTracker.Utilities;
using DataTracker.View;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace DataTracker.ViewModel
{
    /// <summary>
    /// View model
    /// </summary>
    class SetupViewModel : ViewModelBase, InterfaceSetup
    {
        private string _group, 
            _indiv, 
            _eval, 
            _cond, 
            _keys, 
            _collect, 
            _ther;

        public RelayCommand CloseSetupWindow { get; set; }
        public RelayCommand SetupWindowFired { get; set; }
        public RelayCommand AddGroupWindow { get; set; }
        public RelayCommand AddIndividualWindow { get; set; }
        public RelayCommand AddEvaluationWindow { get; set; }
        public RelayCommand AddConditionWindow { get; set; }
        public RelayCommand AddKeyboardWindow { get; set; }
        public RelayCommand AddCollectorWindow { get; set; }
        public RelayCommand AddTherapistWindow { get; set; }

        public GroupListViewModel groupListViewModel { get; set; }
        public IndividualListModel individualListViewModel { get; set; }
        public EvaluationListModel evaluationListViewModel { get; set; }
        public ConditionListModel conditionListViewModel { get; set; }
        public KeyboardListViewModel keyboardListViewModel { get; set; }
        public CollectorListViewModel collectorListViewModel { get; set; }
        public PrimaryTherapistViewModel therapistListViewModel { get; set; }

        string _sessionNumber;
        public string SessionNumber {
            get { return _sessionNumber; }
            set
            {
                _sessionNumber = value;
                OnPropertyChanged("SessionNumber");
            }
        }

        ObservableCollection<KeyDefinitions> _frequencyKeys = new ObservableCollection<KeyDefinitions>();
        ObservableCollection<KeyDefinitions> _durationKeys = new ObservableCollection<KeyDefinitions>();

        public KeyDefinitions SelectedFrequencyString { get; set; }
        public KeyDefinitions SelectedDurationString { get; set; }

        private KeyboardStorage _keyStorage = new KeyboardStorage();

        public ObservableCollection<KeyDefinitions> FrequencyKeys
        {
            get { return _frequencyKeys; }
            set
            {
                _frequencyKeys = value;
                OnPropertyChanged("FrequencyKeys");
            }
        }
        public ObservableCollection<KeyDefinitions> DurationKeys
        {
            get { return _durationKeys; }
            set
            {
                _durationKeys = value;
                OnPropertyChanged("DurationKeys");
            }
        }

        string _dataRole;
        public string SelectedDataRole {
            get { return _dataRole; }
            set
            {
                _dataRole = value;
                OnPropertyChanged("SelectedDataRole");
            }
        }

        string _sessionTime;
        public string SelectedTime
        {
            get { return _sessionTime; }
            set
            {
                _sessionTime = value;
                OnPropertyChanged("SelectedTime");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SetupViewModel()
        {
            CloseSetupWindow = new RelayCommand(param => CloseSettingsWindow(), param => true);
            SetupWindowFired = new RelayCommand(param => OpenSessionWindow(), param => true);
            AddGroupWindow = new RelayCommand(param => OpenAddGroupDialog(), param => true);
            AddIndividualWindow = new RelayCommand(param => OpenAddIndividualDialog(), param => true);
            AddEvaluationWindow = new RelayCommand(param => OpenAddEvaluationDialog(), param => true);
            AddConditionWindow = new RelayCommand(param => OpenAddConditionDialog(), param => true);
            AddKeyboardWindow = new RelayCommand(param => OpenAddKeyboardDialog(), param => true);
            AddCollectorWindow = new RelayCommand(param => OpenAddCollectorDialog(), param => true);
            AddTherapistWindow = new RelayCommand(param => OpenAddTherapistWindow(), param => true);

            groupListViewModel = new GroupListViewModel(new GroupRepository());
            groupListViewModel.mInt = this;

            individualListViewModel = new IndividualListModel();
            individualListViewModel.mInt = this;

            evaluationListViewModel = new EvaluationListModel();
            evaluationListViewModel.mInt = this;

            conditionListViewModel = new ConditionListModel();
            conditionListViewModel.mInt = this;

            keyboardListViewModel = new KeyboardListViewModel();
            keyboardListViewModel.mInt = this;

            collectorListViewModel = new CollectorListViewModel();
            collectorListViewModel.mInt = this;

            therapistListViewModel = new PrimaryTherapistViewModel();
            therapistListViewModel.mInt = this;
        }

        /// <summary>
        /// Close settings window
        /// </summary>
        public void CloseSettingsWindow()
        {
            var mainWindow = WindowTools.GetWindowRef("SetupWindowTag");
            mainWindow.Close();
        }

        /// <summary>
        /// Open session window
        /// </summary>
        public void OpenSessionWindow()
        {
            if (_group == null || _group.Length < 1 ||
                _indiv == null || _indiv.Length < 1 ||
                _eval == null || _eval.Length < 1 ||
                _cond == null || _cond.Length < 1 ||
                _keys == null || _keys.Length < 1 ||
                _sessionNumber == null || _sessionNumber.Length < 1 ||
                _ther == null || _ther.Length < 1 ||
                _collect == null || _collect.Length < 1 ||
                _dataRole == null || _dataRole.Length < 1 ||
                _sessionTime == null || _sessionTime.Length < 1)
            {
                return;
            }

            int session;
            if(!int.TryParse(_sessionNumber, out session))
            {
                return;
            }

            var window = new SessionWindow("Recording Session: Session #" + SessionNumber);
            window.GroupName = _group;
            window.PatientName = _indiv;
            window.EvaluationName = _eval;
            window.ConditionName = _cond;
            window.KeyboardName = _keys + ".json";
            window.SessionCount = session;
            window.TherapistName = _ther;
            window.CollectorName = _collect;
            window.CollectorRole = _dataRole;
            window.SetKeys(keyboardListViewModel.keyboardSelection);
            window.SessionTime = GetSessionLength(_sessionTime);

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            var kbWindow = new ResultsWindow();

            kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            kbWindow.mFrequencyColumns = window.GetFrequencyKeys();
            kbWindow.mDurationColumns = window.GetDurationKeys();

            kbWindow.mainFreqCounts = window.GetMainFrequencyCounts();
            kbWindow.mainFreqMinutes = window.GetMainFrequencyTotals();
            kbWindow.mainFreqRPM = window.GetMainFrequencyRPM();

            kbWindow.mainDurCounts = window.GetMainDurationTime();
            kbWindow.mainDurMinutes = window.GetMainDurationTotalTime();
            kbWindow.mainDurPercent = window.GetMainDurationPercentageSession();

            kbWindow.schOneFreqCounts = window.GetSchOneFrequencyCounts();
            kbWindow.schOneFreqMinutes = window.GetSchOneFrequencyTotals();
            kbWindow.schOneFreqRPM = window.GetSchOneFrequencyRPM();

            kbWindow.schOneDurCounts = window.GetSchOneDurationTime();
            kbWindow.schOneDurMinutes = window.GetSchOneDurationTotalTime();
            kbWindow.schOneDurPercent = window.GetSchOneDurationPercentageSession();

            kbWindow.schTwoFreqCounts = window.GetSchTwoFrequencyCounts();
            kbWindow.schTwoFreqMinutes = window.GetSchTwoFrequencyTotals();
            kbWindow.schTwoFreqRPM = window.GetSchTwoFrequencyRPM();

            kbWindow.schTwoDurCounts = window.GetSchTwoDurationTime();
            kbWindow.schTwoDurMinutes = window.GetSchTwoDurationTotalTime();
            kbWindow.schTwoDurPercent = window.GetSchTwoDurationPercentageSession();

            kbWindow.schThreeFreqCounts = window.GetSchThreeFrequencyCounts();
            kbWindow.schThreeFreqMinutes = window.GetSchThreeFrequencyTotals();
            kbWindow.schThreeFreqRPM = window.GetSchThreeFrequencyRPM();

            kbWindow.schThreeDurCounts = window.GetSchThreeDurationTime();
            kbWindow.schThreeDurMinutes = window.GetSchThreeDurationTotalTime();
            kbWindow.schThreeDurPercent = window.GetSchThreeDurationPercentageSession();

            kbWindow.ShowDialog();

            if (kbWindow.SaveData && window.stopWatch.Elapsed.TotalSeconds > 0)
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                try
                {
                    ISheet page = hssfworkbook.CreateSheet("Cover Page");
                    window.WriteResults(page, window.freqIntervalListMain, window.stopWatch, window.durationIntervalListMain, window.mMultiScheds[0], true, 0);
                    page = hssfworkbook.CreateSheet("Schedule 1 Only");
                    window.WriteResults(page, window.freqIntervalListSchOne, window.scheduleOne, window.durationIntervalListSchOne, window.mMultiScheds[1], false, 1);
                    page = hssfworkbook.CreateSheet("Schedule 2 Only");
                    window.WriteResults(page, window.freqIntervalListSchTwo, window.scheduleTwo, window.durationIntervalListSchTwo, window.mMultiScheds[2], false, 2);
                    page = hssfworkbook.CreateSheet("Schedule 3 Only");
                    window.WriteResults(page, window.freqIntervalListSchThree, window.scheduleThree, window.durationIntervalListSchThree, window.mMultiScheds[3], false, 3);

                    page = hssfworkbook.CreateSheet("FrequencyIntervals");
                    window.WriteFreqIntervalResults(page, window.keyFrequency, window.mKeyboards.frequencyKeys, window.freqIntervalListMain);

                    page = hssfworkbook.CreateSheet("DurationIntervals");
                    window.WriteDurIntervalResults(page, window.keyDuration, window.mKeyboards.durationKeys, window.durationIntervalListMain);

                    var task = new Task<bool>(() => 
                    {
                        var targetDir = Path.Combine(Properties.Settings.Default.SaveLocation, window.GroupName, window.PatientName, window.EvaluationName, window.ConditionName);
                        var di = new DirectoryInfo(targetDir);
                        return di.Exists;
                    });
                    task.Start();

                    bool resp = task.Wait(100) && task.Result;
                    
                    if (resp)
                    {
                        var targetFile = Path.Combine(Properties.Settings.Default.SaveLocation,
                            window.GroupName,
                            window.PatientName,
                            window.EvaluationName,
                            window.ConditionName,
                            "Session_" + window.SessionCount + "_" + window.CollectorRole + ".xlsx");

                        using (FileStream file = new FileStream(targetFile, FileMode.Create))
                        {
                            hssfworkbook.Write(file);
                        }
                    }
                    else
                    {
                        var targetFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                            "DataTracker",
                            window.GroupName,
                            window.PatientName,
                            window.EvaluationName,
                            window.ConditionName,
                            "Session_" + window.SessionCount + "_" + window.CollectorRole + ".xlsx");

                        using (FileStream file = new FileStream(targetFile, FileMode.Create))
                        {
                            hssfworkbook.Write(file);
                        }

                        MessageBox.Show("Saved to local location!");
                    }

                }
                catch (IOException e2)
                {
                    Console.WriteLine(e2.ToString());
                }
            }

            CountSessions();
        }

        /// <summary>
        /// Get session length
        /// </summary>
        /// <param name="mString"></param>
        /// <returns></returns>
        public int GetSessionLength(string mString)
        {
            if (mString.Trim().ToLower() == "untimed")
            {
                return 999;
            }
            else
            {
                string[] mArray = mString.Split(' ');
                return int.Parse(mArray[0]);
            }
        }

        /// <summary>
        /// Open group window
        /// </summary>
        public void OpenAddGroupDialog()
        {
            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Group";
            dialog.QuestionText = "Please give a name to the new group.";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    DirectoryInfo di2 = Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.SaveLocation, dialog.ResponseText));
                    _group = _indiv = _eval = _cond = "";
                    groupListViewModel.RefreshRepository();
                    individualListViewModel.AllIndividuals.Clear();
                    evaluationListViewModel.AllEvaluations.Clear();
                    conditionListViewModel.AllConditions.Clear();
                    collectorListViewModel.AllCollectors.Clear();
                    keyboardListViewModel.AllKeyboards.Clear();
                    therapistListViewModel.AllTherapists.Clear();
                    MessageBox.Show("Successfully Created: " + dialog.ResponseText);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Open individual window
        /// </summary>
        public void OpenAddIndividualDialog()
        {
            if (_group == null || _group.Length < 1)
                return;

            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Individual";
            dialog.QuestionText = "Please give a name to the new individual.";

            if (dialog.ShowDialog() == true)
            {
                DirectoryInfo di2 = Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.SaveLocation, _group, dialog.ResponseText));
                _indiv = _eval = _cond = "";
                individualListViewModel.RefreshRepository(_group);
                evaluationListViewModel.AllEvaluations.Clear();
                conditionListViewModel.AllConditions.Clear();
                collectorListViewModel.AllCollectors.Clear();
                keyboardListViewModel.AllKeyboards.Clear();
                therapistListViewModel.AllTherapists.Clear();

                MessageBox.Show("Successfully Created: " + dialog.ResponseText);
            }
        }

        /// <summary>
        /// Open evaluation window
        /// </summary>
        public void OpenAddEvaluationDialog()
        {

            if (_group == null || _indiv == null || _group.Length < 1 || _indiv.Length < 1)
                return;

            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Evaluation";
            dialog.QuestionText = "Please give a name to the new evaluation.";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    DirectoryInfo di2 = Directory.CreateDirectory(Properties.Settings.Default.SaveLocation + "\\" + _group + "\\" + _indiv + "\\" + dialog.ResponseText + "\\");
                    _eval = _cond = "";
                    evaluationListViewModel.RefreshRepository(_group, _indiv);
                    conditionListViewModel.AllConditions.Clear();
                    MessageBox.Show("Successfully Created: " + dialog.ResponseText);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Open condition window
        /// </summary>
        public void OpenAddConditionDialog()
        {
            if (_group == null || _indiv == null || _eval == null || _group.Length < 1 || _indiv.Length < 1 || _eval.Length < 1)
                return;

            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Condition";
            dialog.QuestionText = "Please give a name to the new condition.";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    DirectoryInfo di2 = Directory.CreateDirectory(Properties.Settings.Default.SaveLocation + "\\" + _group + "\\" + _indiv + "\\" + _eval + "\\" + dialog.ResponseText + "\\");
                    _cond = "";
                    conditionListViewModel.RefreshRepository(_group, _indiv, _eval);
                    MessageBox.Show("Successfully Created: " + dialog.ResponseText);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Open add keyboard window
        /// </summary>
        public void OpenAddKeyboardDialog()
        {
            if (_group == null || _group.Length < 1 || _indiv == null | _indiv.Length < 1)
                return;

            bool editingCurrent = false;

            var editDialog = new DialogEditYesNo();

            if (_keys != null && _keys.Length > 0)
            {
                editDialog.QuestionText = "Do you want new keys or to edit: " + _keys;

                if (editDialog.ShowDialog() == true)
                {
                    editingCurrent = editDialog.ReturnedAnswer;
                }
            }
            
            if (editingCurrent && editDialog.Clicked)
            {
                var mModel = new KeyboardScreenViewModel();
                mModel.PatientName = _indiv;
                mModel.GroupName = _group;
                mModel.FileName = _keys;

                var kbWindow = new KeyboardScreen();
                kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                kbWindow.DataContext = mModel;

                mModel.SetupKeysEditing(editingCurrent);

                if (kbWindow.ShowDialog() == true)
                {
                    using (StreamWriter file = new StreamWriter(Path.Combine(Properties.Settings.Default.SaveLocation, _group, _indiv,  _keys + ".json"), false))
                    {
                        file.WriteLine(JsonConvert.SerializeObject(mModel.mReturnedKeys));
                    }

                    MessageBox.Show("Successfully Edited Keyboard: " + _keys);

                    FrequencyKeys.Clear();
                    DurationKeys.Clear();

                    keyboardListViewModel.RefreshRepository(_group, _indiv);
                }
            }
            else if (!editingCurrent && editDialog.Clicked)
            {
                var dialog = new Dialog.Dialog();
                dialog.Title = "Add New Key Set";
                dialog.QuestionText = "Please give a name to the new key set.";

                if (dialog.ShowDialog() == true)
                {
                    string mKeySetName = dialog.ResponseText;

                    var mModel = new KeyboardScreenViewModel();
                    mModel.PatientName = _indiv;
                    mModel.GroupName = _group;
                    mModel.CurrentlyEditing = editingCurrent;
                    mModel.FileName = mKeySetName;

                    Window MainWindow2 = Application.Current.MainWindow;
                    var kbWindow = new KeyboardScreen();
                    kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    kbWindow.DataContext = mModel;

                    if (kbWindow.ShowDialog() == true)
                    {
                        using (StreamWriter file = new StreamWriter(Path.Combine(Properties.Settings.Default.SaveLocation, _group, _indiv, mKeySetName + ".json"), false))
                        {
                            file.WriteLine(JsonConvert.SerializeObject(mModel.mReturnedKeys));
                        }

                        MessageBox.Show("Successfully Created Keyboard: " + mKeySetName);

                        FrequencyKeys.Clear();
                        DurationKeys.Clear();

                        keyboardListViewModel.RefreshRepository(_group, _indiv);
                    }
                }
            }
        }

        /// <summary>
        /// Open collector window
        /// </summary>
        public void OpenAddCollectorDialog()
        {
            if (_group == null || _group.Length < 1 || _indiv == null | _indiv.Length < 1)
                return;

            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Data Collector";
            dialog.QuestionText = "Please give a name to the new data collector.";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string mKeySetName = dialog.ResponseText;

                    collectorListViewModel.AllCollectors.Add(Collector.CreateCollector(mKeySetName));

                    using (StreamWriter file = new StreamWriter(Path.Combine(Properties.Settings.Default.SaveLocation, _group, _indiv, "DataCollectors.json"), false))
                    {
                        Collectors mCollector = new Collectors();
                        mCollector.DataCollectors = collectorListViewModel.AllCollectors;
                        file.WriteLine(JsonConvert.SerializeObject(mCollector));
                    }

                    MessageBox.Show("Successfully added: " + dialog.ResponseText);

                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Open therapist window
        /// </summary>
        public void OpenAddTherapistWindow()
        {
            if (_group == null || _group.Length < 1 || _indiv == null | _indiv.Length < 1)
                return;

            var dialog = new Dialog.Dialog();
            dialog.Title = "Add New Therapist";
            dialog.QuestionText = "Please give a name to the new therapist.";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string mKeySetName = dialog.ResponseText;
                    therapistListViewModel.AllTherapists.Add(Therapist.CreateTherapist(mKeySetName));
                    using (StreamWriter file = new StreamWriter(Path.Combine(Properties.Settings.Default.SaveLocation, _group, _indiv, "Therapists.json"), false))
                    {
                        Therapists mCollector = new Therapists();
                        mCollector.PrimaryTherapists = therapistListViewModel.AllTherapists;

                        file.WriteLine(JsonConvert.SerializeObject(mCollector));
                    }
                    MessageBox.Show("Successfully added: " + dialog.ResponseText);

                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void GroupChangeInterfaceMethod(string value)
        {
            _group = value;

            individualListViewModel.IndividualListModelUpdate(_group);
            evaluationListViewModel.AllEvaluations.Clear();
            conditionListViewModel.AllConditions.Clear();
            keyboardListViewModel.AllKeyboards.Clear();
            collectorListViewModel.AllCollectors.Clear();
            therapistListViewModel.AllTherapists.Clear();

            FrequencyKeys.Clear();
            DurationKeys.Clear();

            _indiv = _eval = _cond = _keys = _collect = "";
        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void IndividualChangeInterfaceMethod(string value)
        {
            _indiv = value;

            evaluationListViewModel.EvaluationListModelUpdate(_group, _indiv);
            conditionListViewModel.AllConditions.Clear();
            keyboardListViewModel.AllKeyboards.Clear();
            collectorListViewModel.AllCollectors.Clear();
            therapistListViewModel.AllTherapists.Clear();

            FrequencyKeys.Clear();
            DurationKeys.Clear();

            _eval = _cond = _keys = _collect = "";

            if (_group == null || _group.Length < 1 || _indiv == null || _indiv.Length < 1)
                return;

            keyboardListViewModel.RefreshRepository(_group, _indiv);
            collectorListViewModel.RefreshRepository(_group, _indiv);
            therapistListViewModel.RefreshRepository(_group, _indiv);
        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void EvaluationChangeInterfaceMethod(string value)
        {
            _eval = value;

            conditionListViewModel.ConditionListModelUpdate(_group, _indiv, _eval);
            CountSessions();
            _cond = "";
        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void ConditionChangeInterfaceMethod(string value)
        {
            _cond = value;
        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void CollectorChangeInterfaceMethod(string value)
        {
            _collect = value;
        }

        /// <summary>
        /// Interface method
        /// </summary>
        public void KeyboardChangeInterfaceMethod(KeyboardStorage value)
        {
            if (value == _keyStorage)
            {
                return;
            }

            _keys = value.name;
            _keyStorage.frequencyKeys = value.frequencyKeys;
            _keyStorage.durationKeys = value.durationKeys;
            FrequencyKeys = value.frequencyKeys;
            DurationKeys = value.durationKeys;
        }

        /// <summary>
        /// Interface method
        /// </summary>
        /// <param name="value"></param>
        public void TherapistChangeInterfaceMethod(string value)
        {
            _ther = value;
        }

        /// <summary>
        /// Session counter
        /// </summary>
        public void CountSessions()
        {
            try
            {
                var targetDirectory = Path.Combine(Properties.Settings.Default.SaveLocation, _group, _indiv, _eval);

                int comparison = 0;

                foreach (string file in GetFiles(targetDirectory))
                {
                    string[] mStringArray = file.Split('_');
                    int holder;
                    if (mStringArray.Length > 2 && int.TryParse(mStringArray[1], out holder))
                    {
                        comparison = (holder > comparison) ? holder : comparison;
                    }
                }

                comparison++;

                SessionNumber = comparison.ToString();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// File counting method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);

            while (queue.Count > 0)
            {
                path = queue.Dequeue();

                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                string[] files = null;

                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}
