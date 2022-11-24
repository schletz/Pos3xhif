/* *************************************************************************************************
 * 2. PRÜFUNG IN C#: FLIGHT MONITOR
 * 3BHIF, 9. Jan 2020
 * Datenquelle: https://www.viennaairport.com/jart/prj3/va/data/flights/out.json
 * *************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FlightMonitor.Domain;

namespace FlightMonitor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int[] points = new int[8];
            int i = 0;
            FlightContext db = FlightContext.FromFile("departure.json");
            var results = ReadResults("results.json");

            // Musterbeispiel: Liefere Infos zum Flughafen Dublin (IATA Code ist DUB)
            IEnumerable<Destination> result = db.Destinations.Where(d => d.IataCode == "DUB");


            // Beispiel 1: Welche Airline hat keine Webseite (das Property WebSite ist null oder leer).
            //             Liefere eine Liste von Strings mit dem Namen der Airline (Property Name)
            IEnumerable<string> result1 = null!;
            points[i++] = CheckResult(results["result1"], result1);

            // Beispiel 2: Welche Städte sind in Österreich (NameEn ist "Austria")? Liefere eine
            //             Liste von Strings mit dem deutschen Namen (NameDe).
            IEnumerable<string> result2 = null!;
            points[i++] = CheckResult(results["result2"], result2);

            // Beispiel 3: Welche Städte in Deutschland (Country.NameEn ist "Germany") wurden 
            //             zwischen 5:00 und 5:59 UTC (Departure.Scheduledatetime.Hour ist 5)
            //             angeflogen? Liefere eine Stringliste mit dem deutschen Namen 
            //             (City.NameDe). Filtere doppelte Werte heraus.
            //             Beachte, dass ein Abflug mehrere Stopps (Destinations) haben kann.
            //             Wenn bei einem dieser Stopps eine deutsche Stadt dabei ist, ist diese
            //             aufzulisten.
            IEnumerable<string> result3 = null!;
            points[i++] = CheckResult(results["result3"], result3);


            // Beispiel 4: Welche Flugzeugtypen (Aircraft.Description) von Boeing sind im
            //             Datenbestand vorhanden? Der Wert von Description beginnt bei diesen 
            //             Flugzeugen mit dem String "Boeing". Verwende string.StartsWith() für diese
            //             überprüfung. Liefere eine Stringliste mit den Description Feldern dieser
            //             Flugzeuge. Sortiere die Werte der Liste aufsteigend.
            IOrderedEnumerable<string> result4 = null!;
            points[i++] = CheckResult(results["result4"], result4);

            // Beispiel 5: Wie viele Abflüge fanden pro Terminal statt? Das Terminal ist in
            //             Checkin gespeichert. Betrachte nur die Daten, bei denen der Wert von
            //             Terminal nicht null ist.
            var result5 = null as object;
            points[i++] = CheckResult(results["result5"], result5);

            // Beispiel 6: Wie groß ist die durchschnittliche Verspätung? Betrachte nur die Daten
            //             in Departues, wo Actualdatetime größer als Scheduledatetime ist.
            //             Der Mittelwert ist in Minuten zu ermitteln. Datumswerte können in C#
            //             einfach subtrahiert werden. Das Ergebnis ist ein TimeSpan, wo das
            //             Property TotalMinutes verwendet werden kann.
            //             Runde das Ergebnis mit Math.Round() auf 2 Kommastellen.
            double result6 = 0;
            points[i++] = CheckResult(results["result6"], result6);

            // Beispiel 7: Wie oft wurden Städte in Österreich angeflogen? Filtere die Daten nach
            //             Country.NameEn gleich "Austria". Gib eine Liste mit der deutschen
            //             Bezeichnung der Stadt (City.NameDe) und der Anzahl zurück.
            var result7 = null as object;
            points[i++] = CheckResult(results["result7"], result7);


            // Beispiel 8: Welche Airline (Airline.Name) flog am Häufigsten ab? Ermittle dafür
            //             zuerst die Anzahl der häufigsten Abflüge. Danach verwende diesen Wert,
            //             um deine Gruppierungsabfrage nach diesem Wert zu filtern.
            var result8 = null as object;
            points[i++] = CheckResult(results["result8"], result8);

            // *************************************************************************************
            Console.WriteLine("Beispiel " + string.Join(" ", Enumerable.Range(1, 8)));
            Console.WriteLine("Punkte   " + string.Join(" ", points));
            int sum = points.Sum();
            double percent = sum / 8.0;
            int note = percent > 0.875 ? 1 : percent > 0.75 ? 2 : percent > 0.625 ? 3 : percent > 0.5 ? 4 : 5;
            Console.WriteLine($"Note: {note}");

        }

        public static void WriteResults(string filename, object data)
        {
            using FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            using Utf8JsonWriter writer = new Utf8JsonWriter(fs);
            JsonSerializer.Serialize(new Utf8JsonWriter(fs), data);
        }

        public static Dictionary<string, string> ReadResults(string filename)
        {
            var dict = new Dictionary<string, string>();
            using FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var doc = JsonDocument.Parse(fs).RootElement;
            foreach (var result in doc.EnumerateObject())
            {
                dict.Add(result.Name, result.Value.GetRawText());
            }
            return dict;
        }

        private static int CheckResult<T>(string result, T asserted)
        {
            string json = JsonSerializer.Serialize(asserted);
            if (string.IsNullOrEmpty(json)) { return 0; }
            if (result == json)
            {
                Console.WriteLine("OK");
                return 1;
            }
            else
            {
                Console.WriteLine("GELIEFERTES ERGEBNIS: " + json);
                Console.WriteLine("ERWARTETES  ERGEBNIS: " + result);
                return 0;
            }

        }
    }
}
