# Vererbung 2: Interfaces implementieren: IEquatable und IComparable
Vererbung wird hauptsächlich dazu verwendet, um ein bestehendes Framework (in unserem Fall das .NET
Framework) zu erweitern. Oft werden Interfaces, die bereitgestellt werden, in eigenen Klassen implementiert.

Wir implementieren in diesem Beispiel 2 bekannte Interfaces: *IEquatable&lt;T&gt;* und *IComparable&lt;T&gt;*. Diese
Interfaces sind generisch, die bei der Vererbung für einen Typ spezifiziert werden. Diese Technik wird
uns bei der Vererbung noch oft begegnen. In unserem Musterbeispiel implementieren wir die Klasse Position,
die den Längen- und Breitengrad eines Punktes speichert:
```c#
class Position : IEquatable<Position>, IComparable, IComparable<Position>
{
    public Position(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }

    // Read only Properties für Lat und Lng, damit sich der Hash nicht mehr ändert.
    public double Lat { get; }
    public double Lng { get; }
    public double LatRad => Lat * Math.PI / 180;
    public double LngRad => Lng * Math.PI / 180;
    /// <summary>
    /// Vergleicht 2 Positionen über die geografische Breite (Lat)
    /// </summary>
    public int CompareTo(Position? other) => Lat.CompareTo(other?.Lat);
    public int CompareTo(object? obj) => CompareTo(obj as Position);

    /// <summary>
    /// 2 Punkte sind ident, wenn sie die selbe Länge und Breite haben.
    /// </summary>
    public bool Equals(Position? other) => Lat.Equals(other?.Lat) && Lng.Equals(other?.Lng);
    public override bool Equals(object? obj) => Equals(obj as Position);

    public double GetDistance(Position p)
    {
        return Math.Acos(
            Math.Sin(LatRad) * Math.Sin(p.LatRad) +
            Math.Cos(LatRad) * Math.Cos(p.LatRad) * Math.Cos(p.LngRad - LngRad)) * 6370;
    }
    /// <summary>
    /// Überschreibt GetHashCode() von object, sodass ihr Verhalten mit Equals übereinstimmt . Kein Interface nötig.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Lat, Lng);

    /// <summary>
    /// Überschreibt ToString von object. Kein Interface nötig.
    /// </summary>
    public override string ToString() => $"({Lat} {Lng} (Hash: {GetHashCode()})";

    // Überschreiben der Operatoren für den Vergleich, damit sie das selbe Verhalten wie
    // CompareTo() zeigen.
    public static bool operator <(Position p1, Position p2) => p1.CompareTo(p2) < 0;
    public static bool operator <=(Position p1, Position p2) => p1.CompareTo(p2) <= 0;
    public static bool operator >(Position p1, Position p2) => p1.CompareTo(p2) > 0;
    public static bool operator >=(Position p1, Position p2) => p1.CompareTo(p2) >= 0;
}

class Program
{
    static void Main(string[] args)
    {
        // Wien
        Position p1 = new Position(48.2083, 16.3725);
        // St. Pölten
        Position p2 = new Position(48.2043, 15.6229);
        // Eisenstadt
        Position p3 = new Position(47.8387, 16.5362);
        // Wien nocheinmal
        Position p4 = new Position(48.2083, 16.3725);

        Console.WriteLine($"Abstand von Wien nach Eisenstadt: {p3.GetDistance(p1)} km.");
        Console.WriteLine($"Wien = St. Pölten?:               {p1 == p2}");
        Console.WriteLine($"Eisenstadt südlicher als Wien?:   {p3 < p1}");

        Console.WriteLine($"Instanzen von Wien ident?:                 {p1.Equals(p4)}");
        Console.WriteLine($"Instanzen von Wien auch mit == ident?:     {p1 == p4}");
        Console.WriteLine($"Instanzen von Wien auch als object ident?: {p1.Equals((object)p4)}");
        Console.WriteLine($"Gleiche Referenz der Instanzen von Wien?:  {object.ReferenceEquals(p1, p4)}");

        List<Position> positions = new List<Position>() { p1, p2, p3, p4 };
        Console.WriteLine("Punkte sortiert von Süd nach Nord.");
        positions.Sort();
        foreach (Position p in positions)
        {
            Console.WriteLine(p);
        }
        Console.WriteLine("Hashset:");
        HashSet<Position> positionsHash = new HashSet<Position>() { p1, p2, p3, p4 };
        foreach (Position p in positionsHash)
        {
            Console.WriteLine(p);
        }
    }
}
```

