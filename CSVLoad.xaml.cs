using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for CSVLoad.xaml
    /// </summary>
    public partial class CSVLoad : Window
    {

        
        private int counter;
        public CSVLoad()
        {
            InitializeComponent();
            counter = 0;
        }

        private void ChooseFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                filePath.Text = openFileDialog.FileName; // text(string) location
        }



        private void ContinueButton(object sender, RoutedEventArgs e)
        {

          
        }

        private void FilePathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (filePath.Text.Contains(".csv")) counter++;

        }


        private void VerifiedButton(object sender, RoutedEventArgs e)
        {
            string xmlFile = "C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml";
            if (File.Exists(xmlFile)) counter++;
            if (counter == 2) Continue.IsEnabled = true;
        }
    }
}
