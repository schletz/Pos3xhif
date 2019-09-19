# Properties und Initializer in C# #
Betrachen wir eine Klasse *PupilJava*, mit den get und set Methoden, wie wir sie aus Java kennen:
```c#
class PupilJava
{
    string vorname;
    string zuname;
    int alter;

    public PupilJava(string vorname, string zuname)
    {
        this.vorname = vorname;
        this.zuname = zuname;
        setAlter(0);
    }

    public PupilJava(string vorname, string zuname, int alter)
    {
        this.vorname = vorname;
        this.zuname = zuname;
        setAlter(alter);
    }

    public void setAlter(int alter)
    {
        if (alter >= 0)
        {
            this.alter = alter;
        }
        else
        {
            throw new ArgumentException("Ungültiges Alter");
        }
    }

    public int getAlter()
    {
        return alter;
    }
}
```

Es fallen 2 Dinge auf:
- Die get und set Methoden, die für Datenprüfungen zuständig sind, haben oft den gleichen Aufbau.
- Es sind mehrere Konstruktoren vorhanden, je nach dem welche Variablen initialisiert werden sollen.

## Properties ersetzen get und set Methoden
Wir schreiben nun für die Felder *alter*, *vorname* und *zuname* sogenannte Properties. Properties erscheinen
nach außen wie Variablen, es wird aber bei der Zuweisung ein Stück Code - nämlich der in set - ausgeführt.
Das Schema ist folgendes:
- Anlegen der private Variable. Sie beginnen in C# mit einem Kleinbuchstaben.
- Anlegen der Properties. Sie beginnen in C# mit einem Großbuchstaben.
- Die get Methode kann beliebige Anweisungen enthalten. Sie muss allerdings einen Wert zurückgeben.
- Die set Methode kann auch beliebig aufgebaut sein. Der zugewiesene Wert ist in *value* enthalten.
```c#
class Pupil
{
    private string vorname, zuname;
    private int alter;

    public string Vorname
    {
        get { return vorname; }
        set { vorname = value; }
    }
    public string Zuname
    {
        get { return zuname; }
        set { zuname = value; }
    }
    public int Alter
    {
        get { return alter; }
        set { alter = value >= 0 ? value : throw new ArgumentException("Ungültiges Alter!"); }
    }
}
```
Oft ist folgender Fehler zu beobachten. *Alter* ist in der get Methode großgeschrieben, daher wird
eine endlose Rekursion erzeugt:
```c#
public int Alter
{
    get { return Alter; }
}
```

### Default Properties
Die Properties für Vorname und Zuname weisen nur 1:1 zu bzw. geben den Wert 1:1 zurück. In C# gibt
es mit den *Default Properties* einen eleganteren Weg, das zu bewerkstelligen. Der Compiler erledigt
folgende Dinge:
- Es wird automatisch eine private Variable im Hintergrund angelegt.
- Die get und set Methode liefert diese 1:1 zurück bzw. schreibt in diese hinein.
```c#
class Pupil
{
    public string Vorname { get; set; }
    public string Zuname { get; set; } 
}
```

Möchte man die Default Properties gleich initialisieren, ist dies seit C# 6 auch möglich:
```c#
class Pupil
{
    public string Vorname { get; set; } = "";
    public string Zuname { get; set; } = "";
}
```

### Read-only Properties
Wird nur eine get Methode definiert, so kann diesem Property nichts zugewiesen werden:
```c#
class Pupil
{
    public string Longname
    {
        get { return $"{Vorname} {Zuname}"; }
    }
}
```

## Verwendung der Properties, Initializer
Der große Vorteil von Properties liegt in ihrer eleganten Verwendung. Folgende Anweisungen sind
dadurch möglich:
```c#
Pupil p = new Pupil() { Vorname = "VN1", Zuname = "ZN1" };
p.Alter = 18;
```
Was passiert hier? Zuerst wird ein Objekt vom Typ Pupil mit *new Pupil()* erzeugt. In C# ist es durch
den *initializer* möglich, Properties gleich bei der Instanzierung zu initialisieren. Wir brauchen daher
keine Konstruktoren mehr, die einfach nur die Variablen initialisieren. Das Property kann nun wie eine 
public Variable gelesen oder geschrieben werden.

Da die set Methoden durchlaufen werden, wird die Datenprüfung natürlich auch im Initializer ausgeführt:
```c#
try
{
    Pupil p = new Pupil() { Vorname = "VN1", Zuname = "ZN1", Alter = -1 };
}
catch (ArgumentException)
{ 
    Console.Error.WriteLine("Argument Exception!");
}
```

## Übung
Es sind 2 Klassen zu implementieren: *Rechteck* und *Lehrer*. Für die Klasse *Rechteck* gelten folgende
Regeln:
- Es gibt 2 int Properties mit dem Namen *Laenge* und *Breite*.
- Wird diesen Properties ein Wert kleiner als 0 zugewiesen, wird mittels 
  *throw new ArgumentException("Ungültige Länge")*
  eine Exception geworfen.

Für die Klasse *Lehrer* gelten folgende Regeln:
- Die string Properties *Zuname* und *Vorname* sind Default Properties.
- Das Proeprty *Vorname* soll mit einem Leerstring initialisiert werden.
- Das Property *Bruttogehalt* soll decimal Werte speichern und den Standardwert null haben. Überlege
  dir den Datentyp, der diese Werte speichern kann. Für die Zuweisung gilt eine spezielle Regelung:
  Wurde schon ein Bruttogehalt zugewiesen (Wert also ungleich null), so darf dieser nicht überschrieben werden.
  Bei einer Zuweisung soll in diesem Fall einfach nichts passieren.
- Das Property *Kuerzel* soll read-only definiert werden. Es gibt die ersten 3 Stellen des Zunamens
  in Großschreibung zurück. Die Methoden *Substring()* und *ToUpper()* können hier verwendet werden.
  Um eine Exception bei nicht initialisiertem Zunamen zu vermeiden, verwende den Operator ?.
- Das Property *Nettogehalt* liefert 80% des Bruttogehaltes (also * 0.8). Beachte, dass decimal Literale
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

### Program.cs
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
            result = (l1.Nettogehalt == 3200 && l3.Nettogehalt == 0);
            Console.WriteLine($"Nettogehalt OK: {result}");
            // Teste das Kürzel
            result = (l2.Kuerzel == "EIF" && l3.Kuerzel == "");
            Console.WriteLine($"Kuerzel OK: {result}");
            Console.ReadLine();
        }
    }
}
```