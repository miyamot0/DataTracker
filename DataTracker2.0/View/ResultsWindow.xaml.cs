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

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResultsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Build up window grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Add keys to datatable
        /// </summary>
        /// <param name="mTable"></param>
        /// <param name="mCols"></param>
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

        /// <summary>
        /// Add rows to freq table
        /// </summary>
        /// <param name="mTable"></param>
        /// <param name="mRowData"></param>
        public void AddRowsToFrequencyTable(DataTable mTable, string[] mRowData)
        {
            DataRow dr = mTable.NewRow();

            for (int i=0; i< mFrequencyColumns.Length; i++)
            {
                dr[mFrequencyColumns[i]] = mRowData[i];
            }

            mTable.Rows.Add(dr);
        }

        /// <summary>
        /// Add rows to dur table
        /// </summary>
        /// <param name="mTable"></param>
        /// <param name="mRowData"></param>
        public void AddRowsToDurationTable(DataTable mTable, string[] mRowData)
        {
            DataRow dr = mTable.NewRow();

            for (int i = 0; i < mDurationColumns.Length; i++)
            {
                dr[mDurationColumns[i]] = mRowData[i];
            }

            mTable.Rows.Add(dr);
        }

        /// <summary>
        /// Deselect action when focus lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topDataGrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            topDataGrid.UnselectAll();
        }

        /// <summary>
        /// Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            clearTables();

            SaveData = true;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Discard button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            clearTables();

            SaveData = false;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Clear all tables
        /// </summary>
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
