# Referenztypen und der Umgang mit NULL in C#

## Erstellen einer Visual Studio Solution

Um die Beispiele mitmachen zu können, muss eine .NET Konsolenapplikation erstellt werden. Führe
dafür die folgenden Befehle in der Konsole aus. Unter macOs müssen md und rd durch die entsprechenden
Befehle ersetzt werden.

```text
rd /S /Q ReferenceTypesDemo
md ReferenceTypesDemo
cd ReferenceTypesDemo
md ReferenceTypesDemo.Application
cd ReferenceTypesDemo.Application
dotnet new console
cd ..
dotnet new sln
dotnet sln add ReferenceTypesDemo.Application
start ReferenceTypesDemo.sln

```

Öffne danach durch Doppelklick auf das Projekt (*ReferenceTypesDemo.Application*) die Datei
*ReferenceTypesDemo.Application.csproj* und füge die Optionen für
*Nullable* und *TreatWarningsAsError* hinzu. Die gesamte Konfiguration muss nun so aussehen:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

</Project>
```

### Umgang mit NULL

Der bekannteste reference type in C# ist wohl *string*. Mit diesem Typ werden wir nun einige
Beispiele durchspielen. Oftmals führt ein null Wert zu einer *NullReferenceException*. In
C# gibt es 2 Operatoren, die den Umgang mit null erleichtern: ?. und ??. Folgendes Beispiel
zeigt deren Einsatz:

```c#
using System;

string myStr = null;
if (myStr is null)  // Seit C# 7 empfohlen (statt myStr == null)
{
    Console.WriteLine("myStr kann NULL sein, da es ein Referenztyp ist.");
}
if (myStr is not null)  // Seit C# 7 empfohlen (statt myStr != null)
{
    Console.WriteLine("myStr ist nicht NULL.");
}
// Ermittelt die Länge des Strings. Dieser Code 
// liefert eine NullReferenceException
try
{
    int len = myStr.Length;
}
catch (NullReferenceException)
{
    Console.Error.WriteLine("OOPS.");
}
```

Eine Besonderheit ist das *is null* statement. Die Bedingung *myStr == null* funktioniert auch,
der Unterschied ist ein Detail in der Tiefe der C# Sprachdefinition. In C# können Operatoren
überladen werden, d. h. der Autor einer Klasse kann selbst bestimmen, welchen Wert == zurückgibt.
Dadurch ist die Prüfung mit einem Operator, der überladen werden kann, nicht zu 100% sicher.
Seit C# 7 wird der null check daher mit *is null* empfohlen, denn *is* ist nicht überladbar.

#### Der null-conditional member access operator ?. ("Elvis operator")

Soll auf Properties oder Methoden von Referenztypen zugegriffen werden, dann können wir uns mit dem
*null-conditional member access operator* (meist *Elvis operator* durch die Form einer Haarlocke
genannt) eine Prüfung auf NULL sparen. 

Der Operator liefert entweder das Ergebnis des Properties bzw. der Methode oder NULL, wenn das
Objekt null ist.

```c#
using System;

string string1 = "Wert";
string string2 = null;

Console.WriteLine(string1?.Length);             // Liefert wie erwartet 4.
Console.WriteLine(string2?.Length);             // Liefert null (nicht 0!)
Console.WriteLine(string2?.ToUpper());          // Liefert null
// Liefert null und keine Exception, da nach dem ersten ?. schon "abgebrochen" wird.
// Zweimaliges Verwenden von ?. ist nicht nötig.
Console.WriteLine(string2?.ToUpper().Length);   

// Compilerfehler: int? kann nicht in int umgewandelt werden
int len = string1?.Length;
```


### Nullable reference types (C# 8)

Vielleicht hast du dich schon gewundert, warum in der Projektdatei die Option *nullable* aktiviert
wird. C# 8 und höher bietet ein Sicherheitsfeature, welches mit opt-in aktiviert werden muss:
*nullable reference types*.

Die folgende Anweisung liefert mit der Option *Nullable* und *TreatWarningsAsErrors* einen
Compilerfehler:

```c#
string myStr = null; // error CS8600: Converting null literal or possible null value to non-nullable type. 
```

Mit aktivierten nullable reference types müssen wir bei der deklaration von Variablen, die einen
Referenztyp verwenden, nun unterscheiden:

```c#
string neverBeNull = string.Empty;
string? canBeNull = null;

