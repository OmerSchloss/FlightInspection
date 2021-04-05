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
        private TelnetClient tClient;

        public MediaPanelView(string csvPath , string xmlPath)
        {
            InitializeComponent();
            isConnected = false;
            tClient = new TelnetClient();
            mediaViewModel = new MediaPanelViewModel( new FlightgearModel(csvPath,xmlPath,tClient ));

        }

        private void Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                mediaViewModel.connectToFG();
                btn_connect.Content = "stop Connection";
                isConnected = true;
            }
            else
            {
                mediaViewModel.disconnectFromFG();
                btn_connect.Content = "connect to fg";
                isConnected = false;
            }
        }
    }
}
