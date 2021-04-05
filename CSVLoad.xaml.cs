using System.Collections.Generic;
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
        private string xmlFile;
        public CSVLoad()
        {
            InitializeComponent();

            this.xmlFile = "C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml";
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
                csvPath.Text = openFileDialog.FileName;
        }



        private void StartButton(object sender, RoutedEventArgs e)
        {
            /* List<string> featuresList = new List<string>();
             CreateNewCSVFromXml csvFromXaml = new CreateNewCSVFromXml();
             this.xmlFile = xmlPath.Text;
             csvFromXaml.setFeaturesFromXml(this.xmlFile);
             csvFromXaml.createNewCSV(csvPath.Text);*/
            CSVHandler csvHandler = new CSVHandler(csvPath.Text);
            List<string> strings = csvHandler.csvParser();
            this.Hide();
            MainWindow mainWindow = new MainWindow(csvPath.Text);
            mainWindow.ShowDialog();
            this.Show();


        }

        private void ChooseXMLButton(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
                xmlPath.Text = openFileDialog.FileName;
        }

        private void XMLPathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvPath == null || xmlPath == null || Continue == null) return;
            if (csvPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && xmlPath.Text.Contains("data\\Protocol")
)
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }
        private void CSVPathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvPath == null || xmlPath == null || Continue == null) return;
            if (csvPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml") && xmlPath.Text.Contains("data\\Protocol")
)
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }

    }
}
