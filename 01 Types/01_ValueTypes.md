# Wertedatentypen in C#

## Erstellen einer Visual Studio Solution

Um die Beispiele mitmachen zu können, muss eine .NET Konsolenapplikation erstellt werden. Führe
dafür die folgenden Befehle in der Konsole aus. Unter macOs müssen md und rd durch die entsprechenden
Befehle ersetzt werden.

```text
rd /S /Q TypesDemo
md TypesDemo
cd TypesDemo
md TypesDemo.Application
cd TypesDemo.Application
dotnet new console
cd ..
dotnet new sln
dotnet sln add TypesDemo.Application
start TypesDemo.sln
```

Öffne danach durch Doppelklick auf das Projekt (*TypesDemo.Application*) und füge die Optionen für
Nullable und TreatWarningsAsError hinzu. Die gesamte Konfiguration muss nun so aussehen:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

</Project>
```

## Einführung

Wertedatentypen belegen bei der Deklaration schon Speicher für ihren Wert. Sie
können daher - im Gegensatz zu Referenztypen - nicht null enthalten.

Neben den bekannten Datentypen aus Java gibt es in C# noch unsigned Datentypen
wie *uint*, *ulong*, ... Sie werden allerdings selten verwendet, da die Frameworkmethoden mit "normalen" 
signed Typen wie int, ... arbeiten.

Die Dekleration erfolgt wie aus Java gewohnt:

```c#
int myInt = 1;
bool myBool = true;
long myLong;   // Wird mit 0 initialisiert.
```

## Gleitkommatypen: double und decimal

Folgendes Programm liefert eine Endlosschleife, da 0.1 nicht exakt gespeichert werden
kann:
```c#
double sum = 0;
// Vergleiche mit double sind durch die Ungenauigkeit
// kritisch. 1000000 wird nie erreicht.
while (sum != 1000000)
{
    sum += 0.1;
}
```

### decimal

Möchte man Dezimalzahlen exakt speichern, kann der Typ *decimal* verwendet werden.
Die Performance ist allerdings schlechter als bei *double*:

```c#
decimal sum2 = 0;
while (sum2 != 1000000)
{
    sum2 += 0.1M;
}
```

## Explizite und implizite Typencasts

![](https://flylib.com/books/4/8/1/html/2/files/09fig04.gif)
<sup>Quelle: https://flylib.com/books/en/4.8.1.83/1/</sup>

Kann jeder *mögliche* Wert (unabhängig vom aktuellen Wert bei der Zuweisung) zugewiesen werden, ist
ein impliziter Typencast möglich.
Beispiel:

```c#
myLong = myInt;
myInt = myLong; // Geht nicht
```

Möchte man trotzdem eine Zuweisung zu einen "kürzeren" Typ machen, brauchen wir den
expliziten Typencast:

```c#
myInt = (int) myLong; // Wird notfalls abgeschnitten
```

## Nullable value types

Eine Zuweisung von null ist bei Wertetypen wie *int*, *long*, ... nicht möglich. Ist ein Wert
nicht bekannt, findet man oft folgenden Code:

```c#
int count = -1;
...
```

Es wird also -1 verwendet, da es nie eine gültige Anzahl sein kann. Allerdings ergeben sich daraus
folgende Probleme:

- Sie wissen, dass -1 der "Spezialwert" für unbekannt ist. Für Außenstehende ist das nicht klar.
- Wird der Wert versehentlich in einer Berechnung verwendet, wird die Berechnung ohne Fehler
  durchgeführt, aber das Ergebnis ist sinnlos.

Deswegen geht man in C# einen anderen Weg: Man kann sogenannte *nullable Types* definieren. Das sind
Wertetypen, die auch *null* speichern können. Man erkennt sie an dem ? nach dem Typnamen
(int?, double?, ...). Überlegen Sie sich, warum das bei Klassen nicht funktioniert bzw. sinnlos ist.

```c#
int? myInt2;    // ? bedeutet nullable.
myInt2 = null;  // Gültige Zuweisung
```

Allerdings ist im weiteren Programmverlauf darauf zu achten, dass das Ergebnis einer Berechnung
mit einem nullable Type auch *null* liefern kann.

```c#
myInt = myInt2 + 1;  // Geht nicht, da es null liefern kann und myInt ein "normaler" int ist.
```

Es gibt allerdings den *null-coalescing Operator* ??. Er liefert den zweiten Wert falls
der erste Wert null ist:

```c#
myInt2 ?? 0;  // Liefert 0
1 ?? 2;       // Liefert 1
```

Durch diesen Operator können wir unsere Berechnung in myInt schreiben. Ob 0 ein sinnvoller Standardwert
ist, hängt natürlich von der Aufgabenstellung ab.

```c#
myInt = myInt2 ?? 0 + 2;  // Liefert 1, wenn myInt2 null ist. Sehen Sie auf
                          // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/
                          // nach, welcher Operator zuerst ausgeführt wird.
