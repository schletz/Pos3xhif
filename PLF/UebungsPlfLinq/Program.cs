// *************************************************************************************************
// 2. PLF in POS1 (C#), 
// Prüfer: Schletz
// Kompetenzen: Generische Datentypen und Programmiertechniken, Serialisierung von Objekten

// Klasse: 3BHIF
// Datum:  19. Dezember 2018
// Name:   Vorname Zuname
//
// HINWEISE ZUR BEARBEITUNG DER AUFGABEN
// 1) Schreibe deinen Namen in die Kopfzeile dieser Datei (Zeile 8)
// 2) Aller Code wird in die Klasse TankstellenVerwaltung geschrieben. Das Hauptprogramm
//    ruft Methoden dieser Klasse auf und vergleicht sie mit der korrekten Ausgabe.
//    Ist die Ausgabe ident, gibt es 1 oder 2 Punkte (je nach Beispiel).
// 3) Es sind 8 Methoden, die in der unten stehenden Main Methode aufgerufen werden, 
//    zu implementieren. Mit F12 kannst du direkt zur Definition der Methode springen.
// 4) Die korrekte Ausgabe und die genaue Beschreibung jeder Methode ist in den 
//    Methodenkommentaren in der Klasse TankstellenVerwaltung.
// 5) Die Überprüfung ist ein exakter Stringvergleich. Achte daher genau darauf, dass
//    die Groß- und Kleinschreibung der Attribute und deren Reihenfolge genau so wie in der
//    korrekten Lösung sind.
// 6) Wenn eine Methode etwas falsches liefert, so wird die Ausgabe deiner Methode 
//    automatisch in die Konsole geschrieben. So kannst du sie mit der Musterlösung im 
//    Methodenkommentar vergleichen und den Fehler finden.
// 7) Falls eine Abfrage einen Laufzeitfehler verursacht der nicht behebbar ist, kannst du den
//    Aufruf der Methode in der Program Klasse auskommentieren.
// *************************************************************************************************

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TankstellenVw
{
    class Program
    {
        static void Main(string[] args)
        {
            string result, loesung;
            double prozent = 0;
            const int maxPunkte = 10;
            int note = 0;
            int[] punkte = new int[6];

            TankstellenVerwaltung tv = new TankstellenVerwaltung();
            TankstellenVw.Loesung.TankstellenVerwaltung tvLoesung = new TankstellenVw.Loesung.TankstellenVerwaltung();
            
            tv.LoadXml("TankstellenDaten.xml");
            tvLoesung.LoadXml("TankstellenDaten.xml");

            // Fertig gelöstes Muster: Liste alle Tankstellen.
            result = tv.ListeTankstellen("W");
            loesung = tvLoesung.ListeTankstellen("W");
            CheckResult(nameof(tv.ListeTankstellen), result, loesung, 0);

            // Die folgenden 6 aufgerufenen Methoden (bei result = ...) sind zu implementieren.
            result = tv.GetAnzahlVerkaeufe(1001);
            loesung = tvLoesung.GetAnzahlVerkaeufe(1001);
            punkte[0] = CheckResult(nameof(tv.GetAnzahlVerkaeufe), result, loesung, 1);

            result = tv.GetUmsaetze("N");
            loesung = tvLoesung.GetUmsaetze("N");
            punkte[1] = CheckResult(nameof(tv.GetUmsaetze), result, loesung, 1);

            result = tv.GetTageOhneShopverkaeufe("B");
            loesung = tvLoesung.GetTageOhneShopverkaeufe("B");
            punkte[2] = CheckResult(nameof(tv.GetTageOhneShopverkaeufe), result, loesung, 2);

            result = tv.GetMaxPreiseProTag();
            loesung = tvLoesung.GetMaxPreiseProTag();
            punkte[3] = CheckResult(nameof(tv.GetMaxPreiseProTag), result, loesung, 2);

            result = tv.GetPreismittelProBetreiberUndBundesland();
            loesung = tvLoesung.GetPreismittelProBetreiberUndBundesland();
            punkte[4] = CheckResult(nameof(tv.GetPreismittelProBetreiberUndBundesland), result, loesung, 2);

            result = tv.GetDatumDesMaxBenzinpreises();
            loesung = tvLoesung.GetDatumDesMaxBenzinpreises();
            punkte[5] = CheckResult(nameof(tv.GetDatumDesMaxBenzinpreises), result, loesung, 2);

            prozent = (double) punkte.Sum() * 100 / maxPunkte;
            note = prozent < 50 ? 5 : prozent < 62.5 ? 4 : prozent < 75 ? 3 : prozent < 87.5 ? 2 : 1;

            Console.WriteLine("ÜBERSICHT DER BEISPIELE UND PUNKTE");
            Console.WriteLine("BEISPIEL\t" + String.Join("\t", Enumerable.Range(1,6)));
            Console.WriteLine("  PUNKTE\t" + String.Join("\t", punkte));
            Console.WriteLine($"{punkte.Sum()} Punkte von {maxPunkte} erreicht. Note: {note}");
            Console.ReadLine();
        }

        public static int CheckResult(string title, string result, string loesung, int weight = 1)
        {
            if (result == null || result.Length < 20) return 0;
            if (result == loesung)
            {
                Console.WriteLine($"{title} OK.");
                return weight;
            }
            else
            {
                Console.WriteLine($"Fehler bei {title}. Gelieferte Ausgabe:");
                Console.WriteLine(result);
                return 0;
            }
        }
    }
}
