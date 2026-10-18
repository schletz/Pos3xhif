# BusService: Eine Serviceklasse mit DTOs und Geschäftsregeln

In Vorarlberg fährt ein automatisierter Linienbus. Er lenkt selbst, aber eine
**Sicherheitsfahrerin** oder ein **Sicherheitsfahrer** (Safety Operator) sitzt immer an Bord. Wird
es kritisch, übernimmt diese Person die Steuerung. Das nennen wir einen **Eingriff**
(Intervention).

Die Leitstelle braucht eine Serviceklasse `DispatchService`. Sie liefert Daten für die Anzeige als
**DTOs** und ändert die Datenbank: Sie plant Fahrten, speichert Abfahrten und Eingriffe und
storniert Fahrten. Dabei prüft sie Geschäftsregeln. Alle Daten sind erfunden.

## Das Modell

![Klassenmodell](bus_model.svg)

| Klasse | Bedeutung |
|---|---|
| `Line` | Eine Buslinie, z. B. *A1 Bahnhof - FH Campus*. |
| `Stop` | Eine Haltestelle. Mehrere Linien können dieselbe Haltestelle anfahren. |
| `LineStop` | Eine Haltestelle einer Linie. `Position` 1 ist die erste Haltestelle. `MinutesFromStart` ist die Fahrzeit ab der ersten Haltestelle. |
| `Vehicle` | Ein Bus. `IsAutonomous` ist `true` bei automatisierten Bussen und `false` bei normalen Bussen mit Fahrer. |
| `SafetyOperator` | Eine Person, die automatisierte Busse überwacht. Die Ausbildung ist bis einschließlich `CertifiedUntil` gültig. |
| `Trip` | Eine Fahrt einer Linie. `ActualDeparture` ist `null`, solange der Bus nicht abgefahren ist. `PassengerCount` ist `null`, wenn niemand gezählt hat. Normale Busse haben keinen `SafetyOperator` (`null`). |
| `Intervention` | Ein Eingriff während einer Fahrt. `Stop` ist `null`, wenn der Eingriff zwischen zwei Haltestellen war. |

Die Musterdaten umfassen eine Woche: Montag, 5. Oktober, bis Freitag, 9. Oktober 2026. Die Fahrten
15 und 16 am Freitag sind geplant, aber noch nicht abgefahren. Die Fahrten 7 und 14 sind storniert.

## Aufbau der Solution

Öffne die Solution *BusService.sln*.

| Projekt / Datei | Inhalt |
|---|---|
| *BusService.Application/Model* | Die Entity-Klassen. Nicht ändern. |
| *BusService.Application/Infrastructure/BusContext.cs* | Der DbContext mit der Methode `Seed()` für die Musterdaten. Nicht ändern. |
| *BusService.Application/Dtos* | Leere Records für die DTOs. **Hier ergänzt du die Properties.** |
| *BusService.Application/Services/DispatchService.cs* | **Hier programmierst du die Methoden.** |
| *BusService.Application/Services/DispatchServiceException.cs* | Die Exception für verletzte Geschäftsregeln. |
| *BusService.Tests* | Die Tests. Nicht ändern. |
| *bus.db* | Die Musterdaten als SQLite-Datenbank zum Ansehen mit DBeaver oder DataGrip. |

## Teil 1: DTOs und lesende Methoden

Die Records in *Dtos* sind noch leer, z. B. `public record LineDto();`. Ergänze sie als
**positional records** mit genau diesen Properties. Die Tests lesen die Properties über ihren
Namen. Achte daher auf die Schreibweise und die Datentypen.

| Record | Properties |
|---|---|
| `LineDto` | `int Id`, `string Number`, `string Name`, `int StopCount`, `int TripCount` |
| `LineDetailDto` | `string Number`, `string Name`, `List<LineStopDto> Stops` |
| `LineStopDto` | `int Position`, `string StopName`, `int MinutesFromStart` |
| `TripDetailDto` | `int TripId`, `string LineNumber`, `string LicensePlate`, `string? OperatorName`, `DateTime PlannedDeparture`, `DateTime? ActualDeparture`, `bool IsCancelled`, `List<InterventionDto> Interventions` |
| `InterventionDto` | `DateTime Time`, `string? StopName`, `InterventionReason Reason`, `int DurationSeconds` |

### List\<LineDto\> GetLines()

Liefert alle Linien, sortiert nach der Nummer. `StopCount` ist die Anzahl der Haltestellen der
Linie, `TripCount` die Anzahl ihrer **nicht stornierten** Fahrten. Auch Linien ohne Fahrten sind
im Ergebnis.

### LineDetailDto? GetLineDetail(string lineNumber)

Liefert die Linie mit der Nummer `lineNumber` und ihre Haltestellen, sortiert nach der Position.
Gibt es die Linie nicht, liefert die Methode `null`.

