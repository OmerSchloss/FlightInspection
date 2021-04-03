using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace FlightInspection
{
    /// <summary>
    /// Interaction logic for CSVLoad.xaml
    /// </summary>
    public partial class CSVLoad : Window
    {
        public CSVLoad()
        {
            InitializeComponent();
        }



        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                textbox_file_path.Text = openFileDialog.FileName; // text(string) location
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
