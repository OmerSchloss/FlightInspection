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

        private string xmlPath;
        private string learnCsv;
        private string detectCsv;
        private string dllPath;
        private FlightgearModel flightgearModel;
        private TelnetClient tClient;
        private MediaPanelView mediaPanelView;
        private InfoView infoView;
        private AnomalyView anomalyView;


        public MainWindow(string learnCsv, string detectCsv, string xml, string dll)
        {

            InitializeComponent();
            this.xmlPath = xml;
            this.learnCsv = learnCsv;
            this.detectCsv = detectCsv;
            this.dllPath = dll;

            tClient = new TelnetClient();
            flightgearModel = new FlightgearModel(learnCsv, detectCsv, xmlPath, dllPath, tClient);


            JoystickView joystickview = new JoystickView();
            joystickview.setFlightgearModel(flightgearModel);
            canvas_joystick.Children.Add(joystickview);

            mediaPanelView = new MediaPanelView();
            mediaPanelView.setFlightgearModel(flightgearModel);
            grd_media_panel.Children.Add(mediaPanelView);

            infoView = new InfoView();
            infoView.setFlightgearModel(flightgearModel);
            grd_info_view.Children.Add(infoView);

            FeaturesView featuresView = new FeaturesView();
            featuresView.setFlightgearModel(flightgearModel);
            grd_feature_view.Children.Add(featuresView);

            anomalyView = new AnomalyView();
            anomalyView.setFlightgearModel(flightgearModel);
            grd_anomaly_view.Children.Add(anomalyView);

            //DataContext = mediaPanel;
            /*List<string> featuresList = new List<string>();
            CreateNewCSVFromXml csvFromXaml = new CreateNewCSVFromXml();
            csvFromXaml.setFeaturesFromXml(xmlPath.Text);
            csvFromXaml.createNewCSV(csvTrainPath.Text);*/
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

        private void Window_Closed(object sender, System.EventArgs e)
        {
            mediaPanelView.closeWindow();
            File.Delete("input.txt");
            File.Delete("output.txt");
            File.Delete("anomalyTrain.csv");
            File.Delete("anomalyTest.csv");
        }
    }
}
