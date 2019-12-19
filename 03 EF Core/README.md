# Entity Framework Core

## Tools zum Erstellen von Datenbanken

- DBeaver: [Information zur Installation und Konfiguration](Dbeaver.md)
- JetBrains DataGrip: [Information zur Installation und Konfiguration](DataGrip.md)

## (Täglich) verwendete Befehle

Für die Verwendung der CLI Tools müssen diese für EF Core vor der ersten Verwendung und nach einem
Update der .NET Core Version installiert werden:

```powershell
dotnet tool update --global dotnet-ef
```

## Generieren der Modelklassen für Applikationen

Die CLI Befehle sind im Verzeichnis der *csproj* Datei auszuführen. Die Applikation muss kompilierbar
sein.

### SQLite

Beim Verbindungsstring von scaffold sind folgende Dinge anzupassen:

- *DataSource=xxxx*: Ort der SQLite Datei. Der Pfad geht von der ausführbaren Datei aus.

#### CLI

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet ef dbcontext scaffold "DataSource=xxxx" Microsoft.EntityFrameworkCore.Sqlite ^
    --output-dir Model --use-database-names --force --data-annotations
```

#### Packet Manager Console (Visual Studio)

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools   # EF Tools installieren
Install-Package Microsoft.EntityFrameworkCore.Sqlite  # SQLite Treiber installieren
Scaffold-DbContext "DataSource=xxxx" Microsoft.EntityFrameworkCore.Sqlite
    -OutputDir Model -UseDatabaseNames -Force -DataAnnotations
```

### MySQL

Beim Verbindungsstring von scaffold sind folgende Dinge anzupassen:

- *Server=aaaaa*: Durch den Servernamen (DNS oder IP) des MySQL Servers zu ersetzen.
- *Database=bbbbb*: Durch den Datenbanknamen zu ersetzen.
- *User=ccccc*:  Benutzername des Users für den Datenbankzugriff.
- *Password=ddddd*: Passwort des Benutzers.

#### CLI

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet ef dbcontext scaffold ^
    "Server=aaaaa;Database=bbbbb;User=ccccc;Password=ddddd;TreatTinyAsBoolean=true;" Pomelo.EntityFrameworkCore.MySql ^
    --output-dir Model --use-database-names --force --data-annotations
```

#### Packet Manager Console (Visual Studio)

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools   # EF Tools installieren
Install-Package Pomelo.EntityFrameworkCore.MySql      # MySQL Treiber installieren
Scaffold-DbContext "Server=aaaaa;Database=bbbbb;User=ccccc;Password=ddddd;TreatTinyAsBoolean=true;"
    Pomelo.EntityFrameworkCore.MySql -OutputDir Model -UseDatabaseNames -Force -DataAnnotations
```

### SQL Server

Beim Verbindungsstring von scaffold sind folgende Dinge anzupassen:

- *Server=tcp:aaaa,1433*: Durch den Servernamen (DNS oder IP) des SQL Servers zu ersetzen. 1433 ist der Standardport.
- *Initial Catalog=bbbb*: Durch den Datenbanknamen zu ersetzen.
- *User ID=cccc*: Benutzername des Datenbankusers.
- *Password=dddd*: Passwort des Datenbankusers.

#### CLI

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet ef dbcontext scaffold ^
    "Server=tcp:aaaa,1433;Initial Catalog=bbbb;Persist Security Info=False;User ID=cccc;Password=dddd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" ^
    Microsoft.EntityFrameworkCore.SqlServer ^
    --output-dir Model --use-database-names --force --data-annotations
```

#### Packet Manager Console (Visual Studio)

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools      # EF Tools installieren
Install-Package Microsoft.EntityFrameworkCore.SqlServer  # SQL Server Treiber installieren
Scaffold-DbContext "Server=tcp:aaaa,1433;Initial Catalog=bbbb;Persist Security Info=False;User ID=cccc;Password=dddd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -UseDatabaseNames -Force -DataAnnotations
```

## Modell unserer Musterdatenbank

[Download der Tests Datenbank](Tests.db)
[SQL Dump der Tests Datenbank für SQL Server](testsdb_sqlserver.sql)
[Access Datei der Tests Datenbank](Tests.mdb)

![](images/er_diagram.png)

![](images/classdiagram.png)
