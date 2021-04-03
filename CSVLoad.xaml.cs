using System.Windows;
using Microsoft.Win32;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for CSVLoad.xaml
    /// </summary>
    public partial class CSVLoad : Window
    {
        public CSVLoad()
        {
            InitializeComponent();
        }

        private void ChooseFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                textbox_file_path.Text = openFileDialog.FileName; // text(string) location
        }


        private void ContinueButton(object sender, RoutedEventArgs e)
        {

        }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (textbox_file_path.Text.Contains(".csv"))
            {
                Continue.IsEnabled = true;
            }
        }
    }
}
