# Datenbankzugriff mit ADO.NET

## Erstellen der Datenbank
- Öffne das SQL Server Management Studio. Gib dafür im Startmenü *SSMS* ein.
- Erstelle eine neue Datenbank mit dem Namen *WeatherDb*. Klicke dafür mit der rechten Maustaste auf *Datenbanken*
  und wähle *Neue Datenbank...*.
- Kopiere den Inhalt von *WeatherDbDump.sql* in ein neues Abfragefenster. Achte darauf, dass als Datenbank
  *WeatherDb* ausgewählt ist. Führe mit dem Play Button nun alle Befehle aus.

## Erstellen der Modelklassen
Jede Tabelle wird in einer Modelklasse umgesetzt. Wichtig ist dabei die richtige Umsetzung der Datentypen.
So können - wenn nicht anders eingestellt - Datenbankwerte NULL annehmen. Wertetypen in C# allerdings 
kennen keinen NULL Wert. Mit den nullable Types in C# (int?, double?, ...) können diese Werte abgebildet werden.

Betrachen wir die Tabelle Station:
```sql
CREATE TABLE Station (
	S_ID       INTEGER      PRIMARY KEY,
	S_Location VARCHAR(100) NOT NULL,
	S_Height   INTEGER
);
```

In C# lautet die Modelklasse wie folgt:
```c#
public class Station
{
	public int? S_Height { get; set; }      // int?, da im CREATE TABLE NULL Werte erlaubt sind.
	public int S_ID { get; set; }
	public string S_Location { get; set; }   
}
```

## Zugriff mit "rohen" ADO.NET Funktionen

```c#
string connection = "Server=(local);Database=WeatherDb;Trusted_Connection=True;";
// SELECT * FROM Station
SqlCommand command = new SqlCommand("SELECT * FROM Station", connection);
using (connection = new SqlConnection(connectionString))
{
	connection.Open();
	using (SqlDataReader reader = command.ExecuteReader())
	{
		while (reader.Read())
		{
			yield return new Station
			{
				S_ID = (int)reader["S_ID"],
				S_Location = (string)reader["S_Location"],
				S_Height = reader["S_Height"] == System.DBNull.Value ? null : (int?)reader["S_Height"]
			};
		}
	}
}
```