```
Abstand von Wien nach Eisenstadt: 42.8561852967433 km.
Wien = St. Pölten?:               False
Eisenstadt südlicher als Wien?:   True
Instanzen von Wien ident?:                 True
Instanzen von Wien auch mit == ident?:     False
Instanzen von Wien auch als object ident?: True
Gleiche Referenz der Instanzen von Wien?:  False
Punkte sortiert von Süd nach Nord.
(47.8387 16.5362 (Hash: -490148674)
(48.2043 15.6229 (Hash: 1061893839)
(48.2083 16.3725 (Hash: -1148819547)
(48.2083 16.3725 (Hash: -1148819547)
Hashset:
(48.2083 16.3725 (Hash: -1148819547)
(48.2043 15.6229 (Hash: 1061893839)
(47.8387 16.5362 (Hash: -490148674)
```

## Punkt ist ein *Immutable Type*
Es scheint vielleicht seltsam, dass wir Länge und Breite im Konstruktor setzen und diese Werte im Nachhinein
nicht mehr geändert werden können. Dies wird durch ein read-only Property erzielt. Diese Properties können
jedoch im Konstruktor initialisiert werden. Nach dem Durcharbeiten dieses Kapitels ist der Sinn klar.

## *IEquatable&lt;T&gt;* und *object.Equals()*
Bei Referenztypen wird zwischen Referenzgleichheit (Referenzvariable zeigt auf die gleiche Instanz) und
Inhaltsgleichheit (der Inhalt der Instanz ist ident) unterschieden. Aus Java bekannt ist das Überschreiben
der *Equals()* Methode, welche in *object* definiert ist. Da wir auch beim Cast auf *object* diese Methode
aufrufen wollen, verwenden wir das Schlüsselwort *override*. Damit nicht bei jedem Vergleich zwischen
2 identen Typen ein Cast auf *object* vorgenommen werden muss, implementiert der gute .NET Programmierer
zusätzlich das Interface *IEquatable&lt;T&gt;*. Diese Implementierungen müssen natürlich das selbe Verhalten zeigen.
Dies lässt sich am Leichtesten so erreichen, dass *object.Equals()* die spezifischere *Equals()* Methode aufruft.

Es ist darauf zu achten, dass die Methode keine Exception verursacht. Wird null übergeben, so soll diese
Methode einfach *false* liefern. Der *?.* Operator hilft uns hierbei.
```c#
public bool Equals(Position? other) => Lat.Equals(other?.Lat) && Lng.Equals(other?.Lng);
public override bool Equals(object? obj) => Equals(obj as Position);
```

### Was ist der Hashcode?
Equals wird in den darunterliegenden Frameworkmethoden häufig aufgerufen. Daher ist die Performance bei
Vergleichen wesentlich. Damit nicht ganze Felder verglichen werden müssen, ist in .NET die Methode
*object.GetHashCode()* implementiert, die wir entsprechend überschreiben sollten. Sie muss folgendes
Verhalten zeigen:
- Instanzen, die in *Equals()* als ident bestimmt werden, müssen den selben Hashcode liefern.
- Instanzen mit gleichem Hashcode müssen nicht immer ident sein. Da die Instanz auf einen einzigen *int*
  Wert abgebildet wird, ist eine Kollision nicht immer vermeidbar. Diese Kollisionen sollen aber minimiert
  werden. 

Um das zu erreichen, verwenden wir die Methode *HashCode.Combine()* die uns im Namespace System
zur Verfügung steht:
```c#
public override int GetHashCode() => HashCode.Combine(Lat, Lng);
```

### Wer verwendet *Equals()*?
In diesem Beispiel wurden Instanzen von *Position* in einem *HashSet&lt;T&gt;* gespeichert. Beim Hinzufügen
wird über *GetHashCode()* und in weiterer Folge mit *Equals()* geprüft, ob es sich um idente Instanzen
handelt. Deswegen ist es auch wichtig, dass *GetHashCode()* korrekt implementiert ist.

### *Equals()* und der *==* Operator
Wird *Equals()* überschrieben, so liefert die Methode ein anderes Vergleichsergebnis als der == 
und der != Operator.
Die Guidelines zum Überschreiben
(vgl. [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type))
besagen, dass dies optional, aber empfohlen ist.

## *IComparable* sowie *IComparable&lt;T&gt;*
Sollen Instanzen verglichen werden, so können diese beiden Interfaces implementiert werden. Aus Performancegründen
gibt es hier ein generisches Interface, es soll jedoch auch das nicht generische Interface entsprechend
implementiert werden. Dies wird durch folgende Implementierung erreicht:
```c#
public int CompareTo(Position? other) => Lat.CompareTo(other?.Lat);
public int CompareTo(object? obj) => CompareTo(obj as Position);
```

