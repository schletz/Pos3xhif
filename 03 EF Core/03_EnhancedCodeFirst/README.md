# Erweiterte Konfigurationsmöglichkeiten mit EF Core

![](https://www.plantuml.com/plantuml/svg/ZP1FIuCn3CRl_HGXHsL3h-z11xg382ge7w2qsLRedz7cFGpPTxSlQNSe0-sboQ_9yoJT5DQPv-3LmLa22aS-GI0-95kKyReK-EIK2NuKr7FxOc425rcoDnseveMpYtjrLgtZVRUZVBtZ8oSx6wweHOB5OnAlNkdCWYmP5xGN-AeBVn-jvkNbkLvVBj1YB9PujrxV8AnMWFS8NHGV9CHhLNqEgUi9jbKwUAfM-vHF__c62zh62tnKvq6nAsrr_4Dg7662WLJjHeRLnaxPSQ2eDXJj7Fml) 
<sup>
https://www.plantuml.com/plantuml/uml/ZP1FIuCn3CRl_HGXHsL3h-z11xg382ge7w2qsLRedz7cFGpPTxSlQNSe0-sboQ_9yoJT5DQPv-3LmLa22aS-GI0-95kKyReK-EIK2NuKr7FxOc425rcoDnseveMpYtjrLgtZVRUZVBtZ8oSx6wweHOB5OnAlNkdCWYmP5xGN-AeBVn-jvkNbkLvVBj1YB9PujrxV8AnMWFS8NHGV9CHhLNqEgUi9jbKwUAfM-vHF__c62zh62tnKvq6nAsrr_4Dg7662WLJjHeRLnaxPSQ2eDXJj7Fml
</sup>

## Collection Navigations

Im Klassenmodell hat der Pfeil zwischen *Product* und *Offer* zwei Pfeilspitzen. An einem Ende
kann von der Klasse *Offer* über das Property *Product* zum Produkt navigiert werden. In der
Klasse *Product* ist auch eine besondere Navigation vom Typ *ICollection\<Offer\>* vorhanden.
EF Core unterstützt auch diese Art der Navigation. In *Offers* werden automatisch alle
Angebote zum aktuellen Produkt abgerufen. Das macht LINQ Abfragen besonders einfach.

Hier ein Beispiel, wie in *ProductCategory* die Liste der Produkte leichter abgerufen werden kann:

```csharp
public class Product
{
    #pragma warning disable CS8618
    protected Product() { }
    #pragma warning restore CS8618
    public Product(int ean, string name, ProductCategory productCategory)
    {
        Ean = ean;
        Name = name;
        ProductCategory = productCategory;
    }

    [Key]                                               // bestimmt den Key
    [DatabaseGenerated(DatabaseGeneratedOption.None)]   // verhindert AUTO_INCREMENT
    public int Ean { get; private set; }                 // PK
    public string Name { get; set; }
    public ProductCategory ProductCategory { get; set; }
}


public class ProductCategory
{
    public ProductCategory(string name, string? nameEn = null)
    {
        Name = name;
        NameEn = nameEn;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string? NameEn { get; set; }
    // Liste aller Produkte, die diese Produktkategorie verwenden.
    public List<Product> Products = new();
}
```

## Festlegen der Tabellennamen

Die Tabellennamen werden durch den Namen des Properties in der Kontextklasse bestimmt.
So erzeugt der folgende DbContext die Tabellen Stores, Offers, Products und ProductCategories.

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

Oft sind aber Tabellennamen in der Einzahl, also Store, Offer, ... erwünscht. Mit der Annotation *Table*
über der *Modelklasse*  (nicht im Kontext) aus dem Namespace *System.ComponentModel.DataAnnotations.Schema*
kann das Verhalten abgeändert werden.
```csharp
[Table("Offer")]
public class Offer
{
    /* ... */
}
```

### Annotations vs. Konfiguration im DbContext

Der Einsatz von Annotations (also EF Core spezifischen Anweisungen) in den Modelklassen wird oft
diskutiert. Es gibt für alle Konfigurationseinstellungen immer die Möglichkeit, diese auch im
Kontext zu definieren. Dafür wird mit override die Methode *OnModelCreating()* überschrieben.
Der ModelBuilder stellt mit *Entity\<T\>()* eine Methode bereit, die die Konfiguration aller
Properties erlaubt. So kann z. B. der Tabellenname mit *ToTable()* definiert werden:

```csharp
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
Konfiguration in derselben Datei wie die Klasse, ist allerdings ein unschlagbarer Vorteil.

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

```csharp
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

```csharp
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

```csharp
public class StoreContext : DbContext
{
    /* ... */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().HasKey("StoreId", "ProductEan");
    }
}
```

> [!IMPORTANT]
> In der Klasse *Offer* muss mit einer *ForeignKey* Annotation über den Navigations zu *Store* und *Product* der Name des Fremdschlüsselfeldes explizit gesetzt werden.
> Beispiel: `[ForeignKey("StoreId")]` 

**Wir werden allerdings mehrteilige Schlüssel vermeiden!** Navigationen müssen - falls
z. B. eine Bestellung auf ein Offer verweist - auch mehrteilig sein und immer händisch
definiert werden. Daher verwenden wir eine bessere Technik: Wir belassen die auto increment Id und
definieren einen *Unique Index*.

```csharp
public class StoreContext : DbContext
{
    /* ... */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hinweis: Setze [ForeignKey("StoreId")] und [ForeignKey("ProductEan")]
        // über den Navigations, um den Nanem des FK genau zu spezifizieren.
        modelBuilder.Entity<Offer>().HasIndex("StoreId" "ProductEan").IsUnique();
    }
}
```

Durch den Unique Index ist sichergestellt, dass ein Produkt im selben Store nicht zu unterschiedlichen
Preisen angeboten werden kann.

Unter https://docs.microsoft.com/en-us/ef/core/modeling/ gibt es eine sehr detaillierte Beschreibung
der weiteren Einstellungsmöglichkeiten. Meist kommt man aber mit den hier gezeigten
Möglichkeiten aus.

## 1:1 Beziehungen

Betrachten wir folgendes Klassendiagramm.
Es zeigt Kunden, die eine Kundenkarte besitzen können.

![](https://www.plantuml.com/plantuml/svg/NSszgeD04CNnVf_YO7jHxcsj16zIXUGP5dUY0_iXioD4GjuzMKG8LdFu5x_fibhH9NWpiIDGc7L589sIoyRJNQ49mkEaS2mqAATe1czpHhzaJO44JmC0FsUMhD46Gekimd7S-iKUyyiSYDwAsvcohu8M77KhfyVCOildVwk5AUSM_LgWdK3rLw6kQrI_JVEPtxJfABeb-3S0)

Wir stellen bei der genaueren Analyse der 1:1 Beziehung fest:
* Es handelt sich um eine **1 : 0 oder 1** Beziehung, da ein Kunde auch keine Kundenkarte besitzen kann.
* Eine Kundenkarte kann nicht ohne Kunde existieren, umgekehrt jedoch schon.

Besonders die letzte Feststellung ist für den Begriff der *dependent entity* relevant.
Wir müssen vor der konkreten Umsetzung identifizieren, welche Entity von der anderen abhängig ist.
Diese Entity wird dann das Fremdschlüsselfeld beinhalten.

Die Modelklassen haben folgenden Aufbau:

```csharp

