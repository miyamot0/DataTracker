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

using System.Windows;

namespace DataTracker.Dialog
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {

        /// <summary>
        /// Title
        /// </summary>
        public string TitleString { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        /// <summary>
        /// Returned value
        /// </summary>
        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Dialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
