# Klassenmodelle persistieren mit EF Core

> Im Ordner [CodeFirstDemo](../CodeFirstDemo) ist ein lauffähiges Beispiel dieser Erklärungen.

## Anlegen des Musterprojektes (.NET 8)

Um ein Klassenmodell umsetzen zu können, legen wir eine kleine Solution an. Wir nutzen nun
*2 Projekte* und keine Konsolenapplikation:

- **CodeFirstDemo.Application** beinhaltet die Modelklassen und die Logik für den Datenbankzugriff.
  Mit *dotnet add package* können wir die NuGet Pakete für EF Core und Bogus (Musterdaten Generator)
  hinzufügen.
- **CodeFirstDemo.Test** beinhaltet sogenannte *Unit Tests*. Diese rufen unseren Programmcode im
  Application Projekt auf. Dafür wird eine Referenz auf das Application Projekt hinzugefügt.
- **CodeFirstDemo.sln** ist die Solution, die die 2 Projekte beinhaltet und wird in Visual
  Studio (oder Rider) gestartet.

```text
rd /S /Q CodeFirstDemo
md CodeFirstDemo
cd CodeFirstDemo
md CodeFirstDemo.Application
cd CodeFirstDemo.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 8.*
dotnet add package Bogus --version 35.6.1
cd ..
md CodeFirstDemo.Test
cd CodeFirstDemo.Test
dotnet new xunit
dotnet add reference ..\CodeFirstDemo.Application
cd ..
dotnet new sln
dotnet sln add CodeFirstDemo.Application
dotnet sln add CodeFirstDemo.Test
start CodeFirstDemo.sln

```

Nun stellen wir durch Doppelklick auf die Projektdatei (*CodeFirstDemo.Application*) die
Option *TreatWarningsAsErrors* ein.

