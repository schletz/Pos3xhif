# Migrations

## Übersicht: Befehle

| CLI                                    | Visual Studio PM Console    | Beschreibung
| -------------------------------------- | --------------------------- | ------------ |
| dotnet ef migrations add InitialCreate | Add-Migration InitialCreate | Erstes Erstellen der Migration.
| dotnet ef migrations add *(Name)*      | Add-Migration *(Name)*      | Laufendes Erstellen von neuen Migrationen.
| dotnet ef database update              | Update-Database             | Schreiben der Änderung in die DB.
| dotnet ef migrations script            | Script-Migration            | Gibt die SQL Anweisungen aus, die durchgeführt werden würden.

### Einfaches Erzeugen der Datenbank von den Modelklassen aus

Möchte man ganz ohne Migrations einfach nur die Datenbank von den bestehenden Modelklassen aus
erzeugen, so geht dies sehr einfach. Dieser Code kann z. B. in der *ConfigureServices()* Methode in
ASP.NET oder zu Beginn in der *Main()* Methode platziert werden:

```c#
using (var context = new MyDbContext())
{
    context.Database.EnsureCreated();
}
```

Dieser Ansatz funktioniert natürlich nur, wenn keine Datenbank vorhanden ist. Ist eine solche
schon vorhanden, müssen wir uns mit dem Thema Migrations beschäftigen.

Im nachfolgenden Text werden die CLI Befehle verwendet. Statt dessen kann natürlich der
entsprechende Befehl in der Packet Manager Console von Visual Studio abgesetzt werden.

## Was ist eine Migration?

Gerade bei einem Projekt in der Entwicklung ändert sich häufig nach dem
Erstellen der Modelklassen das Datenbankschema. Diese Änderungen können natürlich in einem SQL Editor
mit *ALTER TABLE* durchgeführt werden, die Modelklassen müssen danach jedoch aktualisiert werden.

Mit einem erneuten Lauf des *dotnet ef dbcontext scaffold* Skriptes wird dies durchgeführt,
Anpassungen der Modelklassen gehen jedoch wieder verloren. Deswegen gibt es bei den großen
OR Mappern wie EF Core oder JPA Data das Konzept der *Migrations*.

Migrations erlauben es uns, die Modelklassen im Programmcode einfach zu ändern. Diese Änderung wird
dann mit entsprechenden *ALTER TABLE* Anweisungen in die Datenbank geschrieben.

## Initialisieren der Migration

Es wird davon ausgegangen, dass das Projekt eine gültige Contextklasse hat. Das heißt es existiert
eine Klasse, die von *DbContext* ableitet und korrekt mit einem Connectionstring verbunden wurde.

Zuerst wird mit dem Befehl *dotnet ef migrations add InitialCreate* im Projektverzeichnis in der
Konsole die Migration initialisiert. Hier passiert folgendes:

Im Projekt wird ein Ordner *Migrations* erstellt. In der Datei *20191231235959_InitialCreate.cs*
(je nach Zeitstempel natürlich anders) entsteht eine C# Datei mit 2 Methoden: *Up()* und *Down()*.

```c#
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Period",
        columns: table => new
        {
            P_Nr = table.Column<long>(nullable: false),
            P_From = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
            P_To = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Period", x => x.P_Nr);
        });
        // ...
}
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(
        name: "Lesson");
    // ...
}
```

*InitialCreate* besagt also, dass - ausgehend von einer leeren Datenbank - alle Tabellen neu
erstellt werden.

Das Schreiben erfolgt erst durch den Befehl *dotnet ef database update*. Da alle Tabellen erzeugt
werden, muss der Connection String auf eine leere Datenbank verweisen. Zusätzlich wird die
Tabelle *__EFMigrationsHistory* erzeugt:

| MigrationId                  | ProductVersion |
| ---------------------------- | -------------- |
| 20191231235959_InitialCreate | 3.1.0          |

Diese Tabelle speichert den Zustand der Datenbank ab. Wird *dotnet ef database update* erneut
aufgerufen, so passiert nichts. EF Core list nämlich diese Tabelle und stellt fest, dass sich die
Datenbank im Zustand *20191226061457_InitialCreate* befindet und daher nichts zu tun ist.

### Migration bei Database First (vorhandene DB)

Wurden die Contextklasse und die Modelklassen mit dem *scaffold* Skript erstellt, so führt
der Befehl *dotnet ef database update* zu einem Fehler. EF Core versucht nämlich, die vorhandenen
Tabellen nochmals zu erstellen.

Um das zu verhindern, wird der Inhalt der Methoden *Up()* und *Down()* (nicht die Methoden selbst) in
*Model/00000000000000_InitialCreate.cs* auskommentiert. Nun kann das *update* Skript gestartet werden.
Es wird nur die Tabelle *__EFMigrationsHistory* erzeugt, die besagt, dass sich die Datenbank auf dem
Stand nach *InitialCreate* befindet.

Nach dem Lauf des *update* Skriptes können die Kommentare in den Methoden *Up()* und *Down()* wieder
entfernt werden.

## Durchführen weiterer Migrationen

