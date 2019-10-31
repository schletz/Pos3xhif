# Extension Methods in C#
## Intro: Liegt ein Tag am Wochenende?
Wir wollen eine Methode schreiben, die bestimmt, ob ein Tag auf ein Wochenende (Samstag oder Sonntag) fällt.
In .NET ist der Typ *DateTime* der Standardtyp für Datums- und Zeitangeben. Wir können wir unsere Methode
*IsWeekend()* nun implementieren?

### Variante 1: Ableiten von DateTime
Der erste Ansatz wenn wir zu einem Typ Features hinzufügen wollen führt einmal über die Vererbung. 
Wir könnten einen Typ *MyDateTime* definieren, der von *DateTime* erbt. Jedoch ist dies nicht von Vorteil
bzw. in C# gar nicht möglich:
- Bestehende Methoden im .NET Framework liefern *DateTime* und nicht *MyDateTime*.
- DateTime ist eine [struct](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/structs),
  daher kann nicht von ihr geerbt werden.

### Variante 2: Eine Helperklasse
Wir können eine Klasse *DateTimeHelpers* definieren, die eine Methode *IsWeekend()* anbietet. Die 
Implementierung ist nichts besonderes, außer dass die Methode statisch ist. Da mehrere Instanzen dieser
Klasse im Programm und auch Membervariablen keinen Sinn machen, definieren wir 
sie ebenfalls als statisch:
```c#
static class DateTimeHelpers
{
    public static bool IsWeekend(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday) { return true; }
        if (date.DayOfWeek == DayOfWeek.Sunday) { return true; }
        return false;
    }
}
```

Nun können wir die Methode überall im Programm aufrufen, wo wir sie brauchen:
```c#
DateTime myDate = new DateTime(2019, 11, 2);
Console.WriteLine($"{myDate} ist am Wochenende? {DateTimeHelpers.IsWeekend(myDate)}");
```

Problem gelöst, jedoch mit "unschönen" Effekten:
- Helferklassen mit einzelnen, nicht zusammenhängenden Methoden widersprechen dem OOP Ansatz.
- Da die Methode statisch ist, kann - bei *DateTime* als *struct* zwar nicht, bei Klassen aber schon - null
  übergeben werden. Eine Prüfung in der Helpermethode ist daher notwendig.

## Die Lösung in C#: Extension Methods
Am Schönsten wäre es natürlich, wir können unser *IsHoliday()* so nutzen, wie wenn es in *DateTime* 
implementiert sein würde. Das bedeutet folgenden Aufruf:
```c#
DateTime myDate = new DateTime(2019, 11, 2);
Console.WriteLine($"{myDate} ist am Wochenende? {myDate.IsWeekend()}");
```

Das ist mit *Extension Methods* möglich. Sie erweitern bestehende Objekte aus dem Framework, auf die
wir keinen Einfluss haben, um eigene Methoden. Konkret wird die Extension Method in der Klasse 
*DateTimeExtensions* (der Name ist natürlich frei wählbar) definiert:
```c#
static class DateTimeExtensions
{
    public static bool IsWeekend(this DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday) { return true; }
        if (date.DayOfWeek == DayOfWeek.Sunday) { return true; }
        return false;
    }
}
```

Wir sehen folgende syntaktische Besonderheiten:
- Die Extension Methode befindet sich in einer statischen Klasse. Das bedeutet, dass sie keine 
  Membervariablen und keine nicht statischen Methoden haben kann.
- Die Extension Methode ist statisch.
- Die Extension Methode bekommt als erstes Argument mit dem Schlüsselwort *this* die Instanz übergeben,
  von der sie aufgerufen wird.

### Extension Methods und Interfaces
Wir betrachten nun unseren Logger aus den vorigen Übungsbeispielen. Das Interface *ILogger* bietet eine
*Log()* Methode an, der *ConsoleLogger* implementiert diese Methode so, dass dieser String auf der Konsole
ausgeben wird.
```c#
interface ILogger
{
    void Log(string value);
}

class ConsoleLogger : ILogger
{
    public void Log(string value)
    {
        Console.WriteLine(value);
    }
}
```

Wir möchten nun eine Methode *LogUppercase()* hinzufügen, die den String in Großbuchstaben schreibt.
Diese Methode kommt mit allen vom Interface *ILogger* angebotenen Methoden aus. In der klassischen
Vererbung würden wir aus dem Interface eine abstrakte Klasse machen, da manche Methoden nun implementiert
werden können:
```c#
abstract class Logger
{
    public abstract void Log(string value);
    public void LogUppercase(string value)
    {
        Log(value.ToUpper());
    }
}

class ConsoleLogger : Logger
{
    public override void Log(string value)
    {
        Console.WriteLine(value);
    }
}    
```

Wenn wir die Klassen unter Kontrolle haben, sprich sie von uns stammen, ist dies auch nach wie vor
der sauberste Ansatz vom Standpunkt der OO Modellierung. Wenn wir das Interface *ILogger* allerdings
von externen Bibliotheken eingebunden haben, können wir auch für das Interface *ILogger* Extension 
Methoden schreiben:
```c#
static class LoggerExtensions
{
    public static void LogUppercase(this ILogger logger, string value)
    {
        logger.Log(value.ToUpper());
    }
}
```

Der Effekt ist bei beiden Varianten der Selbe: Wir können unsere Methode *LogUppercase()* aufrufen:
```c#
ConsoleLogger myLogger = new ConsoleLogger();
myLogger.LogUppercase("Hello!");                 // Gibt HELLO! aus.
```

Auf [social.msdn.microsoft.com](https://social.msdn.microsoft.com/Forums/vstudio/en-US/d5603465-04fe-4941-915d-4d940e73fc37/when-to-use-abstract-classes-instead-of-interfaces-with-extension-methods-in-c?forum=csharpgeneral)
gibt es eine interessante Diskussion, wann welcher Ansatz besser ist. Die Meinung bildet das oben 
Genannte ab.

## Bekannte Vertreter im .NET Framework
Der bekannteste Vertreter ist zweifellos LINQ. Es bietet eine Sammlung von Methoden, die das Interface
für Aufzählungen (*IEnumerable*) erweitert. Dadurch können die Methoden wie *Where()*, *Select()*, ...
bei jeder Anweisung, die *IEnumerable* liefert, verwendet werden.

Wir erkennen die Extension Methode an der Signatur der Methode *Where()*. Der erste Parameter beginnt
mit *this*, das bedeutet dass die Methode das Interface *IEnumerable* erweitert:
```c#
public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
```

## Gegenüberstellung: Vererbung und Extension Methods

| Vererbung | Extension Methods |
| --------- | ----------------- |
| Bei Klassen, die wir verfasst haben. | Bei bestehenden Klassen aus dem Framework. |
| Zugriff auf protected Members möglich. | Zugriff nur auf public Members. |
| Erweitern die Klasse meist noch um private Members und Properties.      | Keine Extension Properties möglich. Haben den Charakter von "Helper Functions". |
| Statische Methoden können implementiert werden (z. B. Factorymethoden). | Nicht möglich, alle Extension Methoden sind Instanzgebunden. Deswegen kann *Console* als statische Klasse nicht erweitert werden. |
| In der OOP Modellierung vorgesehen.                                     | Sollten sparsam verwendet werden.