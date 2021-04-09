using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for CSVLoad.xaml
    /// </summary>
    public partial class PathLoader : Window
    {
        public PathLoader()
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

        private void ChooseCSVButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                csvTrainPath.Text = openFileDialog.FileName;
        }

        private void ChooseXMLButton(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
                xmlPath.Text = openFileDialog.FileName;
        }


        private void ChooseDLL_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL files (*.dll)|*.dll";
            if (openFileDialog.ShowDialog() == true)
                dllPath.Text = openFileDialog.FileName;
        }

        private void ChooseCsvAnomaly_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                csvAnomalyPath.Text = openFileDialog.FileName;
        }




        private void XMLPathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvTrainPath == null || csvAnomalyPath == null || dllPath == null || xmlPath == null || Continue == null) return;
            if (csvTrainPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && csvAnomalyPath.Text.Contains(".csv") && dllPath.Text.Contains(".dll"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }
        private void CSVPathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvTrainPath == null || csvAnomalyPath == null || dllPath == null || xmlPath == null || Continue == null) return;
            if (csvTrainPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && csvAnomalyPath.Text.Contains(".csv") && dllPath.Text.Contains(".dll"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }

        private void csvAnomalyPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvTrainPath == null || csvAnomalyPath == null || dllPath == null || xmlPath == null || Continue == null) return;
            if (csvTrainPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && csvAnomalyPath.Text.Contains(".csv") && dllPath.Text.Contains(".dll"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }

        private void dllPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvTrainPath == null || csvAnomalyPath == null || dllPath == null || xmlPath == null || Continue == null) return;
            if (csvTrainPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && csvAnomalyPath.Text.Contains(".csv") && dllPath.Text.Contains(".dll"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }


        private void ContinueButton(object sender, RoutedEventArgs e)
        {
            string detectCsv = "C:\\Users\\User\\Desktop\\CS\\שנה ב\\סמסטר ב\\Advanced Programming 2\\תרגילים\\תרגיל 1\\anomaly_flight.csv";
            this.Hide();
            MainWindow mainWindow = new MainWindow(csvTrainPath.Text, csvAnomalyPath.Text, xmlPath.Text , dllPath.Text);
            mainWindow.ShowDialog();
            this.Show();
        }

    }
}