```

Eine weitere Möglichkeit zu prüfen, ob ein nullable Type einen Wert hat, ist die Eigenschaft
*HasValue*. Eigentlich müsste das nachfolgende Codestück eine Exception liefern, wenn *myInt2* den
Wert *null* hat. Schließlich versuchen wir, eine Eigenschaft einer Variable, die *null* ist, abzurufen.

Das folgende Codestück liefert allerdings *keine Exception*. Der Compiler erstellt nämlich eine
Objekt (genauer: eine *structure*) mit 2 Eigenschaften: *HasValue* und *Value*. Dieses Objekt
ist selbst nicht *null*, nur die *Value* Eigenschaft.

```c#
if (myInt2.HasValue)
{
    Console.WriteLine("int2 hat einen Wert, dieser ist " + myInt2.Value);
}
```

## Spezialkapitel (nicht prüfungsrelevant): Wertetypen und Methoden

Werden Wertetypen als Parameter übergeben, verhält sich C# wie Java: Sie können den Parameter
zwar in der Methode setzen, aber der Wert bleibt in der aufrufenden Methode unverändert.

In C# gibt es mit den Schlüsselwörtern *out* und *ref* die Möglichkeit, die Übergabe als
Referenz zu erreichen. Somit können Sie auch die Variable in der aufrufenden Methode ändern.

Eine gute Übersicht finden Sie auf
https://www.c-sharpcorner.com/UploadFile/ff2f08/ref-vs-out-keywords-in-C-Sharp/

> **Hinweis:** Dies ist ein spezielles Feature in C# und nicht für den alltäglichen Gebrauch bestimmt.
> Sie finden solche Übergabearten bei *TryParse()* und in machen Methoden der Task Parallel Library (*Interlocked*)

```c#
        static void Main(string[] args)
        {
            // Seit C# 7 ist die Deklaration im out möglich. Vorher musste
            // int itemCount
            // vor dem Aufruf extra deklariert werden.
            if (GetCount(out int itemCount))
            {
                Console.WriteLine($"{itemCount} items found.");
            }

            // value muss initialisiert werden, da wir mit ref arbeiten.
            int value = 1;
            SetValue(ref value);                      // Value ist nun 2

            // Bessere Lösung mit Tuples.
            (bool success, int count) = BetterGetCount();
            if (success) { Console.WriteLine($"{count} items found."); }

            // Ganz neu mit C# 8 Pattern matching und dem discard Operator (_)
            // Vgl. https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/pattern-matching
            string result = BetterGetCount() switch
            {
                (true, 0) => "No items found.",
                (true, _) => "Some items found.",
                (false, _) => "Error."
            };
            Console.WriteLine(result);
        }

        /// <summary>
        /// Demonstration von out
        /// count muss in der Methode zugewiesen werden, sonst gibt es einen Compilerfehler.
        /// </summary>
        static bool GetCount(out int count)
        {
            count = 1;
            return true;
        }

        static (bool success, int count) BetterGetCount()
        {
            return (success: true, count: 1);
        }

        /// <summary>
        /// Demonstration von ref
        /// Der Parameter val muss nicht - im Gegensatz von out - zwangsweise zugewiesen werden.
        /// </summary>
        static bool SetValue(ref int val)
        {
            if (val != 0) { val++; return true; }
            return false;
        }

