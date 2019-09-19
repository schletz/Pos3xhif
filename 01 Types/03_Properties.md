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

