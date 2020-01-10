# Database First mit EF Core

## Erzeugen der Modelklassen mit den EF Core Tools

Um eine neue Applikation zu erstellen, die EF Core verwendet, gibt es zwei Wege.

### Möglichkeit 1: Über die dotnet CLI Tools (für VS Code Anwender)

Dieser Weg funktioniert auf allen unterstützten Betriebssystemen (Windows, Linux, macOS). Zuerst
muss in der Konsole (CMD oder Powershell) das CLI Tool für das Entity Framework installiert
werden:

```powershell
dotnet tool update --global dotnet-ef
```

> **Hinweis:** Nach der Installation der ef Tools muss die Konsole neu geöffnet werden, da die PATH
> Variable geändert wurde.

Bei zukünftigen Projekten muss dies nicht mehr durchgeführt werden, bei einem Upgrade der .NET Version
ist das Update allerdings wichtig, um die neuesten Pakete zu laden.

Nun wird im Ordner des Projektes mit *dotnet new* eine neue Konsolenapplikation mit dem Namespace
*DatabaseFirst* im aktuellen Verzeichnis erzeugt. Wird kein Parameter *-n* angegeben, so wird der Verzeichnisname
als Projektname verwendet, was oft der Fall sein wird. Mit den ef Tools steht uns
der Befehl *dotnet ef dbcontext scaffold* zur Verfügung, der die Modelklassen aus einer Datenbank
generiert.

```powershell
dotnet new console -n DatabaseFirst -o .
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet ef dbcontext scaffold "DataSource=Tests.db" Microsoft.EntityFrameworkCore.Sqlite --output-dir Model --use-database-names --force --data-annotations
```

Details zum Tool *dotnet ef* (Parameter, Connectionstrings) können auf [Microsoft Docs](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)
nachgelesen werden.

### Möglichkeit 2: Über die Packet Manager Console in Visual Studio

Natürlich kann auch in Visual Studio eine neue Konsolenanwendung erstellt werden. 
Unter *Tools - NuGet Packet Manager - Packet Manager Console* wird danach die Konsole des
Packet Managers eingeblendet. Hier können die NuGet Commandlets der Powershell verwendet werden.
Leider weichen die Parameter hinsichtlich ihrer Großschreibung ab, achte deshalb auf die Schreibweise.
Mit folgenden Befehlen können dort die Pakete für das Entity Framework geladen sowie das
*Scaffold-DbContext* Skript gestartet werden:

```powershell
dotnet new console -n DatabaseFirst -o .
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Scaffold-DbContext "DataSource=Tests.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Model -UseDatabaseNames -Force -DataAnnotations
```

Nun kann die *csproj* Datei (Jedoch nicht der Ordner!) in Visual Studio geöffnet werden. Beim Speichern
wird zusätzlich eine *sln* Datei von Visual Studio erzeugt und kann im selben Ordner gespeichert werden.

## Die Modelklassen der Tests.db

Wir betrachten nun die Modelklasse der Tabelle *Schoolclass*, die aus 3 Spalten (*C_ID*, *C_Department*
und *C_ClassTeacher*) besteht:

```c#
public partial class Schoolclass
{
    public Schoolclass()
    {
        Lesson = new HashSet<Lesson>();
        Pupil = new HashSet<Pupil>();
        Test = new HashSet<Test>();
    }

    [Key]
    [Column(TypeName = "VARCHAR(8)")]
    public string C_ID { get; set; }
    [Required]
    [Column(TypeName = "VARCHAR(8)")]
    public string C_Department { get; set; }
    [Column(TypeName = "VARCHAR(8)")]
    public string C_ClassTeacher { get; set; }

    [ForeignKey(nameof(C_ClassTeacher))]
    [InverseProperty(nameof(Teacher.Schoolclass))]
    public virtual Teacher C_ClassTeacherNavigation { get; set; }
    [InverseProperty("L_ClassNavigation")]
    public virtual ICollection<Lesson> Lesson { get; set; }
    [InverseProperty("P_ClassNavigation")]
    public virtual ICollection<Pupil> Pupil { get; set; }
    [InverseProperty("TE_ClassNavigation")]
    public virtual ICollection<Test> Test { get; set; }
}
```

Folgende Punkte fallen uns auf:

- Es handelt sich um eine *partial class*. Das bedeutet, dass in einer anderen Datei diese Klasse
  unter dem selben Namen erweitert werden kann. Das ist sinnvoll, da die Datei nach einer Änderung 
  der Datenbank eventuell ersetzt wird.
- Über den Properties befinden sich Annotations. Da Constraints und manche Datentypen (*VARCHAR* mit
  Stringlänge) nicht 1:1 in C# Syntax abgebildet werden kann, werden diese Informationen als
  Annotation hinzugefügt.
- (Nicht sichtbar) Nullable Felder in den Tabellen werden auf einen nullable Type in C# abgebildet.
  Strings können den Wert null haben, bei Wertedatentypen erfolgt die Abbildung auf *int?, long?, ...*
