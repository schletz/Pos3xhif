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
        private readonly string errorLog = "err.txt";
        /// <summary>
        /// Verwendeter HttpClient, nur 1 Instanz!
        /// </summary>
        public readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Die Linien für die ListBox links, sortiert nach Verkehrsmittel und Bezeichnung
        /// </summary>
        public IEnumerable<Linie> Lines
        {
            get
            {
                try
                {
                    using (HaltestellenDb db = new HaltestellenDb())
                    {
                        // ToList, da die Verbindung geschlossen wird.
                        return db.Linies.OrderBy(l => l.L_Verkehrsmittel).ThenBy(l => l.L_Bezeichnung).ToList();
                    }
                }
                catch (Exception e) { LogError(e); return Enumerable.Empty<Linie>(); }
            }
        }

        private Linie selectedLine;
        /// <summary>
        /// Bindingfeld für die selektierte Linie aus der ListBox der Linien.
        /// </summary>
        public Linie SelectedLine
        {
            get => selectedLine;
            set
            {
                if (value != null && selectedLine != value)
                {
                    selectedLine = value;
                    // Aus Performancegründen weisen wir hier schon die Daten zu und lesen nicht
                    // "Live" beim Bindungfeld für Hin und Retour. So muss die DB Verbindung nur
                    // 1x geöffnet werden.
                    try
                    {
                        using (HaltestellenDb db = new HaltestellenDb())
                        {
                            db.Linies.Attach(SelectedLine);
                            HaltestelleHin = (from s in SelectedLine.Steigs
                                              where s.S_Richtung == "H"
                                              orderby s.S_Reihenfolge
                                              select s.Haltestelle).ToList();
                            HaltestelleRetour = (from s in SelectedLine.Steigs
                                                 where s.S_Richtung == "R"
                                                 orderby s.S_Reihenfolge
                                                 select s.Haltestelle).ToList();
                        }
                        // Es hat sich etwas geändert.
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(HaltestelleHin)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(HaltestelleRetour)));
                    }
                    catch (Exception e) { LogError(e); }
                }
            }
        }

        /// <summary>
        /// Bindingfeld für die Haltestellen der Richtung hin.
        /// </summary>
        public IEnumerable<Haltestelle> HaltestelleHin { get; private set; }
        /// <summary>
        /// Bindingfeld für die Haltestellen der Richtung retour.
        /// </summary>
        public IEnumerable<Haltestelle> HaltestelleRetour { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Löscht alle Tabellen der Datenbank. Die Reihenfolge ist wegen der Fremdschlüssel zu
        /// beachten.
        /// </summary>
        /// <returns></returns>
        public async Task ClearDb()
        {
            try
            {
                using (HaltestellenDb db = new HaltestellenDb())
                {
                    // Da dies eine CPU intensive Operation ist (arbeiten mit Speicherlisten), geben wir
                    // diesen Teil in einen eigenen Task.
                    await Task.Run(() =>
                    {
                        db.Steigs.RemoveRange(db.Steigs);
                        db.Haltestelles.RemoveRange(db.Haltestelles);
                        db.Linies.RemoveRange(db.Linies);
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e) { LogError(e); }
        }


        /// <summary>
        /// Lädt die Linien von https://www.data.gv.at/katalog/dataset/stadt-wien_wienerlinienechtzeitdaten
        /// und schreibt sie in die Datenbanktabelle Linien.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist.
        /// </summary>
        public async Task<int> LoadLines()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-linien.csv";
            List<Linie> loaded = new List<Linie>();

            try
            {
                using (HaltestellenDb db = new HaltestellenDb())
                {
                    // Es gibt schon Linien? Nichts laden.
                    if (db.Linies.Count() > 0) { return 0; }
                    string lines = await client.GetStringAsync(url);
                    // Das Parsen der Textdatei soll in einem eigenen Task passieren (CPU Arbeit)
                    await Task.Run(() =>
                    {
                        foreach (string line in lines.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1))
                        {
                            try
                            {
                                string[] cells = line.Split(';');
                                loaded.Add(new Linie
                                {
                                    L_ID = int.Parse(cells[0]),
                                    L_Bezeichnung = cells[1].Replace("\"", string.Empty),
                                    L_Verkehrsmittel = cells[4].Replace("\"", string.Empty)
                                });
                            }
                            catch { }
                        }
                        // Man könnte auch jeden Datensatz mit db.Linies.Add(...) einfügen, aber das
                        // dauert sehr lange, da das einzelne INSERT pro Transaktion verwendet wird.
                        db.Linies.AddRange(loaded);
                    });
                    // Diese Anweisung ist sowieso Async, daher außerhalb des Tasks.
                    await db.SaveChangesAsync();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lines)));
            }
            catch (Exception e) { LogError(e); }
            return loaded.Count;
        }

        /// <summary>
        /// Lädt die Linien von https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv
        /// und schreibt sie in die Datenbanktabelle Haltestelle.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist. Kommentare siehe LoadLinien().
        /// </summary>
        public async Task<int> LoadHaltestellen()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv";
            List<Haltestelle> loaded = new List<Haltestelle>();

            using (HaltestellenDb db = new HaltestellenDb())
            {
                if (db.Haltestelles.Count() > 0) { return 0; }

                string lines = await client.GetStringAsync(url);
                await Task.Run(() =>
                {
                    foreach (string line in lines.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1))
                    {
                        try
                        {
                            string[] cells = line.Split(';');
                            loaded.Add(new Haltestelle
                            {
                                H_ID = int.Parse(cells[0]),
                                H_Name = cells[3].Replace("\"", string.Empty)
                            });
                        }
                        catch { }
                    }
                    db.Haltestelles.AddRange(loaded);
                });
                await db.SaveChangesAsync();
            }
            return loaded.Count;
        }

        /// <summary>
        /// Lädt die Linien von https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv
        /// und schreibt sie in die Datenbanktabelle Steig.
        /// Die Daten werden nur geladen, wenn die Tabelle leer ist. Kommentare siehe LoadLinien().
        /// </summary>
        public async Task<int> LoadSteige()
        {
            string url = "https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv";
            List<Steig> loaded = new List<Steig>();

            try
            {
                using (HaltestellenDb db = new HaltestellenDb())
                {
                    if (db.Steigs.Count() > 0) { return 0; }

                    string lines = await client.GetStringAsync(url);
                    await Task.Run(() =>
                    {
                        foreach (string line in lines.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1))
                        {
                            try
                            {
                                string[] cells = line.Split(';');
                                loaded.Add(new Steig
                                {
                                    S_ID = int.Parse(cells[0]),
                                    S_Linie = int.Parse(cells[1]),
                                    S_Haltestelle = int.Parse(cells[2]),
                                    S_Richtung = cells[3].Replace("\"", string.Empty),
                                    S_Reihenfolge = int.Parse(cells[4]),
                                    S_Steig = cells[7].Replace("\"", string.Empty)
                                });
                            }
                            catch { }
                        }
                        db.Steigs.AddRange(loaded);
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e) { LogError(e); }
            return loaded.Count;
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
