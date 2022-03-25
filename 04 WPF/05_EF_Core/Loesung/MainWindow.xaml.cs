using ListDemo.Model;
using System.Windows;

namespace ListDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            using (SchoolDb db = new SchoolDb())
            {
                // Bei db.CreateDatabase(true) wurde die Datenbank gelöscht und neu erzeugt werden.
                // Das ist sinnvoll, wenn die DB zurückgesetzt werden soll.
                db.CreateDatabase(deleteDb: true);
            }

            InitializeComponent();
        }
    }
}
