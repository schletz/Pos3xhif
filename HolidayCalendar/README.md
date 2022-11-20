# Österreichische Feiertage und Schulferien

Die Klasse *CalendarYear* berechnet alle österreichischen Feiertage sowie die Schulferien, wie
sie in den Bundesländern Wien und Niederösterreich gelten.

Die Applikation ist ein xUnit Test, da die Klasse zur Verwendung in anderen Programmen entwickelt
wurde (Serviceklasse).

Die beweglichen Feiertage sind an Ostern gebunden. Das Osterdatum wird mit der Formel nach
Spencer (https://de.wikipedia.org/wiki/Spencers_Osterformel) berechnet.

```c#
public DateTime CalcEasterSunday(int year)
{
    int a = year % 19;
    int b = year / 100;
    int c = year % 100;
    int d = b / 4;
    int e = b % 4;
    int f = (b + 8) / 25;
    int g = (b - f + 1) / 3;
    int h = (19 * a + b - d - g + 15) % 30;
    int i = c / 4;
    int k = c % 4;
    int l = (32 + 2 * e + 2 * i - h - k) % 7;
    int m = (a + 11 * h + 22 * l) / 451;
    int x = h + l - 7 * m + 114;
    int n = x / 31;
    int p = x % 31;
    return new DateTime(year, n, p + 1);
}
```

## Generieren eines Kalenderfiles

Soll eine Textdatei mit allen Tagen zwischen dem 1.1.2000 und 31.12.2399 generiert werden, kannst
du den Test direkt im Verzeichnis der *csproj* Datei von der Konsole starten:

```
dotnet test --filter CalendarCalculator.CalendarYearTests.WriteFileTest
```

400 Jahre ist eine volle Periode im gregorianischen Kalender. Das bedeutet, dass die Tage
nach diesem Zyklus wieder auf den selben Wochentag fallen. Der 14.11.2022 hat also den selben
Tag wie der 14.11.2422 oder der 14.11.1622. Für Berechnungen des Mittelwertes wird diese volle
Periode herangezogen.

Feiertage sind nicht kollisionsfrei, wenn der Ostersonntag auf den 23.3. fällt, so fällt
Christi Himmelfahrt (39 Tage nach dem Ostersonntag) auf den 1. Mai. Dieser Tag ist in Österreich
der Staatsfeiertag. Dieser "Doppelfeiertag" kommt in den Jahren 2000 - 2400 aber nur 4x vor
(2008, 2160, 2228 und 2380). Im Programm wird in diesem Fall der Staatsfeiertag angegeben.

## Die Felder des Kalenderfiles

```
| DATE       | DATE2000   | YEAR | MONTH | DAY | SCHOOLYEAR | WEEKDAY_NR | WEEKDAY_STR | WORKINGDAY | WORKINGDAY_COUNTER | SCHOOLDAY | SCHOOLDAY_COUNTER | PUBLIC_HOLIDAY | SCHOOL_HOLIDAY | PUBLIC_HOLIDAY_NAME | SCHOOL_HOLIDAY_NAME |
|------------|------------|------|-------|-----|------------|------------|-------------|------------|--------------------|-----------|-------------------|----------------|----------------|---------------------|---------------------|
| 2022-01-01 | 2000-01-01 | 2022 | 1     | 1   | 2021       | 6          | SA          | 0          | 5763               | 0         | 4263              | 1              | 1              | Neujahr             | Weihnachtsferien    |
| 2022-01-02 | 2000-01-02 | 2022 | 1     | 2   | 2021       | 7          | SO          | 0          | 5763               | 0         | 4263              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-03 | 2000-01-03 | 2022 | 1     | 3   | 2021       | 1          | MO          | 1          | 5764               | 0         | 4263              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-04 | 2000-01-04 | 2022 | 1     | 4   | 2021       | 2          | DI          | 1          | 5765               | 0         | 4263              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-05 | 2000-01-05 | 2022 | 1     | 5   | 2021       | 3          | MI          | 1          | 5766               | 0         | 4263              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-06 | 2000-01-06 | 2022 | 1     | 6   | 2021       | 4          | DO          | 0          | 5766               | 0         | 4263              | 1              | 1              | Heilige 3 Könige    | Weihnachtsferien    |
| 2022-01-07 | 2000-01-07 | 2022 | 1     | 7   | 2021       | 5          | FR          | 1          | 5767               | 1         | 4264              | 0              | 0              |                     |                     |
| 2022-01-08 | 2000-01-08 | 2022 | 1     | 8   | 2021       | 6          | SA          | 0          | 5767               | 0         | 4264              | 0              | 0              |                     |                     |
| 2022-01-09 | 2000-01-09 | 2022 | 1     | 9   | 2021       | 7          | SO          | 0          | 5767               | 0         | 4264              | 0              | 0              |                     |                     |
| 2022-01-10 | 2000-01-10 | 2022 | 1     | 10  | 2021       | 1          | MO          | 1          | 5768               | 1         | 4265              | 0              | 0              |                     |                     |
```

Eine bereits generierte Datei ist bz2 komprimiert als [calendar.txt.bz2](calendar.txt.bz2)
hier verfügbar.
Die Datei *calendar.txt* ist Unicode codiert (16bit), hat *CR+LF* als Trennzeichen und *TAB* als
Trennzeichen für Spalten. Strings sind nicht unter Anführungszeichen.

- **DATE** Der Datumswert des Tages.
- **DATE2000** Der Datumswert im Jahr 2000 mit dem Monat und Tag von *DATE*. Das ist für
  Gruppieringen nützlich, wenn z. B. der Mittelwert aller Werte vom 1.1. ermittelt werden soll.
  Da 2000 ein Schaltjahr ist, wird der 29.2. auch als Gruppe ausgegeben.
- **YEAR** Die Jahreskomponente als ganze Zahl.
- **MONTH** Die Monatskomponente als ganze Zahl.
- **DAY** Die Tageskomponente als ganze Zahl.
- **SCHOOLYEAR** Das Schuljahr des Tages. 2000 für 2000/01, 2001 für 2001/02, usw.
- **WEEKDAY_NR** 1 = MO, 2 = DI, ... 6 = SA, 7 = SO. Ermöglicht eine Ermittlung des Wochentages
  unabhängig von der verwendeten LOCALE Einstellung der Session.
- **WEEKDAY_STR** Der String des deutschen Wochentages (MO, DI, MI, DO, FR, SA, SO)
- **WORKINGDAY** 1 wenn der Tag ein Arbeitstag ist (MO - FR), 0 wenn der Tag kein Arbeitstag ist.
  SA und SO haben immer den Wert 0,
- **WORKINGDAY_COUNTER** Durchgängiger Zähler, der an jedem Arbeitstag um 1 hochgezählt wird.
  So kann z. B. das Datum, das 5 Arbeitstage in der Zukunft liegt, exakt ermittelt werden.
- **SCHOOLDAY** 1 wenn der Tag ein Schultag ist (MO - FR), 0 wenn der Tag kein Schultag ist.
  SA und SO haben immer den Wert 0,
- **SCHOOLDAY_COUNTER** Durchgängiger Zähler, der an jedem Schultag um 1 hochgezählt wird.
  So kann z. B. das Datum, das 5 Schultage in der Zukunft liegt, exakt ermittelt werden.
- **PUBLIC_HOLIDAY** 1 wenn der Tag ein Feiertag nach dem Arbeitsruhegesetz ist, sonst 0. Samstag
  und Sonntag sind - wenn nicht ein Feiertag darauf fällt - keine Feiertage (0).
- **SCHOOL_HOLIDAY** 1 wenn der Tag ein schulfreier Tag nach dem Schulzeitgesetz ist, sonst 0. Samstag
  und Sonntag sind - wenn nicht ein Ferientag darauf fällt - keine Ferientage (0).
- **PUBLIC_HOLIDAY_NAME** Name des Feiertages.
- **SCHOOL_HOLIDAY_NAME** Name der Ferien.

## Warum die Tage als Tabelle speichern?

Die Speicherung jedes Tages von 2000 - 2400 klingt einmal verschwenderisch. Es müssen 146.097
Zeilen in der Tabelle gespeichert werden. Für eine Datenbank ist das jedoch keine große Menge,
und die Tabelle benötigt (in SQL Server) gerade einmal 6.2 MB. Die Vorteile, die sich daraus
ergeben, beschleunigen sogar unsere Abfragen:

- Statt komplizierter Datumsarithmetik (z. B. Anzahl der Arbeitstage zwischen 2 Werten) genügt
  ein einfacher JOIN und eine einfache Subtraktion. Den kann die Datenbank sehr schnell bearbeiten.
  Komplexere Funktionen sind für die Datenbank und die Optimierung schwieriger zu bearbeiten.
- Gerade die beweglichen Feiertage, die an Ostern gebunden sind, erfordern viel Ablauflogik
  (Berechnung des Osterdatums). SQL (und relationale Datenbanken) sind mengenorientiert.
  Implementierung von Ablauflogik ist schwieriger, die prozeduralen Erweiterungen und Funktionen in
  SQL sind weit weg von der Eleganz und den Möglichkeiten gängiger Programmiersprachen wie C# oder Java.
- Feiertage oder Ferien können sich in der Zukunft ändern. So wurden z. B. 2020 die Herbstferien
  eingeführt, dafür ist der DI nach Ostern und Pfingsten nicht mehr schulfrei. Um korrekte Werte
  auch aus der Vergangenheit ermitteln zu können, brauchen wir eine Tabellierung.

## Import in SQL Server

In SQL Server kann die Datei z. B. über BULK INSERT eingespielt werden. Dafür sollte eine Tabelle
mit spezifischen Datentypen erstellt werden, um den Speicherbedarf zu verringern. Daher wird auch
*DATE* (nur Tag), *TINYINT*, ... statt *DATETIME* oder *INTEGER* verwendet. *BIT* wird nicht
verwendet, da die Spalte auch über *SUM()* problemlos aufsummiert werden soll.

Für *BULK INSERT* muss die Datei lokal - also im Docker Container von SQL Server - vorliegen.
Wir können das */home* Verzeichnis z. B. auf *C:/Temp/sql-home* mappen und die Datei unter Windows
dorthin kopieren.

```
docker run -d -p 1433:1433 --name sqlserver2019 -v C:/Temp/sql-home:/home -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/mssql/server:2019-latest      
```

```sql
DROP TABLE IF EXISTS CalendarDay;
GO
CREATE TABLE CalendarDay (
    Date     DATE PRIMARY KEY,
    Date2000 DATE NOT NULL,
    Year  SMALLINT NOT NULL,
    Month TINYINT NOT NULL,
    Day   TINYINT NOT NULL,
    Schoolyear SMALLINT NOT NULL,
    WeekdayNr  TINYINT NOT NULL,
    WeekdayStr CHAR(2) NOT NULL,
    Workingday         TINYINT NOT NULL,
    WorkingdayCounter  INTEGER NOT NULL,
    Schoolday          TINYINT NOT NULL,
    SchooldayCounter   INTEGER NOT NULL,
    PublicHoliday TINYINT NOT NULL,
    SchoolHoliday TINYINT NOT NULL,
    PublicHolidayName VARCHAR(20) NOT NULL,
    SchoolHolidayName VARCHAR(20) NOT NULL
);
GO
BULK INSERT CalendarDay FROM '/home/calendar.txt' WITH (    
    FIRSTROW = 2,
    DATAFILETYPE = 'widechar',
    ROWTERMINATOR =  '\r\n',
    FIELDTERMINATOR = '\t'
);    
GO
SELECT Year,
    SUM(Workingday) AS WorkingDays,
    SUM(Schoolday) AS SchoolDays
FROM CalendarDay
WHERE Year >= 2020 AND Year < 2030
GROUP BY Year
ORDER BY Year;
```

```
| Year | WorkingDays | SchoolDays |
| ---- | ----------- | ---------- |
| 2020 | 252         | 185        |
| 2021 | 252         | 184        |
| 2022 | 250         | 184        |
| 2023 | 248         | 182        |
| 2024 | 252         | 184        |
| 2025 | 250         | 185        |
| 2026 | 251         | 184        |
| 2027 | 252         | 184        |
| 2028 | 248         | 183        |
| 2029 | 250         | 183        |
```
