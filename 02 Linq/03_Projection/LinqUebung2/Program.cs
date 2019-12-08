using System;
using System.Collections.Generic;
using System.Linq;
using LinqUebung2.Model;
using Newtonsoft.Json;

namespace LinqUebung2
{
    class Program
    {
        static void WriteJson(object data, string title = "", bool indent = false)
        {
            string result = JsonConvert.SerializeObject(data, indent ? Formatting.Indented : Formatting.None);
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(title);
            Console.ForegroundColor = oldColor;
            Console.WriteLine(result);
        }

        static void Main(string[] args)
        {
            PruefDb db = PruefDb.FromMockup();

            // *************************************************************************************
            // MUSTERBEISPIELE
            // *************************************************************************************

            // Liefere eine Liste aller Prüfungsfächer (IEnumerable<string>)
            // Mit Select kann man sich aussuchen, welche Properties geliefert werden.
            // Select nennt man auch "Projektion"
            IEnumerable<string> uebung1 = db.Pruefungen.Select(p => p.Fach);
            // Liste beinhaltet D, E, E, AM, D, AM, ...
            IEnumerable<string> uebung2 = db.Pruefungen.Select(p => p.Fach).Distinct();
            // Liste beinhaltet D, E, AM, POS, DBI (jedes Fach nur 1x)

            // Liefere eine Liste aller Schüler mit der Anzahl der Prüfungen
            // als Objekt Name, Vorname, Anzahl
            // Der Compiler legt eine anonyme Klasse an:
            // class A {
            //   string Name {get;set;}
            //   string Vorname {get;set;}
            //   int Anzahl {get;set;}
            // }
            var uebung3 = db.Schuelers.Select(s => new
            {
                s.Name,
                s.Vorname,
                Anzahl = s.Pruefungen.Count   // Propertynamen festlegen
            }).OrderBy(x => x.Anzahl).ThenBy(x => x.Name);

            // Welche Schüler haben mehr als 6 Prüfungen?
            var uebung4 = uebung3.Where(p => p.Anzahl > 6);

            // Liefere ein JSON Objekt mit folgendem Aufbau:
            // {
            //    Name: Mustermann,
            //    Vorname: Max,
            //    Pruefer: [KY, FAV]
            // },...
            var uebung5 = db.Schuelers.Select(s => new
            {
                s.Name,
                s.Vorname,
                Pruefer = s.Pruefungen.Select(p => p.Pruefer).Distinct()
            });
            //WriteJson(uebung5, "Beispiel 5 - Schüler mit Prüfer", true);


            // Liefere ein JSON Objekt mit folgendem Aufbau:
            // {
            //    Name: "Mustermann,"
            //    Vorname: "Max",
            //    db.Pruefungen: [{"Pruefer"="KY", "Fach"="AM"}, ...]
            // },...
            var uebung6 = db.Schuelers.Select(s => new
            {
                s.Name,
                s.Vorname,
                Pruefungen = s.Pruefungen.Select(p => new
                {
                    p.Pruefer,
                    p.Fach
                })
            });
            //WriteJson(uebung6, "Beispiel 6 - Schüler mit Prüfungen", true);


            // *************************************************************************************
            // ÜBUNGEN
            // *************************************************************************************

            // 1. Drucke eine Liste aller Fächer. Hinweis: Verwende Distinct, um doppelte Einträge
            //    zu entfernen. Speichere das Ergebnis in IEnumerable<string>.
            // Füge hier die Lösung ein.
            // IEnumerable<Pruefung> --> IEnumerable<string>

            IEnumerable<string> result1 = db.Pruefungen.Select(p => p.Fach).Distinct();
            Console.WriteLine("Die Prüfungsfächer sind " + String.Join(",", result1));

            // 2. Ermittle die schlechteste Prüfungsnote in E. Dabei verwende allerdings die Max
            //    Funktion ohne Parameter, indem vorher eine Liste von int Werten erzeugt wird.
            // Füge hier die Lösung ein.       
            // IEnumerable<Pruefung> --> IEnumerable<int>
            int result2 = db.Pruefungen.Where(p => p.Fach == "E").Select(p => p.Note).Max();
            Console.WriteLine($"Die schlechteste E Note ist {result2}");

            // 3. Liefere eine Liste mit Klasse, Name, Vorname und der Anzahl der Prüfungen
            //    (soll Anzahl heißen) der 3CHIF. Sortiere die Liste nach Klasse und Name.
            //    Hinweis: Verwende OrderBy und dann ThenBy.
            // Füge hier die Lösung ein.       
            var result3 = db.Schuelers
                .Where(s => s.Klasse == "3CHIF")
                .Select(s => new
                {
                    s.Klasse,
                    s.Name,
                    s.Vorname,
                    Anzahl = s.Pruefungen?.Count() ?? 0
                })
                .OrderBy(x => x.Klasse)
                .ThenBy(x => x.Name);
            Console.WriteLine("Beispiel 3");
            result3.ToList().ForEach(s => { Console.WriteLine($"   {s.Klasse}: {s.Name} {s.Vorname} hat {s.Anzahl} Prüfungen."); });

            // 4. Liefere eine Liste der Schüler (Klasse, Name, Vorname) der 3BHIF mit ihren Prüfungsfächern. 
            //    Die Fächer sollen als Stringliste (IEnumerable<string> ohne doppelte Einträge 
            //    in das Property Faecher gespeichert werden.
            // Füge hier die Lösung ein.
            var result4 = db.Schuelers
                .Where(s => s.Klasse == "3BHIF")
                .Select(s => new
                {
                    s.Klasse,
                    s.Name,
                    s.Vorname,
                    Faecher = s.Pruefungen.Select(p => p.Fach).Distinct()
                });
            Console.WriteLine("Beispiel 4");
            result4.ToList().ForEach(s => { Console.WriteLine($"   {s.Klasse}: {s.Name} {s.Vorname} hat {String.Join(",", s.Faecher)}"); });

            // 5. Liefere eine Liste aller Schüler der 3AHIF (Name, Vorname) mit ihren negativen 
            //    Prüfungen. Dabei soll unter dem Property "NegativePruefungen"
            //    ein neues Objekt mit Datum und Fach aller negativen Prüfungen (Note = 5) 
            //    erzeugt werden. Es sind natürlich 2 new Anweisungen nötig.
            //    Außerdem sollen nur Schüler der 3AHIF berücksichtigt werden.
            // Füge hier die Lösung ein.
            Console.WriteLine("Beispiel 5");
            var result5 = db.Schuelers
                .Where(s => s.Klasse == "3AHIF")
                .Select(s => new
                {
                    s.Name,
                    s.Vorname,
                    NegativePruefungen = s.Pruefungen
                                               .Where(p => p.Note == 5)
                                               .Select(p => new
                                               {
                                                   p.Fach,
                                                   p.Datum
                                               })
                });
            foreach (var schueler in result5)
            {
                Console.WriteLine($"   Negative Prüfungen von {schueler.Name} {schueler.Vorname}:");
                foreach (var pruef in schueler.NegativePruefungen)
                {
                    Console.WriteLine($"      {pruef.Fach} am {pruef.Datum.ToString("dd.MM.yyyy")}");
                }
            }

            // 6. Liefere eine Liste aller Prüfer mit ihren besten und schlechtesten 
            //    Prüfungsergebnis. Gehe dabei so vor: Erzeuge mit Select und Distinct
            //    eine Stringliste aller Prüfer. Davon ausgehend erzeuge - wieder mit Select - 
            //    ein neues Objekt mit den Properties Pruefer, BesteNote und SchlechtesteNote.
            //    BesteNote und SchlechtesteNote werden wieder von allen Prüfungen mit entsprechdem
            //    Where Filter berechnet.
            //    Später werden wir dieses Problem mit Group by lösen.
            // Füge hier die Lösung ein.
            var pruefer = db.Pruefungen.Select(p => p.Pruefer).Distinct();
            var result6 = pruefer.Select(p => new
            {
                Pruefer = p,
                BesteNote = db.Pruefungen.Where(pr => pr.Pruefer == p).Min(pr => pr.Note),
                SchlechtesteNote = db.Pruefungen.Where(pr => pr.Pruefer == p).Max(pr => pr.Note)
            });
            Console.WriteLine("Beispiel 6");
            result6.ToList().ForEach(p => { Console.WriteLine($"   {p.Pruefer} gibt Noten von {p.BesteNote} bis {p.SchlechtesteNote}"); });
        }
    }


}

