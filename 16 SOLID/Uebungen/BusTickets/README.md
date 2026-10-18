# BusTickets: Erweitern statt ändern

Im Oktober 2026 ist in Wolfurt (Vorarlberg) der erste voll automatisierte Linienbus Österreichs
gestartet. Er fährt eine Strecke von 4 km mit 7 Haltestellen. Die Fahrt ist vorerst kostenlos.

Für diese Übung stellen wir uns vor: Später verkauft ein Automat im Bus Fahrkarten. Die Software
dafür ist schon programmiert. **Die Kartentypen und Tarife in dieser Übung sind erfunden.** Sie
haben nichts mit dem echten Betrieb zu tun.

Der Code funktioniert, und alle Tests sind grün. Aber die Klasse `Ticket` fragt an vier Stellen
mit `switch` oder `if`/`else` ab, welcher Kartentyp es ist. Bald kommt eine Wochenkarte dazu. Dann
müsstest du alle vier Stellen finden und ändern. Du baust den Code deshalb so um, dass ein neuer
Kartentyp eine **neue Klasse** ist und der bestehende Code unverändert bleibt
(*Open/Closed Principle*).

## Aufbau der Solution

Öffne die Solution *BusTickets.sln* in diesem Ordner.

| Projekt | Datei | Inhalt |
|---|---|---|
| *BusTickets.Application* | *TicketType.cs* | Das Enum mit den Kartentypen. |
| | *Ticket.cs* | Eine Fahrkarte. Preis, Gültigkeit und Text hängen vom Kartentyp ab. |
| | *TicketMachine.cs* | Der Automat. Er verkauft Karten (`Buy`) und summiert die Einnahmen. |
| *BusTickets.Tests* | *TicketTests.cs*, *TicketMachineTests.cs* | Die Regressionstests. Sie sind grün und müssen grün bleiben. |
| | *ExtensionTests.cs* | Die Tests für Schritt 2. Sie sind zuerst ausgeschaltet. |

## Die Tarifregeln

| Kartentyp | `TicketType` | Preis | Ermäßigt | Gültig ab dem Kauf bis ... | `Description` (Beispiel) |
|---|---|---|---|---|---|
| Einzelfahrt | `SingleRide` | 2,40 € | 1,20 € | 60 Minuten nach dem Kauf | `Single ride, valid for 60 minutes` |
| Tageskarte | `DayPass` | 5,60 € | 2,80 € | Mitternacht am Ende des Kauftags | `Day pass for 2026-10-17` |
| Monatskarte | `MonthlyPass` | 42,00 € | 25,00 € | Mitternacht am Ende des Kaufmonats | `Monthly pass for 2026-10` |

- Den ermäßigten Preis zahlen Kinder und Senioren. Der Automat bekommt dafür `isReduced: true`.
- `ExpiresAt` ist der **erste Zeitpunkt, an dem die Karte nicht mehr gilt**. Beispiel: Eine
  Einzelfahrt wird um 8:15 gekauft. `ExpiresAt` ist 9:15. Um 9:14 gilt sie noch, um 9:15 nicht
  mehr. Eine Monatskarte vom 17.10.2026 hat `ExpiresAt` 1.11.2026, 0:00.
- `IsValidAt(time)` liefert `true`, wenn `PurchasedAt <= time` und `time < ExpiresAt` ist.
- Die Datumsangaben in `Description` haben das Format `yyyy-MM-dd` bzw. `yyyy-MM`.

## Aufgabe

### Schritt 1: Eine Klasse pro Kartentyp

Baue die Klasse `Ticket` in eine abstrakte Basisklasse mit drei Unterklassen um. Am Ende gibt es
genau **einen** `switch` über `TicketType`: in `TicketMachine.Buy`. Er erzeugt nur das passende
Objekt.

| Typ | Art | Member |
|---|---|---|
| `Ticket` | `public abstract class` | `protected Ticket(DateTime purchasedAt, bool isReduced)` |
| | | `PurchasedAt` und `IsReduced` wie bisher |
| | | `public abstract decimal Price { get; }` |
| | | `public abstract DateTime ExpiresAt { get; }` |
| | | `public abstract string Description { get; }` |
| | | `public bool IsValidAt(DateTime time)`: nur **einmal** in `Ticket` implementiert, ohne `switch`. Die Methode verwendet `ExpiresAt`. |
| `SingleRideTicket` | `public class`, erbt von `Ticket` | `public SingleRideTicket(DateTime purchasedAt, bool isReduced)`, überschreibt `Price`, `ExpiresAt` und `Description` mit den Regeln der Einzelfahrt. |
| `DayPassTicket` | `public class`, erbt von `Ticket` | wie oben, mit den Regeln der Tageskarte |
| `MonthlyPassTicket` | `public class`, erbt von `Ticket` | wie oben, mit den Regeln der Monatskarte |
| `TicketMachine` | bleibt | `Buy` erzeugt mit einem `switch` über `type` das passende Objekt. Der Rest bleibt gleich. |
| `TicketType` | bleibt | unverändert |

