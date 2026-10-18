using Newtonsoft.Json;
using StationViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StationViewer
{
    /// <summary>
    /// Lädt im Hintergrund die neuen Messwerte von http://schletz.org/fetchData
    /// </summary>
    class BackgroundLoader
    {
        /// <summary>
        /// Event für das Viewmodel, um die Anzeige zu aktualisieren.
        /// </summary>
        public event EventHandler DataLoaded;
        /// <summary>
        /// Abbruch des Ladevorganges
        /// </summary>
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private Task backgroundTask;
        private readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Setzt den CancellationToken und wartet, bis der Task beendet wurde.
        /// </summary>
        /// <returns></returns>
        public async Task CancelAsync()
        {
            cancellation.Cancel();
            // Warten, bis die Verbindungen geschlossen, etc. wurden.
            await backgroundTask;
        }
        /// <summary>
        /// Startet das Laden im Hintergrund.
        /// </summary>
        /// <param name="fetchDuration"></param>
        public void StartLoading(int fetchDuration)
        {
            // Wir speichern den backgroundTask, damit wir beim Abbrechen darauf warten können.
            backgroundTask = Task.Run(async () =>
            {
                try
                {
                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        await Task.Delay(fetchDuration, cancellation.Token);
                        await LoadAsync();
                    }
                }
                // Achtung: Task.Delay wirft eine Exception, die wir abfangen müssen.
                catch (TaskCanceledException) { }

            });
        }
        /// <summary>
        /// Lädt die Daten von http://schletz.org/fetchData und schreibt sie in die Datenbank.
        /// </summary>
        /// <returns></returns>
        private async Task LoadAsync()
        {
            try
            {
                // GetAsync kann einen CancellationToken haben, GetStringAsync nicht.
                HttpResponseMessage response = await client.GetAsync("http://schletz.org/fetchData", cancellation.Token);
                // HTTP 200?
                if (!response.IsSuccessStatusCode) { return; }
                string data = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IEnumerable<Value>>(data);
                using (StationDb db = new StationDb())
                {
                    HashSet<int> existingStations = new HashSet<int>(db.Stations.Select(s => s.ID));
                    db.Values.AddRange(result.Where(v => existingStations.Contains(v.STATION)));
                    try {
                        await db.SaveChangesAsync(cancellation.Token);
                        // Laden ist fertig, daher wird das Event aufgerufen.
                        DataLoaded?.Invoke(this, new EventArgs());
                    }
                    catch { }
                }
            }
            // GetAsync wirft eine Exception, wenn der Task abgebrochen wurde.
            catch (TaskCanceledException) { }
            catch { }
        }
    }
}