- Neben *C_ClassTeacher* wird *C_ClassTeacherNavigation* erzeugt. Durch die Annotation weiß das
  Entity Framework, dass es mit der Tabelle Teacher über Property *Schoolclass* verknüpft wird.
- Zu *Lesson*, *Pupil* und *Test* besteht eine 1:n Beziehung. Die Navigation ist daher als Collection
  abgebildet. Auch hier wird über eine Annotation die Spalte des Fremdschlüssels bekannt gegeben.

## Der Context von Tests.db

Es wird auch eine Datei mit dem Namen *TestsContext.cs* mit einer Klasse *TestsContext* generiert. 
Sie teilt sich in 3 Teile auf

### 1: DbSet als Abbildung der ganzen Tabelle

Der erste Teil besteht aus Proeprties vom Typ *DbSet*. Sie bilden die ganze Tabelle ab und sind der
Startpunkt für unsere LINQ Abfragen.

### 2: Die Methode *OnConfiguring()*

Beim erstellen der Modelklassen haben wir den Namen der Datenbank bzw. den Verbindungsstring angegeben.
Dieser muss natürlich gespeichert werden, schließlich muss das Entity Framework auf die richtige
Datenbank zugreifen.

Die Warnmeldung besagt, dass Connection Strings mit dem Datenbankpasswort, wie sie für MySQL benötigt
werden, im Quelltext gespeichert werden. Wird das Projekt nun auf Github hochgeladen, gelangt das
Datenbankpasswort unbeabsichtigt an die Öffentlichkeit.

Später werden wir diese Information in einer Datei *appsettings.json* hinterlegen, bei SQLite können wir
diese Warnung vorerst einmal auskommentieren.

```c#
if (!optionsBuilder.IsConfigured)
{
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
    optionsBuilder.UseSqlite("DataSource=Tests.db");
}
```

### 3: Die Methode *OnModelCreating()*

Nicht alle Informationen über die Datenbank können als Annotation abgebildet werden. Diese Methode
speichert noch erweiterte Eigenschaften der einzelnen Datenbankfelder (Autowerte, Navigations, ...).
Sie muss in der Regel nicht editiert werden, bei SQLite gibt es jedoch Handlungsbedarf.

#### Anpassungen für SQLite: Autoincrement und Datentypen

Wird eine Spalte als *AUTOINCREMENT* Wert deklariert, wird dies nicht korrekt erkannt. Um das zu
beheben wird beim entsprechenden Entity statt der Methode *ValueGeneratedNever()* die Methode
*ValueGeneratedOnAdd()* verwendet. Achte darauf, dass die richtige Tabelle und nur das Property
für den Primärschlüssel editiert wird. Natürlich dürfen nur Autoincrement Werte so nachbearbeitet
werden.

```c#
modelBuilder.Entity<Pupil>(entity =>                      // Tabelle Pupil
{
    // ...
    entity.Property(e => e.P_ID).ValueGeneratedOnAdd();  // Statt ValueGeneratedNever()
    // ...
});
```

Außerdem werden Datumswerte und *DECIMAL(p, n)* Typen in den einzelnen Modelklassen als Bytearray
generiert. Hier muss *byte[]* durch *DateTime* bzw. einen entsprechenden Typ für die *DECIMAL(p, n)*
Spalte ersetzt werden.

## Übung

Erzeuge in DBeaver oder einem anderen Tool eine SQLite Datenbank mit dem Namen *Project.db*. Sie
soll folgendes Schema haben: 

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

Erstelle danach auf deiner Festplatte einen Ordner *Projects* und kopiere die Datenbank in diesen
Ordner. Danach erzeuge mit den CLI Tools eine neue Konsolenapplikation mit diesem Namen.
Füge danach - ebenfalls mit den CLI Tools - die NuGet Pakete
hinzu und generiere aus der Datenbank *Projects.db* die Modelklassen in einen Unterordner *Model*

Passe danach die Datentypen und Autoincrement Properties deiner Modelklassen und des Context an.
Außerdem wähle bei der Datenbankdatei *Projects.db* im Solution Explorer in Visual Studio
bei *Copy to Output Directory* die Option *Copy always* . Sonst ist der Pfad zur Datenbank ungültig,
da beim Kompilieren die Applikation in einen Unterordner erzeugt wird. Außerdem wird die Datenbank
immer neu kopiert, was die Änderungen zurücksetzt.

Wer in VS Code arbeitet, kann in der Datei *Projects.csproj* das Kopieren der Datenbank in das
Ausgabeverzeichnis direkt eintragen:

```xml
<ItemGroup>
<None Update="Projects.db">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</None>
</ItemGroup>
```

Kopiere danach den Inhalt der Datei [Program.cs](Program.cs) und lasse das Programm laufen. Es
sollte folgende Ausgabe erscheinen:

```text
[{"ProjectId":1,"ProjectName":"First Project","TaskCount":3,"FirstTask":"2019-01-01T00:00:00"},
 {"ProjectId":2,"ProjectName":"Second Project","TaskCount":2,"FirstTask":"2019-03-01T00:00:00"}]
```
