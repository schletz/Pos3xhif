# Fleet: Fuhrpark mit Elektroautos

Eine Firma verwaltet ihre Dienstautos mit einem kleinen Programm. Es berechnet die Energiekosten,
tankt vor langen Fahrten nach und erstellt einen kurzen Bericht. Als das Programm entstand, hatte
die Firma nur Benzin- und Dieselautos. Deshalb steckt alles rund um den Tank in der Basisklasse
`Vehicle`. Dann kamen Elektroautos dazu. Jemand hat `ElectricCar` von `Vehicle` abgeleitet, die
Methode `Refuel()` mit einer `NotSupportedException` überschrieben und im `FleetService` überall
`is ElectricCar` eingebaut.

Der Code funktioniert, und alle Tests sind grün. Er verletzt aber das Liskov Substitution
Principle. Wer ein `ElectricCar` als `Vehicle` bekommt und `Refuel()` aufruft, stürzt ab. Wer ein
neues Fahrzeug ergänzt, muss jede Typprüfung im `FleetService` finden. Deine Aufgabe ist es, den
Code umzubauen, ohne sein Verhalten zu ändern.

## Aufbau der Solution

Öffne die Solution *Fleet.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *Fleet.Application* | `Vehicle`, `CombustionCar`, `ElectricCar`, `EnergyPrices` und `FleetService`. Diesen Code baust du um. |
| *Fleet.Tests* | Tests für die Fahrzeuge und den `FleetService`. Diese Tests **änderst du nicht**. |

## Fachliche Regeln

Die Regeln gelten vor und nach deinem Umbau gleich. Alle Beträge sind in Euro.

| Thema | Verbrennerauto (`CombustionCar`) | Elektroauto (`ElectricCar`) |
|---|---|---|
| Energiespeicher | Tank in Litern | Akku in kWh |
| Verbrauch | Liter pro 100 km | kWh pro 100 km |
| Nachfüllen | `Refuel(liters)` | `Charge(kWh)` |
| Preis | `EnergyPrices.FuelPerLiter` | `EnergyPrices.ElectricityPerKWh` |

1. **Reichweite** = Energie im Speicher / Verbrauch pro 100 km × 100.
   Beispiel: 26 l bei 6,5 l/100 km ergeben 400 km.
2. **Kosten pro 100 km** = Verbrauch pro 100 km × Preis pro Einheit.
   Beispiel: 6,5 l × 1,60 € = 10,40 €. 15 kWh × 0,30 € = 4,50 €.
3. **Monatskosten** (`CalculateMonthlyCost(kmPerVehicle)`): Jedes Fahrzeug fährt
   `kmPerVehicle`. Die Kosten aller Fahrzeuge werden addiert.
4. **Fahrt vorbereiten** (`PrepareForTrip(licensePlate, distanceKm)`): Reicht die Reichweite für
   die Strecke, passiert nichts, und die Methode liefert 0. Sonst wird der Tank voll getankt bzw.
   der Akku voll geladen. Die Methode liefert die Kosten dafür.
5. **Fahrt eintragen** (`RecordTrip(licensePlate, distanceKm)`): Das Fahrzeug verbraucht Energie
   für die Strecke. Reicht sie nicht, wirft die Methode eine `InvalidOperationException`.
6. **Bericht** (`CreateReport()`): eine Zeile pro Fahrzeug, zum Beispiel
   `W-1001A: range 400 km, 10.40 EUR/100 km`.
7. Ein unbekanntes Kennzeichen oder ein doppeltes Kennzeichen beim Hinzufügen führt zu einer
   `ArgumentException`. Füllstände außerhalb von 0 bis zur Kapazität führen zu einer
   `ArgumentOutOfRangeException`.

## Zieldesign

Nach dem Umbau gibt es **keine** Typprüfung (`is`, `as`, Cast, `GetType()`) und **keine**
`NotSupportedException` mehr. Jedes Fahrzeug kann überall verwendet werden, wo ein `Vehicle`
erwartet wird.