```xml
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

**Im Ordner [CodeFirstDemo](../CodeFirstDemo) des Kapitels *03 EF Core* befindet sich eine fertige
Applikation, die eine fertige Implementierung beinhaltet.**

### Nullable reference types und EF Core

In einer Datenbank können manche Felder NULL Werte enthalten (nullable), andere Felder werden
mit NOT NULL definiert. Wir können mit den
Datentypen steuern, ob EF Core ein Feld mit NULL oder NOT NULL anlegt. Deaktivieren wir die
nullable Features von C# 8, dreht sich die Bedeutung um und aus ihrem Modell wird ein anderes
Datenbankschema erzeugt! Wir müssen sich also schon am Beginn Ihres Projektes entscheiden, ob
wir dieses Feature aktivieren oder nicht.

Daher ist das nachträgliche Aktivieren des nullable Features bei EF Core in bestehenden
Projekten sehr gefährlich!


## Code first: Vom Klassenmodell zur Datenbank

Seit Beginn der Programmierausbildung werden Klassen und die Beziehungen zwischen diesen
Klassen als Assoziationen umgesetzt. Betrachten wir das folgende Modell. Es setzt ein kleines
Bestellsystem wie z. B. Geizhals um. Produkte werden in verschiedenen Stores zu unterschiedlichen
Preisen angeboten (offer).

![](https://www.plantuml.com/plantuml/svg/ZP1FIuCn3CRl_HGXHsL3h-z11xg382ge7w2qsLRedz7cFGpPTxSlQNSe0-sboQ_9yoJT5DQPv-3LmLa22aS-GI0-95kKyReK-EIK2NuKr7FxOc425rcoDnseveMpYtjrLgtZVRUZVBtZ8oSx6wweHOB5OnAlNkdCWYmP5xGN-AeBVn-jvkNbkLvVBj1YB9PujrxV8AnMWFS8NHGV9CHhLNqEgUi9jbKwUAfM-vHF__c62zh62tnKvq6nAsrr_4Dg7662WLJjHeRLnaxPSQ2eDXJj7Fml) 
<sup>
https://www.plantuml.com/plantuml/uml/ZP1FIuCn3CRl_HGXHsL3h-z11xg382ge7w2qsLRedz7cFGpPTxSlQNSe0-sboQ_9yoJT5DQPv-3LmLa22aS-GI0-95kKyReK-EIK2NuKr7FxOc425rcoDnseveMpYtjrLgtZVRUZVBtZ8oSx6wweHOB5OnAlNkdCWYmP5xGN-AeBVn-jvkNbkLvVBj1YB9PujrxV8AnMWFS8NHGV9CHhLNqEgUi9jbKwUAfM-vHF__c62zh62tnKvq6nAsrr_4Dg7662WLJjHeRLnaxPSQ2eDXJj7Fml
</sup>

Beachte die Kardinalitäten. Ein Store hat mehrere (*0..n*) Offers, während ein Offer genau einem (*1*) Store zuzuordnen ist.

Nun wollen wir dieses Klassenmodell speichern, also *persistieren*. Dafür stehen uns mehrere
Techniken zur Verfügung:

- Dateien (Serialisierung)
- NoSQL Datenbanken
- Relationale Datenbanken

EF Core unterstützt das Erstellen einer Datenbank, damit diese das Klassenmodell speichern kann.
Dafür schreiben wir wie gewohnt diese Klassen in C#. Um Ordnung zu halten, wird im Projekt *Application*
ein Ordner *Model* erstellt.

### Conventions und Annotations in EF Core

EF Core kann in den meisten Fällen ohne besondere Anweisungen eine Datenbank erzeugen. Das liegt
an den sogenannten *Conventions*, die wir in den nachfolgenden Klassen einhalten:

- Properties mit dem Namen *Id* werden automatisch als Primärschlüssel definiert.
- Id Properties mit dem Datentyp *int* werden automatisch zu AutoIncrement Feldern.
- Properties vom Typ *List&lt;Typname&gt;* finden "automatisch" ihren Weg in die richtige Tabelle. So
  verweist das Property *Pupils* in den vorigen LINQ Beispielen automatisch auf alle Schüler dieser Klasse.  
- Properties vom Typ *Typname* verweisen automatisch auf die Tabelle dieses Typs. So referenziert
  das Property vom Typ *Store* automatisch auf die Tabelle *Store*.
- Fremdschlüsselfelder mit dem Namen *NavigationProperty + PK Name* (wie *StoreId*) werden automatisch zum
  Fremdschlüsselfeld (in diesem Beispiel für die Tabelle *Store*).
- Über NULL oder NOT NULL entscheidet der Datentyp (bei den nullable reference types werden
  Typen mit ? am Ende zu Feldern mit der NULLABLE Eigenschaft).
- Read-only Properties werden nicht in der Datenbank abgebildet.


### Erstellen von Modelklassen: Konstruktoren verwenden

Erstellen wir die Klasse *Store*, bekommen wir durch die Option *Nullable* und *TreatWarningsAsErrors*
eine Fehlermeldung beim Property *Name*:
```csharp
public class Store
{
    public int Id { get; set; }
    public string Name { get; set; }   // Error: not initialized
}
```

Wir müssen daher *Konstruktoren* verwenden, um alle Felder zu initialisieren. Allerdings sollte
nicht blind jedes Feld im Konstruktor initialisiert werden. *Id* ist ein AutoIncrement Wert
(Details unter Conventions), daher kann dieser Wert gar nicht im Konstruktor zugewiesen werden.

Die verbesserte Version sieht nun so aus:
```csharp
public class Store
{
    public Store(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }      // ID by convention, AutoIncrement by convention
    public string Name { get; set; }         // NOT NULL because nullable reference types are enabled
}
```

*Id* hat nun einen private setter. Da der Primärschlüssel in EF Core nicht veränderbar ist,
definieren wir alle Schlüsselfelder mit dieser Sichtbarkeit. Es macht wenig Sinn von einem
bestehenden Eintrag diesen Wert zu ändern.
Alle anderen Properties haben einen public setter, da wir die Spalten in der Datenbank ja auch
ändern wollen (UPDATE Befehl). Wir bestimmen also, welche Properties wir im Programmverlauf
aktualisieren dürfen. Warum wir nicht nur *get* verwenden ist in den Conventions erklärt.

### Optionale Werte

Die Klasse ProductCategory hat ein Property *NameEn* (englischer Name). Diese Spalte ist optional,
darf also den Wert NULL enthalten. Bei aktivierten nullable reference Types legen wir diese Spalte
daher mit dem Datentyp *string?* an. Sollen optionale int, DateTime, ... Werte gespeichert werden,
wird der entsprechende nullable Datentyp (int?, DateTime?, ...) verwendet. 

Es ist in C# möglich, ein *Argument mit default value* zu dafür zu definieren. Diese Form hat den Vorteil, dass
das zusätzliche Property *nameEn* in IntelliSense erscheint. Durch den default value muss kein
Wert angegeben werden, d. h. der Konstruktor von ProductCategory kann auch mit einem Argument
aufgerufen werden.

```csharp
  public class ProductCategory
  {
      public ProductCategory(string name, string? nameEn = null)
      {
          Name = name;
          NameEn = nameEn;
      }

      public int Id { get; private set; }      // ID by convention, AutoIncrement by convention
      public string Name { get; set; }        
      public string? NameEn { get; set; }      // nullable
  }