neverBeNull = null;                    // (1) Converting null literal or possible null value to non-nullable type.
Console.WriteLine(canBeNull.Length);   // (2) Dereference of a possibly null reference.
Console.WriteLine(canBeNull?.Length);  // (3) OK
if (!string.IsNullOrEmpty(canBeNull))
{
    Console.WriteLine(canBeNull.Length);  // (4) OK
}
```

Mit *Type?* deklarieren wir einen *nullable type*. Im Gegensatz zu den *nullable value types*,
welche intern eine structure mit *HasValue* und *Value* erzeugen, wird hier aber keine eigene
Datenstruktur erzeugt. Es ist ein reiner Hinweis für den Compiler.

Wird ein Referenztyp als nullable definiert, zwingt uns der Compiler zur Vorsicht:
- **(1)** Die Zuweisung von *null* ist nicht erlaubt.
- **(2)** Wir können nicht ungeprüft auf Properties oder Methoden zugreifen.
- **(3)** Der "Elvis Operator" kann natürlich verwendet werden.
- **(4)** Der Compiler führt eine Analyse des Programmablaufes durch und
  stellt fest, dass durch das vorige if der Wert nie null sein kann. Daher liefert (4) auch keinen
  Syntaxfehler. Dies wird dadurch ermöglicht, dass die Methode *IsNullOrEmpty()* in .NET wie
  folgt definiert ist: *public static bool IsNullOrEmpty([NotNullWhen(false)] String? value);*
  Durch Attributes, die mit .NET Core 3 eingeführt wurden, kann Information an den Compiler
  gegeben werden. Mehr Details sind auf
  [Attributes for null-state static analysis](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis)
  nachzulesen.

#### Null-forgiving Operator

Die Prüfung auf null kann mit dem null-forgiving Operator (!) deaktiviert werden. Damit würde
das folgende Programm ohne Warnungen kompiliert werden:

```c#
string neverBeNull = string.Empty;
string? canBeNull = null;
neverBeNull = null!;                    // Keine Warnung.
Console.WriteLine(neverBeNull.Length);
Console.WriteLine(canBeNull!.Length);   // Keine Warnung.
```

Es versteht sich von selbst, dass dieser Operator nur in begründeten Fällen verwendet werden soll:
- Bei der Verwendung von älteren Paketen, die nullable reference types noch nicht unterstützen.
- Bei OR Mappern wie EF Core.
- Bei komplexeren Logiken, wo der Compiler keine Codeanalyse machen kann (LINQ Expressions).

## Eigene Typen

Bis jetzt haben wir uns nur mit vordefinierten Referenztypen wie string beschäftigt. Natürlich
können wir auch eigene Typen definieren:

```c#
class Person
{
    public string firstname;  // Properties wurden noch nicht besprochen, deswegen public fields.
    public string lastname;
}

class Student : Person
{
    public DateTime dateOfBirth;
}
```

Allerdings lässt sich dieser Code mit aktiviertem nullable feature und *TreatWarningsAsErrors*
Option nicht kompilieren, da
*firstname* und *lastname* nicht initialisiert wurden. Wir benötigen daher Konstruktoren,
die die Initialisierung sicherstellen.

```c#
class Person
{
    public string firstname;
    public string lastname;

    public Person(string firstname, string lastname)
    {
        this.firstname = firstname;
        this.lastname = lastname;
    }
}

class Student : Person               // Java: Student extends Person
{
    public DateTime dateOfBirth;

    // Vergleichbar mit super() in Java.
    public Student(string firstname, string lastname, DateTime dateOfBirth)
        : base(firstname, lastname)
    {
        this.dateOfBirth = dateOfBirth;
    }
}
```

Nun können wir die Klassen instanzieren.

```c#
// Error (nullable aktiviert), somit auch kein nullwert über den Aufruf des Konstruktors.
Person p0 = new Person(firstname: null, lastname: "Mustermann"); 

