﻿using System.IO;
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

        private void ChooseCSVButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                csvPath.Text = openFileDialog.FileName;
        }



        private void ContinueButton(object sender, RoutedEventArgs e)
        {
            /*List<string> featuresList = new List<string>();
            CreateNewCSVFromXml csvFromXaml = new CreateNewCSVFromXml();
            csvFromXaml.setFeaturesFromXml(xmlPath.Text);
            csvFromXaml.createNewCSV(csvPath.Text);*/

            this.Hide();
            MainWindow mainWindow = new MainWindow(csvPath.Text, xmlPath.Text);
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
            if (csvPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }
        private void CSVPathChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (csvPath == null || xmlPath == null || Continue == null) return;
            if (csvPath.Text.Contains(".csv") && xmlPath.Text.Contains(".xml"))
                Continue.IsEnabled = true;
            else
                Continue.IsEnabled = false;
        }

    }
}