Auch hier ist es am Einfachsten, die *CompareTo()* Methode des zu vergleichenden Feldes einfach aufzurufen.
Damit ist die korrekte Implementierung sichergestellt:
- Es wird ein Wert kleiner als 0 geliefert, wenn die aktuelle Instanz vor *other* eingeordnet werden soll.
- Es wird 0 geliefert, wenn die aktuelle Instanz gleich mit *other* ist.
- Es wird ein Wert größer als 0 geliefert, wenn die aktuelle Instanz nach *other* eingeordnet werden soll.

### Guter Stil: Operatoren ebenfalls überschreiben
> If you implement IComparable&lt;T&gt;, you should overload the op_GreaterThan, op_GreaterThanOrEqual, 
> op_LessThan, and op_LessThanOrEqual operators to return values that are consistent with CompareTo(T). 
> In addition, you should also implement IEquatable&lt;T&gt;. See the IEquatable&lt;T&gt; article for complete information.
> (Vgl. [MSDN: IComparable&lt;T&gt; Interface](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1?view=netframework-4.8))

In C# besteht die Möglichkeit, Operatoren zu überladen. Wenn dies nicht gemacht wird, liefern *CompareTo()*
und die Vergleichsoperatoren unterschiedliche Ergebnisse. Auf die Technik des Überladens wird hier nicht
eingegangen, die nachfolgenden Methoden implementieren bereits die Lambda Schreibweise. Hier wird vom
Compiler automatisch ein *return* vor dem Ausdruck rechts vom => ergänzt.
```c#
public static bool operator < (Position p1, Position p2) => p1.CompareTo(p2) < 0;
public static bool operator <=(Position p1, Position p2) => p1.CompareTo(p2) <= 0;
public static bool operator > (Position p1, Position p2) => p1.CompareTo(p2) > 0;
public static bool operator >=(Position p1, Position p2) => p1.CompareTo(p2) > 0;
```

