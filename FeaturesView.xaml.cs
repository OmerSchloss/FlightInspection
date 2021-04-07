using System.Windows.Controls;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for FeaturesView.xaml
    /// </summary>
    public partial class FeaturesView : UserControl
    {
        FeaturesViewModel featuresViewModel;

        public FeaturesView()
        {
            InitializeComponent();


        }

        internal void setFlightgearModel(FlightgearModel flightgearModel)
        {
            featuresViewModel = new FeaturesViewModel(flightgearModel);
            DataContext = featuresViewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            featuresViewModel.VM_FeatureToDisplay = ListBox_Features_List.SelectedItem.ToString();
        }
    }
}