```

## Parsen von Werten

Oft müssen Zahlen aus einem String gewonnen werden. Diesen Vorgang bezeichnet man als *parsen*. Er
hat nichts mit dem Typencast zu tun, da beim Parsen weit mehr Programmlogik zum Einsatz kommen muss.

In C# gibt es für die Grundtypen die Methoden *Parse()* und *TryParse()*. *Parse()* wirft eine
Exception, wenn der String nicht interpretiert werden kann. *TryParse()* arbeitet mit einem
bool Rückgabewert und setzt den Wert als *out* Parameter.

> **Hinweis:** Der Parser verwendet die Spracheinstellung des Betriebssystems, wenn es um das
> Dezimalzeichen oder das Datumsformat geht. Deswegen können sich die Ergebnisse - wenn nicht
> sorgsam entwickelt wird - bei anderen Spracheinstellungen unterscheiden. Bei DateTime muss
> man sich zusätzlich über die Zeitzone Gedanken machen.

```c#
// Verwendung von Parse()
string myStrNumber1 = "12";
int myIntNumber1 = int.Parse(myStrNumber1);
try
{
    int myIntNumber2 = int.Parse("A");                             // Liefert eine Exception
}
catch { }

// Verwendung von TryParse(). Hier wird keine Exception geworfen, wenn der String
// nicht geparst werden kann.
if (int.TryParse(myStrNumber1, out int myIntNumber3))            // Liefert Success
{ Console.WriteLine("Success"); }
else
{ Console.WriteLine("Error"); }

// Je nach Windowseinstellung kann 12.1 oder 12,1 geparst werden.
if (double.TryParse("12.1", out double result1))  
    Console.WriteLine($"12.1 wurde in {result1} geparst.");
if (double.TryParse("12,1", out double result2))
    Console.WriteLine($"12,1 wurde in {result2} geparst.");

// Unabhängig von der Einstellung wird 12.1 jetzt immer geparst.
if (double.TryParse("12.1", System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result3))
    Console.WriteLine($"12.1 wurde mit InvariantCulture in {result3} geparst.");
// Hier wird 121 gespeichert.
if (double.TryParse("12,1", System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result4))
    Console.WriteLine($"12,1 wurde mit InvariantCulture in {result4} geparst.");

if (DateTime.TryParseExact(
        "2020-01-22 06:07:08",                                 // String, der zu parsen ist.
        "yyyy-MM-dd hh:mm:ss",                                 // Format
        System.Globalization.CultureInfo.InvariantCulture,     // Englische Einstellung für Komma (wenn vorhanden)
        System.Globalization.DateTimeStyles.AdjustToUniversal, // Eine UTC Zeit erstellen
        out DateTime result5))
    Console.WriteLine(result5.ToString("yyyy-MM-ddTHH:mm:ssZ"));    
```

## Übung

Erstellen wie oben beschrieben das Projekt *TypesDemo*. Ersetze nun den Inhalt der Datei
*Program.cs* durch den folgenden Code. Implementieren die Funktionen der Klasse TypeExercise,
sodass die Bildschirmausgaben der Musterlösung weiter unten entsprechen.

```c#
using System;

namespace TypesDemo.Application
{
    class TypeExercise
    {
        /// <summary>
        /// Geben Sie die Fäche des Rechtecks (Länge x Breite) zurück.
        /// Wenn ein Wert (Länge oder Breite) null ist, soll das Ergebnis
        /// auch null sein. Ersetzen Sie ? durch den korrekten Datentyp.
        /// </summary>
        public ? BerechneFlaeche(double? laenge, double? breite)
        {

        }

        /// <summary>
        /// Geben Sie die Fäche des Rechtecks (Länge x Breite) zurück.
        /// Wenn ein Wert (Länge oder Breite) null ist, soll das Ergebnis
        /// 0 sein.
        /// </summary>
        public double BerechneFlaeche2(double? laenge, double? breite)
        {

        }

