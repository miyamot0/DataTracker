﻿/*
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
    /// Interaction logic for DialogYesNo.xaml
    /// </summary>
    public partial class DialogYesNo : Window
    {
        /// <summary>
        /// Title
        /// </summary>
        public string TitleString { get; set; }

        /// <summary>
        /// Response
        /// </summary>
        public bool ReturnedAnswer { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogYesNo()
        {
            ReturnedAnswer = false;
            InitializeComponent();
        }

        /// <summary>
        /// Button yes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = true;
            DialogResult = true;
        }

        /// <summary>
        /// Button no
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = false;
            DialogResult = true;
        }
    }
}