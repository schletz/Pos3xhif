using System;
using System.Collections.Generic;
using System.Linq;
using LinqUebung2.Application.Model;

namespace LinqUebung2.Application
{
    internal class Program
    {
        private static void WriteJson(object? data, string title = "", bool indent = false)
        {
            string result = System.Text.Json.JsonSerializer.Serialize(
                data,
                new System.Text.Json.JsonSerializerOptions() { WriteIndented = indent });
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title);
            Console.ForegroundColor = oldColor;
            Console.WriteLine(result);
        }

        private static void Main(string[] args)
        {
            using ExamsDb db = ExamsDb.FromSeed();

            // *************************************************************************************
            // MUSTERBEISPIELE
            // Aktiviere die WriteJson Methode, um das Ergebnis anzuzeigen.
            // *************************************************************************************

            // 1. Liefere eine Liste aller Prüfungsfächer (IEnumerable<string>)
            //    Mit Select kann man sich aussuchen, welche Properties geliefert werden.
            //    Select nennt man auch "Projektion"
            IEnumerable<string> uebung1 = db.Exams.Select(e => e.Subject);
            //WriteJson(uebung1, "Beispiel 1 - Prüfungsfächer");

            // 2. Wie (1), nur sollen die Fächer nicht doppelt ausgegeben werden.
            IEnumerable<string> uebung2 = db.Exams.Select(e => e.Subject).Distinct();
            //WriteJson(uebung2, "Beispiel 2 - Eindeutige Prüfungsfächer");

            // 3. Liefere eine Liste aller Schüler mit der Anzahl der Prüfungen
            //     als Objekt Lastname, Firstname, ExamsCount
            //     Der Compiler legt eine anonyme Klasse an:
            //     class A {
            //       string Lastname {get;}
            //       string Firstname {get;}
            //       int ExamsCount {get;}
            //     }
            var uebung3 = db.Students.Select(s => new
            {
                s.Lastname,
                s.Firstname,
                ExamsCount = s.Exams.Count()   // Propertynamen festlegen
            }).OrderBy(s => s.ExamsCount).ThenBy(s => s.Lastname);
            //WriteJson(uebung3, "Beispiel 3 - Anzahl der Prüfungen");

            // 4. Welche Schüler haben mehr als 5 Prüfungen?
            var uebung4 = uebung3.Where(s => s.ExamsCount > 5);
            //WriteJson(uebung4, "Beispiel 4 - Schüler mit mehr als 5 Prüfungen", true);

            // 5. Liefere ein JSON Array der Schüler der 3AHIF mit folgendem Aufbau:
            //    [{
            //       Lastname: Mustermann,
            //       Firstname: Max,
            //       Examinators: [KY, FAV]
            //    },...]
            var uebung5 = db.Students
                .Where(s => s.Schoolclass == "3AHIF")
                .Select(s => new
                {
                    s.Lastname,
                    s.Firstname,
                    Examinators = s.Exams.Select(e => e.Examinator).Distinct()
                });
            //WriteJson(uebung5, "Beispiel 5 - Schüler der 3AHIF mit Prüfer", true);

            // 6. Liefere ein JSON Array der Schüler der 3CHIF mit folgendem Aufbau:
            //    [{
            //       Lastname: "Mustermann,"
            //       Firstname: "Max",
            //       Exams: [{"Examinator"="KY", "Subject"="AM"}, ...]
            //    },...]
            var uebung6 = db.Students
                .Where(s => s.Schoolclass == "3CHIF")
                .Select(s => new
                {
                    s.Lastname,
                    s.Firstname,
                    Exams = s.Exams.Select(e => new
                    {
                        e.Examinator,
                        e.Subject
                    })
                });
            //WriteJson(uebung6, "Beispiel 6 - Schüler der 3CHIF mit Prüfungen", true);

            
            // *************************************************************************************
            // ÜBUNGEN
            // Schreibe in den nachfolgenden Übungen statt der Zeile
            // object resultX = null;
            // die korrekte LINQ Abfrage. Verwende den entsprechenden Datentyp statt object.
            // *************************************************************************************

            // 1. Drucke eine Liste aller Fächer. Hinweis: Verwende Distinct, um doppelte Einträge
            //    zu entfernen. Speichere das Ergebnis in IEnumerable<string>.
            object result1 = null;
            Console.WriteLine("Die Prüfungsfächer sind " + string.Join(",", result1));

            // 2. Ermittle die schlechteste Prüfungsnote in E. Dabei verwende allerdings die Max
            //    Funktion ohne Parameter, indem vorher eine Liste von int Werten erzeugt wird.
            object result2 = null;
            Console.WriteLine($"Die schlechteste E Note ist {result2}");

            // 3. Liefere eine Liste mit Klasse, Name, Vorname und der Anzahl der Prüfungen
            //    (soll Anzahl heißen) der 3CHIF. Sortiere die Liste nach Klasse und Name.
            //    Hinweis: Verwende OrderBy und dann ThenBy.
            object result3 = null;
            Console.WriteLine("Beispiel 3");
            result3.ToList().ForEach(s => { Console.WriteLine($"   {s.Schoolclass}: {s.Lastname} {s.Firstname} hat {s.ExamsCount} Prüfungen."); });

            // 4. Liefere eine Liste der Schüler (Klasse, Name, Vorname) der 3BHIF mit ihren Prüfungsfächern.
            //    Die Fächer sollen als Stringliste (IEnumerable<string> ohne doppelte Einträge
            //    in das Property Subjects gespeichert werden.
            object result4 = null;
            Console.WriteLine("Beispiel 4");
            result4.ToList().ForEach(s => { Console.WriteLine($"   {s.Schoolclass}: {s.Lastname} {s.Firstname} hat {string.Join(",", s.Subjects)}"); });

            // 5. Liefere eine Liste aller Schüler der 3AHIF (Lastname, Firstname) mit ihren negativen
            //    Prüfungen. Dabei soll unter dem Property "FailedExams"
            //    ein neues Objekt mit Datum und Fach aller negativen Prüfungen (Note = 5)
            //    erzeugt werden. Es sind natürlich 2 new Anweisungen nötig.
            //    Außerdem sollen nur Schüler der 3AHIF berücksichtigt werden.
            object result5 = null;
            Console.WriteLine("Beispiel 5");
            foreach (var student in result5)
            {
                Console.WriteLine($"   Negative Prüfungen von {student.Lastname} {student.Firstname}:");
                foreach (var exam in student.FailedExams)
                {
                    Console.WriteLine($"      {exam.Subject} am {exam.Date:dd.MM.yyyy}");
                }
            }

            // 6. Liefere eine Liste aller Prüfer mit ihren besten und schlechtesten
            //    Prüfungsergebnis. Gehe dabei so vor: Erzeuge mit Select und Distinct
            //    eine Stringliste aller Prüfer. Davon ausgehend erzeuge - wieder mit Select -
            //    ein neues Objekt mit den Properties Pruefer, BesteNote und SchlechtesteNote.
            //    BesteNote und SchlechtesteNote werden wieder von allen Prüfungen mit entsprechdem
            //    Where Filter berechnet.
            //    Später werden wir dieses Problem mit Group by lösen.
            object result6 = null;
            Console.WriteLine("Beispiel 6");
            result6.ToList().ForEach(e => { Console.WriteLine($"   {e.Examinator} gibt Noten von {e.BestGrade} bis {e.WorstGrade}"); });
            
        }
    }
}