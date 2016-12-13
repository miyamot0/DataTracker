using System;
using System.Data;
using System.Windows;

namespace DataTracker.View
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        static DataTable mainTable = new DataTable();
        static DataTable tableOne = new DataTable();
        static DataTable tableTwo = new DataTable();
        static DataTable tableThree = new DataTable();

        static DataTable mainTableDuration = new DataTable();
        static DataTable tableOneDuration = new DataTable();
        static DataTable tableTwoDuration = new DataTable();
        static DataTable tableThreeDuration = new DataTable();

        public bool SaveData { get; set; }

        public string[] mFrequencyColumns { get; set; }
        public string[] mDurationColumns { get; set; }

        public string[] mainFreqCounts { get; set; }
        public string[] mainFreqMinutes { get; set; }
        public string[] mainFreqRPM { get; set; }

        public string[] mainDurCounts { get; set; }
        public string[] mainDurMinutes { get; set; }
        public string[] mainDurPercent { get; set; }

        public string[] schOneFreqCounts { get; set; }
        public string[] schOneFreqMinutes { get; set; }
        public string[] schOneFreqRPM { get; set; }

        public string[] schOneDurCounts { get; set; }
        public string[] schOneDurMinutes { get; set; }
        public string[] schOneDurPercent { get; set; }

        public string[] schTwoFreqCounts { get; set; }
        public string[] schTwoFreqMinutes { get; set; }
        public string[] schTwoFreqRPM { get; set; }

        public string[] schTwoDurCounts { get; set; }
        public string[] schTwoDurMinutes { get; set; }
        public string[] schTwoDurPercent { get; set; }

        public string[] schThreeFreqCounts { get; set; }
        public string[] schThreeFreqMinutes { get; set; }
        public string[] schThreeFreqRPM { get; set; }

        public string[] schThreeDurCounts { get; set; }
        public string[] schThreeDurMinutes { get; set; }
        public string[] schThreeDurPercent { get; set; }

        public ResultsWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            mainTable.Rows.Clear();
            tableOne.Rows.Clear();
            tableTwo.Rows.Clear();
            tableThree.Rows.Clear();
            mainTableDuration.Rows.Clear();
            tableOneDuration.Rows.Clear();
            tableTwoDuration.Rows.Clear();
            tableThreeDuration.Rows.Clear();

            mainTable.Columns.Clear();
            tableOne.Columns.Clear();
            tableTwo.Columns.Clear();
            tableThree.Columns.Clear();
            mainTableDuration.Columns.Clear();
            tableOneDuration.Columns.Clear();
            tableTwoDuration.Columns.Clear();
            tableThreeDuration.Columns.Clear();

            // Initial setup
            AddColumnsToTable(mainTable, mFrequencyColumns);
            AddColumnsToTable(tableOne, mFrequencyColumns);
            AddColumnsToTable(tableTwo, mFrequencyColumns);
            AddColumnsToTable(tableThree, mFrequencyColumns);

            AddColumnsToTable(mainTableDuration, mDurationColumns);
            AddColumnsToTable(tableOneDuration, mDurationColumns);
            AddColumnsToTable(tableTwoDuration, mDurationColumns);
            AddColumnsToTable(tableThreeDuration, mDurationColumns);
            // END columns

            // Initialize rows
            AddRowsToFrequencyTable(mainTable, mainFreqCounts);
            AddRowsToFrequencyTable(mainTable, mainFreqMinutes);
            AddRowsToFrequencyTable(mainTable, mainFreqRPM);

            AddRowsToFrequencyTable(tableOne, schOneFreqCounts);
            AddRowsToFrequencyTable(tableOne, schOneFreqMinutes);
            AddRowsToFrequencyTable(tableOne, schOneFreqRPM);

            AddRowsToFrequencyTable(tableTwo, schTwoFreqCounts);
            AddRowsToFrequencyTable(tableTwo, schTwoFreqMinutes);
            AddRowsToFrequencyTable(tableTwo, schTwoFreqRPM);

            AddRowsToFrequencyTable(tableThree, schThreeFreqCounts);
            AddRowsToFrequencyTable(tableThree, schThreeFreqMinutes);
            AddRowsToFrequencyTable(tableThree, schThreeFreqRPM);

            AddRowsToDurationTable(mainTableDuration, mainDurCounts);
            AddRowsToDurationTable(mainTableDuration, mainDurMinutes);
            AddRowsToDurationTable(mainTableDuration, mainDurPercent);

            AddRowsToDurationTable(tableOneDuration, schOneDurCounts);
            AddRowsToDurationTable(tableOneDuration, schOneDurMinutes);
            AddRowsToDurationTable(tableOneDuration, schOneDurPercent);

            AddRowsToDurationTable(tableTwoDuration, schTwoDurCounts);
            AddRowsToDurationTable(tableTwoDuration, schTwoDurMinutes);
            AddRowsToDurationTable(tableTwoDuration, schTwoDurPercent);

            AddRowsToDurationTable(tableThreeDuration, schThreeDurCounts);
            AddRowsToDurationTable(tableThreeDuration, schThreeDurMinutes);
            AddRowsToDurationTable(tableThreeDuration, schThreeDurPercent);
            // END rows


            // Initialize binds
            topDataGrid.DataContext = mainTable.DefaultView;            

            dataGridSchedOne.DataContext = tableOne.DefaultView;
            dataGridSchedTwo.DataContext = tableTwo.DefaultView;
            dataGridSchedThree.DataContext = tableThree.DefaultView;

            topDataGridDuration.DataContext = mainTableDuration.DefaultView;
            dataGridSchedOneDuration.DataContext = tableOneDuration.DefaultView;
            dataGridSchedTwoDuration.DataContext = tableTwoDuration.DefaultView;
            dataGridSchedThreeDuration.DataContext = tableThreeDuration.DefaultView;
            // END bindings

            // Display logic
        }

        public void AddColumnsToTable(DataTable mTable, string[] mCols)
        {
            foreach (string mCol in mCols)
            {
                if (!mTable.Columns.Contains(mCol))
                {
                    try
                    {
                        mTable.Columns.Add(mCol);
                    }
                    catch (DuplicateNameException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        public void AddRowsToFrequencyTable(DataTable mTable, string[] mRowData)
        {
            DataRow dr = mTable.NewRow();

            for (int i=0; i< mFrequencyColumns.Length; i++)
            {
                dr[mFrequencyColumns[i]] = mRowData[i];
            }

            mTable.Rows.Add(dr);
        }

        public void AddRowsToDurationTable(DataTable mTable, string[] mRowData)
        {
            DataRow dr = mTable.NewRow();

            for (int i = 0; i < mDurationColumns.Length; i++)
            {
                dr[mDurationColumns[i]] = mRowData[i];
            }

            mTable.Rows.Add(dr);
        }

        private void topDataGrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            topDataGrid.UnselectAll();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            clearTables();

            SaveData = true;

            DialogResult = true;

            Close();
        }

        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            clearTables();

            SaveData = false;

            DialogResult = true;

            Close();
        }

        public void clearTables()
        {
            mainTable.Clear();
            tableOne.Clear();
            tableTwo.Clear();
            tableThree.Clear();
            mainTableDuration.Clear();
            tableOneDuration.Clear();
            tableTwoDuration.Clear();
            tableThreeDuration.Clear();
        }

    }
}
