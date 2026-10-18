# PraterTickets: Doppelten Code in Methoden auslagern

Der Wiener Wurstelprater feiert 2026 sein 260-Jahr-Jubiläum und wurde ins immaterielle
UNESCO-Kulturerbe aufgenommen. Zum Jubiläum bekommt eine Ticketkasse neue Software. Sie berechnet
die Preise für das Riesenrad, die Hochschaubahn und die Geisterbahn. Die Ticketkasse, alle Preise
und alle Regeln in dieser Übung sind erfunden.

Die Klasse `TicketOffice` funktioniert, und alle Tests sind grün. Trotzdem ist der Code schlecht:
Die Methode `CalculatePrice` ist fast 90 Zeilen lang. Für jedes Fahrgeschäft steht fast derselbe
Block da, nur mit anderen Zahlen. Deine Aufgabe ist es, die gemeinsamen Teile in private Methoden
auszulagern. Das Verhalten darf sich dabei nicht ändern.

## Aufbau der Solution

Öffne die Solution *PraterTickets.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *PraterTickets.Application* | Die Klasse `TicketOffice` und das Enum `Ride`. Hier arbeitest du. |
| *PraterTickets.Tests* | Die Klasse `TicketOfficeTests` mit 39 Testfällen. Diese Datei änderst du **nicht**. |

## Die Tarifregeln

`TicketOffice.CalculatePrice(Ride ride, int age, int heightCm, int rides)` liefert den Gesamtpreis
in Euro für eine Person. `age` ist das Alter in Jahren, `heightCm` die Körpergröße in Zentimetern
und `rides` die Anzahl der Fahrten.

| Fahrgeschäft | Grundpreis | Mindestgröße | Kinder (unter 15) | Senioren (ab 65) | Ab 5 Fahrten |
|---|---|---|---|---|---|
| Riesenrad (`FerrisWheel`) | 12,00 € | keine | −50 % | −30 % | −10 % auf den Gesamtpreis |
| Hochschaubahn (`RollerCoaster`) | 8,00 € | 120 cm | −25 % | kein Rabatt | −20 % auf den Gesamtpreis |
| Geisterbahn (`GhostTrain`) | 6,00 € | 100 cm | −30 % | −30 % | −20 % auf den Gesamtpreis |

Dazu kommen diese Regeln:

1. Kinder unter 3 Jahren fahren mit dem Riesenrad gratis.
2. Ist die Person kleiner als die Mindestgröße, wirft die Methode eine `ArgumentException` mit dem
   Parameternamen `heightCm`.
3. Ungültige Eingaben führen zu einer `ArgumentOutOfRangeException`: ein Alter unter 0 oder über
   120, eine Größe von 0 cm oder weniger und weniger als 1 Fahrt.

Beispiel: Ein 70-jähriger Gast fährt 7-mal mit der Geisterbahn. Eine Fahrt kostet
6,00 € − 30 % = 4,20 €. Für 7 Fahrten sind das 29,40 €. Ab 5 Fahrten gibt es 20 % Rabatt, also
zahlt er 23,52 €.

## Die Tests sind dein Sicherheitsnetz

Alle Tests sind jetzt grün. Sie halten fest, was der Code heute richtig macht. Nach deinem Umbau
müssen sie immer noch grün sein. Solche Tests heißen **Regressionstests**: Sie zeigen sofort,
wenn eine Änderung etwas kaputt macht, das vorher funktioniert hat.

Arbeite deshalb in kleinen Schritten. Nach **jedem** Schritt führst du die Tests aus. Ist ein
Test rot, hast du das Verhalten geändert. Dann korrigierst du den letzten Schritt, bevor du
weitermachst.

## Aufgabe

Lege in `TicketOffice` die folgenden **privaten** Methoden an und verwende sie in
`CalculatePrice`. Die Reihenfolge ist absichtlich gewählt: Du beginnst mit der kleinsten Methode.

