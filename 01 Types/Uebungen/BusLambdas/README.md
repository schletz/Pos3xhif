# BusLambdas: Lambdas schreiben und verwenden

In Vorarlberg fährt ein automatisierter Linienbus. Die Leitstelle wertet die Fahrten eines Tages
aus: Welche Fahrten waren verspätet? Wie viele Fahrgäste hatte eine Linie? Welche Busse warten noch
auf die Abfahrt? Du schreibst dafür kleine Funktionen, die **Lambdas** bekommen oder liefern. Eine
Datenbank brauchst du hier nicht, die Fahrten liegen in einer Liste. Alle Daten sind erfunden.

## Der Record Trip

```c#
public record Trip(
    int Id, string LineNumber, string LicensePlate, DateTime PlannedDeparture,
    DateTime? ActualDeparture, int? PassengerCount, bool IsCancelled);
```

| Property | Bedeutung |
|---|---|
| `LineNumber` | Die Nummer der Linie, z. B. `A1`. |
| `LicensePlate` | Das Kennzeichen des Busses. |
| `PlannedDeparture` | Die geplante Abfahrt. |
| `ActualDeparture` | Die tatsächliche Abfahrt. `null`, solange der Bus nicht abgefahren ist. |
| `PassengerCount` | Die Anzahl der Fahrgäste. `null`, wenn niemand gezählt hat. |
| `IsCancelled` | `true`, wenn die Fahrt storniert wurde. |

Die Testdaten stehen in *BusLambdas.Application/TripData.cs*. Sieh sie dir an, bevor du beginnst.

## Aufbau der Solution

Öffne die Solution *BusLambdas.sln*. Du programmierst in den Dateien *TripListFunctions.cs*,
*TripRules.cs* und *TripReport.cs*. Die Dateien *Program.cs*, *TestHelpers.cs* und *TripData.cs*
darfst du nicht ändern, ebenso die öffentlichen Signaturen.

## Teil A: Methoden, die eine Lambda bekommen

Implementiere in *TripListFunctions.cs* die Methoden `Filter`, `Map`, `ForEach`, `Exists`,
`Count`, `FindFirst` und `Sum`. Was sie tun sollen, steht im Kommentar über jeder Methode.

**Verwende in Teil A kein LINQ** (kein `Where`, `Select`, `Any` ...). Schreibe eine
`foreach`-Schleife und rufe den übergebenen Parameter selbst auf. So siehst du, was in LINQ
passiert. Das Testprogramm ruft deine Methoden mit Lambdas auf, z. B.:

```c#
var a2Trips = TripListFunctions.Filter(trips, t => t.LineNumber == "A2");
```

## Teil B: Lambdas schreiben

In *TripRules.cs* stehen Properties und Methoden, die eine Funktion **liefern**. Ersetze jedes
`throw new NotImplementedException()` durch eine Lambda. Ein Beispiel, das nicht in der Aufgabe
vorkommt:

```c#
public static Func<Trip, bool> HasManyPassengers => t => t.PassengerCount > 20;
```

Das erste `=>` gehört zum Property (expression-bodied member). Das zweite `=>` ist der Pfeil der
Lambda. Beachte die Datentypen: Bei `Func<Trip, int>` bekommt die Lambda einen `Trip` und liefert
einen `int`. Bei `Func<Trip, Trip, bool>` bekommt sie zwei Trips.

| Member | Was die Lambda liefert |
|---|---|
| `IsCancelled` | `true`, wenn die Fahrt storniert ist. |
| `HasDeparted` | `true`, wenn `ActualDeparture` nicht `null` ist. |
| `IsDelayed` | `true`, wenn der Bus **mehr als 2 Minuten** nach der geplanten Abfahrt abgefahren ist. Eine Fahrt ohne Abfahrt ist nicht verspätet. |
| `PassengersOrZero` | Die Anzahl der Fahrgäste, 0 wenn niemand gezählt hat. |
| `Label` | Einen Text wie `A1 07:30`: Liniennummer, Leerzeichen, geplante Abfahrt im Format `HH:mm`. |
| `Cancel` | Eine **Kopie** der Fahrt mit `IsCancelled = true`. Records sind unveränderlich, verwende `with`. |
| `SameVehicle` | `true`, wenn beide Fahrten dasselbe Kennzeichen haben. |
| `IsOnLine(lineNumber)` | Eine Lambda, die `true` liefert, wenn die Fahrt zur Linie `lineNumber` gehört. |
| `LogTo(log)` | Eine `Action`, die das `Label` der Fahrt in die Liste `log` schreibt. |

Bei `IsOnLine` und `LogTo` verwendet die Lambda einen Parameter der umgebenden Methode. Die Lambda
„merkt“ sich diesen Wert, auch wenn sie erst später aufgerufen wird.

## Teil C: Alles zusammen

Implementiere in *TripReport.cs* die drei Methoden. Verwende dafür die Methoden aus Teil A und die
Regeln aus Teil B. Jede Methode passt in eine einzige `return`-Anweisung.

| Methode | Ergebnis |
|---|---|
| `GetLabelsOfDelayedTrips(trips)` | Die Labels aller verspäteten Fahrten. |
| `CountPassengersOnLine(trips, lineNumber)` | Die Summe der Fahrgäste aller Fahrten der Linie. Nicht gezählte Fahrten zählen als 0. |
| `GetLicensePlatesOfWaitingTrips(trips)` | Die Kennzeichen aller Fahrten, die weder storniert noch abgefahren sind. |

## Testen

*Program.cs* ist ein Testprogramm. Starte es im Ordner *BusLambdas.Application* mit

```text
dotnet run
```

oder mit *F5* in Visual Studio. Es zeigt dir bei jeder Prüfung *OK* oder *FAILED*. Wirft eine
Prüfung eine Exception, gilt sie als nicht erfüllt. Am Anfang sind daher alle Prüfungen
*FAILED*. Die Prüfungen sind nach den Teilen A, B und C gruppiert. Arbeite sie der Reihe nach ab.

Eine vollständige Lösung liefert folgende Ausgabe:

```text
Checking part A: TripListFunctions
   (1) OK: Filter returns a new list with the matching trips in the same order.
   (2) OK: Map returns the projected value of every trip in the same order.
   (3) OK: ForEach calls the action for every trip.
   (4) OK: Exists returns true if at least one trip matches.
   (5) OK: Count returns the number of matching trips.
   (6) OK: FindFirst returns the first matching trip or null.
   (7) OK: Sum adds up the selected values.
Checking part B: TripRules
   (8) OK: IsCancelled is true only for cancelled trips.
   (9) OK: HasDeparted is true if ActualDeparture is set.
   (10) OK: IsDelayed is true for a delay of more than 2 minutes.
   (11) OK: PassengersOrZero returns 0 if nobody counted.
   (12) OK: Label returns line number and planned departure (HH:mm).
   (13) OK: Cancel returns a cancelled copy and does not change the original trip.
   (14) OK: SameVehicle compares the license plates of two trips.
   (15) OK: IsOnLine returns a rule for the given line.
   (16) OK: LogTo returns an action that adds the label to the list.
Checking part C: TripReport
   (17) OK: GetLabelsOfDelayedTrips returns the labels of the delayed trips.
   (18) OK: CountPassengersOnLine adds up the passengers of a line (not counted = 0).
   (19) OK: GetLicensePlatesOfWaitingTrips returns the vehicles of trips that are neither cancelled nor departed.

Result of the program in C:\...\BusLambdas.Application:
19 of 19 checks passed.
25 of 25 points. Grade: Sehr gut.
```
