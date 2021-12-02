# Erweiterte Konfigurationsmöglichkeiten mit EF Core

## Festlegen der Tabellennamen

Die Tabellennamen werden durch den Namen des Properties in der Kontextklasse bestimmt.
So erzeugt der folgende DbContext die Tabellen Stores, Offers, Products und ProductCategories.

```c#
public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions opt) : base(opt) { }
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
}
```

Oft sind aber Tabellennamen in der Einzahl, also Store, Offer, ... erwünscht. Mit der Annotation *Table*
über der *Modelklasse*  (nicht im Kontext) aus dem Namespace *System.ComponentModel.DataAnnotations.Schema*
kann das Verhalten abgeändert werden.
```c#
[Table("Offer")]
public class Offer
{
    /* ... */
}
```

### Annotations vs. Konfiguration im DbContext

Der Einsatz von Annotations (also EF Core spezifischen Anweisungen) in den Modelkalssen wird oft
diskutiert. Es gibt für alle Konfigurationseinstellungen immer die Möglichkeit, diese auch im
Kontext zu definieren. Dafür wird mit override die Methode *OnModelCreating()* überschrieben.
Der ModelBuilder stellt mit *Entity\<T\>()* eine Methode bereit, die die Konfugiration aller
Properties erlaubt. So kann z. B. der Tabellenname mit *ToTable()* definiert werden:

```c#
public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions opt) : base(opt) { }
    /* .. */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().ToTable("Offer");
    }
}
```

Manche Konfigurationsanweisungen müssen in *OnModelCreating()* durchgeführt werden, da es keine
Annotations dafür gibt. Es steht allerdings für jede Annotation eine entsprechende Methode
des ModelBuilders in *OnModelCreating()* bereit.

Welche Technik man wählt ist wohl Geschmackssache. Der Vorteil der Annotations, nämlich die
Konfiguration in der selben Datei wie die Klasse, ist allerdings ein unschlagbarer Vorteil.

## Definieren der Datentypen

Der Datenbankprovider versucht, die C# Datentypen bestmöglich in SQL Datentypen der entsprechenden
Datenbank umzuwandeln. Für SQL Server ist z. B. auf https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
eine genaue Auflistung zu sehen.

Manchmal gibt es mehrere Möglichkeiten, einen .NET Datentyp in der Datenbank umzusetzen. So können
Stringspalten mit einer Länge als *VARCHAR(n)* definiert werden. Außerdem ist bei numerischen
Datentypen die Angabe der precision möglich wie z. B. *DECIMAL(9,4)*.

Für *decimal* werden standardmäßig 2 Nachkommastellen definiert.

### Problematisch: Die Column Annotation

Oftmals wird die Annotation *Column* zur Definition des SQL Datentyps verwendet:

```c#
public class Store
{
    /* ... */
    [Column(TypeName = "VARCHAR(255")]
    public string Name { get; set; }
}

public class Offer
{
     /* ... */
    [Column(TypeName = "DECIMAL(9,4")]
    public decimal Price { get; set; }
}
```

Allerdings ist der Datentyp abhängig von der Datenbank. So wird z. B. in Oracle *VARCHAR2* zur
Speicherung von strings verwendet. Bei numerischen Werten gibt es mit *DECIMAL* und *NUMBER* (Oracle)
ebenfalls mehrere Möglichkeiten, die abhängig von der Datenbank unterstützt werden oder nicht.

### Besser: Steuerung über entsprechende Annotations

Um die maximale Länge eines String Properties zu steuern, kann die Annotation *MaxLength* verwendet
werden. Der EF Core Provider kann dadurch weiterhin den geeigneten Typ auswählen
(*VARCHAR*, *VARCHAR2*, ...) und eine Längendefinition - wenn unterstützt- angeben.

Für die Angabe der Precision gibt es (in EF Core 6) keine Annotation. Daher muss dieser Wert in der
Methode *OnModelCreating()* der Klasse *StoreContext* definiert werden.

```c#
public class Store
{
    /* ... */
    [MaxLength(255)]
    public string Name { get; set; }
}

public class Offer
{
     /* ... */
    public decimal Price { get; set; }   // No annotation for precision :( --> OnModelCreating
}

public class StoreContext : DbContext
{
    /* ... */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
    }
}
```
## Definieren von Indizes und mehrteiligen Schlüsseln

In einem *CREATE TABLE* Statement können Schlüssel auch aus mehreren Spalten bestehen.
Durch die Methode *HasKey()* kann dies auch im Context definiert werden:

```c#
public class StoreContext : DbContext
{
    /* ... */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().HasKey(o => new { o.StoreId, o.ProductEan });
    }
}
```

**Wir werden allerdings mehrteilige Schlüssel vermeiden!** Navigationen müssen - falls
z. B. eine Bestellung auf ein Offer verweist - auch mehrteilig sein und immer händisch
definiert werden. Daher verwenden wir eine bessere Technik: Wir belassen die auto increment Id und
definieren einen *Unique Index*.

```c#
public class StoreContext : DbContext
{
    /* ... */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
    }
}
```

Durch den Unique Index ist sichergestellt, dass ein Produkt im selben Store nicht zu unterschiedlichen
Preisen angeboten werden kann.

Unter https://docs.microsoft.com/en-us/ef/core/modeling/ gibt es eine sehr detaillierte Beschreibung
der weiteren Einstellungsmöglichkeiten. Meist kommt man aber mit den hier gezeigten
Möglichkeiten aus.

## Übung

Verwende die Übung des letzten Kapitels (*Klassenmodelle persistieren mit EF Core*) und
definiere folgende Einstellungen:

- Die Tabellennamen sollen alle in der Einzahl (Team, HandIn, ...) angelegt werden.
- Der Teamname soll maximal 64 Stellen lang sein, die Klasse maximal 16 Stellen.
- Alle anderen string Properties (Name, Mail) sollen maximal 255 Stellen lang sein.
- Stelle mit einem Unique Index sicher, dass ein Student pro Task nur eine Abgabe einreichen kann.

Hinweis: In SQLite gibt es für Strings nur den Datentyp *TEXT*. Daher ist die Einstellung
der Länge in DBeaver nicht sichtbar.
