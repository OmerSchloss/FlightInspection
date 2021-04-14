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
        private MediaPanelViewModel mediaViewModel;

        public MediaPanelView()
        {
            InitializeComponent();
            isConnected = false;


        }

        internal void setFlightgearModel(FlightgearModel flightgearModel)
        {
            mediaViewModel = new MediaPanelViewModel(flightgearModel);
            DataContext = mediaViewModel;

            sliderTime.Maximum = mediaViewModel.getNumOfLines();
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

            mediaViewModel.playFg(isConnected);

        }

        internal void closeWindow()
        {
            mediaViewModel.closeThread();
            if (isConnected)
            {
                mediaViewModel.disconnectFromFG();
                btn_connect.Content = "connect to fg";
                isConnected = false;
            }
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            mediaViewModel.pauseFg();
        }

        private void btn_play_start_Click(object sender, RoutedEventArgs e)
        {
            mediaViewModel.startFromZeroFg();
        }

        private void btn_backward_Click(object sender, RoutedEventArgs e)
        {
            mediaViewModel.backwardTenSecFg();
        }

        private void btn_forward_Click(object sender, RoutedEventArgs e)
        {
            mediaViewModel.forwardTenSecFg();
        }

        private void btn_end_Click(object sender, RoutedEventArgs e)
        {
            mediaViewModel.endFg();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            btn_pause_Click(sender, e);
            btn_play_start_Click(sender, e);
        }

        private void Btn_update_speed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaViewModel.updateSpeedFg(float.Parse(text_speed.Text));
            }
            catch (Exception)
            {
            }
        }


    }
}
