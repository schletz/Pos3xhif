using StationViewer.ViewModel;
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

namespace StationViewer
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

        /// <summary>
        /// Eventhandler für den Fetch Button. Dieser soll
        /// 1. Vor dem Laden den Text des Controls Statustext auf "Lade Werte..." setzen.
        /// 2. Den FetchButton deaktivieren (IsEnabled Property).
        /// 3. Die Methode FetchValues im ViewModel mit await aufrufen.
        /// 4. Den Fetch Button nach dem Laden wieder aktivieren.
        /// 5. Den Statustext wieder auf leer setzen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            Statustext.Text = "Lade Werte...";
            FetchButton.IsEnabled = false;
            await vm.FetchValues();
            FetchButton.IsEnabled = true;
            Statustext.Text = "";
        }

        /// <summary>
        /// Eventhandler für den Load Stations Button. Dieser soll
        /// 1. Vor dem Laden den Text des Controls Statustext auf "Lade Stationen..." setzen.
        /// 2. Den LoadStationButton deaktivieren (IsEnabled Property).
        /// 3. Die Methode LoadStations im Viewmodel mit await aufrufen.
        /// 4. Den Button nach dem Laden wieder aktivieren.
        /// 5. Den Statustext wieder auf leer setzen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadStationButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            Statustext.Text = "Lade Stationen...";
            LoadStationButton.IsEnabled = false;
            await vm.LoadStations();
            LoadStationButton.IsEnabled = true;
            Statustext.Text = "";
        }
    }
}