Das Property `Type` und der Konstruktor `Ticket(TicketType type, ...)` fallen weg. Die Tests
verwenden sie nicht.

So gehst du vor:

1. Lege die drei Unterklassen an, jede in einer eigenen Datei. Kopiere die Regeln aus dem
   passenden Zweig der `switch`- und `if`-Anweisungen in `Ticket`.
2. Ändere `Ticket` wie in der Tabelle. Dabei verschwinden diese Abfragen.
3. Ändere `TicketMachine.Buy`.
4. Führe `dotnet test` aus. Der Code kompiliert erst wieder, wenn alle drei Teile fertig sind.
5. Kontrolle: Suche in *BusTickets.Application* nach `TicketType.`. Der Typ darf nur noch in
   *TicketType.cs* und in `TicketMachine.Buy` vorkommen.

### Schritt 2: Die Wochenkarte

Der Betreiber will eine Wochenkarte verkaufen. Für sie gelten diese (erfundenen) Regeln:

| Kartentyp | `TicketType` | Preis | Ermäßigt | Gültig ab dem Kauf bis ... | `Description` (Beispiel) |
|---|---|---|---|---|---|
| Wochenkarte | `WeeklyPass` | 14,00 € | 7,00 € | Mitternacht am Ende des 7. Tags. Der Kauftag ist Tag 1. | `Weekly pass from 2026-10-12 to 2026-10-18` |

Beispiel: Die Karte wird am Montag, 12.10.2026 um 8:15 gekauft. Sie gilt bis Sonntag, 18.10.2026,
23:59. `ExpiresAt` ist Montag, 19.10.2026, 0:00. Die `Description` nennt den ersten und den
letzten Tag, an dem die Karte gilt.

1. Öffne *BusTickets.Tests.csproj* und entferne in dieser Zeile die Kommentarzeichen `<!--` und
   `-->`:

   ```xml
   <!-- <PropertyGroup><DefineConstants>$(DefineConstants);EXTENSION</DefineConstants></PropertyGroup> -->
   ```

   Danach steht dort:

   ```xml
   <PropertyGroup><DefineConstants>$(DefineConstants);EXTENSION</DefineConstants></PropertyGroup>
   ```

   Damit kompiliert der Compiler auch den Code in *ExtensionTests.cs*. Er steht zwischen
   `#if EXTENSION` und `#endif`.
2. Führe `dotnet test` aus. Das Testprojekt kompiliert nicht, weil es `WeeklyPassTicket` und
   `TicketType.WeeklyPass` noch nicht gibt. Das ist gewollt.
3. Lege die Klasse `WeeklyPassTicket` an: `public class`, erbt von `Ticket`, Konstruktor
   `public WeeklyPassTicket(DateTime purchasedAt, bool isReduced)`.
4. Ergänze in `TicketType` den Wert `WeeklyPass` und in `TicketMachine.Buy` eine Zeile im
   `switch`.
5. Führe `dotnet test` aus, bis alle Tests grün sind.

**Was du in Schritt 2 ändern darfst:** die neue Datei *WeeklyPassTicket.cs*, einen neuen Wert in
`TicketType` und eine neue Zeile im `switch` von `Buy`. `Ticket`, die drei anderen Kartenklassen
und der Rest von `TicketMachine` bleiben **unverändert**.

Warum ist `Buy` die Ausnahme? Irgendwo muss das Programm entscheiden, welche Klasse zur Auswahl
am Automaten passt. Diese Entscheidung steht an genau einer Stelle. Sie erzeugt nur ein Objekt
und enthält keine Tarifregel. Du ergänzt dort eine Zeile und änderst keine bestehende Logik.
Musst du für die Wochenkarte noch etwas anderes ändern, war das Refactoring in Schritt 1 nicht
vollständig.

## Regeln für die Arbeit

- Ändere die Tests nicht. Einzige Ausnahme ist die Zeile in *BusTickets.Tests.csproj* in
  Schritt 2.
- Die öffentlichen Signaturen, die die Tests verwenden, bleiben gleich: `TicketType`,
  `TicketMachine.Buy`, `SoldTickets`, `TotalRevenue` sowie `PurchasedAt`, `IsReduced`, `Price`,
  `ExpiresAt`, `Description` und `IsValidAt` in `Ticket`.
- Jede Klasse kommt in eine eigene Datei. Neue öffentliche Klassen bekommen einen
  XML-Kommentar (`/// <summary>`).
- Führe nach jedem Schritt die Tests aus. Ein roter Test zeigt dir, dass sich das Verhalten
  geändert hat.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.
