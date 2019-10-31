# Generics in C#
Jeder verwendet sie täglich in seinen Programmen, wenige schreiben jedoch selbst: generische Klassen.
Da uns das .NET Framework schon das Haupteinsatzgebiet von Generics - nämlich Speicherstrukturen - 
durch die vorhandenen Collections zur Verfügung stellt, ist dies auch nicht in jedem Programm notwendig.
Trotzdem können Generics dazu beitragen, das wichtigste Pattern der Programmierung - don't repeat
yourself - elegent umzusetzen.

## Erstes Beispiel: Eine History
Wir möchten eine Klasse erstellen, die bei einer Zuweisung den alten Wert auch noch speichert.
Dieser Wert kann ein beliebiger Datentyp sein, sodass der erste Ansatz über object führen würde:
```c#
History objectHistory = new History();
objectHistory.Value = "Erster!";
objectHistory.Value = "Zweiter!";
Console.WriteLine(objectHistory.OldValue);   // Erster!
```

Eine solche Klasse zu implementieren ist ein Leichtes:
```c#
class History
{
    private object _value;
    public object OldValue { get; private set; }
    public object Value
    {
        get => _value;
        set
        {
            OldValue = _value;
            _value = value;
        }
    }
}
```

Allerdings haben wir die selben Probleme, die auch zur Einführung der generischen Collections geführt
haben:
- *Value* und *OldValue* sind vom Typ *object*. Sollen sie wieder als *string*, *DateTime*, ... erscheinen, 
  muss ein Typencast durchgeführt werden. Dadurch geht die Typensicherheit beim Kompilieren verloren.  
- Bei Wertetypen wie *int*, ... muss ein Boxing nach *object* durchgeführt werden, dies ist schlecht
  für die Performance.

Besser wäre es, wir könnten folgenden Code ausführen:
```c#
History<string> stringHistory = new History<string>();
stringHistory.Value = "Erster!";
stringHistory.Value = "Zweiter!";
Console.WriteLine(stringHistory.OldValue);   // Erster!

History<int> intHistory = new History<int>();
intHistory.Value = 1;
intHistory.Value = 2;
Console.WriteLine(intHistory.OldValue);   // 1
```
Die Umsetzung dieser generischen History Klasse ist auch nicht schwer, die einzige syntaktische
Besonderheit ist der Typparameter nach dem Klassennamen:
```c#
class History<T>
{
    private T _value;
    public T OldValue { get; private set; }
    public T Value
    {
        get => _value;
        set
        {
            OldValue = _value;
            _value = value;
        }
    }
}
```

Was bedeutet die obere Implementierung? Ein Typparameter bedeutet für den Compiler, dass er wie in
Word "Bearbeiten - Ersetzen" muss. Er ersetzt einfach das *T* durch den bei der Instanzierung angegebenen
Typ. Das klingt einfach, und ist es auch, jedoch müssen dadurch auch alle Operationen, die wir in der
Klasse durchführen, mit diesem Typ möglich sein. Folgendes harmlose Beispiel verdeutlicht dies bereits:
```c#
class History<T>
{
    // ...
    public void Undo()
    {
        _value = OldValue;
        OldValue = null;       // Compilerfehler.
    }
}
```

Da *T* ein beliebiger Typ, also auch *int*, ... sein kann, ist eine Zuweisung von null in diesem Falle
nicht möglich. Auch wenn wir in unserem Programm nie ein *int* für diese Klasse verwenden, der Compiler
besteht darauf, dass die Operationen mit allen möglichen Typen machbar sind. Eine Abhilfe schafft der
Einsatz der Funktion *default()*.
```c#
public void Undo()
{
    _value = OldValue;
    OldValue = default(T);
}
```

