# Collections in .NET
Der Zugang zu Collections führt über den Namespace *System.Collections.Generic*. Er muss mit *using*
eingebunden werden:
```c#
using System.Collections.Generic;
```

Für unsere Beispiele verwenden wir die Klasse Person, eine datenhaltende Klasse mit 3 Properties:
```c#
class Person
{
    public int Id { get; set; }
    public string Zuname { get; set; }
    public string Vorname { get; set; }
    public override string ToString()
    {
        return $"{Id} {Zuname} {Vorname}";
    }
}
```

## Die Liste (ArrayList in Java)
Listen sind die am meisten verwendete Collection. Sie werden generisch durch Angabe des zu speichernden
Typs erstellt. Nützlich in C#: Auch der Initializer kann verwendet werden.
```c#
List<Person> persList1 = new List<Person>();
List<Person> persList2 = new List<Person>
{
    new Person() {Id = 1, Zuname = "Zuname1", Vorname = "Vorname1"},
    new Person() {Id = 2, Zuname = "Zuname2", Vorname = "Vorname2"},
    new Person() {Id = 3, Zuname = "Zuname3", Vorname = "Vorname3"},
};
```

### Abfragen, Hinzufügen und Löschen von Elementen
Der Indexer ([]) greift - wie bei einem Array - nullbasierend auf das n-te Element zu. Mit der Methode
*Add()* wird ein neues Element in die Liste eingefügt. Natürlich kann auch mit *foreach* durch die Liste
iteriert werden.
```c#
persList2.Add(new Person() { Id = 4, Zuname = "Zuname4", Vorname = "Vorname4" });

Person found = persList2[1];               // found ist die Persopn mit der ID 2.
foreach (Person p in persList2)
{
    Console.WriteLine(p);
}
```

Werden Elemente gelöscht, wird über die *Equals()* Methode nach Elementen in der Liste gesucht. Daher wird
die Person mit der ID 1 nicht gelöscht, da pe eine andere Rerefenzadresse besitzt.
```c#
persList2.Remove(found);
Person pe = new Person() { Id = 1, Zuname = "Zuname1", Vorname = "Vorname1" };
persList2.Remove(pe);
```

## Das Dictionary (HashMap in Java)
Oft gibt es bei datenhaltenden Klassen ein Property, welches als Key dient. Dies ist eindeutig und kann
in einem Dictionary verwendet werden. Das Dictionary entspricht einer Datenbanktabelle, über dem Key
kann auf den Datensatz zugegriffen werden.

Das folgende Beispiel erstellt ein Dictionary mit einem *int* Feld als Key und einer Person als Wert:
```c#
Dictionary<int, Person> personDict = new Dictionary<int, Person>();
personDict.Add(1, new Person() { Id = 1, Zuname = "Zuname1", Vorname = "Vorname1" });
personDict.Add(2, new Person() { Id = 2, Zuname = "Zuname2", Vorname = "Vorname2" });
personDict.Add(3, new Person() { Id = 3, Zuname = "Zuname3", Vorname = "Vorname3" });
```

Wird in einem Dictionary versucht, einen bestehenden Key hinzuzufügen, wird eine *ArgumentException
(An item with the same key has already been added)* ausgelöst. Um das zu vermeiden, können die Methoden
*ContainsKey()* bzw. *TryAdd()* verwendet werden.
```c#
// ArgumentException: An item with the same key has already been added
personDict.Add(1, new Person() { Id = 4, Zuname = "Zuname4", Vorname = "Vorname4" });
```

Das Löschen von Elementen wird mit *Remove()* über den Key durchgeführt:
```c#
personDict.Remove(2);
```

### Der Indexer
Der Zugriff mit dem Indexoperator ([]) ist im Gegensatz zu Java flexibler, denn er kann überladen werden.
Im Dictionary bedeutet dieser Operator den Zugriff über den Key.
```c#
Dictionary<string, string> lehrerDict = new Dictionary<string, string>();
lehrerDict.Add("SIL", "Siller");
lehrerDict.Add("TT", "Tschernko");
lehrerDict.Add("SCG", "Schildberger");
lehrerDict.Add("SZ", "Schletz");
string lookup = lehrerDict["SZ"];
```

## HashSet (HashSet in Java)
Einen Spezialfall stellt das Hashset dar. Oft sollen - wie bei *DISTINCT* in SQL - doppelte Werte entfernt
werden. Das Hashset speichert durch *Add()* nur den ersten Wert, nachfolgende idente Werte werden ignoriert.
Die Gleichheit wird bei Referenztypen auch über *Equals()* ermittelt.
```c#
HashSet<string> lehrerHashSet = new HashSet<string>();
lehrerHashSet.Add("SZ");
// Wird einfach ignoriert.
lehrerHashSet.Add("SZ");
foreach(string lehrer in lehrerHashSet)
{
    Console.WriteLine(lehrer);
}            
```

## Übung

Erstelle ein Projekt mit dem Namen *ExCollection* durch die untenstehenden Befehle in der Konsole. 
Ersetze danach den Inhalt von Program.cs durch die untenstehende Version. Vervollständige die 2 Klassen 
*SchoolClass* und *Student* so, dass die Ausgaben des Programmes korrekt sind.

```text
md ExCollection
cd ExCollection
dotnet new console
start ExCollection.csproj
```

