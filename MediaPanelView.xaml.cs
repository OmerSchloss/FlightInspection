using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for MediaPanelView.xaml
    /// </summary>
    public partial class MediaPanelView : UserControl
    {
        private bool isConnected;
        private bool isPlayed;
        private MediaPanelViewModel mediaViewModel;

        public MediaPanelView()
        {
            InitializeComponent();
            isConnected = false;
            isPlayed = false;
           //sliderTime.Maximum =
        }

        internal void setFlightgearModel(FlightgearModel flightgearModel)
        {
            mediaViewModel = new MediaPanelViewModel(flightgearModel);
            DataContext = mediaViewModel;
        }

        private void Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {

                MessageBoxResult result = MessageBox.Show("For connecting to FlightGear App,\n First open the app. \n Then, copy these lines to Setting-> Additional Setting: \n\n --generic=socket,in,10,127.0.0.1,5400,tcp,playback_small \n --fdm=null\n\n and now just press fly! \n\n Press Yes to copy these lines. ","FlightGeat connection",MessageBoxButton.YesNoCancel);
                if(result == MessageBoxResult.Yes)
                {
                    Clipboard.SetText("--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small\n--fdm=null");

                }
                MessageBox.Show("ready?","", MessageBoxButton.OKCancel);
                if (mediaViewModel.connectToFG())
                {
                    MessageBox.Show("connected");

                    btn_connect.Content = "Stop connection";
                    isConnected = true;
                }
                else
                {
                    MessageBox.Show("Connection failed!");
                    isConnected = false;
                }
            }
            else
            {
                mediaViewModel.disconnectFromFG();
                btn_connect.Content = "connect to fg";
                isConnected = false;
            }
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlayed)
            {
                isPlayed = true;
                mediaViewModel.playFg(isConnected);
            }
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            isPlayed = false;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