```


### Navigationen

Das Produkt (Klasse *Product*) verwendet die Klasse *ProductCategory*. Dies ist im Klassenmodell
durch die Verwendung des Typs *ProductCategory* leicht ersichtlich. In einer relationalen
Datenbank kennen wir das Konzept des Fremdschlüssels. Möchten wir Produkte und Kategorien speichern,
würde die Produkttabelle einfach eine Spalte für den Fremdschlüssel (der Kategorie ID) beinhalten.

EF Core macht dies automatisch. Legen wir ein Property *ProductCategory* an, wird automatisch ein Feld *ProductCategoryId* erstellt.

Da der Schlüssel *Ean* heißt, greift die Convention (Id als Schlüsselname) nicht mehr. Wir müssen
daher mit Annotations aus dem Namespace *System.ComponentModel.DataAnnotations* das jeweils
nachfolgende Property genauer definieren. Damit der int Wert für die EAN Nummer nicht als
auto increment Wert angelegt wird, setzen wir diese Information mittels der Annotation
*DatabaseGenerated(DatabaseGeneratedOption.None)*

```csharp
public class Product
{
    public Product(int ean, string name, ProductCategory productCategory)
    {
        Ean = ean;
        Name = name;
        ProductCategory = productCategory;
    }

    // Ean is the PK and not an auto increment column. Annotations are used
    // for the next property (ean)
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Ean { get; private set; }
    public string Name { get; set; }
    public ProductCategory ProductCategory { get; set; }  // Navigation property
}
```

Der Konstruktor von Product verlangt diesmal den Primärschlüssel (die EAN), da dieser Wert
von externen Quellen kommt und nicht in der Datenbank generiert wird. Für die Navigation
verlagen wir nur die Instanz von *ProductCategory*.

Nun fehlt noch die Klasse *Offer*, die mit bestehendem Wissen angelegt werden kann:

```csharp
public class Offer
{
    public Offer(Product product, Store store, decimal price, DateTime lastUpdate)
    {
        Product = product;
        Store = store;
        Price = price;
        LastUpdate = lastUpdate;
    }

    public int Id { get; private set; }
    public Product Product { get; set; }
    public Store Store { get; set; }
    public decimal Price { get; set; }
    public DateTime LastUpdate { get; set; }
}
```

### Anlegen von protected Konstruktoren

Zum Abschluss müssen wir noch eine Besonderheit von EF Core berücksichtigen. EF Core versucht
beim Lesen eines Datensatzes eine Instanz der entsprechenden Klasse zu erzeugen. Dafür braucht es
aber einen Default Konstruktor. Diese Default Konstruktoren wollen wir allerdings vermeiden. Ein
guter Weg ist das Anlegen dieses Konstruktors als private oder protected Konstruktor. Damit kann
niemand mit *new Store()* ein uninitialisiertes Store Objekt anlegen. Wir verwenden protected,
da wir im nächsten Kapitel auch Vererbung verwenden wollen.

Legen wir nun einen Default Konstruktor an, ergeben sich aufgrund des aktivierten nullable
Features Warnungen bzw. Fehlermeldungen. Wir können aber in diesem Fall mit einer etwas seltsamen
Anweisung Warnungen unterdrücken. In Visual Studio können die *#pragma* Anweisungen mit
*STRG + .* und *Suppress or Configure issues* - *Suppress CS8618 in Source Code* deaktiviert werden.

> **Hinweis:** Das Unterdrucken von Warnungen ist nur gerechtfertigt, wenn wir mit Sicherheit
> ausschließen können, dass dadurch ein Laufzeitfehler entsteht. EF Core garantiert das Initialisieren
> der Felder, daher kann diese Technik hier verwendet werden.

```csharp
public class Store
{
    public Store(string name)
    {
        Name = name;
    }
    #pragma warning disable CS8618
    protected Store() { }
    #pragma warning restore CS8618
    public int Id { get; private set; }
    public string Name { get; set; }
}
```

## Anlegen der Datenbank: Der Datenbankkontext

Damit eine Datenbank erzeugt werden kann, brauchen wir einen *Datenbankkontext*. Er ist die
Verbindung zur darunterliegenden Datenbank. Dafür legen wir einen Ordner *Infrastructure* an und
erstellen eine Klasse *StoreContext*. Die Klasse kann beliebig benannt werden, die Endung *Context*
hat sich aber bei .NET Entwicklern durchgesetzt.

```csharp
public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions opt) : base(opt) { }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
}
```

Wir erkennen 2 Dinge:

- Es gibt einen Konstruktor, der eine Konfiguration verlangt. Diese Konfiguration gibt an, welche
  Datenbank verwendet werden soll.
- Jede Tabelle wird als *DbSet\<T\>* definiert. Sie geben einfach mit der *Set()* Methode die
  entsprechende Tabelle zurück.

## Schreiben eines Tests zum Anlegen der Datenbank

Wir können das Programm zwar kompilieren, es wird aber nie ausgeführt. Das Projekt *Application*
ist eine Klassenbibliothek und hat daher keine *Main()* Methode. Um den Code aufzurufen, verwenden
wir ein in der Softwareentwicklung sehr bekanntes Werkzeug: Der Unittest.

Unittests sind dafür da, Code aufzurufen und das Ergebnis mit einem erwarteten Wert zu vergleichen.

Zu Beginn haben wir bereits ein Projekt *CodeFirstDemo.Test* angelegt. Nun legen wir eine Klasse
*StoreContextTests* darin an. Die Namensgebung sollte immer *zu testende Klasse* + *Tests* sein.

Die Logik zur Erstellung der Datenbankverbindung geben wir in eine gemeinsame Basisklasse *DatabaseTest*.
Sie erstellt im Konstruktor eine SQLite Datenbank. Damit nach dem Unittest alle Ressourcen
geschlossen werden, implementiert diese Klasse das Interface *IDisposeable*.

**DatabaseTest.cs**
```csharp
public class DatabaseTest : IDisposable
{
    protected readonly StoreContext _db;

