using DataTracker.Model;
using DataTracker.ViewModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DataTracker.Dialog;

namespace DataTracker.View
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        public string PatientName { get; set; }
        public string GroupName { get; set; }
        public string EvaluationName { get; set; }
        public string ConditionName { get; set; }
        public string KeyboardName { get; set; }
        public string TherapistName { get; set; }
        public string CollectorName { get; set; }
        public string CollectorRole { get; set; }
        public int SessionCount { get; set; }
        public int SessionTime { get; set; }
        private bool hideTaskBar = false;
        private string currentTime = string.Empty;
        private string scheduleOneTime = string.Empty;
        private string scheduleTwoTime = string.Empty;
        private string scheduleThreeTime = string.Empty;
        private string[] currentTimes;

        public class DurationModels
        {
            public bool Recording { get; set; }
            public Stopwatch Timer { get; set; }
            public TimeSpan TimeSpanOutput { get; set; }
            public string PastTime { get; set; }
            public int ActiveIntervals = 0;
            public int TotalIntervals = 0;
            public bool WasActive = false;
            public bool WasObserved = false;
        }

        public class KeyEventTag
        {
            public string KeyString { get; set; }
            public Key KeyCode { get; set; }
            public KeyTags KeyTag { get; set; }
            public ScheduleTags ScheduleTag { get; set; }
            public TimeSpan TimePressed { get; set; }
        }

        public enum KeyTags
        {
            Duration,
            Frequency,
            Schedule
        }

        public enum ScheduleTags
        {
            One,
            Two,
            Three
        }

        public class MultipleSchedules
        {
            public DurationModels[] mKeyModels { get; set; }
        }

        private ObservableCollection<KeyEventTag> _list;

        public ObservableCollection<KeyEventTag> KeyEventList
        {
            get { return _list; }
            set { _list = value; }
        }

        public MultipleSchedules[] mMultiScheds;

        KeyboardDualListViewModel mKeyViewModel;

        public KeyboardStorage mKeyboards;

        Key[] mKeysF, mKeysD;

        // Frequency objects
        public List<int[]> freqIntervalListMain = new List<int[]>();
        int[] tempFreqIntervalsMain;

        public List<int[]> freqIntervalListSchOne = new List<int[]>();
        int[] tempFreqIntervalsSchOne;
        bool freqSchedOneWasActive = false;

        public List<int[]> freqIntervalListSchTwo = new List<int[]>();
        int[] tempFreqIntervalsSchTwo;
        bool freqSchedTwoWasActive = false;

        public List<int[]> freqIntervalListSchThree = new List<int[]>();
        int[] tempFreqIntervalsSchThree;
        bool freqSchedThreeWasActive = false;
        // Frequency Objects END

        // Duration objects
        double[] durationHolder;

        public List<double[]> durationIntervalListMain = new List<double[]>();
        double[] tempdurationIntervalsMain;

        public List<double[]> durationIntervalListSchOne = new List<double[]>();
        double[] tempdurationIntervalsSchOne;
        bool durSchedOneWasActive = false;

        public List<double[]> durationIntervalListSchTwo = new List<double[]>();
        double[] tempdurationIntervalsSchTwo;
        bool durSchedTwoWasActive = false;

        public List<double[]> durationIntervalListSchThree = new List<double[]>();
        double[] tempdurationIntervalsSchThree;
        bool durSchedThreeWasActive = false;
        // Duration objects END

        //XSSFWorkbook hssfworkbook;

        ObservableCollection<ProgressMonitor> mListItems = new ObservableCollection<ProgressMonitor>();

        ProgressListViewModel mProgModel = new ProgressListViewModel();

        private readonly BackgroundWorker worker = new BackgroundWorker();

        DispatcherTimer dt = new DispatcherTimer();
        public Stopwatch stopWatch = new Stopwatch();
        public Stopwatch scheduleOne = new Stopwatch();
        public Stopwatch scheduleTwo = new Stopwatch();
        public Stopwatch scheduleThree = new Stopwatch();
        Stopwatch tenSecondIntervalWatch = new Stopwatch();

        Brush mStandardBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#747a79"));

        private bool isReplacing = false;

        public SessionWindow(string titleText)
        {
            InitializeComponent();
            Closing += new CancelEventHandler(MainWindow_Closing);
            KeyboardStorage mKeyboards = new KeyboardStorage();

            mKeyViewModel = new KeyboardDualListViewModel();
            DataContext = mKeyViewModel;

            Title.Content = titleText;
            progressBox.ItemsSource = mListItems;

        }

        public void SetKeys(KeyboardStorage mPassedKeys)
        {
            mKeyboards = mPassedKeys;

            // Multiple Schedules Frequency
            tempFreqIntervalsMain = new int[mKeyboards.frequencyKeys.Count];
            tempFreqIntervalsSchOne = new int[mKeyboards.frequencyKeys.Count];
            tempFreqIntervalsSchTwo = new int[mKeyboards.frequencyKeys.Count];
            tempFreqIntervalsSchThree = new int[mKeyboards.frequencyKeys.Count];

            for (int mInt = 0; mInt < tempFreqIntervalsMain.Length; mInt++)
            {
                tempFreqIntervalsMain[mInt] = 0;
                tempFreqIntervalsSchOne[mInt] = 0;
                tempFreqIntervalsSchTwo[mInt] = 0;
                tempFreqIntervalsSchThree[mInt] = 0;
            }

            // Multiple Schedules Duration
            durationHolder = new double[mKeyboards.durationKeys.Count];

            tempdurationIntervalsMain = new double[mKeyboards.durationKeys.Count];
            tempdurationIntervalsSchOne = new double[mKeyboards.durationKeys.Count];
            tempdurationIntervalsSchTwo = new double[mKeyboards.durationKeys.Count];
            tempdurationIntervalsSchThree = new double[mKeyboards.durationKeys.Count];

            for (int mInt = 0; mInt < tempdurationIntervalsMain.Length; mInt++)
            {
                durationHolder[mInt] = 0;

                tempdurationIntervalsMain[mInt] = 0;
                tempdurationIntervalsSchOne[mInt] = 0;
                tempdurationIntervalsSchTwo[mInt] = 0;
                tempdurationIntervalsSchThree[mInt] = 0;
            }

            mKeysF = new Key[mKeyboards.frequencyKeys.Count];

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mKeyboards.frequencyKeys.ElementAt(i).Counts = 0;
                mKeysF[i] = (Key)mKeyboards.frequencyKeys.ElementAt(i).KeyCode;
            }

            mKeysD = new Key[mKeyboards.durationKeys.Count];

            mMultiScheds = new MultipleSchedules[4];

            for (int cnt = 0; cnt < 4; cnt++)
            {
                mMultiScheds[cnt] = new MultipleSchedules();

                mMultiScheds[cnt].mKeyModels = new DurationModels[mKeyboards.durationKeys.Count];
                currentTimes = new string[mKeyboards.durationKeys.Count];

                for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
                {
                    mKeyboards.durationKeys.ElementAt(i).CountString = "0";
                    mKeyboards.durationKeys.ElementAt(i).isRunning = false;
                    mKeysD[i] = (Key)mKeyboards.durationKeys.ElementAt(i).KeyCode;

                    mMultiScheds[cnt].mKeyModels[i] = new DurationModels();
                    mMultiScheds[cnt].mKeyModels[i].Recording = false;
                    mMultiScheds[cnt].mKeyModels[i].Timer = new System.Diagnostics.Stopwatch();
                    mMultiScheds[cnt].mKeyModels[i].TimeSpanOutput = new TimeSpan();
                }
            }

            foreach (KeyDefinitions key in mKeyboards.frequencyKeys)
            {
                mKeyViewModel.AllFrequencies.Add(key);
            }

            foreach (KeyDefinitions key in mKeyboards.durationKeys)
            {
                mKeyViewModel.AllDurations.Add(key);
            }

            progressBox.ItemsSource = mListItems;
            progressBox.Items.Refresh();

            groupLabelText.Content = GroupName;
            indivLabelText.Content = PatientName;
            evalLabelText.Content = EvaluationName;
            condLabelText.Content = ConditionName;
            collRoleLabelText.Content = CollectorRole;

            if (File.Exists(Path.Combine(Properties.Settings.Default.SaveLocation, GroupName, PatientName, EvaluationName, ConditionName, "Session_" + SessionCount + "_" + CollectorRole + ".xlsx")))
            {
                FileStream fileStream = null;

                try
                {
                    fileStream = File.Open(Path.Combine(Properties.Settings.Default.SaveLocation, GroupName, PatientName, EvaluationName, ConditionName, "Session_" + SessionCount + "_" + CollectorRole + ".xlsx"), FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    MessageBox.Show("The Destination File appears to be locked! Please close it before continuing!");
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                }
            }

        }
        
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                var dialog = new DialogYesNo();
                dialog.Title = "Quit Running Session?";
                dialog.QuestionText = "Do you want to discontinue the current session?";

                if (dialog.ShowDialog() == true)
                {
                    if (!dialog.ReturnedAnswer)
                    {
                        e.Cancel = true;
                        //return;
                    }
                    else
                    {
                    // Main Freq Count Array
                    int[] mCopyTempMainSchedule = new int[tempFreqIntervalsMain.Length];
                    Array.Copy(tempFreqIntervalsMain, mCopyTempMainSchedule, tempFreqIntervalsMain.Length);
                    freqIntervalListMain.Add(mCopyTempMainSchedule);

                    // Individial Freq Count Arrays
                    if (freqSchedOneWasActive)
                    {
                        int[] mCopyTempMultSchedOne = new int[tempFreqIntervalsSchOne.Length];
                        Array.Copy(tempFreqIntervalsSchOne, mCopyTempMultSchedOne, tempFreqIntervalsSchOne.Length);
                        freqIntervalListSchOne.Add(mCopyTempMultSchedOne);
                    }

                    if (freqSchedTwoWasActive)
                    {
                        int[] mCopyTempMultSchedTwo = new int[tempFreqIntervalsSchTwo.Length];
                        Array.Copy(tempFreqIntervalsSchTwo, mCopyTempMultSchedTwo, tempFreqIntervalsSchTwo.Length);
                        freqIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                    }

                    if (freqSchedThreeWasActive)
                    {
                        int[] mCopyTempMultSchedThree = new int[tempFreqIntervalsSchThree.Length];
                        Array.Copy(tempFreqIntervalsSchThree, mCopyTempMultSchedThree, tempFreqIntervalsSchThree.Length);
                        freqIntervalListSchThree.Add(mCopyTempMultSchedThree);
                    }

                    for (int mInt = 0; mInt < tempFreqIntervalsMain.Length; mInt++)
                    {
                        // Set ALL frequncy elements to base 0
                        tempFreqIntervalsMain[mInt] = 0;
                        tempFreqIntervalsSchOne[mInt] = 0;
                        tempFreqIntervalsSchTwo[mInt] = 0;
                        tempFreqIntervalsSchThree[mInt] = 0;
                    }


                    // END FREQUENCY LOG

                    // Main Dur Ararys
                    for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                    {
                        // Only get Actives for Main
                        if (mMultiScheds[0].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[0].mKeyModels[counter].ActiveIntervals = mMultiScheds[0].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[0].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[0].mKeyModels[counter].TotalIntervals = mMultiScheds[0].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[0].mKeyModels[counter].WasActive = false;
                        mMultiScheds[0].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 1
                        if (mMultiScheds[1].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[1].mKeyModels[counter].ActiveIntervals = mMultiScheds[1].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[1].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[1].mKeyModels[counter].TotalIntervals = mMultiScheds[1].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[1].mKeyModels[counter].WasActive = false;
                        mMultiScheds[1].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 2
                        if (mMultiScheds[2].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[2].mKeyModels[counter].ActiveIntervals = mMultiScheds[2].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[2].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[2].mKeyModels[counter].TotalIntervals = mMultiScheds[2].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[2].mKeyModels[counter].WasActive = false;
                        mMultiScheds[2].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 3

                        if (mMultiScheds[3].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[3].mKeyModels[counter].ActiveIntervals = mMultiScheds[3].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[3].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[3].mKeyModels[counter].TotalIntervals = mMultiScheds[3].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[3].mKeyModels[counter].WasActive = false;
                        mMultiScheds[3].mKeyModels[counter].WasObserved = false;
                    }

                    // Here
                    for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                    {
                        if (tempdurationIntervalsMain[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                        {
                            tempdurationIntervalsMain[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchOne[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                        {
                            tempdurationIntervalsSchOne[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchTwo[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleTwo.IsRunning)
                        {
                            tempdurationIntervalsSchTwo[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchThree[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleThree.IsRunning)
                        {
                            tempdurationIntervalsSchThree[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }
                    }

                    for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                    {
                        durationHolder[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds;
                    }

                    double[] mCopyTempDurSchedule = new double[tempdurationIntervalsMain.Length];
                    Array.Copy(tempdurationIntervalsMain, mCopyTempDurSchedule, tempdurationIntervalsMain.Length);
                    durationIntervalListMain.Add(mCopyTempDurSchedule);

                    if (durSchedOneWasActive)
                    {
                        double[] mCopyTempMultSchedOne = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchOne, mCopyTempMultSchedOne, tempdurationIntervalsMain.Length);
                        durationIntervalListSchOne.Add(mCopyTempMultSchedOne);
                    }

                    if (durSchedTwoWasActive)
                    {
                        double[] mCopyTempMultSchedTwo = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchTwo, mCopyTempMultSchedTwo, tempdurationIntervalsMain.Length);
                        durationIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                    }

                    if (durSchedThreeWasActive)
                    {
                        double[] mCopyTempMultSchedThree = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchThree, mCopyTempMultSchedThree, tempdurationIntervalsMain.Length);
                        durationIntervalListSchThree.Add(mCopyTempMultSchedThree);
                    }

                    for (int mInt = 0; mInt < tempdurationIntervalsMain.Length; mInt++)
                    {   // set ALL duration interval counts to base 0
                        tempdurationIntervalsMain[mInt] = 0;
                        tempdurationIntervalsSchOne[mInt] = 0;
                        tempdurationIntervalsSchTwo[mInt] = 0;
                        tempdurationIntervalsSchThree[mInt] = 0;
                    }
                                                                /*

                    hssfworkbook = new XSSFWorkbook();

                    var kbWindow = new ResultsWindow();

                    kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    kbWindow.mFrequencyColumns = GetFrequencyKeys();
                    kbWindow.mDurationColumns = GetDurationKeys();

                    kbWindow.mainFreqCounts = GetMainFrequencyCounts();
                    kbWindow.mainFreqMinutes = GetMainFrequencyTotals();
                    kbWindow.mainFreqRPM = GetMainFrequencyRPM();

                    kbWindow.mainDurCounts = GetMainDurationTime();
                    kbWindow.mainDurMinutes = GetMainDurationTotalTime();
                    kbWindow.mainDurPercent = GetMainDurationPercentageSession();

                    kbWindow.schOneFreqCounts = GetSchOneFrequencyCounts();
                    kbWindow.schOneFreqMinutes = GetSchOneFrequencyTotals();
                    kbWindow.schOneFreqRPM = GetSchOneFrequencyRPM();

                    kbWindow.schOneDurCounts = GetSchOneDurationTime();
                    kbWindow.schOneDurMinutes = GetSchOneDurationTotalTime();
                    kbWindow.schOneDurPercent = GetSchOneDurationPercentageSession();

                    kbWindow.schTwoFreqCounts = GetSchTwoFrequencyCounts();
                    kbWindow.schTwoFreqMinutes = GetSchTwoFrequencyTotals();
                    kbWindow.schTwoFreqRPM = GetSchTwoFrequencyRPM();

                    kbWindow.schTwoDurCounts = GetSchTwoDurationTime();
                    kbWindow.schTwoDurMinutes = GetSchTwoDurationTotalTime();
                    kbWindow.schTwoDurPercent = GetSchTwoDurationPercentageSession();

                    kbWindow.schThreeFreqCounts = GetSchThreeFrequencyCounts();
                    kbWindow.schThreeFreqMinutes = GetSchThreeFrequencyTotals();
                    kbWindow.schThreeFreqRPM = GetSchThreeFrequencyRPM();

                    kbWindow.schThreeDurCounts = GetSchThreeDurationTime();
                    kbWindow.schThreeDurMinutes = GetSchThreeDurationTotalTime();
                    kbWindow.schThreeDurPercent = GetSchThreeDurationPercentageSession();

                    

                    if (kbWindow.ShowDialog() == true)
                    {
                        if (kbWindow.SaveData)
                        {
                            try
                            {
                                ISheet page = hssfworkbook.CreateSheet("Cover Page");
                                WriteResults(page, freqIntervalListMain, stopWatch, durationIntervalListMain, mMultiScheds[0], true, 0);
                                page = hssfworkbook.CreateSheet("Schedule 1 Only");
                                WriteResults(page, freqIntervalListSchOne, scheduleOne, durationIntervalListSchOne, mMultiScheds[1], false, 1);
                                page = hssfworkbook.CreateSheet("Schedule 2 Only");
                                WriteResults(page, freqIntervalListSchTwo, scheduleTwo, durationIntervalListSchTwo, mMultiScheds[2], false, 2);
                                page = hssfworkbook.CreateSheet("Schedule 3 Only");
                                WriteResults(page, freqIntervalListSchThree, scheduleThree, durationIntervalListSchThree, mMultiScheds[3], false, 3);

                                page = hssfworkbook.CreateSheet("FrequencyIntervals");
                                WriteFreqIntervalResults(page, keyFrequency, mKeyboards.frequencyKeys, freqIntervalListMain);

                                page = hssfworkbook.CreateSheet("DurationIntervals");
                                WriteDurIntervalResults(page, keyDuration, mKeyboards.durationKeys, durationIntervalListMain);

                                var targetFile = Path.Combine(Properties.Settings.Default.SaveLocation, GroupName, PatientName, EvaluationName, ConditionName, "Session_" + SessionCount + "_" + CollectorRole + ".xlsx");

                                using (FileStream file = new FileStream(targetFile, FileMode.Create))
                                {
                                    hssfworkbook.Write(file);
                                }

                            }
                            catch (IOException e2)
                            {
                                Console.WriteLine(e2.ToString());
                            }
                        }
                    }

                    kbWindow.Close();

                    */

                    DialogResult = true;
//                    Close();
                    }
                }
            }
            else
            {
                DialogResult = true;
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            
            // Frequency interval trips
            if (!freqSchedOneWasActive && scheduleOne.IsRunning)
            {
                freqSchedOneWasActive = true;
            }

            if (!freqSchedTwoWasActive && scheduleTwo.IsRunning)
            {
                freqSchedTwoWasActive = true;
            }

            if (!freqSchedThreeWasActive && scheduleThree.IsRunning)
            {
                freqSchedThreeWasActive = true;
            }

            // Duration interval trips
            if (!durSchedOneWasActive && scheduleOne.IsRunning)
            {
                durSchedOneWasActive = true;
            }

            if (!durSchedTwoWasActive && scheduleTwo.IsRunning)
            {
                durSchedTwoWasActive = true;
            }

            if (!durSchedThreeWasActive && scheduleThree.IsRunning)
            {
                durSchedThreeWasActive = true;
            }
            
            if (scheduleOne.IsRunning)
            {
                for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                {
                    mMultiScheds[0].mKeyModels[counter].WasObserved = true;
                    mMultiScheds[1].mKeyModels[counter].WasObserved = true;

                    if (mKeyboards.durationKeys.ElementAt(counter).isRunning)
                    {
                        mMultiScheds[0].mKeyModels[counter].WasActive = true;
                        mMultiScheds[0].mKeyModels[counter].Timer.Start();

                        mMultiScheds[1].mKeyModels[counter].Recording = true;
                        mMultiScheds[1].mKeyModels[counter].WasActive = true;
                        mMultiScheds[1].mKeyModels[counter].Timer.Start();

                        mMultiScheds[2].mKeyModels[counter].Recording = false;
                        mMultiScheds[2].mKeyModels[counter].Timer.Stop();
                        mMultiScheds[3].mKeyModels[counter].Recording = false;
                        mMultiScheds[3].mKeyModels[counter].Timer.Stop();
                    }
                    else
                    {
                        mMultiScheds[0].mKeyModels[counter].Timer.Stop();

                        mMultiScheds[1].mKeyModels[counter].Recording = false;
                        mMultiScheds[1].mKeyModels[counter].Timer.Stop();
                    }
                }
            }

            // TODO Intervals logged here
            if (tenSecondIntervalWatch.IsRunning && tenSecondIntervalWatch.Elapsed.Seconds >= 10.01)
            {
                // Main Freq Count Array
                int[] mCopyTempMainSchedule = new int[tempFreqIntervalsMain.Length];
                Array.Copy(tempFreqIntervalsMain, mCopyTempMainSchedule, tempFreqIntervalsMain.Length);
                freqIntervalListMain.Add(mCopyTempMainSchedule);

                // Individial Freq Count Arrays
                if (freqSchedOneWasActive)
                {
                    int[] mCopyTempMultSchedOne = new int[tempFreqIntervalsSchOne.Length];
                    Array.Copy(tempFreqIntervalsSchOne, mCopyTempMultSchedOne, tempFreqIntervalsSchOne.Length);
                    freqIntervalListSchOne.Add(mCopyTempMultSchedOne);
                }

                if (freqSchedTwoWasActive)
                {
                    int[] mCopyTempMultSchedTwo = new int[tempFreqIntervalsSchTwo.Length];
                    Array.Copy(tempFreqIntervalsSchTwo, mCopyTempMultSchedTwo, tempFreqIntervalsSchTwo.Length);
                    freqIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                }

                if (freqSchedThreeWasActive)
                {
                    int[] mCopyTempMultSchedThree = new int[tempFreqIntervalsSchThree.Length];
                    Array.Copy(tempFreqIntervalsSchThree, mCopyTempMultSchedThree, tempFreqIntervalsSchThree.Length);
                    freqIntervalListSchThree.Add(mCopyTempMultSchedThree);
                }

                for (int mInt = 0; mInt < tempFreqIntervalsMain.Length; mInt++)
                {
                    // Set ALL frequncy elements to base 0
                    tempFreqIntervalsMain[mInt] = 0;
                    tempFreqIntervalsSchOne[mInt] = 0;
                    tempFreqIntervalsSchTwo[mInt] = 0;
                    tempFreqIntervalsSchThree[mInt] = 0;
                }
                

                // END FREQUENCY LOG

                // Main Dur Ararys
                for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                {
                    // Only get Actives for Main
                    if (mMultiScheds[0].mKeyModels[counter].WasActive)
                    {
                        mMultiScheds[0].mKeyModels[counter].ActiveIntervals = mMultiScheds[0].mKeyModels[counter].ActiveIntervals + 1;
                    }

                    if (mMultiScheds[0].mKeyModels[counter].WasObserved)
                    {
                        mMultiScheds[0].mKeyModels[counter].TotalIntervals = mMultiScheds[0].mKeyModels[counter].TotalIntervals + 1;
                    }

                    mMultiScheds[0].mKeyModels[counter].WasActive = false;
                    mMultiScheds[0].mKeyModels[counter].WasObserved = false;

                    // Actives for Sched 1
                    if (mMultiScheds[1].mKeyModels[counter].WasActive)
                    {
                        mMultiScheds[1].mKeyModels[counter].ActiveIntervals = mMultiScheds[1].mKeyModels[counter].ActiveIntervals + 1;
                    }

                    if (mMultiScheds[1].mKeyModels[counter].WasObserved)
                    {
                        mMultiScheds[1].mKeyModels[counter].TotalIntervals = mMultiScheds[1].mKeyModels[counter].TotalIntervals + 1;
                    }

                    mMultiScheds[1].mKeyModels[counter].WasActive = false;
                    mMultiScheds[1].mKeyModels[counter].WasObserved = false;

                    // Actives for Sched 2
                    if (mMultiScheds[2].mKeyModels[counter].WasActive)
                    {
                        mMultiScheds[2].mKeyModels[counter].ActiveIntervals = mMultiScheds[2].mKeyModels[counter].ActiveIntervals + 1;
                    }

                    if (mMultiScheds[2].mKeyModels[counter].WasObserved)
                    {
                        mMultiScheds[2].mKeyModels[counter].TotalIntervals = mMultiScheds[2].mKeyModels[counter].TotalIntervals + 1;
                    }

                    mMultiScheds[2].mKeyModels[counter].WasActive = false;
                    mMultiScheds[2].mKeyModels[counter].WasObserved = false;

                    // Actives for Sched 3

                    if (mMultiScheds[3].mKeyModels[counter].WasActive)
                    {
                        mMultiScheds[3].mKeyModels[counter].ActiveIntervals = mMultiScheds[3].mKeyModels[counter].ActiveIntervals + 1;
                    }

                    if (mMultiScheds[3].mKeyModels[counter].WasObserved)
                    {
                        mMultiScheds[3].mKeyModels[counter].TotalIntervals = mMultiScheds[3].mKeyModels[counter].TotalIntervals + 1;
                    }

                    mMultiScheds[3].mKeyModels[counter].WasActive = false;
                    mMultiScheds[3].mKeyModels[counter].WasObserved = false;
                }

                // Here
                for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                {
                    if (tempdurationIntervalsMain[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                    {
                        tempdurationIntervalsMain[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                    }

                    if (tempdurationIntervalsSchOne[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                    {
                        tempdurationIntervalsSchOne[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                    }

                    if (tempdurationIntervalsSchTwo[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleTwo.IsRunning)
                    {
                        tempdurationIntervalsSchTwo[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                    }

                    if (tempdurationIntervalsSchThree[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleThree.IsRunning)
                    {
                        tempdurationIntervalsSchThree[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                    }
                }

                for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                {
                    durationHolder[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds;
                }

                double[] mCopyTempDurSchedule = new double[tempdurationIntervalsMain.Length];
                Array.Copy(tempdurationIntervalsMain, mCopyTempDurSchedule, tempdurationIntervalsMain.Length);
                durationIntervalListMain.Add(mCopyTempDurSchedule);

                if (durSchedOneWasActive)
                {
                    double[] mCopyTempMultSchedOne = new double[tempdurationIntervalsMain.Length];
                    Array.Copy(tempdurationIntervalsSchOne, mCopyTempMultSchedOne, tempdurationIntervalsMain.Length);
                    durationIntervalListSchOne.Add(mCopyTempMultSchedOne);
                }

                if (durSchedTwoWasActive)
                {
                    double[] mCopyTempMultSchedTwo = new double[tempdurationIntervalsMain.Length];
                    Array.Copy(tempdurationIntervalsSchTwo, mCopyTempMultSchedTwo, tempdurationIntervalsMain.Length);
                    durationIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                }

                if (durSchedThreeWasActive)
                {
                    double[] mCopyTempMultSchedThree = new double[tempdurationIntervalsMain.Length];
                    Array.Copy(tempdurationIntervalsSchThree, mCopyTempMultSchedThree, tempdurationIntervalsMain.Length);
                    durationIntervalListSchThree.Add(mCopyTempMultSchedThree);
                }

                for (int mInt = 0; mInt < tempdurationIntervalsMain.Length; mInt++)
                {   // set ALL duration interval counts to base 0
                    tempdurationIntervalsMain[mInt] = 0;
                    tempdurationIntervalsSchOne[mInt] = 0;
                    tempdurationIntervalsSchTwo[mInt] = 0;
                    tempdurationIntervalsSchThree[mInt] = 0;
                }

                freqSchedOneWasActive = freqSchedTwoWasActive = freqSchedThreeWasActive = durSchedOneWasActive = durSchedTwoWasActive = durSchedThreeWasActive = false;

                tenSecondIntervalWatch.Restart();
            }

            if (scheduleTwo.IsRunning)
            {
                for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                {
                    mMultiScheds[0].mKeyModels[counter].WasObserved = true;
                    mMultiScheds[2].mKeyModels[counter].WasObserved = true;

                    if (mKeyboards.durationKeys.ElementAt(counter).isRunning)
                    {
                        mMultiScheds[0].mKeyModels[counter].WasActive = true;

                        mMultiScheds[2].mKeyModels[counter].Recording = true;
                        mMultiScheds[2].mKeyModels[counter].WasActive = true;
                        mMultiScheds[2].mKeyModels[counter].Timer.Start();

                        mMultiScheds[1].mKeyModels[counter].Recording = false;
                        mMultiScheds[1].mKeyModels[counter].Timer.Stop();
                        mMultiScheds[3].mKeyModels[counter].Recording = false;
                        mMultiScheds[3].mKeyModels[counter].Timer.Stop();
                    }
                    else
                    {
                        mMultiScheds[0].mKeyModels[counter].Timer.Stop();

                        mMultiScheds[2].mKeyModels[counter].Recording = false;
                        mMultiScheds[2].mKeyModels[counter].Timer.Stop();
                    }
                }
            }

            if (scheduleThree.IsRunning)
            {
                for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                {
                    mMultiScheds[0].mKeyModels[counter].WasObserved = true;
                    mMultiScheds[3].mKeyModels[counter].WasObserved = true;

                    if (mKeyboards.durationKeys.ElementAt(counter).isRunning)
                    {
                        mMultiScheds[0].mKeyModels[counter].WasActive = true;

                        mMultiScheds[3].mKeyModels[counter].Recording = true;
                        mMultiScheds[3].mKeyModels[counter].WasActive = true;
                        mMultiScheds[3].mKeyModels[counter].Timer.Start();

                        mMultiScheds[1].mKeyModels[counter].Recording = false;
                        mMultiScheds[1].mKeyModels[counter].Timer.Stop();
                        mMultiScheds[2].mKeyModels[counter].Recording = false;
                        mMultiScheds[2].mKeyModels[counter].Timer.Stop();
                    }
                    else
                    {
                        mMultiScheds[0].mKeyModels[counter].Timer.Stop();

                        mMultiScheds[3].mKeyModels[counter].Recording = false;
                        mMultiScheds[3].mKeyModels[counter].Timer.Stop();
                    }
                }
            }

            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                TimeSpan tsSchedOne = scheduleOne.Elapsed;
                TimeSpan tsSchedTwo = scheduleTwo.Elapsed;
                TimeSpan tsSchedThree = scheduleThree.Elapsed;

                if (ts.TotalSeconds <= (SessionTime * 60))
                {
                    currentTime = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    SessionTimeText.Content = currentTime;

                    scheduleOneTime = string.Format("{0:00}:{1:00}.{2:00}", tsSchedOne.Minutes, tsSchedOne.Seconds, tsSchedOne.Milliseconds / 10);
                    SessionTimeTextSchedOne.Content = scheduleOneTime;

                    scheduleTwoTime = string.Format("{0:00}:{1:00}.{2:00}", tsSchedTwo.Minutes, tsSchedTwo.Seconds, tsSchedTwo.Milliseconds / 10);
                    SessionTimeTextSchedTwo.Content = scheduleTwoTime;

                    scheduleThreeTime = string.Format("{0:00}:{1:00}.{2:00}", tsSchedThree.Minutes, tsSchedThree.Seconds, tsSchedThree.Milliseconds / 10);
                    SessionTimeTextSchedThree.Content = scheduleThreeTime;
                }

                if (ts.TotalSeconds >= (SessionTime*60) + 0.01)
                {
                    stopWatch.Stop();

                    if (scheduleOne.IsRunning)
                    {
                        ProgressMonitor mAdd = new ProgressMonitor();
                        mAdd.Key = Key.X.ToString();
                        mAdd.KeyTag = KeyTags.Schedule;
                        mAdd.Code = "Schedule 1 End";
                        mAdd.ScheduleTag = ScheduleTags.One;
                        mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                        mAdd.KeyCode = Key.Z;
                        mAdd.TimePressed = stopWatch.Elapsed;
                        mListItems.Add(mAdd);
                        scheduleOne.Stop();
                    }
                    else if (scheduleTwo.IsRunning)
                    {
                        ProgressMonitor mAdd = new ProgressMonitor();
                        mAdd.Key = Key.X.ToString();
                        mAdd.KeyTag = KeyTags.Schedule;
                        mAdd.Code = "Schedule 2 End";
                        mAdd.ScheduleTag = ScheduleTags.Two;
                        mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                        mAdd.KeyCode = Key.X;
                        mAdd.TimePressed = stopWatch.Elapsed;
                        mListItems.Add(mAdd);
                        scheduleTwo.Stop();
                    }
                    else if (scheduleThree.IsRunning)
                    {
                        ProgressMonitor mAdd = new ProgressMonitor();
                        mAdd.Key = Key.C.ToString();
                        mAdd.KeyTag = KeyTags.Schedule;
                        mAdd.Code = "Schedule 3 End";
                        mAdd.ScheduleTag = ScheduleTags.Three;
                        mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                        mAdd.KeyCode = Key.C;
                        mAdd.TimePressed = stopWatch.Elapsed;
                        mListItems.Add(mAdd);
                        scheduleThree.Stop();
                    }

                    for (int cnt = 0; cnt < mMultiScheds[0].mKeyModels.Length; cnt++)
                    {
                        mMultiScheds[0].mKeyModels[cnt].Timer.Stop();
                        mMultiScheds[1].mKeyModels[cnt].Timer.Stop();
                        mMultiScheds[2].mKeyModels[cnt].Timer.Stop();
                    }

                    //Final Add Freq
                    // Main Freq Count Array
                    int[] mCopyTempMainSchedule = new int[tempFreqIntervalsMain.Length];
                    Array.Copy(tempFreqIntervalsMain, mCopyTempMainSchedule, tempFreqIntervalsMain.Length);
                    freqIntervalListMain.Add(mCopyTempMainSchedule);

                    // Individial Freq Count Arrays
                    if (freqSchedOneWasActive)
                    {
                        int[] mCopyTempMultSchedOne = new int[tempFreqIntervalsSchOne.Length];
                        Array.Copy(tempFreqIntervalsSchOne, mCopyTempMultSchedOne, tempFreqIntervalsSchOne.Length);
                        freqIntervalListSchOne.Add(mCopyTempMultSchedOne);
                    }

                    if (freqSchedTwoWasActive)
                    {
                        int[] mCopyTempMultSchedTwo = new int[tempFreqIntervalsSchTwo.Length];
                        Array.Copy(tempFreqIntervalsSchTwo, mCopyTempMultSchedTwo, tempFreqIntervalsSchTwo.Length);
                        freqIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                    }

                    if (freqSchedThreeWasActive)
                    {
                        int[] mCopyTempMultSchedThree = new int[tempFreqIntervalsSchThree.Length];
                        Array.Copy(tempFreqIntervalsSchThree, mCopyTempMultSchedThree, tempFreqIntervalsSchThree.Length);
                        freqIntervalListSchThree.Add(mCopyTempMultSchedThree);
                    }

                    for (int mInt = 0; mInt < tempFreqIntervalsMain.Length; mInt++)
                    {
                        // Set ALL frequncy elements to base 0
                        tempFreqIntervalsMain[mInt] = 0;
                        tempFreqIntervalsSchOne[mInt] = 0;
                        tempFreqIntervalsSchTwo[mInt] = 0;
                        tempFreqIntervalsSchThree[mInt] = 0;
                    }

                    //Final Add Dur

                    // Main Dur Ararys
                    for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
                    {
                        // Only get Actives for Main
                        if (mMultiScheds[0].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[0].mKeyModels[counter].ActiveIntervals = mMultiScheds[0].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[0].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[0].mKeyModels[counter].TotalIntervals = mMultiScheds[0].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[0].mKeyModels[counter].WasActive = false;
                        mMultiScheds[0].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 1
                        if (mMultiScheds[1].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[1].mKeyModels[counter].ActiveIntervals = mMultiScheds[1].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[1].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[1].mKeyModels[counter].TotalIntervals = mMultiScheds[1].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[1].mKeyModels[counter].WasActive = false;
                        mMultiScheds[1].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 2
                        if (mMultiScheds[2].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[2].mKeyModels[counter].ActiveIntervals = mMultiScheds[2].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[2].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[2].mKeyModels[counter].TotalIntervals = mMultiScheds[2].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[2].mKeyModels[counter].WasActive = false;
                        mMultiScheds[2].mKeyModels[counter].WasObserved = false;

                        // Actives for Sched 3

                        if (mMultiScheds[3].mKeyModels[counter].WasActive)
                        {
                            mMultiScheds[3].mKeyModels[counter].ActiveIntervals = mMultiScheds[3].mKeyModels[counter].ActiveIntervals + 1;
                        }

                        if (mMultiScheds[3].mKeyModels[counter].WasObserved)
                        {
                            mMultiScheds[3].mKeyModels[counter].TotalIntervals = mMultiScheds[3].mKeyModels[counter].TotalIntervals + 1;
                        }

                        mMultiScheds[3].mKeyModels[counter].WasActive = false;
                        mMultiScheds[3].mKeyModels[counter].WasObserved = false;
                    }

                    for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                    {
                        if (tempdurationIntervalsMain[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                        {
                            tempdurationIntervalsMain[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchOne[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleOne.IsRunning)
                        {
                            tempdurationIntervalsSchOne[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchTwo[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleTwo.IsRunning)
                        {
                            tempdurationIntervalsSchTwo[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }

                        if (tempdurationIntervalsSchThree[mCounter] == 0 && mMultiScheds[0].mKeyModels[mCounter].Recording && scheduleThree.IsRunning)
                        {
                            tempdurationIntervalsSchThree[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds - durationHolder[mCounter];
                        }
                    }

                    for (int mCounter = 0; mCounter < tempdurationIntervalsMain.Length; mCounter++)
                    {
                        durationHolder[mCounter] = mMultiScheds[0].mKeyModels[mCounter].Timer.Elapsed.TotalSeconds;
                    }

                    double[] mCopyTempDurSchedule = new double[tempdurationIntervalsMain.Length];
                    Array.Copy(tempdurationIntervalsMain, mCopyTempDurSchedule, tempdurationIntervalsMain.Length);
                    durationIntervalListMain.Add(mCopyTempDurSchedule);

                    if (durSchedOneWasActive)
                    {
                        double[] mCopyTempMultSchedOne = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchOne, mCopyTempMultSchedOne, tempdurationIntervalsMain.Length);
                        durationIntervalListSchOne.Add(mCopyTempMultSchedOne);
                    }

                    if (durSchedTwoWasActive)
                    {
                        double[] mCopyTempMultSchedTwo = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchTwo, mCopyTempMultSchedTwo, tempdurationIntervalsMain.Length);
                        durationIntervalListSchTwo.Add(mCopyTempMultSchedTwo);
                    }

                    if (durSchedThreeWasActive)
                    {
                        double[] mCopyTempMultSchedThree = new double[tempdurationIntervalsMain.Length];
                        Array.Copy(tempdurationIntervalsSchThree, mCopyTempMultSchedThree, tempdurationIntervalsMain.Length);
                        durationIntervalListSchThree.Add(mCopyTempMultSchedThree);
                    }

                    for (int mInt = 0; mInt < tempdurationIntervalsMain.Length; mInt++)
                    {   // set ALL duration interval counts to base 0
                        tempdurationIntervalsMain[mInt] = 0;
                        tempdurationIntervalsSchOne[mInt] = 0;
                        tempdurationIntervalsSchTwo[mInt] = 0;
                        tempdurationIntervalsSchThree[mInt] = 0;
                    }

                    /*
                    hssfworkbook = new XSSFWorkbook();

                    var kbWindow = new ResultsWindow();

                    kbWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    kbWindow.mFrequencyColumns = GetFrequencyKeys();
                    kbWindow.mDurationColumns = GetDurationKeys();

                    kbWindow.mainFreqCounts = GetMainFrequencyCounts();
                    kbWindow.mainFreqMinutes = GetMainFrequencyTotals();
                    kbWindow.mainFreqRPM = GetMainFrequencyRPM();

                    kbWindow.mainDurCounts = GetMainDurationTime();
                    kbWindow.mainDurMinutes = GetMainDurationTotalTime();
                    kbWindow.mainDurPercent = GetMainDurationPercentageSession();

                    kbWindow.schOneFreqCounts = GetSchOneFrequencyCounts();
                    kbWindow.schOneFreqMinutes = GetSchOneFrequencyTotals();
                    kbWindow.schOneFreqRPM = GetSchOneFrequencyRPM();

                    kbWindow.schOneDurCounts = GetSchOneDurationTime();
                    kbWindow.schOneDurMinutes = GetSchOneDurationTotalTime();
                    kbWindow.schOneDurPercent = GetSchOneDurationPercentageSession();

                    kbWindow.schTwoFreqCounts = GetSchTwoFrequencyCounts();
                    kbWindow.schTwoFreqMinutes = GetSchTwoFrequencyTotals();
                    kbWindow.schTwoFreqRPM = GetSchTwoFrequencyRPM();

                    kbWindow.schTwoDurCounts = GetSchTwoDurationTime();
                    kbWindow.schTwoDurMinutes = GetSchTwoDurationTotalTime();
                    kbWindow.schTwoDurPercent = GetSchTwoDurationPercentageSession();

                    kbWindow.schThreeFreqCounts = GetSchThreeFrequencyCounts();
                    kbWindow.schThreeFreqMinutes = GetSchThreeFrequencyTotals();
                    kbWindow.schThreeFreqRPM = GetSchThreeFrequencyRPM();

                    kbWindow.schThreeDurCounts = GetSchThreeDurationTime();
                    kbWindow.schThreeDurMinutes = GetSchThreeDurationTotalTime();
                    kbWindow.schThreeDurPercent = GetSchThreeDurationPercentageSession();

                    if (kbWindow.ShowDialog() == true)
                    {
                        if (kbWindow.SaveData)
                        {
                            try
                            {
                                //Sheet FrontPage
                                ISheet page = hssfworkbook.CreateSheet("Cover Page");
                                WriteResults(page, freqIntervalListMain, stopWatch, durationIntervalListMain, mMultiScheds[0], true, 0);
                                page = hssfworkbook.CreateSheet("Schedule 1 Only");
                                WriteResults(page, freqIntervalListSchOne, scheduleOne, durationIntervalListSchOne, mMultiScheds[1], false, 1);
                                page = hssfworkbook.CreateSheet("Schedule 2 Only");
                                WriteResults(page, freqIntervalListSchTwo, scheduleTwo, durationIntervalListSchTwo, mMultiScheds[2], false, 2);
                                page = hssfworkbook.CreateSheet("Schedule 3 Only");
                                WriteResults(page, freqIntervalListSchThree, scheduleThree, durationIntervalListSchThree, mMultiScheds[3], false, 3);

                                page = hssfworkbook.CreateSheet("FrequencyIntervals");
                                WriteFreqIntervalResults(page, keyFrequency, mKeyboards.frequencyKeys, freqIntervalListMain);

                                page = hssfworkbook.CreateSheet("DurationIntervals");
                                WriteDurIntervalResults(page, keyDuration, mKeyboards.durationKeys, durationIntervalListMain);

                                var targetFile = Path.Combine(Properties.Settings.Default.SaveLocation, GroupName, PatientName, EvaluationName, ConditionName, "Session_" + SessionCount + "_" + CollectorRole + ".xlsx");

                                using (FileStream file = new FileStream(targetFile, FileMode.Create))
                                {
                                    hssfworkbook.Write(file);
                                }
                            }
                            catch (DirectoryNotFoundException e2)
                            {
                                Console.WriteLine(e2.ToString());
                                string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DataTracker\\";
                                var targetFile = Path.Combine(defaultPath, GroupName, PatientName, EvaluationName, ConditionName, "Session_" + SessionCount + "_" + CollectorRole + ".xlsx");
                                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

                                using (FileStream file = new FileStream(targetFile, FileMode.Create))
                                {
                                    hssfworkbook.Write(file);
                                }

                                MessageBox.Show("File had to be saved to the local drive.");
                            }
                            catch (IOException e2)
                            {
                                Console.WriteLine(e2.ToString());
                            }
                        }
                    }
                    */

                    //DialogResult = true;

                    Close();
                    
                }
            }

            for (int counter = 0; counter < mMultiScheds[0].mKeyModels.Length; counter++)
            {
                
                if (mMultiScheds[0].mKeyModels[counter].Recording)
                {
                    TimeSpan ts = stopWatch.Elapsed.Subtract(GetDurationTimeStampKey(counter));
                    currentTimes[counter] = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    mKeyboards.durationKeys[counter].CountString = currentTimes[counter];

                    keyDuration.Items.Refresh();
                }
                else
                {
                    if (mMultiScheds[0].mKeyModels[counter].Timer.Elapsed.TotalMilliseconds > 0)
                    {
                        mMultiScheds[0].mKeyModels[counter].Timer.Reset();
                        mKeyboards.durationKeys[counter].CountString = currentTimes[counter];
                        keyDuration.Items.Refresh();

                    }
                }
            }
        }

        public TimeSpan GetDurationTimeStampKey(int index)
        {
            Key mKey = mKeyboards.durationKeys[index].KeyCode;
            int mCount = 0;

            TimeSpan tempHolder = stopWatch.Elapsed;

            bool lookingForEnd = false;

            foreach (ProgressMonitor pMon in mListItems)
            {
                if (pMon.KeyCode == mKey && !lookingForEnd)
                {
                    tempHolder = pMon.TimePressed;
                    lookingForEnd = true;
                    mCount++;
                }
                else if (pMon.KeyCode == mKey && lookingForEnd)
                {

                    lookingForEnd = false;
                }
            }

            mKeyViewModel.AllDurations[index].Counts = mCount;

            return tempHolder;
        }

        public TimeSpan GetScheduleTotalTime(int sched)
        {
            TimeSpan tempHolder = stopWatch.Elapsed;
            TimeSpan mTotalTime = TimeSpan.Zero;

            if (sched == 0)
                return stopWatch.Elapsed;

            bool lookingForEnd = false;

            for (int counter = 0; counter < mListItems.Count; counter++)
            {
                if (mListItems[counter].Code.Equals("Schedule " + sched + " Start") && !lookingForEnd)
                {
                    tempHolder = mListItems[counter].TimePressed;
                    lookingForEnd = true;
                }
                else if (mListItems[counter].Code.Equals("Schedule " + sched + " Start") && lookingForEnd)
                {
                    mTotalTime = mTotalTime.Add(mListItems[counter].TimePressed.Subtract(tempHolder));
                    lookingForEnd = false;
                }
            }

            return mTotalTime;
        }

        public void GetDurationSumTimeKey(int index)
        {
            Key mKey = mKeyboards.durationKeys[index].KeyCode;
            int mCount = 0;

            TimeSpan tempHolder = stopWatch.Elapsed;

            TimeSpan mTotalTime = TimeSpan.Zero;

            bool lookingForEnd = false;

            for (int counter = 0; counter < mListItems.Count; counter++)
            {
                if (mListItems[counter].KeyCode == mKey && !lookingForEnd)
                {
                    tempHolder = mListItems[counter].TimePressed;
                    lookingForEnd = true;
                    mCount++;
                }
                else if (mListItems[counter].KeyCode == mKey && lookingForEnd)
                {
                    mTotalTime = mTotalTime.Add(mListItems[counter].TimePressed.Subtract(tempHolder));
                    lookingForEnd = false;
                }
            }

            mKeyViewModel.AllDurations[index].Counts = mCount;
            mKeyboards.durationKeys[index].CountString = string.Format("{0:00}:{1:00}.{2:00}", mTotalTime.Minutes, mTotalTime.Seconds, mTotalTime.Milliseconds / 10);
            currentTimes[index] = string.Format("{0:00}:{1:00}.{2:00}", mTotalTime.Minutes, mTotalTime.Seconds, mTotalTime.Milliseconds / 10);
        }

        public void WriteFreqIntervalResults(ISheet sheetPage, ListView mListView, ObservableCollection<KeyDefinitions> keyDefs, List<int[]> intervalList)
        {
            IRow currRow = sheetPage.CreateRow(0);
            int cntOut = 0;

            for (cntOut = 0; cntOut < mListView.Items.Count; cntOut++)
            {
                currRow.CreateCell(cntOut).SetCellValue(keyDefs[cntOut].KeyName);
            }

            cntOut = 1;

            foreach (int[] holder in intervalList)
            {
                currRow = sheetPage.CreateRow(cntOut);

                for (int counter = 0; counter < holder.Length; counter++)
                {
                    currRow.CreateCell(counter).SetCellValue(holder[counter]);
                }

                cntOut += 1;
            }

        }

        public void WriteDurIntervalResults(ISheet sheetPage, ListView mListView, ObservableCollection<KeyDefinitions> keyDefs, List<double[]> intervalList)
        {
            IRow currRow = sheetPage.CreateRow(0);
            int cntOut = 0;

            for (cntOut = 0; cntOut < mListView.Items.Count; cntOut++)
            {
                currRow.CreateCell(cntOut).SetCellValue(keyDefs[cntOut].KeyName);
            }
            //mKeyboards.frequencyKeys
            cntOut = 1;

            foreach (double[] holder in intervalList)
            {
                currRow = sheetPage.CreateRow(cntOut);

                for (int counter = 0; counter < holder.Length; counter++)
                {
                    currRow.CreateCell(counter).SetCellValue(holder[counter].ToString("0.0"));
                }

                cntOut += 1;
            }
        }

        public string[] GetFrequencyKeys()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "F Key:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mResult[i+1] = mKeyboards.frequencyKeys[i].KeyName;
            }

            return mResult;
        }

        public string[] GetDurationKeys()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "D Key:";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = mKeyboards.durationKeys[i].KeyName;
            }

            return mResult;
        }

        public string[] GetMainFrequencyCounts()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Count:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mResult[i + 1] = GetFrequencyCounts(mKeysF[i]).ToString();
            }

            return mResult;
        }

        public string[] GetMainFrequencyTotals()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Minutes:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(stopWatch.Elapsed.TotalSeconds);
                mResult[i+1] = (totalSeconds / 60).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetMainFrequencyRPM()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "RPM:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(stopWatch.Elapsed.TotalSeconds);
                mResult[i+1] = (GetFrequencyCounts(mKeysF[i]) / (totalSeconds / 60)).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetMainDurationTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Logged time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                double mTime = 0.0;
                mTime += mMultiScheds[1].mKeyModels[i].Timer.Elapsed.TotalSeconds;
                mTime += mMultiScheds[2].mKeyModels[i].Timer.Elapsed.TotalSeconds;
                mTime += mMultiScheds[3].mKeyModels[i].Timer.Elapsed.TotalSeconds;

                mResult[i + 1] = mTime.ToString("0.##");
//                mResult[i + 1] = mMultiScheds[0].mKeyModels[i].Timer.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetMainDurationTotalTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Total time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = stopWatch.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetMainDurationPercentageSession()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Percent (%):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                double mTime = 0.0;
                mTime += mMultiScheds[1].mKeyModels[i].Timer.Elapsed.TotalSeconds;
                mTime += mMultiScheds[2].mKeyModels[i].Timer.Elapsed.TotalSeconds;
                mTime += mMultiScheds[3].mKeyModels[i].Timer.Elapsed.TotalSeconds;

                mResult[i + 1] = ((double)(mTime / stopWatch.Elapsed.TotalSeconds) * 100).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchOneDurationTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Logged time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = mMultiScheds[1].mKeyModels[i].Timer.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchOneDurationTotalTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Total time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = scheduleOne.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchOneDurationPercentageSession()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Percent (%):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = ((double)(mMultiScheds[1].mKeyModels[i].Timer.Elapsed.TotalSeconds / scheduleOne.Elapsed.TotalSeconds) * 100).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchTwoDurationTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Logged time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = mMultiScheds[2].mKeyModels[i].Timer.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchTwoDurationTotalTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Total time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = scheduleTwo.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchTwoDurationPercentageSession()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Percent (%):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = ((double)(mMultiScheds[2].mKeyModels[i].Timer.Elapsed.TotalSeconds / scheduleTwo.Elapsed.TotalSeconds) * 100).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchThreeDurationTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Logged time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = mMultiScheds[3].mKeyModels[i].Timer.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchThreeDurationTotalTime()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Total time (s):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = scheduleThree.Elapsed.TotalSeconds.ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchThreeDurationPercentageSession()
        {
            string[] mResult = new string[mKeyboards.durationKeys.Count + 1];
            mResult[0] = "Percent (%):";

            for (int i = 0; i < mKeyboards.durationKeys.Count; i++)
            {
                mResult[i + 1] = ((double)(mMultiScheds[3].mKeyModels[i].Timer.Elapsed.TotalSeconds / scheduleThree.Elapsed.TotalSeconds) * 100).ToString("0.##");
            }

            return mResult;
        }

        public int GetFrequencyCounts(Key mKey)
        {
            int frequencyCount = 0;

            for (int counter = 0; counter < mListItems.Count; counter++)
            {
                if (mListItems[counter].KeyCode == mKey)
                {
                    frequencyCount++;
                }
            }

            return frequencyCount;
        }

        public int GetFrequencyCounts(Key mKey, ScheduleTags sTag)
        {
            int frequencyCount = 0;

            for (int counter = 0; counter < mListItems.Count; counter++)
            {
                if (mListItems[counter].KeyCode == mKey && mListItems[counter].ScheduleTag == sTag)
                {
                    frequencyCount++;
                }
            }

            return frequencyCount;
        }

        public string[] GetSchOneFrequencyCounts()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Count:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mResult[i + 1] = GetFrequencyCounts(mKeysF[i], ScheduleTags.One).ToString();
            }

            return mResult;
        }

        public string[] GetSchOneFrequencyTotals()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Minutes:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleOne.Elapsed.TotalSeconds);
                mResult[i + 1] = (totalSeconds / 60).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchOneFrequencyRPM()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "RPM:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleOne.Elapsed.TotalSeconds);
                mResult[i + 1] = (GetFrequencyCounts(mKeysF[i], ScheduleTags.One) / (totalSeconds / 60)).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchTwoFrequencyCounts()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Count:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mResult[i + 1] = GetFrequencyCounts(mKeysF[i], ScheduleTags.Two).ToString();
            }

            return mResult;
        }

        public string[] GetSchTwoFrequencyTotals()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Minutes:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleTwo.Elapsed.TotalSeconds);
                mResult[i + 1] = (totalSeconds / 60).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchTwoFrequencyRPM()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "RPM:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleTwo.Elapsed.TotalSeconds);
                mResult[i + 1] = (GetFrequencyCounts(mKeysF[i], ScheduleTags.Two) / (totalSeconds / 60)).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchThreeFrequencyCounts()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Count:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                mResult[i + 1] = GetFrequencyCounts(mKeysF[i], ScheduleTags.Three).ToString();
            }

            return mResult;
        }

        public string[] GetSchThreeFrequencyTotals()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "Minutes:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleThree.Elapsed.TotalSeconds);
                mResult[i + 1] = (totalSeconds / 60).ToString("0.##");
            }

            return mResult;
        }

        public string[] GetSchThreeFrequencyRPM()
        {
            string[] mResult = new string[mKeyboards.frequencyKeys.Count + 1];
            mResult[0] = "RPM:";

            for (int i = 0; i < mKeyboards.frequencyKeys.Count; i++)
            {
                double totalSeconds = Math.Floor(scheduleThree.Elapsed.TotalSeconds);
                mResult[i + 1] = (GetFrequencyCounts(mKeysF[i], ScheduleTags.Three) / (totalSeconds / 60)).ToString("0.##");
            }

            return mResult;
        }

        public void WriteResults(ISheet sheetPage, List<int[]> mLocalFreqList, Stopwatch mLocalWatch, List<double[]> mLocalDurList, MultipleSchedules mMulSchIndiv, bool isGlobal, int scheduleNumber)
        {
            IRow currRow = sheetPage.CreateRow(0);
            currRow.CreateCell(0).SetCellValue("Name:");
            currRow.CreateCell(1).SetCellValue(PatientName);

            currRow = sheetPage.CreateRow(1);
            currRow.CreateCell(0).SetCellValue("Date:");
            currRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("M/d/yyyy"));

            currRow = sheetPage.CreateRow(2);
            currRow.CreateCell(0).SetCellValue("Group:");
            currRow.CreateCell(1).SetCellValue(GroupName);

            currRow = sheetPage.CreateRow(3);
            currRow.CreateCell(0).SetCellValue("Evaluation:");
            currRow.CreateCell(1).SetCellValue(EvaluationName);

            currRow = sheetPage.CreateRow(4);
            currRow.CreateCell(0).SetCellValue("Condition:");
            currRow.CreateCell(1).SetCellValue(ConditionName);

            currRow = sheetPage.CreateRow(5);
            currRow.CreateCell(0).SetCellValue("Keyboard Used:");
            currRow.CreateCell(1).SetCellValue(KeyboardName);

            currRow = sheetPage.CreateRow(6);
            currRow.CreateCell(0).SetCellValue("Data Collector:");
            currRow.CreateCell(1).SetCellValue(CollectorName);

            currRow = sheetPage.CreateRow(7);
            currRow.CreateCell(0).SetCellValue("Collector Role:");
            currRow.CreateCell(1).SetCellValue(CollectorRole);

            currRow = sheetPage.CreateRow(8);
            currRow.CreateCell(0).SetCellValue("Session #:");
            currRow.CreateCell(1).SetCellValue(SessionCount);

            currRow = sheetPage.CreateRow(10);
            currRow.CreateCell(0).SetCellValue("Frequency Keys (rates)");

            IRow currRow1 = sheetPage.CreateRow(12);
            IRow currRow2 = sheetPage.CreateRow(13);
            IRow currRow3 = sheetPage.CreateRow(14);
            IRow currRow4 = sheetPage.CreateRow(15);

            currRow1.CreateCell(0).SetCellValue("Keys:");
            currRow2.CreateCell(0).SetCellValue("Counts");
            currRow3.CreateCell(0).SetCellValue("Time (minutes):");
            currRow4.CreateCell(0).SetCellValue("RPM (Counts/Min)");

            for (int cnt = 0; cnt < mKeyViewModel.AllFrequencies.Count; cnt++)
            {
                int freqCount = 0;

                if (scheduleNumber == 0)
                {
                    freqCount = GetFrequencyCounts(mKeysF[cnt]);
                }
                else if (scheduleNumber == 1)
                {
                    freqCount = GetFrequencyCounts(mKeysF[cnt], ScheduleTags.One);
                }
                else if (scheduleNumber == 2)
                {
                    freqCount = GetFrequencyCounts(mKeysF[cnt], ScheduleTags.Two);
                }
                else if (scheduleNumber == 3)
                {
                    freqCount = GetFrequencyCounts(mKeysF[cnt], ScheduleTags.Three);
                }

                currRow1.CreateCell(2 + cnt).SetCellValue(mKeyboards.frequencyKeys[cnt].KeyName);
                currRow2.CreateCell(2 + cnt).SetCellValue(freqCount);

                double totalSeconds = Math.Floor(mLocalWatch.Elapsed.TotalSeconds);
                currRow3.CreateCell(2 + cnt).SetCellValue((totalSeconds/60).ToString("0.##"));

                currRow4.CreateCell(2 + cnt).SetCellValue(((double)(freqCount / (totalSeconds/60))).ToString("0.##"));
            }

            currRow = sheetPage.CreateRow(17);
            currRow.CreateCell(0).SetCellValue("Frequency Keys (10s P/I)");

            currRow1 = sheetPage.CreateRow(18);
            currRow2 = sheetPage.CreateRow(19);
            currRow3 = sheetPage.CreateRow(20);
            currRow4 = sheetPage.CreateRow(21);

            currRow1.CreateCell(0).SetCellValue("Keys:");
            currRow2.CreateCell(0).SetCellValue("Interval Counts");
            currRow3.CreateCell(0).SetCellValue("Total Intervals:");
            currRow4.CreateCell(0).SetCellValue("% Intervals");

            int counts;

            for (int cnt = 0; cnt < mKeyViewModel.AllFrequencies.Count; cnt++)
            {
                currRow1.CreateCell(2 + cnt).SetCellValue(mKeyboards.frequencyKeys[cnt].KeyName);
                    counts = getIntervalCounts(cnt, mLocalFreqList);
                currRow2.CreateCell(2 + cnt).SetCellValue(counts);
                currRow3.CreateCell(2 + cnt).SetCellValue(mLocalFreqList.Count);
                currRow4.CreateCell(2 + cnt).SetCellValue(((double)((double)counts / (double)mLocalFreqList.Count) * 100).ToString("0.##"));

            }


            currRow = sheetPage.CreateRow(23);
           currRow.CreateCell(0).SetCellValue("Duration Keys (Percent of Session)");

           currRow1 = sheetPage.CreateRow(25);
           currRow2 = sheetPage.CreateRow(26);
           currRow3 = sheetPage.CreateRow(27);
           currRow4 = sheetPage.CreateRow(28);

           currRow1.CreateCell(0).SetCellValue("Keys:");
           currRow2.CreateCell(0).SetCellValue("Logged Time (s)");
           currRow3.CreateCell(0).SetCellValue("Total Time (s):");
           currRow4.CreateCell(0).SetCellValue("Percentage Session (%)");

           for (int cnt = 0; cnt < mKeyViewModel.AllDurations.Count; cnt++)
           {
                if (isGlobal)
                {
                    currRow1.CreateCell(2 + cnt).SetCellValue(mKeyboards.durationKeys[cnt].KeyName);


                    double mTime = 0.0;
                    mTime += mMultiScheds[1].mKeyModels[cnt].Timer.Elapsed.TotalSeconds;
                    mTime += mMultiScheds[2].mKeyModels[cnt].Timer.Elapsed.TotalSeconds;
                    mTime += mMultiScheds[3].mKeyModels[cnt].Timer.Elapsed.TotalSeconds;

                    currRow2.CreateCell(2 + cnt).SetCellValue((mTime/60).ToString("0.##"));

                    currRow3.CreateCell(2 + cnt).SetCellValue(stopWatch.Elapsed.TotalSeconds.ToString("0.##"));
                    currRow4.CreateCell(2 + cnt).SetCellValue(((double)((mTime / 60) / stopWatch.Elapsed.TotalSeconds) * 100).ToString("0.##"));
                }
                else
                {
                    currRow1.CreateCell(2 + cnt).SetCellValue(mKeyboards.durationKeys[cnt].KeyName);
                    currRow2.CreateCell(2 + cnt).SetCellValue(mMulSchIndiv.mKeyModels[cnt].Timer.Elapsed.TotalSeconds.ToString("0.##"));
                    currRow3.CreateCell(2 + cnt).SetCellValue(mLocalWatch.Elapsed.TotalSeconds.ToString("0.##"));
                    currRow4.CreateCell(2 + cnt).SetCellValue(((double)(mMulSchIndiv.mKeyModels[cnt].Timer.Elapsed.TotalSeconds / mLocalWatch.Elapsed.TotalSeconds) * 100).ToString("0.##"));
                }
            }
           
           currRow = sheetPage.CreateRow(30);
           currRow.CreateCell(0).SetCellValue("Duration Keys (10s P/I)");

           currRow1 = sheetPage.CreateRow(31);
           currRow2 = sheetPage.CreateRow(32);
           currRow3 = sheetPage.CreateRow(33);
           currRow4 = sheetPage.CreateRow(34);

           currRow1.CreateCell(0).SetCellValue("Keys:");
           currRow2.CreateCell(0).SetCellValue("Interval Counts");
           currRow3.CreateCell(0).SetCellValue("Session Intervals:");
           currRow4.CreateCell(0).SetCellValue("Percentage Intervals (%):");

           for (int cnt = 0; cnt < mKeyViewModel.AllDurations.Count; cnt++)
           {
                currRow1.CreateCell(2 + cnt).SetCellValue(mKeyboards.durationKeys[cnt].KeyName); 
                currRow2.CreateCell(2 + cnt).SetCellValue(mMulSchIndiv.mKeyModels[cnt].ActiveIntervals);
                currRow3.CreateCell(2 + cnt).SetCellValue(mMulSchIndiv.mKeyModels[cnt].TotalIntervals);

                int denom = mMulSchIndiv.mKeyModels[cnt].TotalIntervals;

                if (denom == 0)
                {
                    currRow4.CreateCell(2 + cnt).SetCellValue(0);

                }
                else
                {
                    double mOutput = ((double)mMulSchIndiv.mKeyModels[cnt].ActiveIntervals / (double) denom) * 100;
                    currRow4.CreateCell(2 + cnt).SetCellValue(mOutput.ToString("0.##"));
                }
           }

            sheetPage.AutoSizeColumn(0);
            sheetPage.AutoSizeColumn(1);
        }

        public int getIntervalCounts(int index, List<int[]> mList)
        {
            int counter = 0;

            foreach (int[] mArray in mList)
            {
                if (mArray[index] > 0) { counter++; }
            }

            return counter;
        }

        public int getFrequencySum(int index, List<int[]> mList)
        {
            int count = 0;

            foreach (int[] mArray in mList)
            {
                count += mArray[index];
            }

            return count;
        }

        public void decreaseTempFreqArrayFrequency(int index)
        {
            tempFreqIntervalsMain[index] = tempFreqIntervalsMain[index] - 1;

            if (scheduleOne.IsRunning)
            {
                tempFreqIntervalsSchOne[index] = tempFreqIntervalsSchOne[index] - 1;
            }
            else if (scheduleTwo.IsRunning)
            {
                tempFreqIntervalsSchTwo[index] = tempFreqIntervalsSchTwo[index] - 1;
            }
            else if (scheduleThree.IsRunning)
            {
                tempFreqIntervalsSchThree[index] = tempFreqIntervalsSchThree[index] - 1;
            }
        }

        public void increaseTempFreqArrayFrequency(int index)
        {
            tempFreqIntervalsMain[index] = tempFreqIntervalsMain[index] + 1;

            if (scheduleOne.IsRunning)
            {
                tempFreqIntervalsSchOne[index] = tempFreqIntervalsSchOne[index] + 1;
            }
            else if (scheduleTwo.IsRunning)
            {
                tempFreqIntervalsSchTwo[index] = tempFreqIntervalsSchTwo[index] + 1;
            }
            else if (scheduleThree.IsRunning)
            {
                tempFreqIntervalsSchThree[index] = tempFreqIntervalsSchThree[index] + 1;
            }
        }

        public void increaseTempDurArrayFrequency(int index)
        {
            mMultiScheds[0].mKeyModels[index].WasActive = true;

            if (scheduleOne.IsRunning)
            {
                mMultiScheds[1].mKeyModels[index].WasActive = true;
            }
            else if (scheduleTwo.IsRunning)
            {
                mMultiScheds[2].mKeyModels[index].WasActive = true;
            }
            else if (scheduleThree.IsRunning)
            {
                mMultiScheds[3].mKeyModels[index].WasActive = true;
            }
        }

        public void updateListAtIndex(int index)
        {
            mKeyViewModel.AllFrequencies[index].Counts = mKeyViewModel.AllFrequencies[index].Counts + 1;
            increaseTempFreqArrayFrequency(index);
            keyFrequency.Items.Refresh();
        }

        public void updateListAtIndex(Key oldKey, Key newKey)
        {

            int oldIndex = Array.FindIndex(mKeysF, row => row.Equals(oldKey));
            int newIndex = Array.FindIndex(mKeysF, row => row.Equals(newKey));

            mKeyViewModel.AllFrequencies[oldIndex].Counts = mKeyViewModel.AllFrequencies[oldIndex].Counts - 1;
            decreaseTempFreqArrayFrequency(oldIndex);
            keyFrequency.Items.Refresh();

            mKeyViewModel.AllFrequencies[newIndex].Counts = mKeyViewModel.AllFrequencies[newIndex].Counts + 1;
            increaseTempFreqArrayFrequency(newIndex);
            keyFrequency.Items.Refresh();

            progressBox.Items.Refresh();
        }

        public void UpdateButton()
        {
            if (!stopWatch.IsRunning)
            {
                return;
            }

            if (isReplacing)
            {
                replaceLast.Background = Brushes.Transparent;

                var brush = new SolidColorBrush(Color.FromArgb(255, (byte)116, (byte)122, (byte)121));
                replaceLast.Foreground = brush;

                isReplacing = false;
            }
            else
            {
                replaceLast.Background = Brushes.Green;
                replaceLast.Foreground = Brushes.White;

                isReplacing = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && !stopWatch.IsRunning)
            {   
                // This gets call first and never again, after starting
                dt.Tick += new EventHandler(dt_Tick);
                dt.Interval = new TimeSpan(0, 0, 0, 0, 1);

                stopWatch.Start();

                ProgressMonitor mAdd = new ProgressMonitor();
                mAdd.Code = "Schedule 1 Start";
                mAdd.Key = Key.Z.ToString();
                mAdd.KeyTag = KeyTags.Schedule;
                mAdd.ScheduleTag = ScheduleTags.One;
                mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                mAdd.KeyCode = Key.Z;
                mAdd.TimePressed = stopWatch.Elapsed;
                mListItems.Add(mAdd);

                SessionTimeTextSchedOne.Foreground = Brushes.Green;
                scheduleOne.Start();

                tenSecondIntervalWatch.Start();

                progressBox.Items.Refresh();

                dt.Start();
            }
            else if (e.Key == Key.Back)
            {
                isReplacing = false;

                if (mListItems.Count <= 0)
                    return;

                ProgressMonitor mLast = mListItems[progressBox.Items.Count - 1];

                int mIndex = Array.FindIndex(mKeysF, row => row.Equals(mLast.KeyCode));

                if (mIndex > -1)
                {
                    mKeyViewModel.AllFrequencies[mIndex].Counts = mKeyViewModel.AllFrequencies[mIndex].Counts - 1;
                    tempFreqIntervalsMain[mIndex] = tempFreqIntervalsMain[mIndex] - 1;

                    if (scheduleOne.IsRunning)
                    {
                        tempFreqIntervalsSchOne[mIndex] = tempFreqIntervalsSchOne[mIndex] - 1;
                    }
                    else if (scheduleTwo.IsRunning)
                    {
                        tempFreqIntervalsSchTwo[mIndex] = tempFreqIntervalsSchTwo[mIndex] - 1;
                    }
                    else if (scheduleThree.IsRunning)
                    {
                        tempFreqIntervalsSchThree[mIndex] = tempFreqIntervalsSchThree[mIndex] - 1;
                    }

                    keyFrequency.Items.Refresh();
                    mListItems.RemoveAt(mListItems.Count - 1);
                    progressBox.Items.Refresh();
                }

                mIndex = Array.FindIndex(mKeysD, row => row.Equals(mLast.KeyCode));

                if (mIndex > -1)
                {
                    mListItems.RemoveAt(mListItems.Count - 1);
                    progressBox.Items.Refresh();

                    // Turn off green and such
                    if (mKeyViewModel.AllDurations[mIndex].isRunning)
                    {
                        //mMultiScheds[0].mKeyModels[mIndex].Timer.Reset();

                        mMultiScheds[0].mKeyModels[mIndex].Recording = false;
                        mKeyViewModel.AllDurations[mIndex].isRunning = false;
                        //mKeyViewModel.AllDurations[mIndex].Counts = mKeyViewModel.AllDurations[mIndex].Counts - 1;

                        //currentTimes[mIndex] = String.Format("{0:00}:{1:00}.{2:00}",
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Minutes,
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Seconds,
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Milliseconds / 10);
                        //mKeyboards.durationKeys[mIndex].CountString = currentTimes[mIndex];

                        GetDurationSumTimeKey(mIndex);
                        keyDuration.Items.Refresh();
                    }
                    // Turn on green and such
                    else
                    {
                        //mMultiScheds[0].mKeyModels[mIndex].Timer.Start();

                        mMultiScheds[0].mKeyModels[mIndex].Recording = true;
                        mKeyViewModel.AllDurations[mIndex].isRunning = true;

                        //mKeyViewModel.AllDurations[mIndex].Counts = mKeyViewModel.AllDurations[mIndex].Counts + 1;

                        //currentTimes[mIndex] = String.Format("{0:00}:{1:00}.{2:00}",
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Minutes,
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Seconds,
                        //    mMultiScheds[0].mKeyModels[mIndex].TimeSpanOutput.Milliseconds / 10);
                        //mKeyboards.durationKeys[mIndex].CountString = currentTimes[mIndex];

                        GetDurationSumTimeKey(mIndex);
                        keyDuration.Items.Refresh();
                    }

                }
            }
            else if (e.Key == Key.Escape)
            {
                isReplacing = false;
                Close();
            }
            else if (e.Key == Key.Z)
            {
                isReplacing = false;

                if (scheduleOne.IsRunning)
                {
                    return;
                }

                if (scheduleTwo.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.X.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 2 End";
                    mAdd.ScheduleTag = ScheduleTags.Two;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 1 Start";
                    mAdd2.ScheduleTag = ScheduleTags.One;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);

                }
                else if (scheduleThree.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.C.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 3 End";
                    mAdd.ScheduleTag = ScheduleTags.Three;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 1 Start";
                    mAdd2.ScheduleTag = ScheduleTags.One;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);
                }

                scheduleOne.Start();
                SessionTimeTextSchedOne.Foreground = Brushes.Green;

                scheduleTwo.Stop();
                SessionTimeTextSchedTwo.Foreground = mStandardBrush;

                scheduleThree.Stop();
                SessionTimeTextSchedThree.Foreground = mStandardBrush;

                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
            else if (e.Key == Key.X)
            {
                isReplacing = false;

                if (scheduleTwo.IsRunning)
                {
                    return;
                }

                if (scheduleOne.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.X.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 1 End";
                    mAdd.ScheduleTag = ScheduleTags.One;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 2 Start";
                    mAdd2.ScheduleTag = ScheduleTags.Two;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);

                }
                else if (scheduleThree.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.C.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 3 End";
                    mAdd.ScheduleTag = ScheduleTags.Three;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 2 Start";
                    mAdd2.ScheduleTag = ScheduleTags.Two;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);
                }

                scheduleOne.Stop();
                SessionTimeTextSchedOne.Foreground = mStandardBrush;

                scheduleTwo.Start();
                SessionTimeTextSchedTwo.Foreground = Brushes.Yellow;

                scheduleThree.Stop();
                SessionTimeTextSchedThree.Foreground = mStandardBrush;

                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
            else if (e.Key == Key.C)
            {
                isReplacing = false;

                if (scheduleThree.IsRunning)
                {
                    return;
                }

                if (scheduleOne.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.X.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 1 End";
                    mAdd.ScheduleTag = ScheduleTags.One;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 3 Start";
                    mAdd2.ScheduleTag = ScheduleTags.Three;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);

                }
                else if (scheduleTwo.IsRunning)
                {
                    ProgressMonitor mAdd = new ProgressMonitor();
                    mAdd.Key = Key.C.ToString();
                    mAdd.KeyTag = KeyTags.Schedule;
                    mAdd.Code = "Schedule 2 End";
                    mAdd.ScheduleTag = ScheduleTags.Two;
                    mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd.KeyCode = e.Key;
                    mAdd.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd);

                    ProgressMonitor mAdd2 = new ProgressMonitor();
                    mAdd2.Key = Key.Z.ToString();
                    mAdd2.KeyTag = KeyTags.Schedule;
                    mAdd2.Code = "Schedule 3 Start";
                    mAdd2.ScheduleTag = ScheduleTags.Three;
                    mAdd2.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                    mAdd2.KeyCode = e.Key;
                    mAdd2.TimePressed = stopWatch.Elapsed;
                    mListItems.Add(mAdd2);
                }

                scheduleOne.Stop();
                SessionTimeTextSchedOne.Foreground = mStandardBrush;

                scheduleTwo.Stop();
                SessionTimeTextSchedTwo.Foreground = mStandardBrush;

                scheduleThree.Start();
                SessionTimeTextSchedThree.Foreground = Brushes.Red;

                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
            else if (stopWatch.IsRunning && mKeysF.Contains(e.Key))
            {

                ProgressMonitor mAdd = new ProgressMonitor();
                mAdd.Code = "Frequency";
                mAdd.Key = e.Key.ToString();
                mAdd.KeyTag = KeyTags.Frequency;

                if (scheduleOne.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.One;
                }
                else if (scheduleTwo.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.Two;
                }
                else if (scheduleThree.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.Three;
                }

                mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                mAdd.KeyCode = e.Key;
                mAdd.TimePressed = stopWatch.Elapsed;

                if (isReplacing)
                {
                    var mKeyHold = e.Key;
                    var mOldKeyHold = mListItems[mListItems.Count - 1].KeyCode;

                    mListItems[mListItems.Count - 1].Code = mAdd.Code;
                    mListItems[mListItems.Count - 1].Key = mAdd.Key;
                    mListItems[mListItems.Count - 1].KeyTag = mAdd.KeyTag;
                    mListItems[mListItems.Count - 1].KeyCode = mAdd.KeyCode;
                    mListItems[mListItems.Count - 1].TimePressed = mAdd.TimePressed;
                    mListItems[mListItems.Count - 1].ScheduleTag = mAdd.ScheduleTag;

                    Console.WriteLine("Old Key: " + mListItems[mListItems.Count - 1].KeyCode.ToString() + " New Key: " + mKeyHold.ToString());

                    updateListAtIndex(mOldKeyHold, mKeyHold);

                    UpdateButton();
                }
                else
                {
                    mListItems.Add(mAdd);
                    updateListAtIndex(Array.FindIndex(mKeysF, row => row.Equals(e.Key)));
                }

                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
            else if (stopWatch.IsRunning && mKeysD.Contains(e.Key))
            {
                int mIndex = Array.FindIndex(mKeysD, row => row.Equals(e.Key));
                increaseTempDurArrayFrequency(mIndex);

                ProgressMonitor mAdd = new ProgressMonitor();
                mAdd.Code = "Duration";
                mAdd.Key = e.Key.ToString();
                mAdd.KeyTag = KeyTags.Duration;

                if (scheduleOne.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.One;
                }
                else if (scheduleTwo.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.Two;
                }
                else if (scheduleThree.IsRunning)
                {
                    mAdd.ScheduleTag = ScheduleTags.Three;
                }

                mAdd.Time = string.Format("{0:00}:{1:00}.{2:00}", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
                mAdd.KeyCode = e.Key;
                mAdd.TimePressed = stopWatch.Elapsed;

                mListItems.Add(mAdd);

                //mListItems.Add(mAdd);

                // Updating UI

                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();

                // Updating UI END

                if (mMultiScheds[0].mKeyModels[mIndex].Recording)
                {
                    mMultiScheds[0].mKeyModels[mIndex].Recording = false;
                    mKeyViewModel.AllDurations[mIndex].isRunning = false;

                    ((Stopwatch)mMultiScheds[0].mKeyModels[mIndex].Timer).Stop();

                    mMultiScheds[1].mKeyModels[mIndex].Recording = false;
                    ((Stopwatch)mMultiScheds[1].mKeyModels[mIndex].Timer).Stop();

                    GetDurationSumTimeKey(mIndex);
                    keyDuration.Items.Refresh();
                }
                else
                {
                    mMultiScheds[0].mKeyModels[mIndex].Recording = true;
                    mKeyViewModel.AllDurations[mIndex].isRunning = true;
                    mKeyViewModel.AllDurations[mIndex].Counts += 1;

                    ((Stopwatch)mMultiScheds[0].mKeyModels[mIndex].Timer).Start();

                    mMultiScheds[1].mKeyModels[mIndex].Recording = true;
                    ((Stopwatch)mMultiScheds[1].mKeyModels[mIndex].Timer).Start();
                }

            }
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (hideTaskBar)
            {
                Button mButton = sender as Button;
                mButton.Content = "Fill Window";
                WindowState = WindowState.Normal;
                hideTaskBar = false;

                Title.FontSize /= 2;

                SessionParams00.FontSize /= 3;

                groupLabel.FontSize /= 2;
                indivLabel.FontSize /= 2;
                evalLabel.FontSize /= 2;
                condLabel.FontSize /= 2;
                collRoleLabel.FontSize /= 2;
                groupLabelText.FontSize /= 2;
                indivLabelText.FontSize /= 2;
                evalLabelText.FontSize /= 2;
                condLabelText.FontSize /= 2;
                collRoleLabelText.FontSize /= 2;

                SessionDirs01.FontSize /= 3;
                SessionDirs02.FontSize /= 2;
                SessionDirs03.FontSize /= 2;
                SessionDirs04.FontSize /= 2;
                SessionDirs05.FontSize /= 2;
                SessionDirs06.FontSize /= 2;
                SessionDirs07.FontSize /= 2;

                SessionTimeLabel.FontSize /= 2;
                SessionTimeText.FontSize /= 2;
                SessionTimeLabelSchedOne.FontSize /= 2;
                SessionTimeTextSchedOne.FontSize /= 2;
                SessionTimeLabelSchedTwo.FontSize /= 2;
                SessionTimeTextSchedTwo.FontSize /= 2;
                SessionTimeLabelSchedThree.FontSize /= 2;
                SessionTimeTextSchedThree.FontSize /= 2;

                progressBox.FontSize /= 2;
                keyFrequency.FontSize /= 2;
                keyDuration.FontSize /= 2;

                ResizeListViews();
                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();

            }
            else
            {
                Button mButton = sender as Button;
                mButton.Content = "Shrink Window";
                WindowState = WindowState.Maximized;
                hideTaskBar = true;

                Title.FontSize *= 2;

                SessionParams00.FontSize *= 3;

                groupLabel.FontSize *= 2;
                indivLabel.FontSize *= 2;
                evalLabel.FontSize *= 2;
                condLabel.FontSize *= 2;
                collRoleLabel.FontSize *= 2;
                groupLabelText.FontSize *= 2;
                indivLabelText.FontSize *= 2;
                evalLabelText.FontSize *= 2;
                condLabelText.FontSize *= 2;
                collRoleLabelText.FontSize *= 2;

                SessionDirs01.FontSize *= 3;
                SessionDirs02.FontSize *= 2;
                SessionDirs03.FontSize *= 2;
                SessionDirs04.FontSize *= 2;
                SessionDirs05.FontSize *= 2;
                SessionDirs06.FontSize *= 2;
                SessionDirs07.FontSize *= 2;

                SessionTimeLabel.FontSize *= 2;
                SessionTimeText.FontSize *= 2;
                SessionTimeLabelSchedOne.FontSize *= 2;
                SessionTimeTextSchedOne.FontSize *= 2;
                SessionTimeLabelSchedTwo.FontSize *= 2;
                SessionTimeTextSchedTwo.FontSize *= 2;
                SessionTimeLabelSchedThree.FontSize *= 2;
                SessionTimeTextSchedThree.FontSize *= 2;

                progressBox.FontSize *= 2;
                keyFrequency.FontSize *= 2;
                keyDuration.FontSize *= 2;

                ResizeListViews();
                Decorator border = VisualTreeHelper.GetChild(progressBox, 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
        }

        public void ResizeListViews()
        {
            double remainingSpace = progressBox.ActualWidth;

            if (remainingSpace > 0)
            {
                (progressBox.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 3);
                (progressBox.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 3);
                (progressBox.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 3);
            }

            remainingSpace = keyDuration.ActualWidth;

            if (remainingSpace > 0)
            {
                (keyDuration.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 4);
                (keyDuration.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 4);
                (keyDuration.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 4);
                (keyDuration.View as GridView).Columns[3].Width = Math.Ceiling(remainingSpace / 4);
            }

            remainingSpace = keyFrequency.ActualWidth;

            if (remainingSpace > 0)
            {
                (keyFrequency.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace / 3);
                (keyFrequency.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace / 3);
                (keyFrequency.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace / 3);
            }

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeListViews();
        }

        class KeyboardDualListViewModel : ViewModelBase
        {
            public ObservableCollection<KeyDefinitions> _durations;

            public ObservableCollection<KeyDefinitions> AllDurations
            {
                get { return _durations; }
                set
                {
                    _durations = value;
                    OnPropertyChanged("AllKeyboards");
                }
            }

            public ObservableCollection<KeyDefinitions> _frequencies;

            public ObservableCollection<KeyDefinitions> AllFrequencies
            {
                get { return _frequencies; }
                set
                {
                    _frequencies = value;
                    OnPropertyChanged("AllFrequencies");
                }
            }

            public void IncrementFrequency(int index)
            {

            }

            public KeyboardDualListViewModel()
            {
                if (_durations == null)
                    _durations = new ObservableCollection<KeyDefinitions>();
                if (_frequencies == null)
                    _frequencies = new ObservableCollection<KeyDefinitions>();
            }
        }
    }
}
