# Warum ein OR Mapper (Object-relational mapping)

## Der Namespace System.Data als Unterbau

Natürlich kann auch in .NET Core - so wie in anderen Frameworks auch - ein "roher" Datenbankzugriff
erfolgen. Folgendes Programm zeigt diese Möglichkeit für eine SQLite Datenbank:

```c#
// *************************************************************************************************
// ACHTUNG: DIESER CODE KANN IHRE GESUNDHEIT GEFÄHRDEN!!!
// Vorher die Testdatenbank in den Ausgabeordner kopieren
// *************************************************************************************************

using System;
using System.Data.SQLite;   // NuGet: Install-Package System.Data.SQLite.Core
namespace DbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string nameBeginsWith = "A";
            SQLiteConnection conn = new SQLiteConnection("DataSource=Tests.db");
            conn.Open();

            // Einladung für SQL Injections...
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Pupil WHERE P_Lastname LIKE '" + nameBeginsWith + "%'", conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Wir casten auf "gut Glück", denn alles ist ein object.
                long id = (long)reader["P_ID"];
                string lastname = reader["P_Lastname"] == DBNull.Value ? null : (string)reader["P_Lastname"];
                Console.WriteLine($"{id}: {lastname}");
            }

            // Wer schließt reader und conn?? Es wurde kein using verwendet.
        }
    }
}
```

Allerdings hat diese Technik einige (gravierende) Nachteile:

- Es wird *System.Data.SQLite* eingebunden. Möchten wir ein anderes DBMS verwenden (z. B. SQL Server),
  so müssen alle Funktionsaufrufe geändert werden.
- Das Resultset liefert die Spalten als object. Wir müssen "auf gut Glück" casten, wir haben keine
  Information über den Datentyp in der Tabelle.
- SQL Statements werden durch Stringverknüpfung natürlich ohne Syntaxprüfung mit allen Gefahren
  erzeugt.  
- Der Zugriff auf die Spalten erfolgt mittels der Spaltenbezeichnung als String. Wird diese Spalte
  nicht gefunden, entsteht ein Laufzeitfehler.
- Das Schließen der Verbindungen und Resultsets muss durch den Programmierer händisch gemacht werden.

Der Datenbankzugriff in heute gängigen Frameworks im Enterprise Architecture Bereich erfolgt daher
nie nach dieser Methode, sondern durch eine zusätzliche Zwischenschicht: den OR Mapper. Er erzeugt
generisch aus den Tabellendefinitionen Klassen und ermöglicht einen typisierten Zugriff auf eine
Relationale Datenbank. In .NET ist das *Entity Framework* dieser OR Mapper und ermöglicht auch
den Zugriff über LINQ.
