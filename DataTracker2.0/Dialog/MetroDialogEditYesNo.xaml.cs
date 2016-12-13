using System.Windows;

namespace DataTracker.Dialog
{
    /// <summary>
    /// Interaction logic for MetroDialogEditYesNo.xaml
    /// </summary>
    public partial class MetroDialogEditYesNo : Window
    {
        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        public bool ReturnedAnswer { get; set; }

        public MetroDialogEditYesNo()
        {
            InitializeComponent();
        }

        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = false;
            DialogResult = true;
        }

        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = true;
            DialogResult = true;
        }
    }
}
