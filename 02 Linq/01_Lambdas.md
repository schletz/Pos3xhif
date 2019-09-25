# Lambdas in C#

Betrachten wir eine allgemeine Funktion, die in C# deklariert wird. Sie erwartet 2 Parameter und liefert
ein Ergebnis zurück.
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
{ } ebenfalls weggelassen werden. Der Compiler "denkt" sich automatisch ein return vor dem Statement:
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
In den schreibweisen davor wurde auf eine Information verzichtet: Der Datentyp. Da C# *static typing* umsetzt,
müssen Lambda Ausdrücke natürlich auch einen Datentyp haben. Hier gibt es 2 Verianten:

## Lambdas, die keinen Wert zurückgeben
Diese Ausdrücke sind Instanzen des sogenannten *Action* Delegates. Er ist generisch, in den spitzen Klammern
werden die Datentypen der Parameter geschrieben:
```c#
Action action0 = () => { };
Action<int> action1 = x => Console.WriteLine(x);
Action<int> action2 = x => { Console.WriteLine(x); Console.WriteLine("END"); };
Action<int, int> action3 = (x, y) => Console.WriteLine(x + y);
```

## Lambdas, die Werte zurückgeben
Diese Ausdrücke sind Instanzen des sogenannten *Func* Delegates. Er ist generisch, in den spitzen Klammern
werden die Datentypen der Parameter und zum Schluss der Typ des Rückgabewertes geschrieben:
```c#
Func<bool> func1 = () => true;
Func<int, int> func2 = x => x + 1; 
Func<int, int, bool> func3 = (x, y) => x == y;
```

## Übung
Erstelle eine neue Solution mit dem Namen LambdaUebung. Erstelle danach eine leere Klasse mit dem Namen
LambdaTest.cs und kopiere den Code von unten in die Datei. Danach ersetze die Program Klasse durch den 
Code von unten. Die Mainmethode ruft die Methoden der Klasse LambdaTest auf und übergibt die Funktion.

Die Klasse LambdaTest besitzt verschiedene Methoden, die eines gemeinsam haben: sie bekommen eine Methode 
als Parameter übergeben. Der Typ des Parameters ist mit ??? gekennzeichnet, er muss von dir korrekt 
gesetzt werden. Das Programm muss danach mit *F6* fehlerfrei kompiliert werden können.

Wenn das Programm korrekt kompiliert ersetze die "langwierig" auscodierten Methoden der Programmklasse
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