    public DatabaseTest()
    {
        var opt = new DbContextOptionsBuilder()
            .UseSqlite("Data Source=Stores.db")
            .Options;

        _db = new StoreContext(opt);
    }
    public void Dispose()
    {
        _db.Dispose();
    }
}
```

Nun implementieren wir die eigentliche Testklasse *StoreContextTests*. Sie erbt von *DatabaseTest*,
daher steht die Membervariable *_db* zur Verfügung. Dieser Test prüft nur, ob die Anweisung ohne
Exception ausgeführt werden kann.

**StoreContextTests.cs**
```csharp
[Collection("Sequential")] // A file database does not support parallel test execution.
public class StoreContextTests : DatabaseTest
{
    [Fact]
    public void CreateDatabaseTest()
    {
        _db.Database.EnsureCreated();
    }
}
```

Bis jetzt haben wir nirgends gesagt, welche Datenbank wir verwenden wollen. Nun müssen wir dies
angeben. Dafür verwenden wir die Klasse *DbContextOptionsBuilder()* aus dem Namespace
*Microsoft.EntityFrameworkCore*. EF Core arbeitet datenbankenunabhängig. Wir verwenden mit
*UseSqlite()* eine SQLite Datenbank. Mit *UseMysql()* z. B. können wir jederzeit auch eine MySQL
Datenbank erzeugen. Der Provider abstrahiert datenbankspezifische Anweisungen wie das Erstellen
von Autoincrement Werten und die Definition der SQL Datentypen.

3 Dinge sind zudem noch wichtig:

- Die Testklasse und die Testmethode muss public sein.
- Über jeder Testmethode muss die Annotation *[Fact]* aus dem Namespace *Xunit* geschrieben werden.
- Tests werden normalerweise parallel ausgeführt. Da wir die Filedatenbank jedoch immer löschen
  und neu erzeugen, ist eine parallele Ausführung nicht möglich. *[Collection("Sequential")]*
  konfiguriert xUnit so, dass die Tests nacheinander ausgeführt werden.

Klicken wir mit der rechten Maustaste in Visual Studio auf die Testmethode *CreateDatabaseTest*
finden wir im Kontextmenü den Punkt *Run Tests*. Nach erfolgreicher Ausführung erscheint ein
grünes Häkchen. Im Text Explorer (Menü *Test* - *Test Explorer*) ist der Test ebenfalls
aufgelistet.

### Nutzen einer in-memory SQLite Datenbank

SQLite Datenbanken können auch vollständig im Speicher gehalten werden. Gerade für Unittests
ergeben sich mehrere Vorteile:

- Das Erstellen und befüllen der Datenbank ist natürlich viel schneller.
- Die Datenbank wird immer "frisch", also leer bei einer neuen Verbindung erstellt. Dadurch
  beeinflussen sich die Tests nicht.

Im Programm [CodeFirstDemo](../CodeFirstDemo/CodeFirstDemo.Test) werden die Unittests mit einer
in-memory durchgeführt. Hier sind auch Infos über den Connectionstring und die Besonderheiten
im Code abgebildet.
Wichtig ist, dass eine offene SqliteConnection bei `UseSqlite` übergeben wird.
Sonst wird EF Core nach `EnsureCreated` die Verbindung schließen und beim nächsten Öffnen ist die Datenbank wieder leer.


```csharp
public class DatabaseTest : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly StoreContext _db;

    public DatabaseTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Enable Data Source=stores.db instead of UseSqlite(_connection)
        // if you want to open the database in DBeaver.
        var opt = new DbContextOptionsBuilder()
            //.UseSqlite("Data Source=stores.db") 
            .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
            .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        _db = new StoreContext(opt);
    }
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
```

## Ansehen der Datenbank in DBeaver

Nachdem der Unittest ausgeführt wurde, findet sich im Ordner *CodeFirstDemo.Test\bin\Debug\net8.0*
die Datei *Stores.db*. Zum Betrachten der Datenbank kann z. B. DBeaver (https://dbeaver.io/)
verwendet werden. Öffnen wir mit DBeaver die Datenbank und klicken doppelt auf *Tables*,
kann das ER Diagramm der erstellen Datenbank angezeigt werden:

![](ermodell20211207.png)

Die Fremdschlüssel wurden also korrekt gesetzt. Sehen wir uns die DDL Statements der Tabelle
*Stores* an, findet sich ein klassisches *CREATE TABLE* Statement, wie es auch von Hand
geschrieben worden wäre:

```sql
CREATE TABLE "ProductCategories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ProductCategories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "NameEn" TEXT NULL
);

