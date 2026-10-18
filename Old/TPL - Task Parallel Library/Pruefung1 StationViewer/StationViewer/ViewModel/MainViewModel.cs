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
        /// <summary>
        /// Binding für die in der Liste ausgewählte Station.
        /// </summary>
        public Station SelectedStation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Lädt die neuesten Stationswerte von http://schletz.org/fetchData?state=WIE
        /// In der einfachsten Version werden diese Werte einfach in die Datenbanktabelle Values
        /// geschrieben. Hierfür muss folgendes erledigt werden:
        /// 1. Asynchrones Anfordern der Daten mit 
        ///    client.GetStringAsync("http://schletz.org/fetchData?state=WIE")
        /// 2. Parsen der Daten mit 
        ///    JsonConvert.DeserializeObject<IEnumerable<Value>>(data);
        /// 3. Schreiben des Ergebnisses von (2) in die Datenbank.
        /// 
        /// Das funktioniert, da die Stationen von Wien (WIE) schon in der Datenbank
        /// vorhanden sind. Für die Erweiterung - schreiben der Werte nur wenn die Station vorhanden
        /// ist - kann die Methode wie folgt ergänzt werden:
        /// 1. Laden aller Stationsdaten von der URL http://schletz.org/fetchData (ohne state Einschränkung)
        /// 2. Erzeugen eines neuen HashSet<int>. Im Konstruktor des HashSet können die vorhandenen
        ///    ID Werte der Stationen in der Station Tabelle als IEnumerable<int> übergeben werden.
        /// 3. Es werden alle gelesenen Stationen eingefügt, dessen Property STATION in diesem
        ///    HashSet sind (verwende Contains in Where).
        ///    
        /// Falls die Netzverbindung ausfällt, kann mittels
        ///    using (var reader = File.OpenText("fetchData_WIE.json"))
        ///    {
        ///        string data = await reader.ReadToEndAsync();
        ///    }
        /// aus der Datei fetchData_WIE.json statt des Requests von http://schletz.org/fetchData?state=WIE bzw.
        /// aus der DateifetchData.json statt des Requests von http://schletz.org/fetchData gelesen werden.
        /// 
        /// Zum Testen kann das SQL Skript im Management Studio neu ausgeführt werden, es setzt die
        /// Datenbank wieder zurück. 
        /// </summary>
        /// <returns></returns>
        public void FetchValues()
        {
            throw new NotImplementedException();
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
        public void LoadStations()
        {
            throw new NotImplementedException();
        }
    }
}
