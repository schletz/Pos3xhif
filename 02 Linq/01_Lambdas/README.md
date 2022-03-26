# Lambdas in C#

## Erstellen einer Visual Studio Solution

Um die Beispiele mitmachen zu können, muss eine .NET Konsolenapplikation erstellt werden. Führe
dafür die folgenden Befehle in der Konsole aus. Unter macOs müssen md und rd durch die entsprechenden
Befehle ersetzt werden.

```text
rd /S /Q LambdaDemo
md LambdaDemo
cd LambdaDemo
md LambdaDemo.Application
cd LambdaDemo.Application
dotnet new console
cd ..
dotnet new sln
dotnet sln add LambdaDemo.Application
start LambdaDemo.sln
```

Öffne danach durch Doppelklick auf das Projekt (*LambdaDemo.Application*) die Datei
*LambdaDemo.Application.csproj* und füge die Optionen für
*Nullable* und *TreatWarningsAsError* hinzu. Die gesamte Konfiguration muss nun so aussehen:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

</Project>
```

## Intro in LINQ und Lambdas

Als Einstieg definieren wir einen Record *Pupil* (records sind im Kapitel
[Equality](../../01%20Types/07_Equality.md#records-in-c-9) beschrieben), der Daten von Schülern erfasst.
Eine Klasse *PupilList* verwaltet diese in einer Liste. Wir möchten in dieser *PupilList* auch
filtern können. Ein Filter, der alle Schüler einer Klasse heraussucht, kann so aussehen:

```c#
record Pupil(int Id, string Firstname, string Lastname, string SchoolClass, DateTime DateOfBirth);

