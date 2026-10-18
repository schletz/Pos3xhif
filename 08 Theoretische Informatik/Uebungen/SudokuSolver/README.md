# Ein Sudoku-Löser mit Backtracking

Im Kapitel *Backtracking* im Kurs hast du gesehen, wie man ein Problem als Suchbaum
formuliert: eine Teillösung schrittweise erweitern, bei einem Konflikt den letzten Schritt
zurücknehmen und die nächste Möglichkeit probieren. In dieser Übung löst du damit Sudokus. Du
zählst dabei mit, wie oft deine rekursive Methode aufgerufen wird. Danach verbesserst du die
Reihenfolge, in der die Felder ausgefüllt werden, und siehst am Zähler, wie viel Suche du dir
damit sparst.

## Aufbau der Solution

Öffne die Solution *SudokuSolver.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *SudokuSolver.Application* | Class Library mit den Stubs. Hier programmierst du. |
| *SudokuSolver.Tests* | xUnit-Tests. Die Tests darfst du nicht ändern. |

In *SudokuSolver.Application* findest du folgende Dateien:

| Datei | Was du tun musst |
|---|---|
| *SudokuBoard.cs* | `Parse`, Indexer, `Clone` und `ToString` (Teil 1), `CanPlace`, `Candidates`, `IsValid` und `IsComplete` (Teil 2) implementieren. |
| *Solver.cs* | `Solve` implementieren (Teil 3 und 4). |
| *CellSelection.cs* | Vorgegeben, nicht ändern. |
| *SolveResult.cs* | Vorgegeben, nicht ändern. |

Du darfst private Felder und private Hilfsmethoden ergänzen. Die öffentlichen Signaturen bleiben
so, wie sie sind.

## Die Spielregeln

Ein Sudoku hat 9 × 9 Felder. Sie sind in 9 Zeilen, 9 Spalten und 9 Blöcke mit je 3 × 3 Feldern
aufgeteilt. Einige Felder sind vorgegeben, die anderen sind leer. Gesucht ist eine Belegung aller
Felder mit den Ziffern 1 bis 9, sodass **jede Zeile, jede Spalte und jeder Block jede Ziffer genau
einmal** enthält. Die vorgegebenen Ziffern dürfen nicht verändert werden.

Zeilen und Spalten sind von 0 bis 8 nummeriert, beginnend links oben. Das Feld in Zeile 4,
Spalte 1 liegt also in der fünften Zeile und der zweiten Spalte. Der Block eines Feldes beginnt in
Zeile `row / 3 * 3` und Spalte `column / 3 * 3` (ganzzahlige Division).

In den Beispielen dieser Angabe verwenden wir das bekannte Rätsel aus dem Wikipedia-Artikel
*Sudoku* (gemeinfrei). Ein Punkt steht für ein leeres Feld:

```text
53. .7. ...
6.. 195 ...
.98 ... .6.
8.. .6. ..3
4.. 8.3 ..1
7.. .2. ..6
.6. ... 28.
... 419 ..5
... .8. .79
```

## Teil 1: Das Brett

Die Klasse `SudokuBoard` speichert die 81 Felder. Ein leeres Feld hat den Wert
`SudokuBoard.Empty` (0). Es gibt keinen öffentlichen Konstruktor, ein Brett entsteht immer über
`Parse`.

### Parse

`SudokuBoard.Parse(string text)` liest ein Brett aus einem String, Zeile für Zeile:

- Whitespace (Leerzeichen, Tabulatoren, Zeilenumbrüche, also alles, wofür `char.IsWhiteSpace`
  `true` liefert) wird ignoriert. Danach müssen **genau 81 Zeichen** übrig bleiben. Das Rätsel
  oben kannst du also in einer Zeile oder wie abgebildet in 9 Zeilen mit Leerzeichen angeben.
- Die Zeichen `1` bis `9` sind vorgegebene Ziffern. `0` und `.` sind leere Felder. Beide
  Schreibweisen sind verbreitet, deshalb sind beide erlaubt.
