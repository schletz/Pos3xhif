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
using System.Data.SqlClient;  // Für SqlCommand
using CrudTest.Model;

namespace CrudTest
{
    class Program
    {
        // Wir verwenden immer kurzlebige Verbindungen, damit die DB Verbindung nicht während
        // der ganzen Laufzeit des Programmes aktiv bleibt.
        static void Main(string[] args)
        {
            // Da die Datenbank die Kommastellen bei den Millisekunden abschneidet, generieren
            // wir einen Zeitwert, der einfach auf ganze Sekunden genau ist.
            DateTime now = new DateTime(DateTime.Now.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);

            // *************************************************************************************
            // CREATE
            // Erstelle eine neue Station und füge ein paar Measurements hinzu.
            // *************************************************************************************
            Random rnd = new Random();
            int stationId = rnd.Next();  // Immer anderer Key, wenn wir mehrmals starten.
            Station myStation = new Station
            {
                S_ID = stationId,
                S_Location = "Spengergasse",
                S_Height = 170
            };

            // Wir nutzen die Navigation von Station, um Werte einzufügen.
            myStation.Measurements.Add(new Measurement
            {
                // M_Station wird NICHT angegeben, der Fremdschlüssel wird dann vom
                // Entity Framework gesetzt, da ich in die Collection von myStation
                // schreibe.
                M_Date = now,
                M_Temperature = 12
            });

            using (WeatherDb db = new WeatherDb())
            {
                // Mit Add fügen wir neue Objekte hinzu. Die Measurements werden auch
                // geschrieben.
                db.Stations.Add(myStation);
                // Hier wird auch der Fremdschlüsselwert im Measurement von 
                // myStation (M_Station) auf den PK von myStation (S_ID) gesetzt.
                db.SaveChanges();
            }

            // Nachgrägliches Einfügen von Measurements
            Measurement newMeasurement = new Measurement
            {
                // Auch hier kein M_Station, es wird dann vom OR Mapper gesetzt.
                M_Date = now.AddHours(1),
                M_Temperature = 12
            };

            using (WeatherDb db = new WeatherDb())
            {
                // Sonst gibt es einen Fehler, dass myStation nicht bekannt ist.
                db.Stations.Attach(myStation);
                myStation.Measurements.Add(newMeasurement);
                db.SaveChanges();
            }

            // *************************************************************************************
            // UPDATE
            // Aktualisiere die Location von myStation von Spengergasse
            // auf Spengergasse 20.
            // *************************************************************************************
            myStation.S_Location = "Spengergasse 20";
            using (WeatherDb db = new WeatherDb())
            {
                // myStation wird wieder dem OR Mapper bekannt gemacht. myStation muss
                // aber in der DB existieren (sonst wäre es ja Add).
                db.Stations.Attach(myStation);
                // Wir haben das Objekt ja geändert, auch das müssen wir dem OR Mapper
                // mitteilen.
                db.Entry(myStation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            // *************************************************************************************
            // DELETE
            // Würde mit dem OR Mapper mit db.Stations.Remove(myStation) gehen, aber
            // hier gibt es eine ConcurrencyException. Deswegen "rohe" Lösung mit
            // SQL.
            // *************************************************************************************
            using (WeatherDb db = new WeatherDb())
            {
                db.Stations.Attach(myStation);
                // Bei einer 1:n Beziehung müssen alle n Datensätze gelöscht werden.
                myStation.Measurements.Clear();
                db.Stations.Remove(myStation);
                db.SaveChanges();

                // Wir können auch SQL hinschicken.
                // db.Database.ExecuteSqlCommand("DELETE FROM Measurement WHERE M_Station = @StationId",
                //    new SqlParameter("@StationId", myStation.S_ID));
                // db.Database.ExecuteSqlCommand("DELETE FROM Station WHERE S_ID = @StationId",
                //    new SqlParameter("@StationId", myStation.S_ID));
            }
        }
    }
}
```
