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
using WienerLinienApp.ViewModels;

namespace WienerLinienApp
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

        private async Task LoadAllData()
        {
            MainViewModel vm = DataContext as MainViewModel;

            int linesCount = await vm.LoadLines();
            int haltestellenCount = await vm.LoadHaltestellen();
            int steigeCount = await vm.LoadSteige();
            Statustext.Text = $"{linesCount} Linien, {haltestellenCount} Haltestellen und {steigeCount} Steige geladen.";

        }
        private async void Window_Initialized(object sender, EventArgs e)
        {
            Statustext.Text = "Lade Daten...";
            await LoadAllData();
        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadButton.IsEnabled = false;
            MainViewModel vm = DataContext as MainViewModel;

            Statustext.Text = "Lösche Daten...";
            await vm.ClearDb();
            Statustext.Text = "Lade Daten...";
            await LoadAllData();
            ReloadButton.IsEnabled = true;
        }
    }
}
