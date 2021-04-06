using System.Windows.Controls;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for FeaturesView.xaml
    /// </summary>
    public partial class FeaturesView : UserControl
    {
        FeaturesViewModel featuresViewModel;

        public FeaturesView(string csvPath, string xmlPath)
        {
            InitializeComponent();
            featuresViewModel = new FeaturesViewModel(new FlightgearModel(csvPath, xmlPath, new TelnetClient()));
            DataContext = featuresViewModel;

            /* for (int i = 0; i < 10; ++i)
             {
                 ListBoxItem newItem = new ListBoxItem();
                 newItem.Content = "Item " + i;
                 listBox.Items.Add(newItem);
             }*/
        }
    }
}