- Jedes andere Zeichen ist ein Fehler. Das gilt auch für Ziffern aus anderen Schriften, für die
  `char.IsDigit` ebenfalls `true` liefert (z. B. arabisch-indische Ziffern). Erlaubt sind nur die
  ASCII-Zeichen `0` bis `9`.
- `Parse` prüft **nicht**, ob die Spielregeln eingehalten sind. Ein Brett mit zwei Fünfen in einer
  Zeile lässt sich einlesen, `IsValid` (Teil 2) liefert dafür dann `false`.

| Fehler | Exception |
|---|---|
| `text` ist `null` | `ArgumentNullException` |
| Ohne Whitespace mehr oder weniger als 81 Zeichen | `FormatException` |
| Ein Zeichen ist keine Ziffer von `0` bis `9` und kein `.` | `FormatException` |

### Indexer, ToString und Clone

- `board[row, column]` liefert den Wert eines Feldes (0 bis 9) und setzt ihn. Eine Zeile oder
  Spalte außerhalb von 0 bis 8 und ein Wert außerhalb von 0 bis 9 führen zu einer
  `ArgumentOutOfRangeException`. Der Setter prüft die Spielregeln nicht. Der Löser setzt Ziffern
  probeweise und nimmt sie wieder zurück, das muss ohne Exception gehen.
- `ToString()` liefert das Brett als 81 Zeichen ohne Whitespace. Leere Felder werden als `.`
  geschrieben, auch wenn sie beim Einlesen eine `0` waren. Für das Rätsel oben ergibt sich
  `53..7....6..195....98....6.8...6...34..8.3..17...2...6.6....28....419..5....8..79`.
- `Clone()` liefert eine unabhängige Kopie. Änderst du die Kopie, bleibt das Original gleich und
  umgekehrt.

## Teil 2: Die Regeln prüfen

### CanPlace und Candidates

`CanPlace(row, column, digit)` prüft, ob eine Ziffer in ein Feld passt: Sie darf weder in derselben
Zeile noch in derselben Spalte noch im selben Block vorkommen. **Das Feld selbst wird dabei nicht
betrachtet.** Deshalb liefert `CanPlace` auch für ein schon belegtes Feld eine sinnvolle Antwort:
Die 5 links oben im Rätsel steht nicht im Konflikt mit sich selbst. Eine Ziffer außerhalb von 1
bis 9 führt zu einer `ArgumentOutOfRangeException`.

`Candidates(row, column)` liefert alle Ziffern, für die `CanPlace` bei einem **leeren** Feld
`true` liefert, aufsteigend sortiert. Für ein belegtes Feld ist die Liste leer.

Beispiele für das Rätsel oben:

| Feld | Zeile enthält | Spalte enthält | Block enthält | `Candidates` |
|---|---|---|---|---|
| Zeile 0, Spalte 2 | 5, 3, 7 | 8 | 5, 3, 6, 9, 8 | `[1, 2, 4]` |
| Zeile 4, Spalte 1 | 4, 8, 3, 1 | 3, 9, 6 | 8, 4, 7 | `[2, 5]` |
| Zeile 0, Spalte 0 | (belegt) | | | `[]` |

Ein leeres Feld kann auch **gar keinen** Kandidaten haben. Dann lässt sich das Rätsel in diesem
Zustand nicht mehr lösen.

### IsValid und IsComplete

- `IsValid()` liefert `true`, wenn keine Ziffer in einer Zeile, einer Spalte oder einem Block
  doppelt vorkommt. Leere Felder werden ignoriert, auch ein leeres Brett ist also gültig. Dass
  ein gültiges Brett auch eine Lösung hat, ist damit nicht gesagt (siehe Teil 3).
- `IsComplete()` liefert `true`, wenn kein Feld leer ist. Die Regeln prüft diese Methode nicht.
  Ein Brett ist gelöst, wenn es vollständig **und** gültig ist.