        /// <summary>
        /// Berechnen Sie den Preis nach folgender Vorschrift:
        /// In steuerProdukt und steuerKategorie sind Steuersätze als Faktoren
        /// gespeichert, also 1.2 für 20%. Sie müssen daher bei der Berechnung
        /// nur den Preis mit diesem Wert multiplizieren.
        /// 
        /// Ist ein Wert für steuerProdukt gesetzt (nicht null), so ist nur dieser
        /// Wert für die Berechnung heranzuziehen (also nettopreis x steuerProdukt).
        /// 
        /// Ist kein Wert für steuerProdukt gesetzt, so ist der Wert in steuerKategorie
        /// heranzuziehen (nettopreis x steuerKategorie).
        /// 
        /// Sind beide Werte nicht gesetzt, ist der Nettopreis x 1.2 zurückzugeben.
        /// 
        /// Verwenden Sie den ?? Operator.
        /// </summary>
        public decimal BerechnePreis(decimal nettopreis, decimal? steuerProdukt, decimal? steuerKategorie)
        {

        }

        /// <summary>
        /// Geben Sie die durchschnittliche Schülerzahl pro Klasse zurück. Sie
        /// berechnet sich aus schuelerGesamt / klassenGesamt.
        /// </summary>
        public double BerechneSchuelerProKlasse(int schuelerGesamt, int klassenGesamt)
        {

        }

        /// <summary>
        /// Geben Sie ein Achtel (also wert / 8) des übergebenen Wertes
        /// zurück. Achten Sie auf den Datentyp des Rückgabewertes.
        /// Kann in dieser Funktion eine Exception auftreten?
        /// </summary>
        public int BerechneAchtel(long wert)
        {

        }

        /// <summary>
        /// Berechne die Länge des gesamten Namens (Vor- und Zuname).
        /// Ist vorname oder zuname null, so ist für diesen Teil des Namens die Länge 0
        /// anzunehmen.
        /// </summary>
        public int BerechneLaenge(string? vorname, string? nachname)
        {

        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            TypeExercise typeExercise = new TypeExercise();

            Console.WriteLine("BerechneFlaeche(3,4):              " + typeExercise.BerechneFlaeche(3, 4));
            Console.WriteLine("BerechneFlaeche(3,null):           " + typeExercise.BerechneFlaeche(3, null));
            Console.WriteLine("BerechneFlaeche2(3,null):          " + typeExercise.BerechneFlaeche2(3, null));

            Console.WriteLine("BerechnePreis(100,1.2,null):       " + typeExercise.BerechnePreis(100, 1.2M, null));
            Console.WriteLine("BerechnePreis(100,1.2,1.1):        " + typeExercise.BerechnePreis(100, 1.2M, 1.1M));
            Console.WriteLine("BerechnePreis(100,null,1.1):       " + typeExercise.BerechnePreis(100, null, 1.1M));
            Console.WriteLine("BerechnePreis(100,null,null):      " + typeExercise.BerechnePreis(100, null, null));

            Console.WriteLine("BerechneSchuelerProKlasse(100, 6): " + typeExercise.BerechneSchuelerProKlasse(100, 6));
            Console.WriteLine("BerechneAchtel(120):               " + typeExercise.BerechneAchtel(120));
            Console.WriteLine("BerechneLaenge(null, nachname):    " + typeExercise.BerechneLaenge(null, "nachname"));            
        }
    }
}

```

```text
BerechneFlaeche(3,4):              12
BerechneFlaeche(3,null):
BerechneFlaeche2(3,null):          0
BerechnePreis(100,1.2,null):       120.0
BerechnePreis(100,1.2,1.1):        120.0
BerechnePreis(100,null,1.1):       110.0
BerechnePreis(100,null,null):      120.0
BerechneSchuelerProKlasse(100, 6): 16.666666666666668
BerechneAchtel(120):               15
BerechneLaenge(null, nachname):    8
```
