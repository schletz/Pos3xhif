# Properties und Initializer in C#

## Erstellen einer Visual Studio Solution

Um die Beispiele mitmachen zu können, muss eine .NET Konsolenapplikation erstellt werden. Führe
dafür die folgenden Befehle in der Konsole aus. Unter macOs müssen md und rd durch die entsprechenden
Befehle ersetzt werden.

```text
rd /S /Q PropertiesDemo
md PropertiesDemo
cd PropertiesDemo
md PropertiesDemo.Application
cd PropertiesDemo.Application
dotnet new console
cd ..
dotnet new sln
dotnet sln add PropertiesDemo.Application
start PropertiesDemo.sln

```

Öffne danach durch Doppelklick auf das Projekt (*PropertiesDemo.Application*) die Datei
*PropertiesDemo.Application.csproj* und füge die Optionen für
*Nullable* und *TreatWarningsAsError* hinzu. Die gesamte Konfiguration muss nun so aussehen:

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
## Wozu Properties?

Betrachten wir eine Klasse *Student*, mit den get und set Methoden, wie wir sie aus Java kennen:

```c#
class Student
{
    string vorname;
    string zuname;
    int alter;

    public Student(string vorname, string zuname)
    {
        this.vorname = vorname;
        this.zuname = zuname;
        setAlter(0);
    }

    public Student(string vorname, string zuname, int alter)
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
class Student
{
    private string vorname, zuname;
    private int alter;

    public Student(string vorname, string zuname, int alter)
    {
        this.vorname = vorname;
        this.zuname = zuname;
        this.alter = alter;
    }

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

> **Achtung:** Oft ist folgender Fehler zu beobachten. *Alter* ist in der get Methode
> großgeschrieben, daher wird eine endlose Rekursion erzeugt:

```c#
public int Alter
{
    get { return Alter; }
}
```

### Default Properties

Die Properties für *Vorname* und *Zuname* weisen nur 1:1 zu bzw. geben den Wert 1:1 zurück. In C# gibt
es mit den *Default Properties* einen eleganteren Weg, das zu bewerkstelligen. Der Compiler erledigt
folgende Dinge:

- Es wird automatisch eine private Variable im Hintergrund (das *backing field*) vom Compiler angelegt.
- Die get und set Methode liefert diese 1:1 zurück bzw. schreibt in diese hinein.
  
```c#
class Student
{
    public Student(string vorname, string zuname)
    {
        Vorname = vorname;
        Zuname = zuname;
    }