## In-dept: Generics in C# und Java sowie Templates in C++
Mit *GetType()* können wir den (CLR) Typ einer Instanz bestimmen. Wir sehen folgende Ausgabe:
```c#
Console.WriteLine(stringHistory.GetType());  // _09_Generics.History`1[System.String]
Console.WriteLine(intHistory.GetType());     // _09_Generics.History`1[System.Int32]
```

Das bedeutet, dass der Compiler wirklich 2 verschiedene Implementierungen der Klasse angelegt hat.
Java arbeitet anders, intern ersetzt es den Typ einfach durch *object* (falls keine Einschränkung
für den Typparameter definiert wurde) und der Compiler fügt den Code für den Typencast automatisch 
ein. Allerdings führt dies weiterhin zu Boxing von Wertetypen (vgl. [Oracle Docs](https://docs.oracle.com/javase/tutorial/java/data/autoboxing.html)).

Da in C# eine eigene Implementierung für die *int* Version von History angelegt wurde, wird das Programm
auch wirklich so ausgeführt, wie wenn wir die Klasse nur für *int* geschrieben hätten. Das ist der große
Unterschied zu den Java Generics.

Bei Referenztypen wird nur eine Implementierung angelegt ("Code Sharing"), da der Code sonst natürlich 
sehr groß werden würde.

Im Gegensatz zu den Templates in C++ gibt es allerdings in C# Einschränkungen. In der MSDN sind diese
sehr gut aufgezählt:
> C# generics do not provide the same amount of flexibility as C++ templates. For example, it is not 
> possible to call arithmetic operators in a C# generic class, although it is possible to call user defined operators.
>
> C# does not allow non-type template parameters, such as template C<int i> {}.
>
> C# does not support explicit specialization; that is, a custom implementation of a template for a specific type.
> 
> C# does not support partial specialization: a custom implementation for a subset of the type arguments.
> 
> C# does not allow the type parameter to be used as the base class for the generic type.
> 
> C# does not allow type parameters to have default types.
> 
> In C#, a generic type parameter cannot itself be a generic, although constructed types can be used as generics. C++ does allow template parameters.
> 
> C++ allows code that might not be valid for all type parameters in the template, which is then 
> checked for the specific type used as the type parameter. C# requires code in a class to be 
> written in such a way that it will work with any type that satisfies the constraints. For example, 
> in C++ it is possible to write a function that uses the arithmetic operators + and - on objects of 
> the type parameter, which will produce an error at the time of instantiation of the template with 
> a type that does not support these operators. C# disallows this; the only language constructs 
> allowed are those that can be deduced from the constraints.
>
> (Vgl. https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/differences-between-cpp-templates-and-csharp-generics)

## Generische Methoden und Typeinschränkungen (generic type constraint)
Wir wollen in einer beliebigen Klasse (die nicht generisch sein muss), eine allgemeine *Min()* Funktion
definieren. Generische Methoden haben - wie generische Klassen - einen Typparameter nach dem 
Methodennamen. Die erste Idee führt zu folgender Implementierung:
```c#
T Min<T>(T val1, T val2)
{
    if (val1 < val2 )
    { return val1; }
    return val2;
}
```

Das ist allerdings aus 2 Gründen nicht kompilierbar:
- Arithmetische Operationen sind nicht möglich (siehe voriger Punkt).
- Es ist nicht für jeden Typ ein &lt; Operator definiert.

Für alle möglichen Typen können wir unsere *Min()* Funktion also nicht umsetzen. Wir können sie aber
für Typen umsetzen, die *IComparable* implementieren.  Die Einschränkung auf *IComparable* können wir mit 
Hilfe der *where* Anweisung schreiben:
```c#
T Min<T>(T val1, T val2) where T : IComparable
{
    if (val1.CompareTo(val2) < 0)
    { return val1; }
    return val2;
}
```

Es dürfen also nur Typen als Typparameter verwendet werden, die die entsprechenden Constraints (hier
Ableitung von *IComparable*) erfüllen. Ein häufig verwendetes Constraint ist *where T : class*. Das
bedeutet, dass es sich um einen Referenztyp handeln muss.

Nun können wir unsere *Min()* Funktion flexibel nutzen:
```c#
Console.WriteLine(Min<int>(3, 4));                                                       // 3
Console.WriteLine(Min<DateTime>(new DateTime(2000, 1, 1), new DateTime(2001, 1, 1)));    // 1.1.2000
```

## Realitätsnahes Beispiel: Cached Objects
Wir erstellen uns zur Demonstration eine *Pupil* Klasse:
```c#
class Pupil
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public override string ToString() => $"{Id}: {Firstname} {Lastname}";
}
```

Nun wollen wir eine Klasse *Cached* implementieren. Diese soll sich so verhalten: Es wird eine Methode
übergeben, wie die Klasse zu einer Instanz dieses Objektes kommt. Das ist eine Lambda Expression vom
Typ *Func&lt;T&gt;*, also eine Funktion die *T* liefert. *T* kann durch einen Webrequest oder einfach - 
wie bei uns - ein mit *new* erstelles Objekt sein.

Die Instanz (oder allgemeiner: der Wert) wird über das Property *Value* zur Verfügung gestellt. Dabei soll
folgendes beachtet werden:
- Erst beim erstmaligem Auslesen wird das Objekt erzeugt. Dies kann z. B. ein Serverrequest sein.
- Wird das Property *Value* weitere Male ausgelesen, dann soll kein neues Objekt erzeugt, sondern die "alte"
   Instanz geliefert werden. Bei Serverrequests würde jedes Mal Nachfragen zu lange dauern.
- Wird das Property *Value* nach einer gewissen Zeit ausgelesen, soll wieder ein neues Objekt erstellt werden.

Mit dieser Technik könnten wir z. B. die Anzeige des Stundenplanes (also eine Collection von Stunden)
in einer App umsetzen. Wird er mehrmals abgerufen, so wird nicht jedesmal ein neuer Serverrequest 
gesendet. Erst nach einer gewissen Zeit soll neu geladen werden.

Die Implementierung kann so aussehen:
```c#
class Cached<T>
{
    // Der aktuelle Stand (der "Cache")
    private T _value;
    // Eine Function, die das Objekt liefert. Kann z. B. ein Webrequest sein, der die Collection
    // liefert.
    private readonly Func<T> _objectBuilder;
    // Wann wurde das Objekt zum letzten Mal geladen?
    private DateTime _generated = DateTime.MinValue;
    public T Value
    {
        get
        {
            // Achtung: Nicht Threadsafe!
            if (_generated + Expiration < DateTime.Now)
            {
                Console.WriteLine("Generiere neues Objekt.");
                _generated = DateTime.Now;
                // Aufruf der übergebenen Function, die das Objekt laden soll.
                _value = _objectBuilder();
            }
            return _value;
        }
    }
    // Nach welcher Zeitspanne soll das Objekt "ablaufen"?
    public TimeSpan Expiration { get; }
    public Cached(TimeSpan expires, Func<T> objectBuilder)
    {
        Expiration = expires;
        _objectBuilder = objectBuilder;
    }
}
```

Die Verwendung der Klasse ist sehr einfach. Etwas ungewähnlich ist vielleicht die Übergabe einer
Build-Funktion. Hier kann später ein Webrequest stehen, der das Ergebnis der JSON Antwort geparst als
neues *Pupil* Objekt zurückgibt.
```c#
// Die Lambda Expression wird noch nicht aufgerufen!
Cached<Pupil> myPupil = new Cached<Pupil>(TimeSpan.FromSeconds(2), () => new Pupil());
Pupil p;

Console.WriteLine("Lese Pupil Objekt:");
// Erstmaliger Abruf. Es wird die Ausgabe "Generiere neues Objekt." ausgegeben.
p = myPupil.Value;
Console.WriteLine("Lese ein weiteres Mal das Pupil Objekt:");
 // Keine Ausgabe, da wir den Wert des Caches liefern.
p = myPupil.Value; 
System.Threading.Thread.Sleep(3000);
Console.WriteLine("Lese ein weiteres Mal das Pupil Objekt:");
// Nach 3 Sekunden wird wieder "Generiere neues Objekt." ausgegeben, da das Objekt nur 2 Sekunden
// gültig ist.
p = myPupil.Value;
```

Eine ähnliche Idee verfolgt auch die im .NET Framework enthaltene *[Lazy](https://docs.microsoft.com/en-us/dotnet/api/system.lazy-1?view=netframework-4.8)* Klasse, allerdings fordert 
sie ein einmal erstelltes Objekt nicht mehr erneut an.