CREATE TABLE "Products" (
    "Ean" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "ProductCategoryId" INTEGER NOT NULL,
    CONSTRAINT "FK_Products_ProductCategories_ProductCategoryId" FOREIGN KEY ("ProductCategoryId") REFERENCES "ProductCategories" ("Id") ON DELETE CASCADE
);
```

Falls Fehler in der Datenbank auftreten, können die Modelklassen einfach geändert und mittels
Unittest die Datenbank neu erzeugt werden.

> **Achtung:** Trenne in DBeaver vor dem Starten des Unittests die Verbindung (roter Stecker). Sonst
> kann der Test die Datenbank nicht löschen und bricht mit einem Zugriffsfehler ab.

## Übung

Lege wie unten beschrieben eine Solution mit dem Namen *TeamsManager.sln* und 2
Projekte (*TeamsManager.Application* und *TeamsManager.Test*) an. Vergiss nicht,
die Option *\<TreatWarningsAsErrors\>true\</TreatWarningsAsErrors\>* in der Projektdatei 
zu setzen.

Eine kleine
Datenbank soll erzeugt werden, um Abgaben in MS Teams verwalten zu können. Dabei können
pro Team mehrere Aufgaben (Tasks) definiert werden. Für Aufgaben können Studenten
Arbeiten einrechen (HandIn).

![](https://www.plantuml.com/plantuml/svg/hLBDJiCm3BxtAQoS1dGJNATfsm4GI80Gsmk4rh10aaBZmYJ4krFNwTPEvGINslaI-_jHbu5qIPMpIMPr2B2YAaTFy9K0d5oQCf3N3c4AWKhZdnczqGFWXTF6Tf7nUCnut3SI79xQZIb4nEe307dJKVATH4LhrDa6otzJ5FVzJP4JENTTKfZDAc_UAvpF6-VPfYYiJ0ogSAs47bdOZp7bZbE7L-5SLOylc57FwzgGnJx26gV0fMEQ4UcQXBPlfjHa2d-kYSUvh8r3lhpLs_l2U6BnTLdifREKLdeGCJlrn49IIOZg3xV2J5BJ7GbWZvxDYajrEfaSgveDr9c2Q4JDcquDluaSyamEgatkIllysajspUmKk7H-pXg97OC3MJpWXy7FWui5t_mIaN2E6ZggFsbB045g9uOyceyw-yxYc3YB1pNybCda7NSgMQBFAsT_0000)
<sup>
https://www.plantuml.com/plantuml/uml/hLBDJiCm3BxtAQoS1dGJNATfsm4GI80Gsmk4rh10aaBZmYJ4krFNwTPEvGINslaI-_jHbu5qIPMpIMPr2B2YAaTFy9K0d5oQCf3N3c4AWKhZdnczqGFWXTF6Tf7nUCnut3SI79xQZIb4nEe307dJKVATH4LhrDa6otzJ5FVzJP4JENTTKfZDAc_UAvpF6-VPfYYiJ0ogSAs47bdOZp7bZbE7L-5SLOylc57FwzgGnJx26gV0fMEQ4UcQXBPlfjHa2d-kYSUvh8r3lhpLs_l2U6BnTLdifREKLdeGCJlrn49IIOZg3xV2J5BJ7GbWZvxDYajrEfaSgveDr9c2Q4JDcquDluaSyamEgatkIllysajspUmKk7H-pXg97OC3MJpWXy7FWui5t_mIaN2E6ZggFsbB045g9uOyceyw-yxYc3YB1pNybCda7NSgMQBFAsT_0000
</sup>

Das Klassenmodell zeigt keine EF Core spezifischen Properties wie Fremdschlüsselfelder.
Überlege auch einen passenden primary key (auto increment Wert oder ein vorhandenes
Property).

Definiere die public Konstruktoren so, dass die benötigten Informationen bei der
Initialisierung übergeben werden müssen. Für EF Core sind dann protected Konstruktoren
ohne Parameter anzulegen.

Der Kontext soll den Namen *TeamsContextTests* haben. Weiter unten ist der Mustercode
für die Definition der Klasse. Er beinhaltet auch eine Methode *Seed()*, die die Datenbank
mit Musterdaten füllt.

Erstelle danach eine Testklasse im Unittest Projekt mit dem Namen *TeamsContextTests*. Kopiere
den Code weiter unten in diese Testklasse. Sie beinhaltet 2 Tests:

- *CreateDatabaseSuccessTest()* versucht, eine leere Datenbank anzulegen.
- *SeedDatabaseTest()* versucht, Musterdaten zu generieren und die Datenbank zu befüllen.

Es müssen beide Tests erfolgreich durchlaufen. Führe die Tests zur Sicherheit nacheinander
aus, um Zugriffskonflikte bei der Datenbank zu vermeiden.

Nach dem Test *CreateDatabaseSuccessTest* soll das ER Modell der Datenbank in DBeaver so
aussehen. Die Datenbank wird in *TeamsManager.Test\bin\Debug\net8.0* unter dem Namen
*Teams.db* angelegt.

![](ermodell20211202.png)

> **Hinweis:** Die Klasse *Task* kommt im Namespace System.Threading.Task ebenfalls vor.
> Bei mehrdeutigen Referenzen muss der volle Klassenname (*Model.Task*) angegeben werden.

### Anlegen des Projektes

```text
rd /S /Q TeamsManager
md TeamsManager
cd TeamsManager
md TeamsManager.Application
cd TeamsManager.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 8.*
cd ..
md TeamsManager.Test
cd TeamsManager.Test
dotnet new xunit
dotnet add reference ..\TeamsManager.Application
cd ..
dotnet new sln
dotnet sln add TeamsManager.Application
dotnet sln add TeamsManager.Test
start TeamsManager.sln

