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
        public CSVLoad()
        {
            InitializeComponent();
            string xmlFile = "C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml";

            if (File.Exists(xmlFile))
            {
                xmlPath.Text = xmlFile;
                xmlPath.IsEnabled = false;
                Verified.IsEnabled = false;
                Verified.Content = "DONE!";
                textChooseXml.Text = "Playback XML file:";
            }
        }

        private void ChooseFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                filePath.Text = openFileDialog.FileName;
        }



        private void ContinueButton(object sender, RoutedEventArgs e)
        {


        }

        private void VerifiedButton(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
                xmlPath.Text = openFileDialog.FileName;
        }

        private void xmlPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (filePath == null || xmlPath == null || Continue == null) return;
            if (filePath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }
        private void FilePathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (filePath == null || xmlPath == null || Continue == null) return;
            if (filePath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }
    }
}
