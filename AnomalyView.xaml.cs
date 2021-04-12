using System.Windows.Controls;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for AnomalyView.xaml
    /// </summary>
    public partial class AnomalyView : UserControl
    {
        AnomalyViewModel anomalyViewModel;
        public AnomalyView()
        {
            InitializeComponent();
        }

        internal void setFlightgearModel(FlightgearModel flightgearModel)
        {
            anomalyViewModel = new AnomalyViewModel(flightgearModel);
            DataContext = anomalyViewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            anomalyViewModel.updateCurrentLine(int.Parse(List_Anomaly.SelectedItem.ToString().Split(',')[0]));
        }
    }
}
