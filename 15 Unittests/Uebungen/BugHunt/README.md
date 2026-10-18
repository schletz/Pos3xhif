# BugHunt: Fehler finden mit Tests

Eine Parkgarage berechnet die Parkgebühr mit der Klasse `ParkingFeeCalculator`. Die Klasse ist
fertig programmiert, und der vorhandene Test ist grün. Trotzdem beschweren sich Kunden über
falsche Beträge. Im Code stecken **genau 4 Fehler**. Deine Aufgabe ist es, sie mit Unit Tests
nachzuweisen.

## Aufbau der Solution

Öffne die Solution *BugHunt.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *BugHunt.Application* | Die Klasse `ParkingFeeCalculator`. Diesen Code darfst du **nicht ändern**. |
| *BugHunt.Tests* | Ein Beispieltest. Hier schreibst du deine Tests. |

## Die Tarifregeln

`ParkingFeeCalculator.Calculate(DateTime entry, DateTime exit)` liefert die Gebühr in Euro.
`entry` ist der Zeitpunkt der Einfahrt, `exit` der Zeitpunkt der Ausfahrt.

1. Parkt ein Auto **höchstens 30 Minuten**, ist das Parken gratis (0 €).
2. Parkt es **länger als 30 Minuten**, kostet **jede begonnene Stunde 2,50 €**. Die 30 Minuten
   Gratiszeit werden dabei nicht abgezogen. Beispiel: 2 Stunden 10 Minuten sind 3 begonnene
   Stunden, das kostet 7,50 €.
3. Für **24 Stunden** zahlt man **höchstens 20 €**. Bei längerem Parken kostet jeder volle Block
   von 24 Stunden 20 €. Die restliche Zeit wird nach Regel 2 berechnet, ebenfalls mit höchstens
   20 €. Die Gratiszeit aus Regel 1 gilt nur, wenn die gesamte Parkdauer höchstens 30 Minuten
   beträgt.
4. Liegt die Ausfahrt **vor** der Einfahrt, wirft die Methode eine `ArgumentException`. Gleiche
   Zeitpunkte sind erlaubt (Regel 1).

Zeitpunkte erzeugst du mit `new DateTime(2026, 10, 12, 8, 0, 0)` (Jahr, Monat, Tag, Stunde,
Minute, Sekunde). Mit `entry.AddMinutes(30)` oder `entry.AddHours(24)` berechnest du davon
ausgehend die Ausfahrt.

## Aufgabe

1. Stelle eine Tabelle mit allen **Äquivalenzklassen** und **Grenzwerten** auf. Schreibe zu jedem
   Fall das erwartete Ergebnis, das du aus den Tarifregeln von Hand berechnest.
2. Schreibe für jeden Fall einen Test in *BugHunt.Tests*. Verwende `[Theory]` und `[InlineData]`,
   wo mehrere Fälle dasselbe Verhalten prüfen. Für Regel 4 brauchst du `Assert.Throws`.
3. Führe die Tests aus. Für **jeden der 4 Fehler** muss mindestens ein Test rot sein.
4. Schreibe unter jeden roten Test einen Kommentar: Welche Regel wird verletzt, und was liefert
   der Code stattdessen?

Regeln für die Arbeit:

- Ändere den Code in *BugHunt.Application* nicht und verwende keinen Debugger. Du findest die
  Fehler nur über Tests.
- Benenne die Tests nach dem Muster `Calculate_Situation_ErwartetesErgebnis`. Ein Test prüft ein
  Verhalten. Dann zeigt schon die Liste der roten Tests, was kaputt ist.
- Grüne Tests sind nicht umsonst. Sie halten fest, was schon funktioniert, und schützen später
  vor Regressionen.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.

**Zusatzaufgabe:** Wenn deine Tests alle 4 Fehler zeigen, darfst du den Code reparieren. Du bist
fertig, wenn alle Tests grün sind.