Aktiviere die nullable reference types, indem der Eintrag `<Nullable>enable</Nullable>` zur csproj Datei
hinzugefügt wird. Es dürfen keine Warnings im fertigen Programm enthalten sein.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>

```

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExColletions
{
    /// <summary>
    /// TODO: 
    ///    - Create a constructor to initialize Name, ClassTeacher (KV) and City (Wohnort).
    ///    - Add a List of students to manage the students in this class.
    ///    - Use IReadOnlyList for your public property. It sould NOT be possible to add or remove students from outside
    ///      without calling AddStudent or RemoveStudent.
    ///    - Add a read-only property of type HashSet<string> to get the different cities in this class.
    /// </summary>
    class SchoolClass
    {
        public string Name { get; }
        public string ClassTeacher { get; }
        public string City { get; }
        /// <summary>
        /// Adds a student and modifies the schoolclass reference of the provided
        /// student.
        /// </summary>
        public void AddStudent(Student s)
        {
        }

        /// <summary>
        /// Removes a student and modifies the schoolclass reference of the provided
        /// student.
        /// </summary>
        public void RemoveStudent(Student s)
        {
        }
    }

    /// <summary>
    /// TODO: 
    ///    - Add a constructor to initialize the properties Id, Firstname, Lastname and City.
    ///    - Add a reference to the class of the student (type SchoolClass). This reference is optional,
    ///      if a student is not assigned to a class is has the value null.
    ///    - Add an annotation [JsonIgore] above this property to suppress the content of
    ///      the class object in your serialized output.
    /// </summary>
    class Student
    {
        public int Id { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string City { get; set; }
        /// <summary>
        /// Updates the reference of the student and adds the student to the new class.
        /// </summary>
        /// <param name="k"></param>
        public void ChangeClass(SchoolClass k)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, SchoolClass> classes = new();
            classes.Add("3AHIF", new SchoolClass(name: "3AHIF", classTeacher: "KV1"));
            classes.Add("3BHIF", new SchoolClass(name: "3BHIF", classTeacher: "KV2"));
            classes.Add("3CHIF", new SchoolClass(name: "3CHIF", classTeacher: "KV3"));

            classes["3AHIF"].AddStudent(new Student(id: 1001, firstname: "FN1", lastname: "LN1", city: "CTY1"));
            classes["3AHIF"].AddStudent(new Student(id: 1002, firstname: "FN2", lastname: "LN2", city: "CTY1"));
            classes["3AHIF"].AddStudent(new Student(id: 1003, firstname: "FN3", lastname: "LN3", city: "CTY2"));
            classes["3BHIF"].AddStudent(new Student(id: 1011, firstname: "FN4", lastname: "LN4", city: "CTY1"));
            classes["3BHIF"].AddStudent(new Student(id: 1012, firstname: "FN5", lastname: "LN5", city: "CTY1"));
            classes["3BHIF"].AddStudent(new Student(id: 1013, firstname: "FN6", lastname: "LN6", city: "CTY1"));

            Student s = classes["3AHIF"].Students[0];
            Console.WriteLine($"s sitzt in der Klasse {s.SchoolClass?.Name} mit dem KV {s.SchoolClass?.ClassTeacher}.");
            Console.WriteLine($"In der 3AHIF sind folgende Städte: {JsonSerializer.Serialize(classes["3AHIF"].Cities)}.");

            Console.WriteLine("3AHIF vor ChangeKlasse:");
            Console.WriteLine(JsonSerializer.Serialize(classes["3AHIF"].Students));
            s.ChangeClass(classes["3BHIF"]);
            Console.WriteLine("3AHIF nach ChangeKlasse:");
            Console.WriteLine(JsonSerializer.Serialize(classes["3AHIF"].Students));
            Console.WriteLine("3BHIF nach ChangeKlasse:");
            Console.WriteLine(JsonSerializer.Serialize(classes["3BHIF"].Students));
            Console.WriteLine($"s sitzt in der Klasse {s.SchoolClass?.Name} mit dem KV {s.SchoolClass?.ClassTeacher}.");
        }
    }
}
```

### Korrekte Ausgabe:
```
s sitzt in der Klasse 3AHIF mit dem KV KV1.
In der 3AHIF sind folgende Städte: ["CTY1","CTY2"].
3AHIF vor ChangeKlasse:
[{"City":"CTY1","Id":1001,"Lastname":"LN1","Firstname":"FN1"},{"City":"CTY1","Id":1002,"Lastname":"LN2","Firstname":"FN2"},{"City":"CTY2","Id":1003,"Lastname":"LN3","Firstname":"FN3"}]
3AHIF nach ChangeKlasse:
[{"City":"CTY1","Id":1002,"Lastname":"LN2","Firstname":"FN2"},{"City":"CTY2","Id":1003,"Lastname":"LN3","Firstname":"FN3"}]
3BHIF nach ChangeKlasse:
[{"City":"CTY1","Id":1011,"Lastname":"LN4","Firstname":"FN4"},{"City":"CTY1","Id":1012,"Lastname":"LN5","Firstname":"FN5"},{"City":"CTY1","Id":1013,"Lastname":"LN6","Firstname":"FN6"},{"City":"CTY1","Id":1001,"Lastname":"LN1","Firstname":"FN1"}]
s sitzt in der Klasse 3BHIF mit dem KV KV2.
```