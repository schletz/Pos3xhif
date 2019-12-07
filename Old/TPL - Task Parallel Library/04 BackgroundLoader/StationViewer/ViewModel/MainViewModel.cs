using StationViewer.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace StationViewer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly HttpClient client = new HttpClient();
        BackgroundLoader loader = new BackgroundLoader();

        /// <summary>
        /// Wird in Window_Initialized aufgerufen.
        /// </summary>
        public void StartLoading()
        {
            // Abonnieren des Events. Wenn geladen wurde, wird das Property
            // Value aktualisiert.
            loader.DataLoaded += (sender, e) =>
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
            };
            // Alle 1000ms vom Server laden.
            loader.StartLoading(1000);
        }

        /// <summary>
        /// Stoppt das Laden. Könnte auch als Command für den Stop Fetch Button ausgeführt werden.
        /// </summary>
        /// <returns></returns>
        public async Task StopLoading()
        {
            await loader.CancelAsync();
        }
        /// <summary>
        /// Liefert die in der Datenbank gespeicherten Stationen sortiert nach dem Namen.
        /// </summary>
        public IEnumerable<Station> Stations
        {
            get
            {
                using (StationDb db = new StationDb())
                {
                    return db.Stations.OrderBy(s => s.NAME).ToList();
                }
            }
        }
        private Station selectedStation;
        /// <summary>
        /// Binding für die in der Liste ausgewählte Station.
        /// </summary>
        public Station SelectedStation
        {
            get => selectedStation;
            set
            {
                if (value != null && selectedStation != value)
                {
                    selectedStation = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
                }
            }
        }
        /// <summary>
        /// Liefert die gespeicherten Werte zur ausgewählten Station. Dabei wird so vorgegangen:
        /// 1. Öffnen der Datenbankverbindung
        /// 2. Mit Attach muss die SelectedStation wieder in den Context eingebunden werden.
        /// 3. Mit dem Befehl
        ///    db.Entry(SelectedStation).Collection(s => s.Values).Load() 
        ///    können die Werte aus der value Tabelle geladen werden, denn diese haben sich ja geändert.
        /// 4. Mit ToList() werden die Werte zurückgegeben.   
        /// </summary>
        public IEnumerable<Value> Values
        {
            get
            {
                if (SelectedStation == null) { return Enumerable.Empty<Value>(); }
                using (StationDb db = new StationDb())
                {
                    db.Stations.Attach(SelectedStation);
                    db.Entry(SelectedStation).Collection(s => s.Values).Load();
                    return SelectedStation.Values.ToList();
                }
            }
        }

        public void RefreshList()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        /// <summary>
        /// Lädt die Stationen von http://schletz.org/getStations. Dabei muss allerdings beachtet
        /// werden, dass manche Stationen schon in der Datenbank sind. Deswegen kann so vorgegangen
        /// werden:
        /// 1. Anfordern der Daten mittels HttpClient.GetStringAsync() von http://schletz.org/getStations
        /// 2. Parsen mittels  JsonConvert.DeserializeObject<IEnumerable<Station>>(data)
        /// 3. Laden aller ID Werte aus der Tabelle Station in ein HashSet<int>. Der Konstruktur von
        ///    HashSet kann ein IEnumerable als Parameter varerbeiten.
        /// 4. Es werden nur Stationen eingefügt, die nicht in der Tabelle Station existieren. Dies
        ///    kann mittels Contains in Where herausgefunden werden.
        /// 5. Mit AddRange() werden die neuen Stationen eingefügt.
        /// 
        /// Falls die Netzverbindung ausfällt, kann mittels
        ///    using (var reader = File.OpenText("getStations.json"))
        ///    {
        ///        string data = await reader.ReadToEndAsync();
        ///    }
        /// aus der Datei getStations.json statt des Requests von http://schletz.org/getStations gelesen werden.
        /// Zum Testen kann das SQL Skript im Management Studio neu ausgeführt werden, es setzt die
        /// Datenbank wieder zurück.
        /// </summary>
        /// <returns></returns>
        public async Task LoadStations()
        {
            try
            {
                string data = await client.GetStringAsync("http://schletz.org/getStations");
                var result = JsonConvert.DeserializeObject<IEnumerable<Station>>(data);
                using (StationDb db = new StationDb())
                {
                    HashSet<int> existingStations = new HashSet<int>(db.Stations.Select(s => s.ID));
                    var newStations = result.Where(s => !existingStations.Contains(s.ID));
                    db.Stations.AddRange(newStations);
                    try { await db.SaveChangesAsync(); }
                    catch { }
                }
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Stations)));
            }
            catch { }
        }
    }
}
