using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WienerLinienApp.Model;

namespace WienerLinienApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string errorLog = "err.txt";
        /// <summary>
        /// Verwendeter HttpClient, nur 1 Instanz!
        /// </summary>
        public readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Die Linien für die ListBox links, sortiert nach Verkehrsmittel und Bezeichnung
        /// </summary>
        public IEnumerable<Linie> Lines { get; set; }

        /// <summary>
        /// Bindingfeld für die selektierte Linie aus der ListBox der Linien.
        /// </summary>
        public Linie SelectedLine { get; set; }

        /// <summary>
        /// Bindingfeld für die Haltestellen der Richtung hin.
        /// </summary>
        public IEnumerable<Haltestelle> HaltestelleHin { get; private set; }
        /// <summary>
        /// Bindingfeld für die Haltestellen der Richtung retour.
        /// </summary>
        public IEnumerable<Haltestelle> HaltestelleRetour { get; private set; }

        /// <summary>
        /// Löscht alle Tabellen der Datenbank. Die Reihenfolge ist wegen der Fremdschlüssel zu
        /// beachten.
        /// </summary>
        /// <returns></returns>
        public void ClearDb()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Lädt die Linien von https://www.data.gv.at/katalog/dataset/stadt-wien_wienerlinienechtzeitdaten
        /// und schreibt sie in die Datenbanktabelle Linien.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist.
        /// </summary>
        public void LoadLines()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-linien.csv";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lädt die Linien von https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv
        /// und schreibt sie in die Datenbanktabelle Haltestelle.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist. Kommentare siehe LoadLinien().
        /// </summary>
        public void LoadHaltestellen()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lädt die Linien von https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv
        /// und schreibt sie in die Datenbanktabelle Steig.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist. Kommentare siehe LoadLinien().
        /// </summary>
        public void LoadSteige()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Schreibt die Fehlermeldung in eine Datei.
        /// </summary>
        /// <param name="e"></param>
        public void LogError(Exception e)
        {
            lock (errorLog)
            {
                try
                {
                    File.WriteAllText(errorLog, $"{DateTime.UtcNow.ToString("o")} {e?.Message}");
                    File.WriteAllText(errorLog, $"{DateTime.UtcNow.ToString("o")} {e?.InnerException}");
                    File.WriteAllText(errorLog, $"{DateTime.UtcNow.ToString("o")} {e?.StackTrace}");
                }
                catch { }
            }
        }
    }
}
