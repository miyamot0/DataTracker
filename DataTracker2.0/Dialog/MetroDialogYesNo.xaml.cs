using System.Windows;

namespace DataTracker.Dialog
{
    /// <summary>
    /// Interaction logic for MetroDialogYesNo.xaml
    /// </summary>
    public partial class MetroDialogYesNo : Window
    {
        public string TitleString { get; set; }
        public bool ReturnedAnswer { get; set; }

        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        public MetroDialogYesNo()
        {
            ReturnedAnswer = false;
            InitializeComponent();
        }

        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = true;
            DialogResult = true;
        }

        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = false;
            DialogResult = true;
        }
    }
}
