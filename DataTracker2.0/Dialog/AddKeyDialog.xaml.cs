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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DataTracker.Dialog
{
    /// <summary>
    /// Interaction logic for AddKeyDialog.xaml
    /// </summary>
    public partial class AddKeyDialog : Window
    {
        bool isLooking = false;
        Key mHolder;

        /// <summary>
        /// Keys to avoid assigning
        /// </summary>
        Key[] mExcludedKeys =
        {
            Key.Tab,
            Key.Escape,
            Key.Z,
            Key.X,
            Key.C,
            Key.Back
        };

        /// <summary>
        /// Default constuctor
        /// </summary>
        public AddKeyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Text to return
        /// </summary>
        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        /// <summary>
        /// Stored Value
        /// </summary>
        public Key KeyValue
        {
            get { return mHolder; }
            set { mHolder = value; }
        }

        /// <summary>
        /// Set result to true if clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Begin listener for specific key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonKey.Background = Brushes.Red;
            buttonKey.Content = "WAITING FOR KEY PRESS";
            isLooking = true;
        }

        /// <summary>
        /// Listener for all keys while active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isLooking && !Array.Exists(mExcludedKeys, element => element == e.Key))
            {
                mHolder = e.Key;
                buttonKey.Background = Brushes.Green;
                buttonKey.Content = e.Key.ToString();
                isLooking = false;
            }
        }
    }
}
