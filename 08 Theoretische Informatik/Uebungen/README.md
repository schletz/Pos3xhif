# Übungsprojekte zur Theoretischen Informatik

Jedes Übungsprojekt besteht aus einer Class Library mit leeren Methoden und einem Testprojekt.
Die Methoden werfen zu Beginn eine *NotImplementedException*. Deine Aufgabe ist es, sie so zu
implementieren, dass alle Tests grün werden. Mit

```text
dotnet test
```

im Ordner des Projekts siehst du jederzeit, wie weit du bist. Musterlösungen gibt es bewusst nicht:
Die Tests sagen dir, ob deine Lösung stimmt.

## Formale Sprachen

| Projekt | Thema | Gehört zu |
|---|---|---|
| [Dfa](Dfa/README.md) | Generischer deterministischer endlicher Automat, damit drei konkrete Automaten bauen | [Endliche Automaten](../13_Endliche_Automaten.adoc) |
| [RegexLogAnalyzer](RegexLogAnalyzer/README.md) | Eine Logdatei mit benannten Gruppen auswerten, Validatoren mit `[GeneratedRegex]` | [Reguläre Ausdrücke](../14_Regulaere_Ausdruecke.adoc) |
| [JsonParser](JsonParser/README.md) | EBNF für eine JSON-Teilmenge, Tokenizer und rekursiver Abstiegsparser | [Parser](../15_Parser.adoc) |

## Algorithmen

| Projekt | Thema | Gehört zu |
|---|---|---|
| [Routenplaner](Routenplaner/README.md) | Das Wiener U-Bahn-Netz als Graph, Verbindungen mit den wenigsten Stationen (BFS) und die schnellste Verbindung (Dijkstra) | [Graphen](../20_Graphen.adoc), [Breiten- und Tiefensuche](../21_Breiten_und_Tiefensuche.adoc), [Kürzeste Wege](../22_Kuerzeste_Wege.adoc) |
| [SudokuSolver](SudokuSolver/README.md) | Sudoku mit Backtracking lösen und die Suche mit „wenigste Kandidaten zuerst“ verbessern | [Backtracking](../25_Backtracking.adoc) |
| [Rechtschreibhilfe](Rechtschreibhilfe/README.md) | Editierdistanz naiv rekursiv und mit dynamischer Programmierung, Korrekturvorschläge aus einer Wortliste | [Dynamische Programmierung](../27_Dynamische_Programmierung.adoc) |