| Schritt | Methode | Zuständigkeit |
|---|---|---|
| 1 | `decimal ApplyDiscount(decimal price, decimal discount)` | Zieht einen Rabatt vom Preis ab. `discount` ist ein Anteil, z. B. `0.25m` für 25 %. |
| 2 | `decimal CalculateTotal(decimal pricePerRide, int rides, decimal multiRideDiscount)` | Berechnet den Preis für alle Fahrten. Ab 5 Fahrten zieht sie den Mehrfahrtenrabatt ab. |
| 3 | `decimal ApplyAgeDiscount(decimal price, int age, decimal childDiscount, decimal seniorDiscount)` | Zieht den Kinder- bzw. Seniorenrabatt vom Preis einer Fahrt ab. |
| 4 | `void CheckMinimumHeight(int heightCm, int minimumHeightCm, string rideName)` | Wirft eine `ArgumentException`, wenn die Person kleiner als die Mindestgröße ist. |
| 5 | `void ValidateInput(int age, int heightCm, int rides)` | Prüft die Eingaben und wirft bei ungültigen Werten eine `ArgumentOutOfRangeException`. |

So gehst du bei jedem Schritt vor:

1. Suche im Code alle Stellen, die diese Aufgabe erledigen. Was ist überall gleich? Was ist
   verschieden?
2. Markiere eine der Stellen und lass die IDE die Methode erzeugen: *Extract Method* in Visual
   Studio mit `Ctrl+R, Ctrl+M` oder `Ctrl+.`, in Rider mit `Ctrl+Alt+M`. Gib ihr den Namen und
   die Parameter aus der Tabelle.
3. Ersetze die anderen Stellen durch einen Aufruf der neuen Methode.
4. Führe die Tests aus. Erst wenn alle grün sind, machst du den nächsten Schritt.

Am Ende besteht `CalculatePrice` nur noch aus der Prüfung der Eingaben und einem kurzen Block pro
Fahrgeschäft.

## Messbare Vorgaben

Bevor du beginnst, misst du den Code. Danach misst du noch einmal. Dein Ergebnis muss diese
Grenzen einhalten:

| Metrik | Grenze pro Methode |
|---|---|
| Länge | höchstens 25 Zeilen, gezählt von der Signatur bis zur schließenden `}`. Der Doc-Kommentar zählt nicht. |
| Zyklomatische Komplexität | höchstens 6 |

Die **zyklomatische Komplexität** zählt die Wege durch eine Methode. Eine Methode ohne
Verzweigung hat den Wert 1. Jedes `if`, jede Schleife, jedes `case`, jedes `&&` und jedes `||`
erhöht ihn um 1. Mehr dazu steht im Kapitel
*DRY* im Kurs, Abschnitt *Code Metrics: messen statt schätzen*.

**In jeder IDE und in der Konsole:** Das Projekt *PraterTickets.Application* enthält eine
vorbereitete Prüfung. Die Datei *CodeMetricsConfig.txt* legt die Grenze für die Komplexität
fest. Führe im Ordner der Solution diesen Befehl aus:

```text
dotnet build --no-incremental -p:CheckMetrics=true
```

Ist eine Methode zu komplex, bricht der Build mit dem Fehler `CA1502` ab. Die Meldung nennt die
Methode und ihre Komplexität. Läuft der Build ohne Fehler durch, hältst du die Grenze ein. Ohne
`-p:CheckMetrics=true` ist die Prüfung ausgeschaltet. Deshalb lässt sich die Solution auch jetzt
schon bauen.

**In Visual Studio:** Wähle *View → Other Windows → Code Metrics* und dann *Calculate Code
Metrics for Solution*. In Visual Studio 2022 heißt der Befehl *Analyze → Calculate Code Metrics
→ For Solution*. Klappe die Klasse `TicketOffice` auf. Du siehst für jede Methode die Spalten
*Cyclomatic Complexity*, *Lines of Source Code* und *Maintainability Index*. *Lines of Source
Code* zählt den Doc-Kommentar mit.

Notiere für `CalculatePrice` die Komplexität, die Zeilenzahl und, falls du Visual Studio
verwendest, den Maintainability Index vor und nach deinem Umbau.

## Regeln für die Arbeit

- Ändere die Tests nicht. Die öffentliche Signatur von `CalculatePrice` und das Enum `Ride`
  bleiben, wie sie sind.
- Ändere kein Verhalten. Auch die Texte der Exceptions bleiben gleich.
- Lege keine neuen Klassen an. Alle neuen Methoden sind `private` und stehen in `TicketOffice`.
- Führe Konstanten für Preise und Altersgrenzen jetzt **nicht** ein. Wie man Werte und Regeln an
  genau einer Stelle definiert, lernst du im Kapitel
  *O, Teil 1: Werte und Regeln an genau einer Stelle* im Kurs.
- Nutze die Refactoring-Werkzeuge der IDE (*Extract Method*, *Rename*), statt Code von Hand zu
  kopieren.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis der Tests.
