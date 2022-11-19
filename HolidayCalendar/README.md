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

## Die Felder des Kalenderfiles

```
| DATE       | DATE2000   | YEAR | MONTH | DAY | WEEKDAY_NR | WEEKDAY_STR | WORKINGDAY | WORKINGDAY_COUNTER | SCHOOLDAY | SCHOOLDAY_COUNTER | PUBLIC_HOLIDAY | SCHOOL_HOLIDAY | PUBLIC_HOLIDAY_NAME | SCHOOL_HOLIDAY_NAME |
| ---------- | ---------- | ---- | ----- | --- | ---------- | ----------- | ---------- | ------------------ | --------- | ----------------- | -------------- | -------------- | ------------------- | ------------------- |
| 2022-01-01 | 2000-01-01 | 2022 | 1     | 1   | 6          | SA          | 0          | 5511               | 0         | 4078              | 1              | 1              | Neujahr             | Weihnachtsferien    |
| 2022-01-02 | 2000-01-02 | 2022 | 1     | 2   | 7          | SO          | 0          | 5511               | 0         | 4078              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-03 | 2000-01-03 | 2022 | 1     | 3   | 1          | MO          | 1          | 5512               | 0         | 4078              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-04 | 2000-01-04 | 2022 | 1     | 4   | 2          | DI          | 1          | 5513               | 0         | 4078              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-05 | 2000-01-05 | 2022 | 1     | 5   | 3          | MI          | 1          | 5514               | 0         | 4078              | 0              | 1              |                     | Weihnachtsferien    |
| 2022-01-06 | 2000-01-06 | 2022 | 1     | 6   | 4          | DO          | 0          | 5514               | 0         | 4078              | 1              | 1              | Heilige 3 Könige    | Weihnachtsferien    |
| 2022-01-07 | 2000-01-07 | 2022 | 1     | 7   | 5          | FR          | 1          | 5515               | 1         | 4079              | 0              | 0              |                     |                     |
| 2022-01-08 | 2000-01-08 | 2022 | 1     | 8   | 6          | SA          | 0          | 5515               | 0         | 4079              | 0              | 0              |                     |                     |
| 2022-01-09 | 2000-01-09 | 2022 | 1     | 9   | 7          | SO          | 0          | 5515               | 0         | 4079              | 0              | 0              |                     |                     |
| 2022-01-10 | 2000-01-10 | 2022 | 1     | 10  | 1          | MO          | 1          | 5516               | 1         | 4080              | 0              | 0              |                     |                     |
| 2022-01-11 | 2000-01-11 | 2022 | 1     | 11  | 2          | DI          | 1          | 5517               | 1         | 4081              | 0              | 0              |                     |                     |
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
    Date     DATE  NOT NULL,
    Date2000 DATE NOT NULL,
    Year  SMALLINT NOT NULL,
    Month TINYINT NOT NULL,
    Day   TINYINT NOT NULL,
    WeekdayNr  INTEGER NOT NULL,
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
WHERE Year < 2100
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
