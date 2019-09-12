# Übung zu Properties

Es sind 2 Klassen zu implementieren: *Rechteck* und *Lehrer*. Für die Klasse *Rechteck* gelten folgende
Regeln:
- Es gibt 2 int Properties mit dem Namen *Laenge* und *Breite*.
- Wird diesen Properties ein Wert kleiner als 0 zugewiesen, wird mittels 
  *throw new ArgumentException("Ungültige Länge")*
  eine Exception geworfen.

Für die Klasse Lehrer gelten folgende Regeln:
- Die string Properties Zuname und Vorname sind Default Properties.
- Das Proeprty Vorname soll mit einem Leerstring initialisiert werden.
- Das Property *Bruttogehalt* soll decimal Werte speichern und den Standardwert null haben. Überlege
  dir den Datentyp, der diese Werte speichern kann. Für die Zuweisung gilt eine spezielle Regelung:
  Wurde schon ein Bruttogehalt zugewiesen (Wert also ungleich null), so darf dieser nicht überschrieben werden.
  Bei einer Zuweisung soll in diesem Fall einfach nichts passieren.
- Das Property Kuerzel soll read-only definiert werden. Es gibt die ersten 3 Stellen des Zunamens
  in Großschreibung zurück. Die Methoden *Substring()* und *ToUpper()* können hier verwendet werden.
  Um eine Exception bei nicht initialisiertem Zunamen zu vermeiden, verwende den Operator ?.
- Das Property Nettogehalt liefert 80% des Bruttogehaltes (also * 0.8). Beachte, dass decimal Literale
  mit M enden müssen (also 0.8M). Ist das Bruttogehalt null, so soll das Nettogehalt 0 sein. Der Wert
  von Nettogehalt ist also niemals null. Löse diese Berechnung mit dem ?? Operator.

Erstelle ein neues Projekt mit dem Titel *ExProperties*. Ersetze danach die Datei *Program.cs* durch die
untenstehende Version. Implementiere deine Klassen in eigenen Dateien. Es dürfen keine nicht abgefangenen 
Exceptions auftreten. Die Ausgabe des Programmes muss dann wie folgt lauten. 
```
Fläche OK: True
Exception bei Länge und Breite OK: True
Vor- und Zuname Initialisierung OK: True
Bruttogehalt Initialisierung OK: True
Bruttogehalt Zuweisung OK: True
Nettogehalt OK: True
Kuerzel OK: True
```

## Program.cs
```c#
using System;

namespace ExProperties.App
{
    class Program
    {
        static void Main(string[] args)
        {
            bool result;
            // Teste die Klasse Rechteck
            Rechteck r1 = new Rechteck();
            Rechteck r2 = new Rechteck() { Laenge = 10, Breite = 20 };

            // Fläche OK?
            result = (r2.Flaeche == 200);
            Console.WriteLine($"Fläche OK: {result}");
            // Ungültige Zuweisung wird richtig erkannt:
            try
            {
                r1.Laenge = -1;
                result = false;
            }
            catch (ArgumentException)
            {
                try
                {
                    r1.Breite = -1;
                    result = false;
                }
                catch (ArgumentException)
                {
                    result = true;
                }
            }
            Console.WriteLine($"Exception bei Länge und Breite OK: {result}");

            // TESTE DIE KLASSE LEHRER
            Lehrer l1 = new Lehrer() { Vorname = "Heinrich", Zuname = "Schlau", Bruttogehalt = 4000 };
            Lehrer l2 = new Lehrer() { Vorname = "Daniela", Zuname = "Eifrig"};
            Lehrer l3 = new Lehrer();
            // Teste die Initialisierung von Zuname
            result = (l3.Vorname == "" && l3.Zuname == null);
            Console.WriteLine($"Vor- und Zuname Initialisierung OK: {result}");
            // Teste das Property Bruttogehalt
            result = (l1.Bruttogehalt == 4000 && l2.Bruttogehalt == null);
            Console.WriteLine($"Bruttogehalt Initialisierung OK: {result}");
            // Teste die Zuweisung von Bruttogehalt
            l1.Bruttogehalt = 20;
            l2.Bruttogehalt = 2000;
            result = (l1.Bruttogehalt == 4000 && l2.Bruttogehalt == 2000);
            Console.WriteLine($"Bruttogehalt Zuweisung OK: {result}");
            // Teste das Nettogehalt
            result = (l1.Nettogehalt == 3200);
            Console.WriteLine($"Nettogehalt OK: {result}");
            // Teste das Kürzel
            result = (l2.Kuerzel == "EIF" && l3.Kuerzel == "");
            Console.WriteLine($"Kuerzel OK: {result}");
            Console.ReadLine();
        }
    }
}
```