## Teil 3: Backtracking

`Solver.Solve(board, selection)` sucht eine Lösung und liefert ein `SolveResult`:

```c#
public sealed record SolveResult(SudokuBoard? Solution, long Calls)
{
    public bool IsSolved => Solution is not null;
}
```

- `Solution` ist das gelöste Brett oder `null`, wenn es keine Lösung gibt.
- `Calls` ist die Anzahl der Aufrufe deiner rekursiven Methode (Regel siehe unten).
- Das übergebene Brett wird **nicht verändert**. Arbeite auf einer Kopie.
- Ist `board` gleich `null`, wirf eine `ArgumentNullException`.
- Ist das Brett schon am Anfang ungültig (`IsValid()` liefert `false`), gibt es keine Lösung.
  `Solve` liefert dann sofort `Solution = null` und `Calls = 0`, ohne zu suchen. Ohne diese
  Prüfung würde der Löser die leeren Felder ausfüllen und ein Brett mit zwei gleichen Ziffern
  in einer Zeile als „Lösung“ liefern, denn er prüft nur die Ziffern, die er selbst setzt.

### Der Ablauf

Zuerst implementierst du die Auswahl `CellSelection.FirstEmpty`: Gefüllt wird immer das **erste
leere Feld**, Zeile für Zeile von links oben nach rechts unten gelesen. Ein Aufruf deiner
rekursiven Methode macht Folgendes:

1. Zähle den Aufruf.
2. Wähle das nächste leere Feld. Gibt es keines mehr, ist das Brett gelöst: Liefere `true`.
3. Probiere die Kandidaten des Feldes **aufsteigend** (1 vor 2 vor 3 …). Setze den Kandidaten ins
   Feld und rufe die Methode rekursiv auf. Liefert sie `true`, liefere ebenfalls `true`.
   Andernfalls mache das Feld wieder leer und probiere den nächsten Kandidaten.
4. Hat kein Kandidat zum Ziel geführt, liefere `false`. Der Aufrufer nimmt dann seine Ziffer
   zurück.

Die erste Lösung, die du findest, ist das Ergebnis. Die Rätsel, deren Lösung die Tests Feld für
Feld vergleichen, haben genau eine Lösung. Beim leeren Brett, das viele Lösungen hat, prüfen die
Tests nur die Regeln.

### Die Aufrufe zählen

`Calls` zählt **jeden Aufruf** der rekursiven Methode, **den ersten Aufruf mitgezählt**. Weil du
nur Ziffern setzt, die laut `CanPlace` passen, und nach jedem Setzen genau einmal rekursiv
aufrufst, gilt:

> `Calls` = 1 + Anzahl der Ziffern, die probeweise gesetzt wurden.

Daraus folgt: Hat ein Rätsel *n* leere Felder, braucht jede Lösung mindestens *n* + 1 Aufrufe.
Ein schon gelöstes Brett braucht genau einen Aufruf. Jeder Aufruf mehr bedeutet, dass der Löser
in eine Sackgasse gelaufen ist und Ziffern zurücknehmen musste.

Setzt du eine Ziffer, ohne sie vorher zu prüfen, und prüfst erst im nächsten Aufruf, bekommst du
andere Zahlen. Halte dich deshalb an den Ablauf oben, dann kannst du deine Zahlen mit der Tabelle
in Teil 4 vergleichen.

## Teil 4: Das Feld mit den wenigsten Kandidaten zuerst

Bei `FirstEmpty` kann es passieren, dass der Löser ein Feld mit fünf Kandidaten füllt, während
ein anderes Feld nur einen einzigen Kandidaten hat. Rät er beim ersten Feld falsch, merkt er das
oft erst viele Ebenen tiefer und muss alles bis dorthin zurücknehmen.

Mit `CellSelection.FewestCandidates` wählt der Löser in Schritt 2 deshalb das leere Feld mit den
**wenigsten Kandidaten**. Haben mehrere Felder gleich wenige Kandidaten, nimmt er das erste davon
(wieder Zeile für Zeile gelesen). Alles andere bleibt gleich: aufsteigende Reihenfolge der
Kandidaten, gleiche Zählregel.