**`Vehicle`** (abstrakte Klasse): enthält nur noch, was *jedes* Fahrzeug hat und kann.

| Member | Zuständigkeit |
|---|---|
| `protected Vehicle(string licensePlate)` | Konstruktor, speichert nur das Kennzeichen. |
| `public string LicensePlate { get; }` | Das Kennzeichen. |
| `public abstract decimal RangeKm { get; }` | Reichweite in km mit der aktuellen Energie (Regel 1). |
| `public abstract decimal CalculateCostPer100Km(EnergyPrices prices)` | Kosten pro 100 km (Regel 2). |
| `public abstract decimal RestoreFullRange(EnergyPrices prices)` | Tank füllen bzw. Akku voll laden. Liefert die Kosten dafür. |
| `public abstract void Drive(decimal distanceKm)` | Energie für die Strecke verbrauchen (Regel 5). |

`Vehicle` hat **kein** `Refuel()`, keinen Tank und keinen Verbrauch in Litern mehr.

**`CombustionCar`** (erbt von `Vehicle`): bekommt alles rund um den Tank, das bisher in `Vehicle`
steht: `TankCapacityLiters`, `ConsumptionLitersPer100Km`, `FuelLevelLiters` und
`Refuel(decimal liters)`. Es implementiert die vier abstrakten Member von `Vehicle`.

**`ElectricCar`** (erbt von `Vehicle`): ruft nur noch `base(licensePlate)` auf. Es hat **kein**
`Refuel()` mehr. Es implementiert die vier abstrakten Member von `Vehicle` mit seinen
Akku-Werten.

**`FleetService`**: verwendet nur noch die Member von `Vehicle`. Die Namen `ElectricCar` und
`CombustionCar` kommen darin nicht mehr vor. Die privaten Hilfsmethoden `GetRangeKm()` und
`GetCostPer100Km()` fallen weg.

Die öffentlichen Konstruktoren von `CombustionCar` und `ElectricCar`, ihre Properties, `Refuel()`,
`Charge()` und alle öffentlichen Methoden von `FleetService` behalten Namen und Parameter. Die
Tests verwenden sie.

## Aufgabe

Arbeite in kleinen Schritten und führe nach jedem Schritt `dotnet test` aus.

1. Führe die Tests aus. Alle sind grün. Markiere im `FleetService` jede Stelle mit
   `is ElectricCar`. Lies auch die Konstruktoren: Welche Werte übergibt `ElectricCar` an `base`?
2. Ergänze die vier abstrakten Member in `Vehicle` und implementiere sie in `CombustionCar` und
   `ElectricCar`. Die alten Member bleiben vorerst stehen.
3. Stelle den `FleetService` auf die neuen Member um. Entferne alle Typprüfungen und die beiden
   privaten Hilfsmethoden.
4. Verschiebe Tank, Verbrauch in Litern und `Refuel()` von `Vehicle` nach `CombustionCar`.
   Entferne das Überschreiben von `Refuel()` in `ElectricCar`. Der Konstruktor von `Vehicle`
   bekommt nur noch das Kennzeichen.
5. Prüfe dein Ergebnis: Suche in *Fleet.Application* nach `is `, `NotSupportedException`,
   `virtual` und `ElectricCar`. Jeder Treffer, der noch übrig ist, muss begründet sein.

## Regeln für die Arbeit

- Ändere die Tests nicht. Sie sind dein Sicherheitsnetz: Sie zeigen, dass dein Umbau das
  Verhalten nicht ändert.
- Halte dich an das Zieldesign. Öffentliche Namen und Signaturen, die die Tests verwenden,
  bleiben gleich.
- Ergänze keine neuen Funktionen. Ein Refactoring ändert nur die Struktur.
- Jede öffentliche Klasse und jeder öffentliche Member behält bzw. bekommt einen XML-Kommentar.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.

**Zum Nachdenken:** Die Firma kauft ein Plug-in-Hybridauto. Es hat einen Tank *und* einen Akku.
Passt es in dein Design? Welche Fragen musst du der Firma stellen, bevor du die Klasse schreibst?
