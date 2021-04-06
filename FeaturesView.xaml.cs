using System;
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

            /* for (int i = 0; i < 10; ++i)
             {
                 ListBoxItem newItem = new ListBoxItem();
                 newItem.Content = "Item " + i;
                 listBox.Items.Add(newItem);
             }*/
        }

        internal void setFlightgearModel(FlightgearModel flightgearModel)
        {
            featuresViewModel = new FeaturesViewModel(flightgearModel);
            DataContext = featuresViewModel;
        }
    }
}