Soll nun während der Entwicklung die Datenbank geändert werden, so werden die Modelklassen in C#
angepasst. Achte dabei auf die korrekten Annotations (*Key*, *Required*, ...) vor allem bei Fremdschlüsseln.
In der Contextklasse müssen neue Tabellen als *DbSet* registriert sowie die Methode *OnConfiguring()*
angepasst werden. Eine Beschreibung der Annotations ist auf
[Microsoft Docs - Creating and configuring a model](https://docs.microsoft.com/en-us/ef/core/modeling/)
nachzulesen.

Nachdem alles angepasst wurde, wird mit *dotnet ef migrations add (Name)* eine neue Migration
erzeugt. Sie generiert die Anweisungen, die - ausgehend vom letzten Migrationsstand - die Datenbank
aktualisiert. Verwende für *(Name)* einen sprechenden Namen (z. B. *AddUserTable*, ...).

Nun wird mit *dotnet ef database update* die Änderung geschrieben.

## Migration und vorhandene Daten

Migrations behalten - wenn möglich - die vorhandenen Daten in der Datenbank. Allerdings ist eine
Migration vor allem in Produktivsystemen immer ein heikler Schritt. Deswegen sollte vorher eine
Sicherung der Datenbank durchgeführt werden.

## Generieren von Musterdaten

Wenn eine neue Datenbank mit den Modelklassen erstellt wird, ist diese natürlich einmal leer. Oft
möchte man aber schon Musterdaten zum Testen der Applikation haben. Hier können in der Methode
*OnModelCreating()* der Contextklasse mittels *HasData()* Datensätze geschrieben werden, die mit
der Migration schon angelegt werden sollen. Details dazu gibt es auf
[Microsoft Docs - Data Seeding](https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding)
nachzulesen.

## Übertragung in eine andere Datenbank

Möchte man sein generiertes Modell z. B. auf Azure oder in ein anderes Datenbanksystem übertragen,
muss man zuerst die Methode *OnConfiguring()* in der Contextklasse anpassen. Im Fall von SQLite
sieht sie so aus:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder
            .UseSqlite("DataSource=MyDatabase.db");
    }
}
```

Um z. B. in eine SQL Server Datenbank zu schreiben, muss zuerst das notwendigen NuGet Paket
installiert werden. In Fall von SQL Server geht dies mit *Install-Package Microsoft.EntityFrameworkCore.SqlServer*
in der Packet Manager Console. Danach steht die Methode *UseSqlServer()* zur Verfügung, die statt
*UseSqlite()* verwendet werden kann. Natürlich muss der Verbindungsstring auch angepasst werden.
Diese Strings gibt es auf der [Startseite des Themas EF Core](..) nachzulesen.

Mit *dotnet ef database update* wird nun das Modell übertragen.

## Übung

### Erstellen der SQLite Datenbank mittels Database First

Erstelle wie im Kapitel Database First mit DBeaver oder einem anderen SQL Tool eine leere SQLite
Datenbank mit dem Namen "Project.db" und 2 Tabellen:

```sql
DROP TABLE IF EXISTS Task;
DROP TABLE IF EXISTS Project;

CREATE TABLE Project (
    ID            INTEGER PRIMARY KEY AUTOINCREMENT,
    Name          VARCHAR(200) NOT NULL,
    EstimatedCost NUMERIC(9,4)
);

CREATE TABLE Task (
    Project       INTEGER REFERENCES Project(ID),
    Name          VARCHAR(200) NOT NULL,
    DateStarted   DATETIME,
    DateFinished  DATETIME,
    PRIMARY KEY (Project, Name)
);
```

Erstelle danach ein neues Projekt (console Applikation) und erstellt mit dem *scaffold* Skript
die Modelklassen und die Contextklasse. Achte darauf, dass die Datentypen korrekt abgebildet werden.
Im Kapitel [Database First](../02_DatabaseFirst#anpassungen-für-sqlite-autoincrement-und-datentypen)
sind Hinweise für SQLite in Verbindung mit dem *scaffold* Skript.

### Hinzufügen von Spalten und Musterdaten

Füge nun in der Modelklasse von Task ein Textfeld Comment mit dem Typ VARCHAR(200) hinzu. Erstelle
danach eine Migration mit dem Namen *AddTaskComment* und schreibe die Änderung in deine originale 
SQLite Datenbank mit Hilfe des *update* Skriptes zurück.

Danach befülle die Datenbank mit 2 Projekten und 4 Tasks (2 pro Projekt). Verwende dafür die
*HasData()* Funktion wie unter *Generieren von Musterdaten* beschrieben. Natürlich muss danach wieder
eine Migration erstellt werden.

### Migration in eine SQL Server Datenbank

Erstelle in Azure eine SQL Server Datenbank. Die erforderlichen Schritte sind im Repository
*Pos4xhif* unter
[Einrichtung einer SQL Server Datenbank](https://github.com/schletz/Pos4xhif/blob/master/Azure/01_Database.md)
erklärt.

Danach passe deine Contextklasse so an, dass sie sich mit der Azure Datenbank verbindet. Nun führe
die Migration durch, sodass in deiner Azure SQL Server Datenbank die beiden Tabellen korrekt erzeugt
werden.

Als abschließenden Test füge in deiner *Program.cs* einen neuen Eintrag in der Azure Datenbank mit den
folgenden Anweisungen ein:

```c#
using (ProjectContext context = new ProjectContext())
{
    context.Project.Add(new Project
    {
        Name = "Test in Azure",
        EstimatedCost = 100000M,     // Decimal literal
        Task = new List<Task>
        {
            new Task { Name = "First Task", DateStarted = DateTime.UtcNow, Comment = "It works!" }
        }
    });
    context.SaveChanges();
}
```

## Weitere Informationen

- [Microsoft Docs - Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
