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
        Key[] mExcludedKeys =
        {
            Key.Tab,
            Key.Escape,
            Key.Z,
            Key.X,
            Key.C,
            Key.Back
        };

        public AddKeyDialog()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        public Key KeyValue
        {
            get { return mHolder; }
            set { mHolder = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonKey.Background = Brushes.Red;
            buttonKey.Content = "WAITING FOR KEY PRESS";
            isLooking = true;
        }

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