class PupilList : List<Pupil>
{
    public PupilList Filter(string schoolClass)
    {
        PupilList result = new PupilList();
        foreach(Pupil pupil in this)
        {
            if (pupil.SchoolClass == schoolClass)
            {
                result.Add(pupil);
            }
        }
        return result;
    }
}
```

Der Aufruf der Filtermethode ist wie erwartet:

```c#
static void Main(string[] args)
{
    DateTime dob18 = DateTime.Now.Date.AddYears(-18);

    PupilList pupils = new PupilList
    {
        new Pupil(Id: 1, Firstname: "FN1", Lastname: "LN1", SchoolClass: "3AHIF", DateOfBirth: dob18.AddMonths(-2)),
        new Pupil(Id: 2, Firstname: "FN2", Lastname: "LN2", SchoolClass: "3BHIF", DateOfBirth: dob18.AddMonths(-1)),
        new Pupil(Id: 3, Firstname: "FN3", Lastname: "LN3", SchoolClass: "3BHIF", DateOfBirth: dob18),
        new Pupil(Id: 4, Firstname: "FN4", Lastname: "LN4", SchoolClass: "3BHIF", DateOfBirth: dob18.AddMonths(1))
    };

    PupilList pupils3bhif = pupils.Filter("3BHIF");
}
```

Nun wollen wir auch nach dem Zunamen filtern. Unsere *Filter()* Funktion filtert fix nach der Klasse,
daher haben wir mehrere Möglichkeiten:
- Wir schreiben eine Funktion *FilterLastname()*. Der Code ist allerdings meist ident zur *Filter()* Methode,
  und Code kopieren ist bekanntlich das schlechteste Design.
- Wir könnten den Propertynamen, den wir filtern möchten, auch übergeben. Über Reflection wäre das zwar
  möglich, aber sehr fehleranfällig falls einmal so ein Property nicht existiert.
- **Wir übergeben keinen String, nach den wir filtern sollen, sondern eine Funktion, die für
  jedes Element entscheidet, ob es ausgewählt wird.**

Wir betrachten den letzten Punkt. Eine Funktion zu übergeben ist in C# möglich. Allerdings setzt C#
static Typing um, das bedeutet, wir müssen dem Compiler einmal sagen, wie unsere Funktion aufgebaut
ist.

## Lambdas im Detail      

Betrachten wir eine Funktion, die in C# mit einem Namen deklariert wird. Sie erwartet 2 Parameter 
und liefert ein Ergebnis zurück.
```c#
T myFunction(T1 param1, T2 param2, ...)
{
    // Function Body mit return
}
```

Diese Funktion kann als Lambda Expression kürzer geschrieben werden:
```c#
(param1, param2) => { /* Function Body mit return */ }
```

Lambdas werden hauptsächlich dann eingesetzt, wenn Funktionen nur sehr kurz sind. Dafür gibt es auch
syntaktische Erleichterungen. Wird z. B. nur ein Parameter übergeben, so können die runden Klammern weggelassen werden:
```c#
param1 => { /* Function Body mit return */ }
```

Besteht die Funktion nur aus einem Statement, und soll dieses mit return zurückgegeben werden, so kann
{ } ebenfalls weggelassen werden. Der Compiler "denkt" sich automatisch ein *return* vor dem Statement:
```c#
param1 => statement
```

## Lambdas für Funktionen, die nichts (void) liefern

| Langform                                                                   |  Lambda                                                         |
| -------------------------------------------------------------------------- | --------------------------------------------------------------- |
| `void action0() { }`                                                         | `()     => { }`                                                 |
| `void action1(int x) { Console.WriteLine(x); }`                              | `x      => Console.WriteLine(x)`                                |
| `void action2(int x) { Console.WriteLine(x); Console.WriteLine("END"); }`    | `x      => { Console.WriteLine(x); Console.WriteLine("END"); }` |
| `void action3(int x, int y) { Console.WriteLine(x + y); }`                   | `(x, y) => Console.WriteLine(x + y)`                            |

## Lambdas für Funktionen, die einen Rückgabewert besitzen

| Langform                                                                   |  Lambda                                                         |
| -------------------------------------------------------------------------- | --------------------------------------------------------------- |
| `int func1() { return 1; }`                                                 | `()     => true`  oder `() => { return true; }`                 |
| `int func2(int x) { return x + 1; }`                                        | `x => x + 1` oder seltener:  `x => { return x + 1; }` bzw. `(x) => x + 1` |
| `bool func3(int x, int y) { return x == y; }`                               | `(x, y) => x == y` oder  `(x, y) => { return x == y; }`         |


## Datentypen für Lambdas

In den Schreibweisen davor wurde auf eine Information verzichtet: Der Datentyp. Da C# *static typing* umsetzt,
müssen Lambda Ausdrücke natürlich auch einen Datentyp haben. Hier gibt es 2 Verianten:

### Lambdas, die keinen Wert zurückgeben

Diese Ausdrücke sind Instanzen des sogenannten *Action* Delegates. Er ist generisch, in den spitzen Klammern
werden die Datentypen der Parameter geschrieben:
```c#
Action action0 = () => { };
Action<int> action1 = x => Console.WriteLine(x);
Action<int> action2 = x => { Console.WriteLine(x); Console.WriteLine("END"); };
Action<int, int> action3 = (x, y) => Console.WriteLine(x + y);
```

Natürlich können diese Variablen, die jetzt eine Lambda Expression - also eine Funktion - beinhalten,
auch wie eine Funktion aufgerufen werden:
```c#
action2(2); // Gibt 2 und END aus.
```

### Lambdas, die Werte zurückgeben
Diese Ausdrücke sind Instanzen des sogenannten *Func* Delegates. Er ist generisch, in den spitzen Klammern
werden die Datentypen der Parameter und zum Schluss der Typ des Rückgabewertes geschrieben:
```c#
Func<bool> func1 = () => true;
Func<int, int> func2 = x => x + 1; 
Func<int, int, bool> func3 = (x, y) => x == y;
```

### Lambdas und der Zugriff auf äußere Variablen

Lambdas können auf die äußeren Variablen zugreifen. Das ist besonders hilfreich, wenn private
Variablen in Lambdas gelesen oder gesetzt werden sollen. Wir betrachten noch einmal die *Main()* 
Methode aus dem Einführungsbeispiel:

```c#
static void Main(string[] args)
{
    // ...
    string classToFilter = "3BHIF";
    Func<Pupil, bool> classFilter = p => p.SchoolClass == classToFilter;   // (1)
    classToFilter = "3AHIF";                                               // (2)
    PupilList filtered = pupils.Filter(classFilter);                       // (3)
}
```
In (1) wird die Funktion **deklariert**. Sie kann auf *classToFilter* zugreifen, da die Variable davor in der *Main()*
Methode deklariert wurde. Sie wird jedoch noch nicht ausgeführt, deswegen führt die Neuzuweisung
der Variable *classToFilter* in (2) dazu, dass die Filterfunktion in (3) nach der 3AHIF sucht. **Wir
müssen daher immer zwischen der Deklaration und dem tatsächlichen Ausführen von Funktionen unterscheiden!**

Nun können wir die Methode Filter() in der Klasse PupilList so allgemein definieren, dass sie
jede Art von Filterung unterstützt. Der Parameter predicate ist eine Function, die ein Pupil
Objekt auf einen bool Wert (nehmen oder nicht nehmen) abbildet.

```c#
class PupilList : List<Pupil>
{
    public PupilList Filter(Func<Pupil, bool> predicate)
    {
        PupilList result = new PupilList();
        foreach (Pupil pupil in this)
        {
            if (predicate(pupil))
            {
                result.Add(pupil);
            }
        }
        return result;
    }
}
```

Somit sind flexible Aufrufe möglich:

```c#
PupilList pupils3bhif = pupils.Filter(p => p.SchoolClass == "3BHIF");
PupilList pupilsLukas = pupils.Filter(p => p.Firstname == "Lukas");
PupilList fullAged = pupils.Filter(p => p.dateOfBirth <= DateTime.Now.AddYears(-18));
```

Wir können Filterausdrücke auch in der Klasse *Pupil* speichern, damit die Bedingung "ist volljährig"
nicht mehrfach in Form von Lambdas geschrieben werden muss. Dafür gibt es 2 Ansätze:
- Definition des Filterausdruckes (Predicate) als statisches Feld.
- Definition eines read-only Properties.

```c#
record Pupil(int Id, string Firstname, string Lastname, string SchoolClass, DateTime DateOfBirth)
{
    public static readonly Func<Pupil, bool> IsFullAgePredicate = (p) => p.DateOfBirth <= DateTime.Now.Date.AddYears(-18);
    public bool IsFullAge => DateOfBirth <= DateTime.Now.Date.AddYears(-18);
}