### *CompareTo()* und *Equals()* sollten das selbe Verhalten zeigen
> Typically, types that provide an IComparable&lt;T&gt; implementation also implement the IEquatable&lt;T&gt; interface. 
> The IEquatable&lt;T&gt; interface defines the Equals method, which determines the equality of instances of 
> the implementing type.
> (Vgl. [MSDN: IComparable&lt;T&gt; Interface](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1?view=netframework-4.8#notes-to-implementers))

## Wann soll *Equals()* überschrieben werden?
> Guidelines for Reference Types
>
> The following guidelines apply to overriding Equals(Object) for a reference type:
> Consider overriding Equals if the semantics of the type are based on the fact that the type represents 
> some value(s).
>
> Most reference types must not overload the equality operator, even if they override Equals. However, 
> if you are implementing a reference type that is intended to have value semantics, such as a complex 
> number type, you must override the equality operator.
>
> You should not override Equals on a mutable reference type. This is because overriding Equals requires 
> that you also override the GetHashCode method, as discussed in the previous section. This means 
> that the hash code of an instance of a mutable reference type can change during its lifetime, which 
> can cause the object to be lost in a hash table. 
> (Vgl. [MSDN: Object.Equals Method](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=netframework-4.8#System_Object_Equals_System_Object_))

Diese Erklärung gibt uns schon eindeutig zu verstehen, dass das Überschreiben von *Equals()* und *CompareTo()*
zwar beherrscht werden sollten, ihre Anwendung jedoch weit seltener als vielleicht gedacht ist.

### Warum diese Strenge?
Wir betrachten diese technisch vollkommen korrekte Implementierung von *Equals()*:
```c#
class Person : IEquatable<Person>
{
    public int Nr { get; set; }
    public string Zunamne { get; set; } = string.Empty;
    public string Vorname { get; set; } = string.Empty;

    public bool Equals(Person? other) => Nr.Equals(other?.Nr);
    public override bool Equals(object? obj) => Equals(obj);
    public override int GetHashCode() => Nr.GetHashCode();
}
```

Speichern wir nun Instanzen in einem *HashSet&lt;T&gt;* und ändern nachträglich die Nummer einer Person,
hat unser Hashset auf einmal 2 idente Instanzen:
```c#
Person pe1 = new Person { Nr = 1, Vorname = "VN1", Zunamne = "ZN1" };
Person pe2 = new Person { Nr = 2, Vorname = "VN2", Zunamne = "ZN2" };
HashSet<Person> personHash = new HashSet<Person>() { pe1, pe2 };
pe2.Nr = 1;                        // Nachträgliche Änderung von pe2.
// personHash hat nun 2 Einträge, die jedoch lt. Equals() ident sind!
```

## Records in C# 9

Die Klasse *Position* hat 2 Eigenschaften, die oft benötigt werden:
- Alle Properties sind read-only.
- Sie besitzt einen Konstruktor mit allen Properties zur Zuweisung der Werte.
- Sie überschreibt Equals() so, dass 2 Instanzen gleich sind, wenn alle Properties gleich sind.

Seit C# 9 können wir die Klasse wesentlich kürzer definieren:

```c#
using System;

record Position(double Lat, double Lng);   // "Positional record" in C# 9

public class Program
{
	public static void Main()
	{
		Position position1 = new Position(Lat: 48, Lng: 16);
		Position position2 = new Position(Lat: 48, Lng: 16);
		Console.WriteLine(position1 == position2);              // True
		Console.WriteLine(position1.Equals(position2));         // True
        // Erzeugt eine neue Position, indem die Werte von position1 geändert in ein neues
        // Objekt geschrieben werden (Position selbst ist natürlich immutable).
		Position position3 = position1 with {Lng = 17};
		Console.WriteLine(position3);           // ToString liefert Position { Lat = 48, Lng = 17 }
        // Deconstruct
		var (lat, lng) = position3;
		Console.WriteLine($"{lat}° NB, {lng}° ÖL");        
	}
}
```

Es ist natürlich auch möglich, zusätzliche Properties (sowohl get und set) oder Methoden in einem
Record zu definieren:

```c#
record Position(double Lat, double Lng)
{
	public double LatRad => Lat * Math.PI / 180;
    public double LngRad => Lng * Math.PI / 180;
    public double GetDistance(Position p)
    {
        return Math.Acos(
            Math.Sin(LatRad) * Math.Sin(p.LatRad) +
            Math.Cos(LatRad) * Math.Cos(p.LatRad) * Math.Cos(p.LngRad - LngRad)) * 6370;
    }	
}
```

## Übung

Erstelle eine neue Solution mit dem Titel *EqualsExercise*. Kopiere danach die Klasse *Program* von der
untenstehenden Angabe in deine cs Datei. 

Ergänze danach die Klasse *PhoneNr* so, sodass das Programm kompilierbar und die untenstehenden Ausgaben 
erreicht werden. Zwei Instanzen von *PhoneNr* sind ident, wenn ihre Vorwahl und ihre Telefonnummer gleich
sind. Die Sortierung erfolgt zuerst nach der Vorwahl und dann nach der Telefonnummer.

Erstelle danach eine Klasse *PhoneRecord*, wo diese Klasse als C# 9 Record definiert wird.
Equals muss natürlich nicht mehr überschrieben werden, nur *IComparable*

```c#
class PhoneNr : IEquatable<PhoneNr>, IComparable<PhoneNr>, IComparable
{
    public long Vorwahl { get; }
    public long Telefonummer { get; }
}


class Program
{
    static void Main(string[] args)
    {
        // HTL Wien V
        PhoneNr nr1 = new PhoneNr(01, 54615);
        // BMBWF
        PhoneNr nr2 = new PhoneNr(01, 53120);
        // Handynummer
        PhoneNr nr3 = new PhoneNr(0699, 99999999);
        // HTL Wien V
        PhoneNr nr4 = new PhoneNr(01, 54615);

        Console.WriteLine($"nr1 ist ident mit nr2?:           {nr1.Equals(nr2)}");
        Console.WriteLine($"nr1 ist ident mit nr4?:           {nr1.Equals(nr4)}");
        Console.WriteLine($"nr1 ist ident mit (object) nr4?:  {nr1.Equals((object) nr4)}");
        Console.WriteLine($"nr3 ist ident mit null?:          {nr3.Equals(null)}");
        Console.WriteLine($"nr3 ist größer als n4?:           {nr3.CompareTo(nr4) > 0}");
        Console.WriteLine($"nr3 ist größer als n4?:           {nr3 > nr4}");

        Console.WriteLine($"Hash von nr1:           {nr1.GetHashCode()}");
        Console.WriteLine($"Hash von nr4:           {nr4.GetHashCode()}");

        List<PhoneNr> numbers = new List<PhoneNr>() { nr1, nr2, nr3, nr4 };
        numbers.Sort();
        foreach (PhoneNr n in numbers)
        {
            Console.WriteLine(n);
        }
    }
}
```

```
nr1 ist ident mit nr2?:           False
nr1 ist ident mit nr4?:           True
nr1 ist ident mit (object) nr4?:  True
nr3 ist ident mit null?:          False
nr3 ist größer als n4?:           True
nr3 ist größer als n4?:           True
Hash von nr1:           (nr1 muss ident mit nr4 sein)
Hash von nr4:           (nr4 muss ident mit nr4 sein)
01/53120
01/54615
01/54615
0699/99999999
```
