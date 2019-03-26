# Create, Read, Update und Delete (CRUD)
```c#
// *************************************************************************************************
// MUSTERDATENBANK:
//
//     CREATE TABLE Station(
//         S_ID INTEGER  PRIMARY KEY,
//         S_Location    VARCHAR(100) NOT NULL,
//         S_Height      INTEGER
//     );
//
//     CREATE TABLE Measurement(
//         M_Date        DATETIME,
//         M_Station     INTEGER,
//         M_Temperature DECIMAL(4,1),  -- xxx.x
//         PRIMARY KEY(M_Date, M_Station),
//         FOREIGN KEY(M_Station) REFERENCES Station(S_ID)
//     );
// *************************************************************************************************

using System;
using System.Linq;
using CrudDemo.Model;   // Für die DB Klassen

namespace CrudDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Zufällig generierter Key für die Station, damit beim zweimaligen Ausführen keine PK
            // Verletzung entsteht.
            Random rnd = new Random();
            int stationKey = rnd.Next();           

            // CREATE
            // Erstelle eine Station mit der Location Spannberg.
            // Mit new wird ein neues Objekt erstellt, welches allerdings
            // untracked ist.
            using (WeatherDb db = new WeatherDb())
            {
                Station newStation = new Station
                {
                    S_ID = stationKey,
                    S_Location = $"Location{stationKey}",
                    S_Height = rnd.Next(170, 1000)
                };
                // Füge die neue Station zur Collection hinzu.
                db.Stations.Add(newStation);
                // Wir verwenden gleich die Collection der Measurements von newStation.
                // Das bedeutet, wir müssen nicht den Fremdschlüssel "händisch" mitgeben.
                newStation.Measurements.Add(new Measurement
                {
                    M_Date = DateTime.Now,
                    M_Temperature = rnd.Next(0, 380) / 10.0M
                });
                db.SaveChanges();
            }

            // UPDATE
            // Ändere S_Location der Station 1003 auf Spengergasse 20.
            // Schritt 1: Suche die Station in der DB. Da 1003 der PK ist, können
            //            wir Find verwenden.
            using (WeatherDb db = new WeatherDb())
            {
                Station station = db.Stations.Find(stationKey);
                // Vorsicht, wenn der Wert nicht gefunden wird ist station NULL!
                if (station != null)
                {
                    station.S_Location = "Spengergasse 20";
                    db.SaveChanges();   // NICHT VERGESSEN!
                }
            }

            // READ
            // Zeige die Durschschnittstemperaturen aller
            // Stationen über 1000m an. Hier wird SQL erzeugt, die die Berechnung
            // schon serverseitig durchführt.
            using (WeatherDb db = new WeatherDb())
            {
                var avgTemp = from s in db.Stations
                              where s.S_Height > 100
                              select new
                              {
                                  StationNr = s.S_ID,
                                  Location = s.S_Location,
                                  AvgTemp = s.Measurements.Average(m => m.M_Temperature)
                              };
                foreach (var a in avgTemp)
                {
                    Console.WriteLine($"{a.StationNr} {a.Location}: {a.AvgTemp}");
                }
            }

            // DELETE
            // Lösche die Station mit der Location Spengergasse 20.
            using (WeatherDb db = new WeatherDb())
            {
                // Schritt 1: Suche die Station, die gelöscht werden
                ///           soll.
                Station toDelete =
                    db
                    .Stations
                    .FirstOrDefault(s => s.S_Location == "Spengergasse 20");
                if (toDelete != null)
                {
                    // Schritt 2: Lösche alle Messerte, sonst gibt es eine Exception
                    //            (außer on DELETE SET NULL ist gesetzt, frage deinen DBI Lehrer)
                    toDelete.Measurements.Clear();
                    db.Stations.Remove(toDelete);
                    db.SaveChanges();
                }
            }   // Schließt die DB, indem Dispose() aufgerufen wird.
        }
    }
}
```
