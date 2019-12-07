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
using WeatherDbCrud.ViewModels;

namespace WeatherDbCrud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel ?? throw new Exception("ViewModel is null.");
            // Füge hier den Code ein.
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel ?? throw new Exception("ViewModel is null.");
            // Füge hier den Code ein.
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel ?? throw new Exception("ViewModel is null.");
            // Füge hier den Code ein.
        }
    }
}