public class Customer
{
    #pragma warning disable CS8618
    protected Customer() { }
    #pragma warning restore CS8618
    public Customer(string firstname, string lastname)
    {
        Firstname = firstname;
        Lastname = lastname;
    }

    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public CustomerCard? CustomerCard { get; set; }
}

public class CustomerCard
{
    #pragma warning disable CS8618
    protected CustomerCard() { }
    #pragma warning restore CS8618
    public CustomerCard(int cardNr, DateOnly expirationDate, Customer customer)
    {
        CardNr = cardNr;
        ExpirationDate = expirationDate;
        Customer = customer;
    }

    public int Id { get; set; }
    public int CardNr { get; set; }
    public DateOnly ExpirationDate { get; set; }
    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; }
}
```

Wir erkennen folgende Besonderheiten bei der Umsetzung:

* Das Property *CustomerCard* von *Customer* ist *nullable*.
  Ein Kunde muss schließlich keine Kundenkarte haben.
* Das Property *CustomerCard* ist nicht im Konstruktor von *Customer* enthalten.
  Wir müssen ja zuerst einen Kunden anlegen, um ihn eine Kundenkarte zuweisen zu können.
* Das Entity *Customer* ist das sogenannte *principal entity*.
  Es kann alleine existieren.

### Das ForeignKey Attribut

Im Code fällt das Attribut *[ForeignKey("CustomerId")]* auf.
Würden wir keinen ForeignKey definieren, entsteht beim Erstellen der Datenbank eine Exception:

```
System.InvalidOperationException : The dependent side could not be determined for the one-to-one relationship between 'CustomerCard.Customer' and 'Customer.CustomerCard'. To identify the dependent side of the relationship, configure the foreign key property. If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'. See https://go.microsoft.com/fwlink/?LinkId=724062 for more details.
```

Diese Exception sagt folgendes aus: EF Core muss wissen, in welcher Tabelle der Fremdschlüssel angelegt werden soll.
Bei 1:n Beziehungen ist dies klar (auf der 1er Seite), bei 1:1 Beziehungen wird der FK üblicherweise beim *dependent entity* angelegt.
Durch diese Annotation bestimmen wir, dass *CustomerCard* das *dependent entity* ist und dass dort der Fremdschlüssel angelegt werden kann.

### Grenzen der 1:1 Beziehung

Die Datenbank hat keine eigenen Constraints für eine 1:1 Beziehung.
Es wird ein "normaler" Fremdschlüssel wie bei einer 1:n Beziehung angelegt.
Dadurch ist es aber möglich, dass ein Kunde mehrere Kundenkarten besitzt.
Um dies zu verhindern, legt der OR Mapper einen *unique index* auf das Feld *CustomerId* in *CustomerCard* an.

Wir haben in diesem Fall eine "1 : 0 oder 1" Beziehung modelliert.
Wie können wir eine exakte 1:1 Beziehung abbilden, sprich: Jeder Kunde *muss* eine Kundenkarte haben?

In der Doku von EF Core gibt es einen Hinweis, dass dies nicht möglich ist:

> A required relationship ensures that every dependent entity must be associated with some principal entity.
> However, a principal entity can always exist without any dependent entity.
> That is, a required relationship does not indicate that there will always be a dependent entity.
> There is no way in the EF model, and also no standard way in a relational database, to ensure that a principal is associated with a dependent.
> If this is needed, then it must be implemented in application (business) logic. See Required navigations for more information.
> 
> <sup>https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-one</sup>

Alle möglichen Szenarien von Beziehungen und deren Konfiguration sind auf
https://docs.microsoft.com/en-us/ef/core/modeling/relationships beschrieben.

## Guid Werte als Alternate Key

Im Bereich der Webapplikationen wird oft eine eindeutige ID benötigt, die als Parameter einer
URL verwendet werden kann. Die interne ID soll dabei nicht verwendet werden, da z. B. aus der URL
*/Store/Delete/4* leicht ableitbar ist, dass es auch einen Store 3 oder 5 geben kann.

Die GUID (oft auch UUID genannt) ist ein 128 bit langer Wert, der oft als Hexstring angegeben wird.
Somit könnte die URL */Store/Delete/6d925883-f6f9-46c4-b47f-0bc80524d0b9* lauten. Hier ist es schon
schwieriger, auf einen anderen Store zu schließen.

Die GUID wird mit dem .NET Typ *Guid* einfach im Model definiert. Da wir sie nicht zuweisen wollen,
hat sie eine *private set* Methode.

```csharp
public class Store
{
    public Store(string name, User? manager = null)
    {
        Name = name;
        Manager = manager;
        ManagerId = manager?.Id;
        Guid = Guid.NewGuid();
    }
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Store() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public int Id { get; private set; } 
    public Guid Guid { get; private set; }
    [MaxLength(255)]      // Produces NVARCHAR(255) in SQL Server
    public string Name { get; set; }
    public int? ManagerId { get; set; }
    public User? Manager { get; set; }
}
```

### Konfiguration

Das GUID Property bekommt noch zwei spezielle Einstellungen: Mit *HasAlternateKey()* können
wir EF Core veranlassen, dass es einen UNIQUE Index bekommt. Außerdem ist dies notwendig für
die zweite Konfiguration. Mit *ValueGeneratedOnAdd()* wird EF Core - wie der Name schon sagt -
selbstständig einen GUID Wert beim Einfügen des Datensatzes generieren. Dies funktioniert allerdings
nur, wenn das Property mit *HasAlternateKey()* konfiguriert wurde.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    /* ... */
    modelBuilder.Entity<Store>().HasAlternateKey(s => s.Guid);
    modelBuilder.Entity<Store>().Property(s => s.Guid).ValueGeneratedOnAdd();
}
```