    public string Vorname { get; set; }
    public string Zuname { get; set; }
}
```

Möchte man die Default Properties gleich initialisieren, ist dies seit C# 6 auch möglich:
```c#
class Student
{
    public string Vorname { get; set; } = string.Empty;
    public string Zuname { get; set; } = string.Empty;
}
```

### Read-only Properties

#### Berechnete Werte

Wird nur eine get Methode definiert, so kann diesem Property nichts zugewiesen werden. Dies ist
bei berechneten Werten sinnvoll.

```c#
class Student
{
    public string Longname
    {
        get { return $"{Vorname} {Zuname}"; }
    }
}
```

Ab C# 7 wurden Expression-bodied members auch für Properties eingeführt. Dadurch kann das
folgende Property kürzer definiert werden. Es handelt sich aber immer noch um eine Methode,
d. h. sie wird bei jedem Zugriff auf *Longname* ausgeführt und liefert die aktuellen Werte.

```c#
class Student
{
    public string Longname => $"{Vorname} {Zuname}";
}
```

#### Unveränderliche Werte (immutable)

Oft sollen Properties nach ihrer Initialisierung nicht mehr verändert werden. So ist z. B. die
Änderung von Vor- und Zuname nach Instanzierung der *Student* Klasse nicht notwendig. Die E-Mail
Adresse soll jedoch geändert werden können. Durch die Definition mit *get* lassen sich die
Properties *Vorname* und *Zuname* nur im Konstruktor oder durch Initialisierung mit =
setzen.

```c#
class Student
{
    public Student(string vorname, string zuname)
    {
        Vorname = vorname;
        Zuname = zuname;
    }
    public string Vorname { get; }
    public string Zuname { get; } 
    public string? Email { get; set; }  // Nullable, daher nicht im Konstruktor.
}
```

#### private set Properties

Sollen Properties wie Id nur innerhalb der Klasse geschrieben werden können, so kann die
set Methode auch private (oder protected) definiert werden.

```c#
class Student
{
    public Student(string vorname, string zuname)
    {
        Vorname = vorname;
        Zuname = zuname;
    }
    public int Id { get; private set; }
    public string Vorname { get; }
    public string Zuname { get; } 
    public string? Email { get; set; }
    public void GenerateId()
    {
        Id = new Random().Next();
    }
}
```

## Verwendung der Properties, Initializer

Der große Vorteil von Properties liegt in ihrer eleganten Verwendung. Folgende Anweisungen sind
dadurch möglich:

```c#
Student student = new Student(vorname: "VN1", zuname: "ZN1")
{
    Email = "test@mail.at"
};
```
Was passiert hier? Zuerst wird ein Objekt vom Typ Student mit *new Student(...)* erzeugt. Da ein
Konstruktor definiert wurde, existiert kein default Konstruktor und wir müssen daher die notwendigen
Argumente übergeben. In C# ist es durch den *initializer* möglich, Properties gleich *nach* der
Instanzierung zu initialisieren.

Da die set Methoden durchlaufen werden, wird die Datenprüfung natürlich auch im Initializer ausgeführt.

## Übung

Erstelle wie oben beschrieben die Solution *PropertiesDemo*. Es sind 2 Klassen zu implementieren:
*Rectangle* und *Teacher*.

Für die Klasse *Rectangle* gelten folgende Regeln:
- Es gibt 2 int Properties mit dem Namen *Width* und *Height*.
- Diese Properties dürfen nur in der Klasse zugewiesen werden und nicht von außen.
- Wird diesen Properties ein Wert kleiner als 0 zugewiesen, wird mittels 
  *throw new ArgumentException("Ungültige Länge")*
  eine Exception geworfen.
- Das Property *Area* ist read-only und wird mit Länge x Breite ermittelt.
- Die Methode *Scale()* skaliert Länge und Breite mit dem übergebenen Faktor. Die Prüfung, ob
  ein negativer Scalingfaktor zu ungültigen Werten in *Width* und *Height* führt soll nicht in
  der Methode implementiert werden.


Für die Klasse *Teacher* gelten folgende Regeln:
- Die string Properties *Firstname* und *Lastname* sind default Properties und immutable.
- Das Property *Longname* soll den Namen in der Form *Vorname Zuname* zurückgeben.
- Das Property *Shortname* soll read-only definiert werden. Es gibt die ersten 3 Stellen des Zunamens
  in Großschreibung zurück. Die Methoden *Substring(0, 3)* und *ToUpper()* können hier verwendet werden.
- Das string Property *Email* kann leere Werte enthalten und kann auch nach der Instanzierung
  gesetzt werden.
- Das Property *IsSchoolEmail* ist ein berechneter Wert und liefert true, wenn in der Email der String
  "@spengergasse.at" gefunden wurde. Prüfe dies mit *EndsWith()*
- Das Property *Salary* soll decimal Werte speichern und den Standardwert null haben. Überlege
  dir den Datentyp, der diese Werte speichern kann. Für die Zuweisung gilt eine spezielle Regelung:
  Wurde schon ein Gehalt zugewiesen (Wert also ungleich null), so darf dieser nicht überschrieben werden.
  Bei einer Zuweisung soll in diesem Fall einfach nichts passieren.
- Das Property *NetSalary* liefert 80% des Bruttogehaltes (also * 0.8M). Beachte, dass decimal Literale
  mit M enden müssen (also 0.8M). Ist das Bruttogehalt null, so soll das Nettogehalt 0 sein. Der Wert
  von Nettogehalt ist also niemals null. Löse diese Berechnung mit dem ?? Operator.

Verwende wenn möglich C# 7 Expression Bodies. Überlege dir, ob *Longname*, *IsSchoolEmail* und
*NetSalary* Methoden sein müssen oder die Werte fix gesetzt werden können.

Die Ausgabe des Programmes muss am Ende so lauten:

```
********************************************************************************
TESTS FÜR RECTANGLE
********************************************************************************
1 Kein default Konstruktor OK
2 Area OK
3 Scale OK
4 Kein Setzen der Breite und Höhe: OK
5 Exception bei negativer Breite OK
6 Exception bei negativer Höhe OK
7 Exception bei Scale OK
********************************************************************************
TESTS FÜR TEACHER
********************************************************************************
1 Kein default Konstruktor OK
2 Vor- und Zuname sind immutable: OK
3 Longname OK
4 Shortname OK
5 NetSalary OK
6 Salary OK
7 IsSchoolEmail OK
```

### Program.cs
```c#
using System;