Person p = new Person(firstname: "Max", lastname: "Mustermann"); // (1)
Person p2 = p;                 // (2)
p2.firstname = "Max2";         // (3)
```

- **(1)** Eine Person wird am Heap erstellt und die Referenzadresse in *p* geschrieben.
          Named arguments helfen, die Zuordnung der Parameter deutlicher zu machen. Außerdem kann
          keine Verwechslung der Reihenfolge passieren (beides sind string Typen). In Java wird
          dieses Problem mit dem Builder Pattern gelöst.
- **(2)** Nun habe ich eine Instanz, auf die 2 Referenzvariablen (*p* und *p2*) zeigen.
- **(3)** *p2.firstname* liefert natürlich auch Max2.

In Zusammenhang mit Vererbung ergeben sich folgende Besonderheiten:

```c#
Student student = new Student("Max", "Musterstudent", new DateTime(2004, 12, 31));
Person p3 = student;                                      // (1)
p3.dateOfBirth = new DateTime(2003, 12, 31);              // (2)
Console.WriteLine(((Student)p3).dateOfBirth);             // (3)
Console.WriteLine(((Student)p).dateOfBirth);              // (4)
```

- **(1)** "Hinaufcasten" ist (auch implizit) möglich, da die Vererbung ja eine "is-a" Beziehung ist. 
- **(2)** Diese Anweisung ist nicht möglich, da eine Person kein Geburtsdatum hat.
- **(3)** Es wird 31.12.2004 ausgegeben. Die Daten werden durch den Typecast also nicht gelöscht,
          es handelt sich immer noch um das selbe Student Objekt am Heap.
- **(4)** Es entsteht ein Laufzeitfehler. Der Compiler prüft bei expliziten Typencasts nicht,
          ob das auch möglich ist.

## is und as

In C# erleichtern die Schlüsselwörter *is* und *as* den Umgang mit Typencasts.

```c#
if (p3 is Student) { Console.WriteLine("p3 ist ein Student."); }  // (1)
Student? s2 = p as Student;                                       // (2)
if (s2 is not null) { Console.WriteLine(s2.dateOfBirth); }        // (3)
```

- **(1)** Liefert true, da p3 zwar eine Person ist, aber in einen Student umgewandelt werdne kann.
          *is* prüft also, ob der Typ castbar ist.
- **(2)** *as* versucht einen Typecast durchzuführen. Ist das nicht möglich, wird null geliefert.
          Dadurch muss ein nullable type verwendet werden.
- **(3)** Wird nicht ausgeführt, da *p* eine Person ist und *s2* daher null ist.

## Equals und ==

Javaentwickler würden bei Referenztypen *equals()* verwenden, wenn der Inhalt zweier Instanzen
verglichen werden soll. In C# kann der == Operator überladen werden. Dadurch liefert folgender
Code auch die gewünschten Ausgaben:

```c#
string str1 = "MAX";
string str2 = "MAX";

if (str1 == str2) { Console.WriteLine("Inhalt str1 ist gleich str2."); }      // true
if (str1.Equals(str2)) { Console.WriteLine("Inhalt str1 ist gleich str2."); } // true
// Durch die Stringverwaltung wird in C# - da die Strings gleich sind - vorerst nur 1 Instanz
// angelegt. Daher ist dieser Vergleich true.
if (object.ReferenceEquals(str1, str2))
{ Console.WriteLine("str1 ist die selbe Instanz wie str2."); }

// str2 wird durch die Umwandlung MAX --> max --> MAX zur neuen Instanz.
str2 = str2.ToLower().ToUpper();
if (str1 == str2) { Console.WriteLine("Inhalt str1 ist gleich str2."); }      // true
if (object.ReferenceEquals(str1, str2))
{ Console.WriteLine("str1 ist die selbe Instanz wie str2."); }
```

## Für Profis: Konkrete Anwendung von is und as

Der folgende Code zeigt ein Anwendungsbeispiel aus der generischen Programmierung. Solcher Code
kommt z. B. in JSON Serializern vor.

```c#
class Program
{
    static string GetBeautifulString<T>(T val)
    {
        // Aus bool Werten wollen wir 1 oder 0 machen.
        if (val is bool)
        {
            // Ich muss casten, da T ein beliebiger Typ sein kann. Ohne Cast könnten nur die
            // Methoden von Object verwendet werden. Der explizite Typencast mit
            // (Type) variable
            // liefert einen Compilerfehler, da der Compiler nicht jeden Typ in bool casten kann.
            return (val as bool?) == true ? "1" : "0";
        }
        // Bei int und long (Ganzzahlen) geben wir die Zahl als String zurück.
        if (val is int || val is long)
        {
            return val.ToString();
        }
        if (val is float || val is double)
        {
            // Bei Gleitkommatypen wollen wir 2 Kommastellen mit englischem . als Dezimalzeichen
            // zurückgeben.
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}", val);
        }
        return "Unknown Type";
    }

    static void Main(string[] args)
    {
        Console.WriteLine(GetBeautifulString<bool>(true));             // 1
        Console.WriteLine(GetBeautifulString<int>(12));                // 12
        Console.WriteLine(GetBeautifulString<double>(3.123));          // 3.12
        // Ausblick: Durch Type inference wird der Typparameter automatisch gesetzt.
        Console.WriteLine(GetBeautifulString(3.123));                  // 3.12
        Console.WriteLine(GetBeautifulString<decimal>(3.123M));        // 3.12
    }
}
```
