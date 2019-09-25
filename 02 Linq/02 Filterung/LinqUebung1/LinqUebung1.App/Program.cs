using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LinqUebung1.App.Model;

namespace LinqUebung1.App
{
    class Program
    {
        static void Main(string[] args)
        {
            PruefDb db = PruefDb.FromMockup();

            // *************************************************************************************
            // MUSTERBEISPIELE
            // *************************************************************************************
            // 1. Suche den Schüler mit der ID 1003
            //    Where liefert IEnumerable, also immer eine Collecion.
            //    Deswegen brauchen wir First, um auf das erste Element zugreifen
            //    zu können.
            Schueler result1 = db.Schuelers.Where(s => s.Id == 1003).First();

            // 2. Welcher Schüler hat die Id 999?
            //    First liefert eine Exception, da die Liste leer ist.
            //    FirstOrDefault liefert in diesem Fall den Standardwert (null).
            Schueler result2 = db.Schuelers.Where(s => s.Id == 999).FirstOrDefault();

            // 3. Wie viele Schüler sind in der Liste?
            int result3 = db.Schuelers.Count();

            // 4. Wie viele Schüler gehen in die 3BHIF?
            int result4 = db.Schuelers.Where(s => s.Klasse == "3BHIF").Count();
            //    Für Count existiert eine Überladung, die auch eine Filterfunktion
            //    bekommen kann.
            result4 = db.Schuelers.Count(s => s.Klasse == "3BHIF");

            // *************************************************************************************
            // ÜBUNGEN
            // *************************************************************************************
            // 5. Welche Note hat die Prüferin FAV bei ihrer schlechtesten Prüfung vergeben.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result5 (kein var verwenden!).
            Console.WriteLine($"Beispiel 5: FAV gab schlechtestens die Note {result5}.");

            // 6. Welchen Notendurchschnitt haben die weiblichen Schülerinnen in POS?
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result6 (kein var verwenden!).
            Console.WriteLine($"Beispiel 6: Notenschnitt der Schülerinnen in POS: {result6:0.00}");

            // 7. Welche Schüler haben 6 oder mehr Prüfungen? Gib eine Liste von Schülern zurück und gib Sie aus.
            Console.WriteLine("Beispiel 7: Schüler mit mehr als 6 Prüfungen.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result7 (kein var verwenden!).
            result7.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} hat mehr 6 oder mehr Prüfungen."); });

            // 8. Welche Schüler haben eine DBI Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombiniere Where und Any, indem Any in der Where Funktion verwendet wird.
            Console.WriteLine("Beispiel 8: Schüler mit DBI Prüfungen.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result8 (kein var verwenden!).
            result8.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} hat eine DBI Prüfung."); });

            // 9. Gibt es Schüler, die nur in POS eine Prüfung haben? Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombiniere Where und All, indem All in der Where Funktion verwendet wird.
            Console.WriteLine("Beispiel 9: Schüler, die nur in POS eine Prüfung haben.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result9 (kein var verwenden!).
            result9.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} hat nur in POS eine Prüfung."); });

            // 10. Welche Schüler haben keine POS Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombinieren Where und Any, indem Any in der Where Funktion verwendet wird.
            Console.WriteLine("Beispiel 10: Schüler, die keine POS Prüfung haben.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result10 (kein var verwenden!).
            result10.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} hat keine POS Prüfung."); });

            // 11. Welche Schüler haben überhaupt keine Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //     Hinweis: Verwende das Count Property.
            Console.WriteLine("Beispiel 11: Schüler, die überhaupt keine Prüfung haben.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result11 (kein var verwenden!).
            result11.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} hat keine Prüfung."); });

            // 12. Welche Schüler hatten in Juni AM Prüfungen? Gib eine Liste von Prüfungen zurück und gib sie mit dem Schülernamen aus.
            //     Hinweis: Verwende das Month Property des Datum Feldes.
            Console.WriteLine("Beispiel 12: Schüler, die im Juni eine AM Prüfung hatten.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result12 (kein var verwenden!).
            result12.ToList().ForEach(p => { Console.WriteLine($"   {p.Schueler.Name} {p.Schueler.Vorname} hat bei {p.Pruefer} eine Prüfung in AM."); });

            // 13. Welche Schüler haben bei einer AM Prüfung eine negative Note, aber in E nur positive Prüfungen?
            Console.WriteLine("Beispiel 13: Schüler, die in AM einmal negativ, aber in E immer positiv waren.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result13 (kein var verwenden!).
            result13.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} war in AM negativ, in E aber nie."); });

            // 14. Welche Schüler haben im Mittel bessere DBI Prüfungen als D Prüfungen. Anders gesagt: Bei wem
            //     ist der Notenschnitt der DBI Prüfungen kleiner als der Notenschnitt der D Prüfungen.
            //     Gib eine Liste von Schülern zurück, auf die das zutrifft.
            //     Hinweise:
            //       -) Wenn mit Where gefiltert wird, kann es sein, dass eine leere Liste zurückkommt.
            //       -) Average kann nur von einer Liste mit Elementen aufgerufen werden.
            //       -) Erstelle daher eine Lambda Expression mit try und catch im inneren, die false im Fehlerfall liefert.
            Console.WriteLine("Beispiel 14: Schüler, in DBI bessere Prüfungen (Notenschnitt) als in D hatten.");
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result14 (kein var verwenden!).
            result14.ToList().ForEach(s => { Console.WriteLine($"   {s.Name} {s.Vorname} ist in DBI besser als in D."); });


        }
        static void WriteJson(object data)
        {
            string result = JsonConvert.SerializeObject(data);
            Console.WriteLine(result);


        }
    }


}

