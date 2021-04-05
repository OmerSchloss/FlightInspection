using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        JoystickViewModel viewModel;
        UserControl1 joystickView;

        private string xmlPath;
        private string csvPath;

        public MainWindow(string csv, string xml)
        {

            InitializeComponent();
            this.xmlPath = xml;
            this.csvPath = csv;
            joystickView = new UserControl1(csvPath,xmlPath);
            canvas_joystick.Children.Add(joystickView);
            MediaPanelView mediaPanelView = new MediaPanelView(csvPath, xmlPath);
            grd_media_panel.Children.Add(mediaPanelView);


            //DataContext = mediaPanel;
            /*List<string> featuresList = new List<string>();
            CreateNewCSVFromXml csvFromXaml = new CreateNewCSVFromXml();
            csvFromXaml.setFeaturesFromXml(xmlPath.Text);
            csvFromXaml.createNewCSV(csvPath.Text);*/
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                File.ReadAllText(openFileDialog.FileName); // text(string) location
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void list_features_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
