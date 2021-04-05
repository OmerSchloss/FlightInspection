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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {

        JoystickViewModel joystickViewModel;
        public UserControl1(string csvPath, string xmlPath)
        {
            InitializeComponent();
            joystickViewModel = new JoystickViewModel(new FlightgearModel(csvPath, xmlPath, new TelnetClient()));
            DataContext = joystickViewModel;
        }

        private void joystick_sides_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