In der Klasse *Store* wird im Konstruktor trotzdem eine neue GUID zugewiesen, da wir vom
Standpunkt des Domain Modellings nicht warten wollen, bis EF Core durch SaveChanges() diese
ID geniert.

Es ist auch möglich, die Properties mit dem Namen *Guid* in *allen Entity Klassen* mit der
oben beschriebenen Konfiguration zu versehen:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    /* ... */
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        // Generic config for all entities
        // ON DELETE RESTRICT instead of ON DELETE CASCADE
        foreach (var key in entityType.GetForeignKeys())
            key.DeleteBehavior = DeleteBehavior.Restrict;

        foreach (var prop in entityType.GetDeclaredProperties())
        {
            // Define Guid as alternate key. The database can create a guid fou you.
            if (prop.Name == "Guid")
            {
                modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
            }
            // Default MaxLength of string Properties is 255.
            if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
            // Seconds with 3 fractional digits.
            if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
            if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
        }
    }
}
```

## Übung

Lade die Datei [StoresWithOneToOne20251106.7z](StoresWithOneToOne20251106.7z) herunter und entpacke sie in dein Repo.
Alternativ kannst du mit `git clone https://github.com/Die-Spengergasse/course-pos-csharp_basics` das Repo klonen und das Programm aus dem Verzeichnis *03 EF Core/03_EnhancedCodeFirst/StoresWithOneToOne* kopieren.


