using DataTracker.Model;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace DataTracker.ViewModel
{
    static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

    class ReliabilityViewModel : ViewModelBase
    {
        public RelayCommand Initialize { get; set; }
        public RelayCommand RunReliabilityCommand { get; set; }
        public RelayCommand SelectAll { get; set; }
        public RelayCommand UnselectAll { get; set; }

        ObservableCollection<Group> _groups;
        public ObservableCollection<Group> AllGroups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged("AllGroups");
            }
        }

        Group _selectedGroup;
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (value == null)
                    return;

                _selectedGroup = value;
                UpdateIndividuals();
                OnPropertyChanged("SelectedGroup");

            }
        }

        ObservableCollection<Individual> _individuals;
        public ObservableCollection<Individual> AllIndividuals
        {
            get { return _individuals; }
            set
            {
                _individuals = value;
                OnPropertyChanged("AllIndividuals");
            }
        }

        Individual _selectedIndividual;
        public Individual SelectedIndividual
        {
            get { return _selectedIndividual; }
            set
            {
                if (value == null)
                    return;

                _selectedIndividual = value;
                UpdateEvaluations();
                OnPropertyChanged("SelectedIndividual");

            }
        }

        ObservableCollection<Evaluation> _evaluations;
        public ObservableCollection<Evaluation> AllEvaluations
        {
            get { return _evaluations; }
            set
            {
                _evaluations = value;
                OnPropertyChanged("AllEvaluations");
            }
        }

        Evaluation _selectedEvaluation;
        public Evaluation SelectedEvaluation
        {
            get { return _selectedEvaluation; }
            set
            {
                if (value == null)
                    return;

                _selectedEvaluation = value;
                OnPropertyChanged("SelectedEvaluation");
                RunReliability();
            }
        }

        ObservableCollection<ReliabilityIndex> _reliIndices;
        public ObservableCollection<ReliabilityIndex> AllReliabilityIndices
        {
            get { return _reliIndices; }
            set
            {
                _reliIndices = value;
                OnPropertyChanged("AllReliabilityIndices");
            }
        }

        public static XSSFWorkbook hssfworkbook;
        public static List<FileIndexClass> mPrimaryList = new List<FileIndexClass>();
        public static List<FileIndexClass> mReliList = new List<FileIndexClass>();

        public ReliabilityViewModel()
        {
            this.Initialize = new RelayCommand(param => UpdateGroups(), param => true);
            this.RunReliabilityCommand = new RelayCommand(param => CalculateReliability(), param => true);
            this.SelectAll = new RelayCommand(param => SelectingAll(), param => true);
            this.UnselectAll = new RelayCommand(param => DeselectingAll(), param => true);

            _groups = new ObservableCollection<Group>();
            AllGroups = new ObservableCollection<Group>();

            _selectedGroup = new Group();
            SelectedGroup = new Group();

            _individuals = new ObservableCollection<Individual>();
            AllIndividuals = new ObservableCollection<Individual>();

            _selectedIndividual = new Individual();
            SelectedIndividual = new Individual();

            _evaluations = new ObservableCollection<Evaluation>();
            AllEvaluations = new ObservableCollection<Evaluation>();

            _selectedEvaluation = new Evaluation();
            SelectedEvaluation = new Evaluation();

            _reliIndices = new ObservableCollection<ReliabilityIndex>();
            AllReliabilityIndices = new ObservableCollection<ReliabilityIndex>();

        }

        void SelectingAll()
        {
            foreach (ReliabilityIndex index in _reliIndices)
            {
                index.IsSelected = true;
            }
        }

        void DeselectingAll()
        {
            foreach (ReliabilityIndex index in _reliIndices)
            {
                index.IsSelected = false;
            }
        }

        public void UpdateGroups()
        {
            var mPath = Path.Combine(Properties.Settings.Default.SaveLocation);

            AllGroups.Clear();

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(mPath);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllGroups.Add(Group.CreateGroup(group[group.Length - 1]));
                }
            }
            catch
            {

            }
        }

        public void UpdateIndividuals()
        {
            if (AllIndividuals == null)
                return;

            var mPath = Path.Combine(Properties.Settings.Default.SaveLocation, SelectedGroup.GroupName);

            AllIndividuals.Clear();
            AllEvaluations.Clear();
            AllReliabilityIndices.Clear();

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(mPath);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    string[] group = subdirectory.Split('\\');
                    AllIndividuals.Add(Individual.CreateIndividual(group[group.Length - 1]));
                }
            }
            catch
            { }
        }

        public void UpdateEvaluations()
        {

            if (AllIndividuals == null || AllEvaluations == null)
                return;

            var mPath = Path.Combine(Properties.Settings.Default.SaveLocation, SelectedGroup.GroupName, SelectedIndividual.IndividualName);

            AllEvaluations.Clear();

            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(mPath);
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

        public void RunReliability()
        {

            if (SelectedGroup.GroupName == null || SelectedIndividual.IndividualName == null || SelectedEvaluation.EvaluationName == null)
                return;

            if (SelectedGroup.GroupName.Length > 0 && SelectedIndividual.IndividualName.Length > 0 && SelectedEvaluation.EvaluationName.Length > 0)
            {
                AllReliabilityIndices.Clear();

                Path.Combine(Properties.Settings.Default.SaveLocation, SelectedGroup.GroupName);
//                var targetDirectory = Properties.Settings.Default.SaveLocation + "\\" + SelectedGroup.GroupName + "\\" + SelectedIndividual.IndividualName + "\\" + SelectedEvaluation.EvaluationName + "\\";
                var targetDirectory = Path.Combine(Properties.Settings.Default.SaveLocation, SelectedGroup.GroupName, SelectedIndividual.IndividualName, SelectedEvaluation.EvaluationName);

                DirectoryInfo di = new DirectoryInfo(@targetDirectory);

                mPrimaryList.Clear();
                mReliList.Clear();

                foreach (var fi in di.GetFiles("*Primary.xlsx", SearchOption.AllDirectories))
                {
                    InitializeWorkbook(fi.FullName, mPrimaryList);
                }

                foreach (var fi in di.GetFiles("*Reliability.xlsx", SearchOption.AllDirectories))
                {
                    InitializeWorkbook(fi.FullName, mReliList);
                }

                if (mReliList.Count < 1)
                {
                    MessageBox.Show("There have not been any collectors for reliability!");
                }

                foreach(FileIndexClass primary in mPrimaryList)
                {

                    foreach(FileIndexClass reli in mReliList)
                    {
                        if (primary.SessionNumber == reli.SessionNumber && primary.Condition == reli.Condition)
                        {

                            //TODO
                            //Aggreements based on Dur and Freq Measures
                            AllReliabilityIndices.Add(new ReliabilityIndex
                            {
                                TitleName = "Session #" + primary.SessionNumber + " - Condition: " + primary.Condition + " (Reli)",
                                PatientName = primary.PatientName,
                                PatientGroup = primary.PatientGroup,
                                DateCollected = primary.DateCollected,
                                DataCollector = primary.DataCollector,
                                Evaluation = primary.Evaluation,
                                Keyboard = primary.Keyboard,
                                Session = primary.SessionNumber,
                                Condition = primary.Condition,
                                Role = primary.Role,
                                Rows = primary.Rows,
                                FrequencyKeys = primary.FrequencyKeys,
                                DurationKeys = primary.DurationKeys,
                                DurationEntries = primary.DurationEntries,
                                FrequencyTags = primary.FrequencyTags,
                                FrequencyValues = primary.FrequencyValues,
                                DurationValues = primary.DurationValues,

                                ReliCollector = reli.DataCollector,
                                ReliFrequencyValues = reli.FrequencyValues,
                                ReliDurationValues = reli.DurationValues,
                                HasReli = true
                            });
                        }
                        else
                        {
                            AllReliabilityIndices.Add(new ReliabilityIndex
                            {
                                TitleName = "Session #" + primary.SessionNumber + " - Condition: " + primary.Condition + "",
                                PatientName = primary.PatientName,
                                PatientGroup = primary.PatientGroup,
                                DateCollected = primary.DateCollected,
                                DataCollector = primary.DataCollector,
                                Evaluation = primary.Evaluation,
                                Keyboard = primary.Keyboard,
                                Session = primary.SessionNumber,
                                Condition = primary.Condition,
                                Role = primary.Role,
                                Rows = primary.Rows,
                                FrequencyKeys = primary.FrequencyKeys,
                                DurationKeys = primary.DurationKeys,
                                DurationEntries = primary.DurationEntries,
                                FrequencyTags = primary.FrequencyTags,
                                FrequencyValues = primary.FrequencyValues,
                                DurationValues = primary.DurationValues,
                                HasReli = false
                            });
                        }

                        AllReliabilityIndices.Sort();
                    }
                }

            }
        }

        public void CalculateReliability()
        {
            if (AllReliabilityIndices.Count < 1)
                return;

            XSSFWorkbook hssfworkbook = new XSSFWorkbook();
            ISheet page = hssfworkbook.CreateSheet("Reliability");

            ReliabilityIndex mBase = AllReliabilityIndices[0];

            IRow currRow = page.CreateRow(0);
            currRow.CreateCell(0).SetCellValue("Name:");
            currRow.CreateCell(1).SetCellValue(mBase.PatientName);

            currRow = page.CreateRow(1);
            currRow.CreateCell(0).SetCellValue("Evaluation:");
            currRow.CreateCell(1).SetCellValue(mBase.Evaluation);

            //Skip row

            currRow = page.CreateRow(3);
            currRow.CreateCell(0).SetCellValue("Session:");
            currRow.CreateCell(1).SetCellValue("Date:");
            currRow.CreateCell(2).SetCellValue("Condition");
            currRow.CreateCell(3).SetCellValue("Has Reli?");
            currRow.CreateCell(4).SetCellValue("Primary");
            currRow.CreateCell(5).SetCellValue("Reliability");

            int currentRowNumber = 5;
            string temp = string.Empty;
            FileIndexClass mMain;

            IRow keyTitles = page.GetRow(3);
            page.CreateRow(4);
            IRow reliTitles = page.CreateRow(5);

            bool firstIndex = true;
            int rowSpacer = 6;
            //int titleSpacer;

            foreach (ReliabilityIndex index in AllReliabilityIndices)
            {
                if (firstIndex)
                {
                    mMain = GetFromListBySession(index.Session, mPrimaryList);
                    rowSpacer = 6;

                    List<string> keyList = new List<string>(mMain.FrequencyValues.Keys);

                    for (int fCount = 0; fCount < keyList.Count; fCount++)
                    {
                        currRow = page.CreateRow(4);
                        keyTitles.CreateCell(rowSpacer).SetCellValue(keyList[fCount]);

                        rowSpacer = rowSpacer + 6;
                    }

                    List<string> dKeyList = new List<string>(mMain.DurationValues.Keys);

                    for (int dCount = 0; dCount < dKeyList.Count; dCount++)
                    {
                        currRow = page.CreateRow(4);
                        keyTitles.CreateCell(rowSpacer).SetCellValue(dKeyList[dCount]);

                        rowSpacer = rowSpacer + 6;
                    }

                    firstIndex = false;
                }

                if (index.IsSelected)
                {
                    currRow = page.CreateRow(currentRowNumber);
                    currRow.CreateCell(0).SetCellValue(index.Session);
                    currRow.CreateCell(1).SetCellValue(index.DateCollected);
                    currRow.CreateCell(2).SetCellValue(index.Condition);
                    currRow.CreateCell(3).SetCellValue(index.HasReli ? "yes" : "no");
                    currRow.CreateCell(4).SetCellValue(index.DataCollector);
                    currRow.CreateCell(5).SetCellValue(index.ReliCollector);

                    if (index.HasReli)
                    {

                        mMain = GetFromListBySession(index.Session, mPrimaryList);
                        rowSpacer = 6;

                        mMain = GetFromListBySession(index.Session, mPrimaryList);
                        List<string> fKeyList = new List<string>(mMain.FrequencyValues.Keys);

                        for (int fCount = 0; fCount < fKeyList.Count; fCount++)
                        {
                            if (index.FrequencyValues.ContainsKey(fKeyList[fCount]) && index.ReliFrequencyValues.ContainsKey(fKeyList[fCount]))
                            {
                                int[] mFreqHolderP = index.FrequencyValues[fKeyList[fCount]];
                                int[] mFreqHolderR = index.ReliFrequencyValues[fKeyList[fCount]];

                                currRow.CreateCell(rowSpacer).SetCellValue(getFreqEIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 1).SetCellValue(getFreqPIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 2).SetCellValue(getFreqTIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 3).SetCellValue(getFreqOIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 4).SetCellValue(getFreqNIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 5).SetCellValue(getFreqPMA(mFreqHolderP, mFreqHolderR));

                                page.GetRow(4).CreateCell(rowSpacer).SetCellValue("EIA");
                                page.GetRow(4).CreateCell(rowSpacer + 1).SetCellValue("PIA");
                                page.GetRow(4).CreateCell(rowSpacer + 2).SetCellValue("TIA");
                                page.GetRow(4).CreateCell(rowSpacer + 3).SetCellValue("OIA");
                                page.GetRow(4).CreateCell(rowSpacer + 4).SetCellValue("NIA");
                                page.GetRow(4).CreateCell(rowSpacer + 5).SetCellValue("PMA");

                                rowSpacer = rowSpacer + 6;
                            }
                        }


                        List<string> dKeyList = new List<string>(mMain.DurationValues.Keys);

                        for (int dCount = 0; dCount < dKeyList.Count; dCount++)
                        {
                            if (index.DurationValues.ContainsKey(dKeyList[dCount]) && index.ReliDurationValues.ContainsKey(dKeyList[dCount]))
                            {
                                double[] mFreqHolderP = index.DurationValues[dKeyList[dCount]];
                                double[] mFreqHolderR = index.ReliDurationValues[dKeyList[dCount]];

                                currRow.CreateCell(rowSpacer).SetCellValue(getDurationEIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 1).SetCellValue(getDurationPIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 2).SetCellValue(getDurationTIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 3).SetCellValue(getDurationOIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 4).SetCellValue(getDurationNIA(mFreqHolderP, mFreqHolderR));
                                currRow.CreateCell(rowSpacer + 5).SetCellValue(getDurationPMA(mFreqHolderP, mFreqHolderR));

                                page.GetRow(4).CreateCell(rowSpacer).SetCellValue("EIA");
                                page.GetRow(4).CreateCell(rowSpacer + 1).SetCellValue("PIA");
                                page.GetRow(4).CreateCell(rowSpacer + 2).SetCellValue("TIA");
                                page.GetRow(4).CreateCell(rowSpacer + 3).SetCellValue("OIA");
                                page.GetRow(4).CreateCell(rowSpacer + 4).SetCellValue("NIA");
                                page.GetRow(4).CreateCell(rowSpacer + 5).SetCellValue("PMA");
                            }

                            rowSpacer = rowSpacer + 6;
                        }

                    }

                    currentRowNumber = currentRowNumber + 1;
                }
            }

            page.AutoSizeColumn(0);
            page.AutoSizeColumn(1);
            page.AutoSizeColumn(2);

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Reliabilty-Output"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel File (.xlsx)|*.xlsx"; // Filter files by extension


            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName;
                try
                {
                    using (FileStream file = new FileStream(filename, FileMode.Create))
                    {
                        hssfworkbook.Write(file);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }

        public static double getFreqEIA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] == mSecondary[i])
                {
                    sum += 1.0;
                }
                count++;
            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationEIA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (Convert.ToInt32(mPrimary[i]) == Convert.ToInt32(mSecondary[i]))
                {
                    sum += 1.0;
                }
                count++;
            }

            return (sum / (((double)count))) * 100;
        }

        public static double getFreqPIA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            double higher, lower;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] == mSecondary[i])
                {
                    sum += 1.0;
                }
                else
                {
                    higher = (mPrimary[i] > mSecondary[i]) ? (double)mPrimary[i] : (double)mSecondary[i];
                    lower = (mPrimary[i] > mSecondary[i]) ? (double)mSecondary[i] : (double)mPrimary[i];

                    sum += (lower / higher);
                }
                count++;
            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationPIA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            double higher, lower;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (Convert.ToInt32(mPrimary[i]) == Convert.ToInt32(mSecondary[i]))
                {
                    sum += 1.0;
                }
                else
                {
                    higher = (mPrimary[i] > mSecondary[i]) ? mPrimary[i] : mSecondary[i];
                    lower = (mPrimary[i] > mSecondary[i]) ? mSecondary[i] : mPrimary[i];

                    sum += (lower / higher);
                }
                count++;
            }

            return (sum / (((double)count))) * 100;
        }

        public static double getFreqTIA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] == 0 && mSecondary[i] == 0)
                {
                    sum += 1.0;
                }
                else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                {
                    sum += 1.0;
                }

                count++;
            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationTIA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (Convert.ToInt32(mPrimary[i]) == 0 && Convert.ToInt32(mSecondary[i]) == 0)
                {
                    sum += 1.0;
                }
                else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                {
                    sum += 1.0;
                }

                count++;
            }

            return (sum / (((double)count))) * 100;
        }

        public static double getFreqOIA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] > 0 || mSecondary[i] > 0)
                {
                    if (mPrimary[i] == 0 && mSecondary[i] == 0)
                    {
                        sum += 1.0;
                    }
                    else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                    {
                        sum += 1.0;
                    }
                    count++;
                }

            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationOIA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] > 0 || mSecondary[i] > 0)
                {
                    if (Convert.ToInt32(mPrimary[i]) == 0 && Convert.ToInt32(mSecondary[i]) == 0)
                    {
                        sum += 1.0;
                    }
                    else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                    {
                        sum += 1.0;
                    }
                    count++;
                }

            }

            return (sum / (((double)count))) * 100;
        }

        public static double getFreqNIA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] < 1 || mSecondary[i] < 1)
                {
                    if (mPrimary[i] == 0 && mSecondary[i] == 0)
                    {
                        sum += 1.0;
                    }
                    else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                    {
                        sum += 1.0;
                    }
                    count++;
                }

            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationNIA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0;
            double sum = 0.0;

            for (int i = 0; i < runLength; i++)
            {
                if (mPrimary[i] < 1 || mSecondary[i] < 1)
                {
                    if (Convert.ToInt32(mPrimary[i]) == 0 && Convert.ToInt32(mSecondary[i]) == 0)
                    {
                        sum += 1.0;
                    }
                    else if (mPrimary[i] > 0 && mSecondary[i] > 0)
                    {
                        sum += 1.0;
                    }
                    count++;
                }

            }

            return (sum / (((double)count))) * 100;
        }

        public static double getFreqPMA(int[] mPrimary, int[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ?
                mPrimary.Length :
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0, innerCount = 0;
            double sum = 0.0, innerPrim = 0.0, innerReli = 0.0;
            double higher, lower;


            for (int i = 0; i < runLength; i++)
            {
                innerPrim += mPrimary[i];
                innerReli += mSecondary[i];
                innerCount++;
                if ((innerCount+1) % 6 == 0)
                {
                    if (innerPrim == innerReli)
                    {
                        sum += 1.0;
                    }
                    else
                    {
                        higher = (innerPrim > innerReli) ? (double)innerPrim : (double)innerReli;
                        lower = (innerPrim > innerReli) ? (double)innerReli : (double)innerPrim;

                        sum += (lower / higher);
                    }

                    innerPrim = innerReli = 0;
                    count++;
                }
            }

            return (sum / (((double)count)))*100;
        }

        public static double getDurationPMA(double[] mPrimary, double[] mSecondary)
        {
            if (mPrimary.Length == 0 || mSecondary.Length == 0)
                return -1;

            int runLength = (mPrimary.Length == mSecondary.Length) ? 
                mPrimary.Length : 
                (mPrimary.Length > mSecondary.Length) ? mSecondary.Length : mPrimary.Length;

            int count = 0, innerCount = 0;
            double sum = 0.0;
            int innerPrim = 0, innerReli = 0;
            double higher, lower;


            for (int i = 0; i < runLength; i++)
            {
                innerPrim += Convert.ToInt32(mPrimary[i]);
                innerReli += Convert.ToInt32(mSecondary[i]);
                innerCount++;
                if ((innerCount + 1) % 6 == 0)
                {
                    if (innerPrim == innerReli)
                    {
                        sum += 1.0;
                    }
                    else
                    {
                        higher = (innerPrim > innerReli) ? (double)innerPrim : (double)innerReli;
                        lower = (innerPrim > innerReli) ? (double)innerReli : (double)innerPrim;

                        sum += (lower / higher);
                    }

                    innerPrim = innerReli = 0;
                    count++;
                }
            }

            return (sum / (((double)count))) * 100;
        }

        public static FileIndexClass GetFromListBySession(int index, List<FileIndexClass> mList)
        {
            foreach (FileIndexClass mIndex in mList)
            {
                if (mIndex.SessionNumber == index)
                    return mIndex;
            }

            return null;
        }

        public class ReliabilityIndex : IComparable, INotifyPropertyChanged
        {
            public int CompareTo(object o)
            {
                ReliabilityIndex a = this;
                ReliabilityIndex b = (ReliabilityIndex)o;
                return a.Session.CompareTo(b.Session);
            }

            public string TitleName { get; set; }
            public string PatientName { get; set; }
            public string PatientGroup { get; set; }
            public string DateCollected { get; set; }
            public string DataCollector { get; set; }
            public string Evaluation { get; set; }
            public string Keyboard { get; set; }

            public int Session { get; set; }
            public string Condition { get; set; }
            public string Role { get; set; }
            public int Rows { get; set; }
            public int FrequencyKeys { get; set; }
            public int DurationKeys { get; set; }
            public int[][] FrequencyEntries { get; set; }
            public double[][] DurationEntries { get; set; }
            public string[] FrequencyTags { get; set; }

            public Dictionary<string, double[]> DurationValues { get; set; }
            public Dictionary<string, int[]> FrequencyValues { get; set; }

            public string ReliCollector { get; set; }
            public Dictionary<string, double[]> ReliDurationValues { get; set; }
            public Dictionary<string, int[]> ReliFrequencyValues { get; set; }

            public float ReliMeasure { get; set; }

            public bool Primary { get; set; }

            private bool _isSelected;

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }

            public bool HasReli { get; set; }
        }

        public class FileIndexClass
        {
            public FileIndexClass()
            {

            }

            public static FileIndexClass CreateFileIndexClass(string _dataCollector, int _sessionNumber, string _condition, string _role, int _durKeys, int _freKeys, int _rows)
            {
                return new FileIndexClass { DataCollector = _dataCollector, SessionNumber = _sessionNumber, Condition = _condition, Role = _role, Rows = _rows, DurationKeys = _durKeys, FrequencyKeys = _freKeys };
            }

            public string PatientName { get; set; }
            public string PatientGroup { get; set; }
            public string DateCollected { get; set; }
            public string DataCollector { get; set; }
            public string Evaluation { get; set; }
            public string Keyboard { get; set; }
            public int SessionNumber { get; set; }
            public string Condition { get; set; }
            public string Role { get; set; }
            public int Rows { get; set; }
            public int FrequencyKeys { get; set; }
            public int DurationKeys { get; set; }
            public double[][] DurationEntries { get; set; }
            public string[] FrequencyTags { get; set; }
            public Dictionary<string, double[]> DurationValues { get; set; }
            public Dictionary<string, int[]> FrequencyValues { get; set; }

            public Dictionary<string, double[]> DurationValuesReli { get; set; }
            public Dictionary<string, int[]> FrequencyValuesReli { get; set; }
        }
        
        static void InitializeWorkbook(string path, List<FileIndexClass> localList)
        {
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                    ISheet worksheet = hssfworkbook.GetSheet("Cover Page");
                    ISheet dursheet = hssfworkbook.GetSheet("DurationIntervals");
                    ISheet fresheet = hssfworkbook.GetSheet("FrequencyIntervals");

                    int lastRowNumber = dursheet.LastRowNum;
                    int durKeys = dursheet.GetRow(0).LastCellNum;

                    double[][] DurationMatrix = new double[durKeys][];

                    Dictionary<string, double[]> durDictionary = new Dictionary<string, double[]>();

                    for (int i = 0; i < durKeys; i++)
                    {
                        DurationMatrix[i] = new double[lastRowNumber];

                        double[] temp = new double[lastRowNumber];

                        for (int j = 0; j < lastRowNumber; j++)
                        {
                            DurationMatrix[i][j] = double.Parse(dursheet.GetRow(j + 1).GetCell(i).ToString());
                            temp[j] = double.Parse(dursheet.GetRow(j + 1).GetCell(i).ToString());
                        }

                        durDictionary.Add(dursheet.GetRow(0).GetCell(i).ToString(), temp);
                    }

                    int freKeys = fresheet.GetRow(0).LastCellNum;
                    string[] freTags = new string[freKeys];

                    Dictionary<string, int[]> freqDictionary = new Dictionary<string, int[]>();
                
                    for (int i = 0; i < durKeys; i++)
                    {
                        freTags[i] = fresheet.GetRow(0).GetCell(i).ToString().Trim();
                        int[] freqTemp = new int[lastRowNumber];
                        int mValue = -1;

                        for (int j = 0; j < lastRowNumber; j++)
                        {
                            mValue = -1;
                            int.TryParse(fresheet.GetRow(j + 1).GetCell(i).ToString().Trim(), out mValue);
                            freqTemp[j] = mValue;
                        }

                        freqDictionary.Add(fresheet.GetRow(0).GetCell(i).ToString(), freqTemp);
                    }

                    FileIndexClass mFile = new FileIndexClass();
                    mFile.PatientName = worksheet.GetRow(0).GetCell(1).ToString();
                    mFile.DateCollected = worksheet.GetRow(1).GetCell(1).ToString();
                    mFile.PatientGroup = worksheet.GetRow(2).GetCell(1).ToString();
                    mFile.Evaluation = worksheet.GetRow(3).GetCell(1).ToString();
                    mFile.Condition = worksheet.GetRow(4).GetCell(1).ToString();
                    mFile.Keyboard = worksheet.GetRow(5).GetCell(1).ToString();
                    mFile.DataCollector = worksheet.GetRow(6).GetCell(1).ToString();
                    mFile.Role = worksheet.GetRow(7).GetCell(1).ToString();
                    mFile.SessionNumber = int.Parse(worksheet.GetRow(8).GetCell(1).ToString());

                    mFile.Rows = lastRowNumber;
                    mFile.DurationKeys = durKeys;
                    mFile.FrequencyKeys = freKeys;
                    mFile.FrequencyTags = freTags;
                    mFile.DurationEntries = DurationMatrix;
                    mFile.DurationValues = durDictionary;
                    mFile.FrequencyValues = freqDictionary;

                    localList.Add(mFile);

                }
        }
    }
}