PupilList fullAged = pupils.Filter(Pupil.IsFullAgePredicate);
PupilList fullAged2 = pupils.Filter(p => p.IsFullAge);
```

Beachte, dass *IsFullAgePredicate* statisch ist, da die Function mit den Instanzen der Liste aufgerufen wird.
Beim Aufrufen von *pupils.Filter()* haben wir noch keine konkrete Instanz von Pupil.

Der 2. Ansatz (Property) ist in C# weiter verbreitet. Der 1. Ansatz wird im Javabereich, wo es keine
Properties aber den double colon (::) operator gibt, in Java Streams verwendet.

## Übung

Erstelle wie oben beschrieben die Solution *LambdaDemo*. Erstelle danach eine leere Klasse mit dem Namen
*LambdaTest.cs* und kopiere den Code von unten in die Datei. Danach ersetze die Program Klasse durch den 
Code von unten.

1. Die Klasse *LambdaTest* besitzt verschiedene Methoden, die eines gemeinsam haben: sie bekommen eine Methode 
als Parameter übergeben. Der Typ des Parameters ist mit ??? gekennzeichnet, er muss von dir korrekt 
gesetzt werden. Das Programm muss danach fehlerfrei kompiliert werden können.
2. Wenn das Programm korrekt kompiliert ersetze die "langwierig" auscodierten Methoden der Programmklasse
durch Lambdas, die du direkt als Parameter statt den ursprünglichen Methoden übergibst.


### Klasse LambdaTest
```c#
class LambdaTest
{
    /// <summary>
    /// Konvertiert jedes Element des übergebenen Arrays anhand der übergebenen Funktion.
    /// </summary>
    /// <param name="values">Wertearray</param>
    /// <param name="converterFunc">Funktion, die den Wert konvertiert</param>
    /// <returns>Array mit den konvertierten Werten.</returns>
    public static decimal[] Converter(decimal[] values, ??? converterFunc)
    {
        if (values == null) { return new decimal[0]; }

        decimal[] result = new decimal[values.Length];
        int i = 0;
        foreach (decimal value in values)
        {
            result[i++] = converterFunc(value);
        }
        return result;
    }

