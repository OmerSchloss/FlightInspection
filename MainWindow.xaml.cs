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

        public MainWindow(string csvPath, string xmlPath)
        {

            InitializeComponent();
            joystickView = new UserControl1(csvPath,xmlPath);
            canvas_joystick.Children.Add(joystickView);

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