> **Achtung:** Die 7z Datei soll nicht im Repo sein. Das Archiv soll auch nicht in einen eigenen Ordner entpackt werden,
> sodass eine Struktur StoresWithOneToOne20251106/StoresWithOneToOne entsteht. Der Ordner StoresWithOneToOne soll im Hauptordner des Repos sein.

Implementiere das folgende Modell und beachte die folgenden Informationen:

* Primary Keys heißen - wenn nicht im Modell angegeben - *Id*.
* Die Foreign Keys werden nach der Convention Propertyname + Name des PK generiert.
  Dies ist bei der Zuweisung des FKs der dependent Entities wichtig.
* Zwischen *Store* und *StoreDetails* sowie zwischen *Product* und *ProductDetails* liegt eine 1:1 Beziehung vor.
  Definiere korrekt das principal und das dependent Entity.
* Die Enum *Weekday* ist als String zu speichern.
  Verwende die Konfiguration `HasConversion<string>()` für das Property *OpeningHour.Weekday* in *OnModelCreating*.
* Erstelle alle Tabellen mit den im Modell angegeben Namen (also Offer statt Offers, etc.).
* Stringfelder sollen mit 255 Zeichen begrenzt werden.
* *Offer.Price* soll mit 9 Stellen, davon 4 Nachkommastellen gespeichert werden.
* Alle Klassen sollen Konstruktoren mit den notwendigen Feldern sowie protected default Konstruktoren für EF Core besitzen. Es soll keine Klasse einen public default Konstruktor besitzen!

Im Musterprojekt sind in der Klasse *StoreContextTests.cs* folgende Unittests zu implementieren. Die Beschreibung befindet sich im Musterprojekt:

* EnsureProductAndStoreInOfferIsUniqueTest()
* InsertStoreAndStoreDetailsTest()
* InsertProductAndProductDetailsTest()
* InsertOpeningHourTest()

Die vordefinierten Tests in *GradingTests.cs* dienen zur Selbstkontrolle.

![](https://www.plantuml.com/plantuml/svg/RL9DZzCm4BtxLunoGO9sHQzHM-r2M85sIrVi8YHkLZnjBM8xsED3GVntx4r6TQf4iZ9-ysRUVFWsaFGKJITEMY6WwUaC1kdaL13Y3Mms6tZeP0XeiotQRCCpEO_mLq3wtdsL1g66G5xR8y1w7Qt-7N6x5Vz4oM-GfEuuwaufZyd5WGFwK_TNo4TDvojetwDDbyyk-Xp_g0Ej2-nL5sqqy0FnbvBdKNAtNq3pPG6xT9fattzRmFDTEljBeKKQBhIilIux6ToiKRFQ0DzxPnBseWtkRJU2Bww4acWfjrnoXWbSlR-vULjbRxMgO5fCjIs1Yfjuyax5bhY1Jz8chhOqjwxfjgmZ1A-F9m9tu3nrdJAasAS7WLP1vbmfJJVRO6YzxaaxUxMudRJn85kA7lt5THd-uO3JiVF01uZxgbcNOTrcfi9M6pckvL46-juhITdJf5yUUvBYD_q1_sWyHNiSttNMdXrsCUPgbiLLPHvVgYopgbVBGaOCwNB8_387ArI2CnRK4PeXtw9LqNJ_0G00)
<sup>https://www.plantuml.com/plantuml/uml/RLDDZzCm4BtxLunyGO9sHQzHM-r2M85sIrVi8YHkLZnjBHexsED3GVntx4r6TQ9KiZf-ysRUV3YtWPIds9x5oMW4R3kwG8jqSZe8yGOscmqyd5K8QBCjccp3CpcFy5T0-htxgbg42W9vOuy0wtMqvdV4xLRy4oM_HbBcp55tMdjCBWxiqP-cco0V3LqcqBv3cowVNVGv_j44MXVOgoxQsC8Fn5zQNGR9tNu1zPS5xDAfatt_RG7FT-dihuKMQRZGilQgxAHtiqR5QZryxrsRi1VJujwU1_35bL0IBUMMIyumWkNxcvktCdlbIiAu6Baj0SbDF7cdOqFSm2Tb4zTOcblNU5lM4O8tnnE1Et2UgaePKkpJWq2h8NCcIkFDrXWQRpeopauMjrCs7bH3qQE_kgx3FnnmF7OUU0znNnNhCcoQpDJOc5ODQtbKWRvtMb7srEalZpr9yPl-W7yCdg8z3kywQyyEknYpd6LnLPd75vbbLjLgMSY8FbqEcLzcRmKfE6D17Q0PihX5g_Ft-ny0</sup>

