# PowerbankCheck: Grenzwerte an genau einer Stelle

Überhitzte Powerbanks führen immer öfter zu Notlandungen und Umleitungen von Flügen (derStandard,
Oktober 2026). Eine Airline lässt deshalb am Gate das Gepäck mit einem Programm prüfen. Die Klasse
`BaggageCheck` funktioniert, und alle Tests sind grün. Trotzdem hat der Code ein Problem: Die
Grenzwerte und die Liste der verbotenen Gegenstände stehen als Literale an vielen Stellen. Ändert
die Airline eine Regel, muss jemand alle Stellen finden. Deine Aufgabe ist es, den Code so
umzubauen, dass jeder Wert und jede Regel an **genau einer Stelle** steht.

Die Theorie dazu steht im Kapitel
*O, Teil 1: Werte und Regeln an genau einer Stelle* im Kurs.

## Aufbau der Solution

Öffne die Solution *PowerbankCheck.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *PowerbankCheck.Application* | `Powerbank`, `Bag`, `Passenger`, `CheckResult` und `BaggageCheck`. Diesen Code baust du um. |
| *PowerbankCheck.Tests* | Die Tests für `BaggageCheck`. Sie sind grün und dürfen **nicht geändert** werden. |

- Ein `Passenger` hat zwei Gepäckstücke (`Bag`): `CarryOn` (Handgepäck) und `CheckedBag`
  (Aufgabegepäck).
- Jede `Bag` hat eine Liste `Powerbanks` und eine Liste `Items` mit anderen Gegenständen, zum
  Beispiel `"laptop"`.
- `BaggageCheck` prüft das Handgepäck (`CheckCarryOn`) und das Aufgabegepäck (`CheckCheckedBag`),
  zählt die nötigen Genehmigungen (`CountRequiredApprovals`) und erstellt einen Bericht für das
  Personal am Gate (`CreateReport`).

## Die fachlichen Regeln

Die Grenzen 100 Wh und 160 Wh verwenden viele Airlines. Die anderen Regeln sind für diese Übung
erfunden.

1. Die Energie einer Powerbank in **Wattstunden (Wh)** ist Kapazität in mAh × Spannung in V / 1000.
   Beispiel: 20000 mAh × 3,7 V / 1000 = 74 Wh.
2. Powerbanks sind **nur im Handgepäck** erlaubt. Im Aufgabegepäck ist jede Powerbank verboten.
3. Im Handgepäck gilt:
   - **bis 100 Wh** (einschließlich): erlaubt.
   - **über 100 Wh bis 160 Wh** (einschließlich): nur mit Genehmigung der Airline.
   - **über 160 Wh**: verboten.
4. Im Handgepäck sind **höchstens 2 Powerbanks** erlaubt.
5. Die Gegenstände *fireworks*, *camping gas* und *lighter fluid* sind in beiden Gepäckstücken
   verboten. Groß- und Kleinschreibung spielt keine Rolle.
6. Das Ergebnis einer Prüfung ist `Forbidden`, wenn eine Regel verletzt ist. Sonst ist es
   `ApprovalRequired`, wenn eine Powerbank eine Genehmigung braucht. Sonst ist es `Allowed`.

## Aufgabe

Führe zuerst `dotnet test` aus. Alle Tests sind grün. Baue dann den Code in diesen Schritten um.
Führe nach jedem Schritt `dotnet test` aus.

1. **Grenzwerte benennen:** Lege die statische Klasse `BaggageRules` (Datei *BaggageRules.cs*) an.
   Sie enthält alle Grenzwerte der Airline an genau einer Stelle:

   | Member | Typ | Bedeutung |
   |---|---|---|
   | `ApprovalLimitWattHours` | `public const decimal` | Über diesem Wert braucht eine Powerbank eine Genehmigung (100 Wh). |
   | `MaxWattHours` | `public const decimal` | Über diesem Wert ist eine Powerbank verboten (160 Wh). |
   | `MaxPowerbanksInCarryOn` | `public const int` | So viele Powerbanks sind im Handgepäck höchstens erlaubt (2). |
   | `ForbiddenItems` | `public static readonly IReadOnlySet<string>` | Die verbotenen Gegenstände. Ein `HashSet<string>` mit `StringComparer.OrdinalIgnoreCase` vergleicht ohne Rücksicht auf Groß- und Kleinschreibung. |

2. **Properties in `Powerbank`:** Die Powerbank beantwortet die Fragen zu ihren Daten selbst. Ergänze
   diese berechneten, schreibgeschützten Properties:

   | Property | Typ | Bedeutung |
   |---|---|---|
   | `WattHours` | `decimal` | Die Energie nach Regel 1. Die Zahl 1000 bekommt dafür eine `private const` mit dem Namen `MilliampHoursPerAmpHour`. |
   | `RequiresApproval` | `bool` | `true`, wenn die Powerbank im Handgepäck eine Genehmigung braucht (Regel 3). |
   | `IsTooPowerful` | `bool` | `true`, wenn die Powerbank über `MaxWattHours` liegt. |

3. **Properties in `Bag` und `Passenger`:**

   | Klasse | Property | Typ | Bedeutung |
   |---|---|---|---|
   | `Bag` | `ContainsForbiddenItem` | `bool` | `true`, wenn mindestens ein Gegenstand in `Items` in `BaggageRules.ForbiddenItems` steht. |
   | `Passenger` | `HasTooManyPowerbanks` | `bool` | `true`, wenn im Handgepäck mehr als `MaxPowerbanksInCarryOn` Powerbanks sind. |

4. **`BaggageCheck` aufräumen:** Ersetze in allen Methoden die Bedingungen durch die neuen
   Properties und Konstanten. Am Ende enthält `BaggageCheck` keine Zahl 100, 160, 1000 oder 2 und
   keinen Namen eines verbotenen Gegenstands mehr. Auch der Text `max. 2` im Bericht verwendet
   die Konstante.

5. **Kontrolle:** Suche in der ganzen Solution nach `100`, `160`, `1000` und `"fireworks"`. Jeder
   Wert darf in *PowerbankCheck.Application* nur noch an **einer** Stelle stehen.

6. **Zum Nachdenken:** Die Airline senkt die Grenze von 160 Wh auf 150 Wh. Wie viele Stellen
   musst du jetzt ändern? Wie viele Stellen wären es im ursprünglichen Code gewesen? Welche
   Stelle hättest du dort am ehesten vergessen?

Regeln für die Arbeit:

- Ändere die Tests nicht. Die öffentlichen Klassen, Konstruktoren, Methoden und Properties, die
  die Tests verwenden, bleiben gleich.
- Ändere kein Verhalten. Das ist ein Refactoring. Wird ein Test rot, hast du einen Fehler
  eingebaut.
- Die Literale in den Tests bleiben. In Tests sollen die erwarteten Werte konkret und von Hand
  nachrechenbar sein.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.
