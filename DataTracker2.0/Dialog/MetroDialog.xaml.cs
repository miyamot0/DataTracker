using System.Windows;

namespace DataTracker.Dialog
{
    /// <summary>
    /// Interaction logic for MetroDialog.xaml
    /// </summary>
    public partial class MetroDialog : Window
    {

        public string TitleString { get; set; }

        public string QuestionText
        {
            set { QuestionTextBox.Text = value; }
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        public MetroDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
