using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
using WienerLinien.Model;

namespace WienerLinien
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Erstellt die Datenbank, falls sie noch nicht vorhanden ist.
            using (LinienContext db = new LinienContext())
            {
                // So kann die Datenbank gelöscht werden:
                // db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            InitializeComponent();
        }

        /// <summary>
        /// Lädt die Linien in die Datenbank, falls die lokalen Daten zu alt oder nicht vorhanden sind.
        /// Das Datum des letzten Aktualisierung (DateTime.UtcNow) wird in der Tabelle Config gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            // TODO: Die Linien - wenn erforderlich - in die Datenbank laden.
        }
    }
}
