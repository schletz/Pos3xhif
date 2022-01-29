# OR Mapping mit Entity Framework Core

## Inhalt

- [Warum OR Mapping?](01_WhyORMapping/README.md)
- [Erzeugen von Datenbanken](02_CodeFirstEfCore5/README.md)
- [Erweiterte Konfiguration](03_EnhancedCodeFirst/README.md)
- [**Rich Domain Models](04_RichDomainModels/README.md)
- [Abfragen](05_Queries/README.md)
- [CRUD Operationen](06_Crud/README.md)
- [**Migrations](08_Migrations/README.md)

### Musterprogramme

- [Demoprogramm zu Code First](CodeFirstDemo)
- [Demoprogramm zu Rich Domain Models](RichDomainModelDemo)

## Tools zum Betrachten der Datenbanken

- DBeaver: [Information zur Installation und Konfiguration](Dbeaver.md)
- JetBrains DataGrip: [Information zur Installation und Konfiguration](DataGrip.md)

## EF Core Connection Strings

Die nachfolgenden Beispiele zeigen verschiedene Connection Strings. In den Programmen wird *UseSqlite*
beim Erstellen der DbContextOptions zum Instanzieren der DbContext Klasse verwendet.

```c#
var opt = new DbContextOptionsBuilder()
    .UseSqlite(@"Data Source=Stores.db")
    .Options;
```

Die Methode *UseSqlite()* kann leicht durch andere Methoden ersetzt werden. Die Version 6 von
den angeführten Providern setzt ein .NET 6 Projekt voraus. Es wird immer der Verbindungsstring
zur Datenbank *MeineDb* mit dem User *MeinUser* und dem Passwort *MeinPasswort* angegeben. Ersetze
ggf. diese Daten. Bei MySQL (MariaDB) muss mit *SELECT VERSION()* die Version der Datenbank
herausgefunden und angepasst werden.

Der Standard Adminuser unter MySql ist *root* (ohne Passwort). Bei SQL Server ist das der User
*sa* mit dem bei *docker run* angegebenen Passwort.

| Provider             | Package Reference                                                                      | Connection String                                                                                                                      |
| -------------------- | -------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Oracle               | `<PackageReference Include="Oracle.EntityFrameworkCore" Version="6.*" />`              | `UseOracle($"User Id=MeinUser;Password=MeinPasswort;Data Source=localhost:1521/XEPDB1")`                                               |
| MySQL                | `<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="6.*" />`        | `UseMySql(@"server=localhost;database=MeineDb;user=MeinUser;password=MeinPasswort", new MariaDbServerVersion(new Version(10, 4, 22)))` |
| SQL Server           | `<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.*" />` | `UseSqlServer(@"UseSqlServer(@"Server=127.0.0.1,1433;Initial Catalog=MeineDb;User Id=MeinUser;Password=MeinPasswort")")`               |
| SQL Server (LocalDB) | `<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.*" />` | `UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MeineDb")`                                                          |
| SQLite               | `<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="6.*" />`    | `UseSqlite(@"Data Source=MeineDb.db")`                                                                                                 |
| SQLite (in-memory)   | `<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="6.*" />`    | `UseSqlite(@"Data Source=:memory:")`                                                                                                   |

### SQLite in-memory Datenbank für Unittests

Die in-memory Datenbank von SQLite ist für Unittests geeignet. Die Verbindung muss allerdings
vorkonfiguriert werden, damit EF Core die Verbindung nach einer Operation nicht trennt. Daher
verwenden wir bei Unittests eine Basisklasse *DatabaseTest*. Diese Klasse instanziert z. B.
einen Context mit dem Namen *StoreContext* im Konstruktor.

```c#
public class DatabaseTest : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly StoreContext _db;

    public DatabaseTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var opt = new DbContextOptionsBuilder()
            .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
            .UseLazyLoadingProxies()
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

Der konkrete Test kann nun mit *EnsureCreated* die Datenbank erstellen, d. h. die Tabellen werden
auf Basis der DbSets angelegt.

```c#
public class StoreContextTests : DatabaseTest
{
    [Fact]
    public void CreateDatabaseTest()
    {
        _db.Database.EnsureCreated();
    }
}

```