```

### Kontextklasse
```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TeamsManager.Application.Model;

namespace TeamsManager.Application.Infrastructure
{
    public class TeamsContext : DbContext
    {
        public TeamsContext(DbContextOptions opt) : base(opt) { }
        /* TODO: Add your DbSets here */
    }

}

```

### Unittest
```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TeamsManager.Application.Infrastructure;
using Xunit;

namespace TeamsManager.Test
{
    [Collection("Sequential")]
    public class TeamsContextTests
    {
        private TeamsContext GetDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var opt = new DbContextOptionsBuilder()
                .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
                .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

            var db = new TeamsContext(opt);
            db.Database.EnsureCreated();
        }
        [Fact]
        public void CreateDatabaseSuccessTest()
        {
            using var db = GetDatabase();
        }

        [Fact]
        public void AddHandinSuccessTest()
        {
            // Todo: Add a handin and check with Assert.Any() if there is a Handin in your database.
        }
    }
}

```

### Ausführen der Tests

Gehe zur Kontrolle in der Konsole in das Verzeichnis des Testprojektes. Mit

```text
dotnet test -l "console;verbosity=normal"
```

können von der Kommandozeile aus alle Tests ausgeführt werden. Die Ergebnisse werden über den
Logger auf die Konsole geschrieben. Dieser Befehl funktioniert in dieser Form ab .NET 6.