    /// <summary>
    /// Führt einen Befehl für jedes Element des übergebenen Arrays aus.
    /// </summary>
    /// <param name="values">Wertearray</param>
    /// <param name="action">Funktion, die ausgeführt wird.</param>
    public static void ForEach(decimal[] values, ??? action)
    {
        foreach (decimal value in values)
        {
            action(value);
        }
    }

    /// <summary>
    /// Führt eine übergebene Operation mit den ersten 2 Argumenten aus.
    /// </summary>
    /// <param name="x">1. Zahl</param>
    /// <param name="y">2. Zahl</param>
    /// <param name="operation">Funktion mit der arithmetischen Operation</param>
    /// <returns></returns>
    public static decimal ArithmeticOperation(decimal x, decimal y, ??? operation)
    {
        return operation(x, y);
    }

    /// <summary>
    /// Führt eine übergebene Operation mit den ersten 2 Argumenten aus. Schlägt diese Fehl, wird
    /// die Fehlermeldung der Exception der logFunction übergeben.
    /// </summary>
    /// <param name="x">1. Zahl</param>
    /// <param name="y">2. Zahl</param>
    /// <param name="operation">Funktion mit der arithmetischen Operation</param>
    /// <param name="logFunction">Funktion, die die Fehlermeldung weiterverarbeitet.</param>
    /// <returns></returns>
    public static decimal ArithmeticOperation(decimal x, decimal y,
        ??? operation,
        ??? logFunction)
    {
        try
        {
            return operation(x, y);
        }
        catch (Exception e)
        {
            logFunction(e.Message);
        }
        return 0;
    }


    /// <summary>
    /// Ruft die übergebene Funktion auf.
    /// </summary>
    /// <param name="command">Die Funktion, die aufgerufen werden soll.</param>
    public static void RunCommand(??? command)
    {
        command();
    }

    /// <summary>
    /// Gibt nur jene Elemente zurück, bei denen die übergebene filterFunction true liefert.
    /// </summary>
    /// <param name="values">Array von Werten.</param>
    /// <param name="filterFunction">Filterfunktion</param>
    /// <returns></returns>
    public static decimal[] Filter(decimal[] values, ??? filterFunction)
    {
        List<decimal> result = new List<decimal>();
        foreach (decimal value in values)
        {
            if (filterFunction(value))
            {
                result.Add(value);
            }
        }
        return result.ToArray();
    }
}
```

### Klasse Program
```c#
class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Beispiel 1: Converter");
        decimal[] converted = LambdaTest.Converter(new decimal[] { -10, 0, 10, 20, 30 }, CelsiusToKelvin);
        LambdaTest.ForEach(converted, PrintValue);

        Console.WriteLine("Beispiel 2: Filter");
        decimal[] freezed = LambdaTest.Filter(converted, WasserGefriert);
        LambdaTest.ForEach(freezed, PrintValue);

        Console.WriteLine("Beispiel 3: Division");
        decimal result = LambdaTest.ArithmeticOperation(2, 0, DivideSafe);
        Console.WriteLine(result);
        result = LambdaTest.ArithmeticOperation(2, 0, Divide, PrintError);
        Console.WriteLine(result);

        Console.WriteLine("Beispiel 4: Callback Funktion");
        LambdaTest.RunCommand(SayHello);

        Console.ReadLine();
    }

    public static decimal CelsiusToKelvin(decimal value)
    {
        return value + 273.15M;
    }

    public static void PrintValue(decimal value)
    {
        Console.WriteLine(value);
    }

    public static decimal Divide(decimal x, decimal y)
    {
        return x / y;
    }

    public static decimal DivideSafe(decimal x, decimal y)
    {
        if (y == 0) { return 0; }
        return x / y;
    }

    public static void PrintError(string message)
    {
        Console.Error.WriteLine(message);
    }

    public static void SayHello()
    {
        Console.WriteLine("Hello World.");
        Console.WriteLine("Hello World again.");
    }

    public static bool WasserGefriert(decimal val)
    {
        return val < 273.15M;
    }
}
```