- Hat ein Feld nur **einen** Kandidaten, muss diese Ziffer dort stehen. Der Löser setzt sie, ohne
  zu raten.
- Hat ein leeres Feld **keinen** Kandidaten, wird es sofort gewählt. Die Schleife über die
  Kandidaten läuft dann kein einziges Mal und der Aufruf liefert `false`. Die Sackgasse wird also
  erkannt, bevor weitere Ziffern gesetzt werden.

Dein Code soll beide Varianten über den Parameter `selection` anbieten. Schreibe dafür nicht den
ganzen Löser zweimal: Die beiden Varianten unterscheiden sich nur darin, **welches** Feld in
Schritt 2 gewählt wird.

### Zum Vergleichen

Die Tests prüfen die Zahl der Aufrufe nicht genau, sondern nur Grenzen und den Vergleich der beiden
Varianten. Wenn du dich an die Regeln oben hältst, solltest du trotzdem genau diese Werte
bekommen:

| Rätsel (Konstante in *Puzzles.cs*) | Leere Felder | `FirstEmpty` | `FewestCandidates` |
|---|---|---|---|
| `Wikipedia` | 51 | 4 209 | 52 |
| `ManyDeadEnds` | 56 | 267 054 | 59 |
| `NoSolutionDeep` (keine Lösung) | 50 | 4 579 | 23 |
| Leeres Brett | 81 | 392 | 82 |

Beim Rätsel `Wikipedia` gibt es in jedem Schritt ein Feld mit genau einem Kandidaten. Die
verbesserte Variante kommt deshalb mit der kleinstmöglichen Zahl von 51 + 1 Aufrufen aus.

## Tests ausführen

Öffne ein Terminal im Ordner dieser README und starte die Tests:

```text
dotnet test
```

Am Anfang schlagen alle Tests fehl, weil die Stubs eine `NotImplementedException` werfen. Gehe in
der Reihenfolge der Teile vor, denn der Löser braucht ein funktionierendes Brett. Mit einem Filter
führst du nur die Tests einer Klasse aus:

| Testklasse | Was geprüft wird |
|---|---|
| `SudokuBoardTests` | `Parse`, Indexer, `ToString` und `Clone` (Teil 1) |
| `SudokuRulesTests` | `CanPlace`, `Candidates`, `IsValid` und `IsComplete` (Teil 2) |
| `SolverTests` | Lösungen, unlösbare und ungültige Rätsel (Teil 3) |
| `CallCountTests` | Der Zähler und der Vergleich der beiden Varianten (Teil 3 und 4) |

```text
dotnet test --filter "FullyQualifiedName~SudokuBoardTests"
dotnet test --filter "FullyQualifiedName~SudokuRulesTests"
dotnet test --filter "FullyQualifiedName~SolverTests"
dotnet test --filter "FullyQualifiedName~CallCountTests"
```

Die Rätsel, die die Tests verwenden, stehen in *SudokuSolver.Tests/Puzzles.cs*. Dort findest du
auch zwei Rätsel, die zwar gültig sind, aber keine Lösung haben.

## Erweiterungen (ohne Tests)

- Schreibe eine Methode `CountSolutions(board, limit)`, die nach einer Lösung nicht aufhört, sondern
  weitersucht und die Lösungen zählt (höchstens bis `limit`). Ein gutes Sudoku hat genau eine
  Lösung. Prüfe damit, ob das wirklich für alle Rätsel in *Puzzles.cs* gilt.
- Erzeuge eigene Rätsel: Fülle ein leeres Brett mit zufälliger Reihenfolge der Kandidaten und
  entferne dann so lange Ziffern, wie das Rätsel noch genau eine Lösung hat.
- Gib das Brett mehrzeilig mit Trennlinien zwischen den Blöcken aus.