### TripDetailDto? GetTripDetail(int tripId)

Liefert die Fahrt mit allen Eingriffen, sortiert nach der Zeit. `OperatorName` ist Vorname und
Nachname mit einem Leerzeichen dazwischen, z. B. `Lena Moser`, oder `null`, wenn die Fahrt keinen
Safety Operator hat. `StopName` ist `null`, wenn der Eingriff zwischen zwei Haltestellen war. Gibt
es die Fahrt nicht, liefert die Methode `null`.

## Teil 2: Methoden, die die Datenbank ändern

Verletzt ein Aufruf eine Regel, wirft die Methode eine `DispatchServiceException` mit dem
angegebenen Text. Prüfe die Regeln **in der angegebenen Reihenfolge**. Der Text muss genau
stimmen, auch der Punkt am Ende. Wird eine Regel verletzt, darf nichts gespeichert werden.

### Trip ScheduleTrip(int lineId, int vehicleId, int? safetyOperatorId, DateTime plannedDeparture)

Plant eine neue Fahrt und liefert sie zurück. Die neue Fahrt ist noch nicht abgefahren, nicht
storniert und hat keine Fahrgastzählung.

| Nr. | Regel | Text der Exception |
|---|---|---|
| 1 | Die Linie muss existieren. | `Line not found.` |
| 2 | Das Fahrzeug muss existieren. | `Vehicle not found.` |
| 3 | Ein automatisierter Bus braucht einen Safety Operator (`safetyOperatorId` ist nicht `null`). | `An autonomous vehicle needs a safety operator.` |
| 4 | Ein normaler Bus hat keinen Safety Operator (`safetyOperatorId` ist `null`). | `A conventional bus has no safety operator.` |
| 5 | Der Safety Operator muss existieren. | `Safety operator not found.` |
| 6 | Die Ausbildung muss am Tag der Fahrt noch gültig sein. Der Tag `CertifiedUntil` selbst ist noch gültig. | `The certification of the safety operator has expired.` |
| 7 | Das Fahrzeug darf zur selben Zeit keine andere, nicht stornierte Fahrt haben. | `The vehicle is not available.` |

Zu Regel 7: Eine Fahrt dauert so lange wie der größte Wert von `MinutesFromStart` ihrer Linie. Die
Linie A1 dauert z. B. 9 Minuten. Eine Fahrt um 07:00 belegt das Fahrzeug von 07:00 bis 07:09. Zwei
Fahrten überschneiden sich, wenn die eine beginnt, bevor die andere endet, und umgekehrt. Beginnt
eine Fahrt genau um 07:09, ist das also erlaubt.

### void RecordDeparture(int tripId, DateTime actualDeparture)

Speichert die tatsächliche Abfahrt einer Fahrt in `ActualDeparture`.

| Nr. | Regel | Text der Exception |
|---|---|---|
| 1 | Die Fahrt muss existieren. | `Trip not found.` |
| 2 | Die Fahrt darf nicht storniert sein. | `The trip is cancelled.` |
| 3 | Die Fahrt darf noch nicht abgefahren sein. | `The trip has already departed.` |

### Intervention RecordIntervention(int tripId, int? stopId, DateTime time, InterventionReason reason, int durationSeconds)

Speichert einen Eingriff und liefert ihn zurück. `stopId` ist `null`, wenn der Eingriff zwischen
zwei Haltestellen war.

| Nr. | Regel | Text der Exception |
|---|---|---|
| 1 | Die Fahrt muss existieren. | `Trip not found.` |
| 2 | Die Fahrt muss abgefahren sein. | `The trip has not departed yet.` |
| 3 | Ist `stopId` angegeben, muss die Haltestelle zur Linie der Fahrt gehören. | `The stop is not served by this line.` |
| 4 | `durationSeconds` muss größer als 0 sein. | `The duration must be positive.` |

### void CancelTrip(int tripId)

Storniert eine Fahrt. Die Fahrt wird **nicht gelöscht**, sondern `IsCancelled` wird auf `true`
gesetzt.

| Nr. | Regel | Text der Exception |
|---|---|---|
| 1 | Die Fahrt muss existieren. | `Trip not found.` |
| 2 | Die Fahrt darf noch nicht storniert sein. | `The trip is already cancelled.` |
| 3 | Die Fahrt darf noch nicht abgefahren sein. | `The trip has already departed.` |

## Testen

Führe im Ordner der Solution

```text
dotnet test
```

aus oder verwende den Test Explorer. Am Anfang sind alle Tests rot. Ändere die Tests und die
öffentlichen Signaturen nicht. Die Tests berechnen die erwarteten Werte mit SQL direkt aus der
Datenbank. Die Meldung eines roten Tests sagt dir, welcher Fall nicht stimmt.
