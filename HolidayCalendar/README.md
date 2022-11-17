# Österreichische Feiertage und Schulferien

Die Klasse *CalendarYear* berechnet alle österreichischen Feiertage sowie die Schulferien, wie
sie in den Bundesländern Wien und Niederösterreich gelten.

Die Applikation ist ein xUnit Test, da die Klasse zur Verwendung in anderen Programmen entwickelt
wurde (Serviceklasse).

## Generieren eines Kalenderfiles

Soll eine Textdatei mit allen Tagen zwischen dem 1.1.2000 und 31.12.2400 generiert werden, kannst
du den Test direkt im Verzeichnis der *csproj* Datei von der Konsole starten:

```
dotnet test --filter CalendarCalculator.CalendarYearTests.WriteFileTest
```

400 Jahre ist eine volle Periode im gregorianischen Kalender. Das bedeutet, dass die Tage
nach diesem Zyklus wieder auf den selben Wochentag fallen. Der 14.11.2022 hat also den selben
Tag wie der 14.11.2422 oder der 14.11.1622. Für Berechnungen des Mittelwertes wird diese volle
Periode herangezogen.
