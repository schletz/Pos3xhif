# Lotto-Quicktipps

Beim Lotto „6 aus 45“ werden 6 Zahlen zwischen 1 und 45 gezogen. Keine Zahl darf doppelt
vorkommen (Ziehen ohne Zurücklegen). Die Klasse *LottoTipp* soll Quicktipps erzeugen: Der
Zufallszahlengenerator wählt für jeden Tipp 6 Zahlen. Danach prüft sie, wie viele Richtige ein Tipp
hat. Das Testprogramm erzeugt am Ende 1 000 000 Tipps und zeigt dir, wie schnell deine Lösung ist
und wie viel Speicher sie braucht.

## Klasse LottoTipp

Speichere die Tipps in einer internen Liste. Diese Liste muss *private* sein. Die Klasse hat außer
*TippCount* keine Properties und keine public Felder.

Der Zufallszahlengenerator hat einen fixen Seed (906). Er liefert daher immer dieselbe Folge von
Zahlen, und das Testprogramm kann die erzeugten Tipps prüfen. Lege ihn so an:

```c#
private readonly Random _random = new Random(906);  // Fixed seed, always creates the same sequence.
```

- **TippCount** ist read-only und liefert die Anzahl der gespeicherten Tipps.
- **GetNumbers()** ist *private* und liefert 6 Zahlen zwischen 1 und 45 als `int[]`. Erzeuge die
  Zahlen mit `_random.Next(1, 46)`. Prüfe bei jeder neuen Zahl, ob sie schon im Array vorkommt.
  Wenn ja, erzeuge einfach die nächste Zahl. Nur so bekommst du dieselben Zahlen wie das
  Testprogramm.
- **AddQuicktipps(int count)** erzeugt *count* Tipps mit *GetNumbers()* und fügt sie zur internen
  Liste hinzu.
- **GetTipp(int number)** liefert den Tipp mit der Nummer *number* als `int[]`. Der erste Tipp hat
  die Nummer 0. Liefere eine **Kopie** des Arrays. Ein Array ist ein Referenztyp: Gibst du das
  interne Array zurück, kann der Aufrufer deinen Tipp ändern.
- **CheckTipp(int tippNr, int[] drawnNumbers)** liefert die Anzahl der Zahlen des Tipps *tippNr*,
  die auch in *drawnNumbers* vorkommen.

Die Reihenfolge der Zahlen in einem Tipp ist egal. Das Testprogramm sortiert die Zahlen vor dem
Vergleich.

## Durchzuführende Arbeiten

Öffne die Solution *LottoTipp.sln* in diesem Ordner. Lege die Klasse *LottoTipp* in einer eigenen
Datei an und verwende den Namespace *LottoTipp.Application*. Die Dateien *Program.cs* und
*TestHelpers.cs* darfst du nicht ändern.

*Program.cs* ist ein Testprogramm. Es kompiliert erst, wenn die Klasse mit allen Properties und
Methoden existiert. Starte es im Ordner *LottoTipp.Application* mit `dotnet run`. Es zeigt dir bei
jeder Prüfung *OK* oder *FAILED*. Wirft eine Prüfung eine Exception, gilt sie als nicht erfüllt.

Eine vollständige Lösung liefert folgende Ausgabe. Die Zeit und der Speicher in der Zeile *Info*
hängen von deinem Rechner und deiner Implementierung ab.

```text
Checking class implementation
   (1) OK: LottoTipp has no public fields and TippCount is the only property.
   (2) OK: TippCount is read-only and returns the number of stored tipps.
Checking quicktipps
   (3) OK: 1000 tipps have 6 different numbers between 1 and 45.
   (4) OK: The first 5 tipps match the numbers of the fixed seed 906.
   (5) OK: Changing the array of GetTipp does not change the stored tipp.
   (6) OK: CheckTipp returns the number of right numbers.
Checking 1 000 000 tipps
   (7) OK: 1 000 000 tipps have 26 tipps with 5 and tipp 499 182 with 6 right numbers.
   Info: 166 ms, 53.78 MBytes used.

Result of the program in C:\...\LottoTipp.Application:
7 of 7 checks passed.
11 of 11 points. Grade: Sehr gut.
```

## Für echte Profis

Bis jetzt liegen die Tipps in einer Liste von `int` Arrays. Ein Tipp braucht daher 6 × 4 = 24
Bytes für die Zahlen, dazu kommt der Verwaltungsaufwand für jedes Array am Heap. Außerdem musst du
die Arrays oft durchsuchen:

- Beim Erzeugen prüfst du, ob eine Zahl schon im Array ist.
- Beim Prüfen der Richtigen suchst du jede gezogene Zahl im Array des Tipps.

Ein Tipp lässt sich auch als *Bitmaske* speichern: Jede Zahl von 1 bis 45 bekommt ein Bit. Ein
gesetztes Bit (1) bedeutet, dass die Zahl getippt wurde. Du brauchst also 45 Bits. Ein `long` hat
64 Bits, daher reicht ein einziger `long` Wert für einen Tipp:

![](lotto_bitwise.svg)

Ändere deine Klasse so, dass sie die Tipps intern in einer Liste von `long` Werten speichert. Die
public Methoden bleiben gleich, das Testprogramm muss also weiterhin funktionieren. *GetTipp()*
baut das Array aus den gesetzten Bits.

Auch die Anzahl der richtigen Zahlen kannst du mit Bitoperationen schneller berechnen: Verknüpfe
die Maske des Tipps und die Maske der gezogenen Zahlen mit `&`. Die gesetzten Bits im Ergebnis sind
die Richtigen. Zum Zählen der gesetzten Bits verwende
[Brian Kernighan's Algorithm](https://iq.opengenus.org/brian-kernighan-algorithm/). Vergleiche
danach die Zeit und den Speicher in der Zeile *Info*.

> [!TIP]
> Brauchst du bei einem Bitshift ein Ergebnis vom Typ `long`, schreibe `1L` statt `1`. Sonst
> rechnet C# mit `int` (nur 32 Bits), und das Ergebnis ist falsch. Achte auch auf die Rangfolge
> der Operatoren: Vergleiche (`==`, `!=`) werden vor `&` und `|` ausgeführt. Setze daher
> Klammern, z. B. `(mask & bit) != 0`.
