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
Erstelle eine Soluition ExCollections mit dem Projekt ExCollection.App. Installiere danach das Paket
*Newtonsoft.Json*, indem du in Visual Studio mittels *Tools* - *NuGet Package Manager* - *Package Manager Console*
die Konsole öffnest. Das Paket kann durch die Eingabe von *Install-Package Newtonsoft.Json* in der Konsole
installiert werden. Bei Problemen (Newtonsoft wird nicht erkannt) kann die Installation auch über die GUI
erfolgen: Rechte Maustaste auf das Projekt im Solution Explorer - *Manage NuGet Packages*. Unter *Browse*
kann das Paket *Newtonsoft.Json* gesucht und installiert werden.

Ersetze danach den Inhalt von Program.cs durch die untenstehende Version. Vervollständige die 2 Klassen 
*Schueler* und *Klasse* so, dass die Ausgaben des Programmes korrekt sind.

```c#
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExCollection.App
{
    class Klasse
    {
        // TODO: Erstelle ein Property Schuelers, welches alle Schüler der Klasse in einer
        //       Liste speichert.
        public string Name { get; set; }
        public string KV { get; set; }
        /// <summary>
        /// Fügt den Schüler in die Liste hinzu und setzt das Property KlasseNavigation
        /// des Schülers korrekt auf die aktuelle Instanz.
        /// </summary>
        /// <param name="s"></param>
        public void AddSchueler(Schueler s)
        {
            // HIER DEN CODE EINFÜGEN
        }
    }
    class Schueler
    {
        // TODO: Erstelle ein Proeprty KlasseNavigation vom Typ Klasse, welches auf
        //       die Klasse des Schülers zeigt.
        // Füge dann über das Proeprty die Zeile
        // [JsonIgnore]
        // ein, damit der JSON Serializer das Objekt ausgeben kann.
        public int Id { get; set; }
        public string Zuname { get; set; }
        public string Vorname { get; set; }
        /// <summary>
        /// Ändert die Klassenzugehörigkeit, indem der Schüler
        /// aus der alten Klasse, die in KlasseNavigation gespeichert ist, entfernt wird.
        /// Danach wird der Schüler in die neue Klasse mit der korrekten Navigation eingefügt.
        /// </summary>
        /// <param name="k"></param>
        public void ChangeKlasse(Klasse k)
        {
            // HIER DEN CODE EINFÜGEN
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Klasse> klassen = new Dictionary<string, Klasse>();
            klassen.Add("3AHIF", new Klasse() { Name = "3AHIF", KV = "KV1" });
            klassen.Add("3BHIF", new Klasse() { Name = "3BHIF", KV = "KV2" });
            klassen.Add("3CHIF", new Klasse() { Name = "3CHIF", KV = "KV3" });
            klassen["3AHIF"].AddSchueler(new Schueler() { Id = 1001, Vorname = "VN1", Zuname = "ZN1" });
            klassen["3AHIF"].AddSchueler(new Schueler() { Id = 1002, Vorname = "VN2", Zuname = "ZN2" });
            klassen["3AHIF"].AddSchueler(new Schueler() { Id = 1003, Vorname = "VN3", Zuname = "ZN3" });
            klassen["3BHIF"].AddSchueler(new Schueler() { Id = 1011, Vorname = "VN4", Zuname = "ZN4" });
            klassen["3BHIF"].AddSchueler(new Schueler() { Id = 1012, Vorname = "VN5", Zuname = "ZN5" });
            klassen["3BHIF"].AddSchueler(new Schueler() { Id = 1013, Vorname = "VN6", Zuname = "ZN6" });

            Schueler s = klassen["3AHIF"].Schuelers[0];
            Console.WriteLine($"s sitzt in der Klasse {s.KlasseNavigation.Name} mit dem KV {s.KlasseNavigation.KV}.");            
            Console.WriteLine("3AHIF vor ChangeKlasse:");
            Console.WriteLine(JsonConvert.SerializeObject(klassen["3AHIF"].Schuelers));
            s.ChangeKlasse(klassen["3BHIF"]);
            Console.WriteLine("3AHIF nach ChangeKlasse:");
            Console.WriteLine(JsonConvert.SerializeObject(klassen["3AHIF"].Schuelers));
            Console.WriteLine("3BHIF nach ChangeKlasse:");
            Console.WriteLine(JsonConvert.SerializeObject(klassen["3BHIF"].Schuelers));
            Console.WriteLine($"s sitzt in der Klasse {s.KlasseNavigation.Name} mit dem KV {s.KlasseNavigation.KV}.");
        }
    }
}

```

### Korrekte Ausgabe:
```
s sitzt in der Klasse 3AHIF mit dem KV KV1.
3AHIF vor ChangeKlasse:
[{"Id":1001,"Zuname":"ZN1","Vorname":"VN1"},{"Id":1002,"Zuname":"ZN2","Vorname":"VN2"},{"Id":1003,"Zuname":"ZN3","Vorname":"VN3"}]
3AHIF nach ChangeKlasse:
[{"Id":1002,"Zuname":"ZN2","Vorname":"VN2"},{"Id":1003,"Zuname":"ZN3","Vorname":"VN3"}]
3BHIF nach ChangeKlasse:
[{"Id":1011,"Zuname":"ZN4","Vorname":"VN4"},{"Id":1012,"Zuname":"ZN5","Vorname":"VN5"},{"Id":1013,"Zuname":"ZN6","Vorname":"VN6"},{"Id":1001,"Zuname":"ZN1","Vorname":"VN1"}]
s sitzt in der Klasse 3BHIF mit dem KV KV2.
```