namespace PropertiesDemo.Application
{
    internal class Program
    {
        private class Rectangle
        {
            // TODO: Implementierung von Rectangle

        }

        private class Teacher
        {
            // TODO: Implementierung von Teacher
        }

        // DON'T TOUCH!
        private static void Main(string[] args)
        {
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("TESTS FÜR RECTANGLE");
            Console.WriteLine("********************************************************************************");
            if (typeof(Rectangle).GetConstructor(Type.EmptyTypes) is null) { Console.WriteLine("1 Kein default Konstruktor OK"); }
            Rectangle rect = new Rectangle(width: 10, height: 20);
            if (rect.Area == 200) { Console.WriteLine("2 Area OK"); }
            rect.Scale(2);
            if (rect.Area == 800) { Console.WriteLine("3 Scale OK"); }

            if (typeof(Rectangle).GetProperty(nameof(Rectangle.Width))?.SetMethod?.IsPublic == false
                && typeof(Rectangle).GetProperty(nameof(Rectangle.Height))?.SetMethod?.IsPublic == false)
            {
                Console.WriteLine("4 Kein Setzen der Breite und Höhe: OK");
            }
            try
            {
                Rectangle rect2 = new Rectangle(width: -1, height: 20);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("5 Exception bei negativer Breite OK");
            }
            try
            {
                Rectangle rect2 = new Rectangle(width: 10, height: -1);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("6 Exception bei negativer Höhe OK");
            }
            try
            {
                rect.Scale(-1);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("7 Exception bei Scale OK");
            }

            Console.WriteLine("********************************************************************************");
            Console.WriteLine("TESTS FÜR TEACHER");
            Console.WriteLine("********************************************************************************");
            if (typeof(Teacher).GetConstructor(Type.EmptyTypes) is null) { Console.WriteLine("1 Kein default Konstruktor OK"); }
            if (typeof(Teacher).GetProperty(nameof(Teacher.Firstname))?.CanWrite == false
                && typeof(Teacher).GetProperty(nameof(Teacher.Lastname))?.CanWrite == false)
            {
                Console.WriteLine("2 Vor- und Zuname sind immutable: OK");
            }

            Teacher t1 = new Teacher(firstname: "Fn", lastname: "Ln");
            Teacher t2 = new Teacher(firstname: "Fn", lastname: "Lastname") { Email = "test@spengergasse.at", Salary = 2000M };
            if (typeof(Teacher).GetProperty(nameof(Teacher.Longname))?.CanWrite == false
                && t1.Longname == "Fn Ln") { Console.WriteLine("3 Longname OK"); }
            if (typeof(Teacher).GetProperty(nameof(Teacher.Shortname))?.CanWrite == false
                && t1.Shortname == "LN" && t2.Shortname == "LAS") { Console.WriteLine("4 Shortname OK"); }
            if (t1.NetSalary == 0 && t2.NetSalary == 1600) { Console.WriteLine("5 NetSalary OK"); }
            t1.Salary = 1000;
            t2.Salary = 1000;
            if (t1.Salary == 1000 && t2.Salary == 2000) { Console.WriteLine("6 Salary OK"); }
            if (typeof(Teacher).GetProperty(nameof(Teacher.IsSchoolEmail))?.CanWrite == false
                && !t1.IsSchoolEmail && t2.IsSchoolEmail) { Console.WriteLine("7 IsSchoolEmail OK"); }
        }
    }
}
```