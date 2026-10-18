# Eine Rechtschreibhilfe

Tippst du in einer Suchmaschine „fahrad“, fragt sie: „Meintest du *fahrrad*?“ Dahinter steckt
ein Maß dafür, wie ähnlich sich zwei Wörter sind: die **Editierdistanz**. Im
Kapitel *Dynamische Programmierung* im Kurs hast du ihre Tabelle per Hand
ausgefüllt. In dieser Übung programmierst du sie, zuerst als einfache Rekursion und dann mit
einer Tabelle. Damit baust du eine kleine Rechtschreibhilfe, die zu einem Wort ähnliche Wörter
aus einer Wortliste vorschlägt.

## Aufbau der Solution

Öffne die Solution *Rechtschreibhilfe.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *Rechtschreibhilfe.Application* | Class Library mit den Stubs und der Wortliste. Hier programmierst du. |
| *Rechtschreibhilfe.Tests* | xUnit-Tests. Die Tests darfst du nicht ändern. |

In *Rechtschreibhilfe.Application* findest du folgende Dateien:

| Datei | Was du tun musst |
|---|---|
| *EditDistance.cs* | `Naive` (Teil 1), `Table` und `Compute` (Teil 2) implementieren. |
| *SpellChecker.cs* | Konstruktor, `Count`, `Contains` und `Suggest` implementieren (Teil 3). |
| *Suggestion.cs* | Vorgegeben, nicht ändern. |
| *WordList.cs* | Vorgegeben, nicht ändern. Lädt die Wortliste. |
| *Data/woerter.txt* | Die Wortliste. Nicht ändern, die Tests hängen von ihr ab. |

Du darfst private Felder und private Hilfsmethoden ergänzen. Die öffentlichen Signaturen bleiben
so, wie sie sind.

## Die Wortliste

Die Datei *Data/woerter.txt* enthält rund 2300 gängige deutsche Wörter, darunter auch
österreichische wie *jause*, *semmel*, *paradeiser* und *matura*. Die Liste wurde für diese
Übung zusammengestellt und ist nicht vollständig: Meist steht nur die Grundform eines Wortes
darin (*fahren*, aber nicht *fährt*).

- **Alle Wörter sind kleingeschrieben**, auch die Nomen (*haus*, *straße*). Die Rechtschreibhilfe
  prüft nur die Buchstaben, nicht die Groß- und Kleinschreibung. Weil die Wörter Zeichen für
  Zeichen verglichen werden, gilt *Haus* trotzdem als anderes Wort als *haus*.
- Die Wörter bestehen nur aus den Buchstaben `a` bis `z`, `ä`, `ö`, `ü` und `ß`. Es gibt keine
  Leerzeichen, Bindestriche oder Ziffern.
- Pro Zeile steht ein Wort. Die Datei ist in UTF-8 gespeichert, jedes Wort kommt nur einmal
  vor, und die Wörter sind nach ihren Zeichencodes sortiert (dieselbe Ordnung wie in Teil 3).
  Deshalb stehen die Wörter, die mit `ä`, `ö` oder `ü` beginnen, ganz am Ende der Datei.

Die Datei ist als *Embedded Resource* in die Class Library eingebunden. Du musst dich also nicht
um Pfade kümmern: `WordList.LoadGerman()` liefert die Wörter als `IReadOnlyList<string>`.

## Die Editierdistanz

Die Editierdistanz (nach ihrem Erfinder auch *Levenshtein-Distanz*) zweier Wörter ist die
**kleinste Anzahl von Operationen**, mit denen man das erste Wort in das zweite verwandelt.
Erlaubt sind drei Operationen, jede kostet 1:

| Operation | Beispiel |
|---|---|
| ein Zeichen **einfügen** | *hau* → *haus* |
| ein Zeichen **löschen** | *haus* → *aus* |
| ein Zeichen **ersetzen** | *haus* → *maus* |

Weitere Beispiele:

| Von | Nach | Distanz | Warum |
|---|---|---|---|
| *sonntag* | *montag* | 2 | *s* durch *m* ersetzen, ein *n* löschen |
| *straße* | *strasse* | 2 | *ß* ist **ein** Zeichen: *ß* durch *s* ersetzen, *s* einfügen |
| *apfel* | *äpfel* | 1 | *a* und *ä* sind verschiedene Zeichen |
| *Haus* | *haus* | 1 | *H* und *h* sind verschiedene Zeichen |
| *ab* | *ba* | 2 | Vertauschen ist keine eigene Operation, also zweimal ersetzen |
| *haus* | *haus* | 0 | |

Zwei Zeichen sind gleich, wenn sie denselben Zeichencode haben (`==` auf `char`). Die Distanz
ist symmetrisch: Von *a* nach *b* ist sie genauso groß wie von *b* nach *a*, denn Einfügen und
Löschen sind zueinander umgekehrte Operationen.

Für alle Methoden in `EditDistance` gilt: Ist ein Argument `null`, wirf eine
`ArgumentNullException`. Ein leerer String ist dagegen erlaubt. Die Distanz vom leeren String
zu *abc* ist 3.

## Teil 1: Naiv rekursiv

`EditDistance.Naive(source, target)` berechnet die Distanz mit einer rekursiven Methode, ohne
Zwischenergebnisse zu speichern. Betrachte dafür die ersten *i* Zeichen von `source` und die
ersten *j* Zeichen von `target`. Für die Distanz d(*i*, *j*) dieser beiden Präfixe gilt:

- Ist eines der Präfixe leer, besteht die Distanz nur aus Einfügen bzw. Löschen:
  d(0, *j*) = *j* und d(*i*, 0) = *i*.
- Stimmen die jeweils **letzten Zeichen** der beiden Präfixe überein, kosten sie nichts:
  d(*i*, *j*) = d(*i* − 1, *j* − 1).
- Sonst ist die letzte Operation entweder ein Löschen, ein Einfügen oder ein Ersetzen. Du
  probierst alle drei und nimmst die günstigste:
  d(*i*, *j*) = 1 + min(d(*i* − 1, *j*), d(*i*, *j* − 1), d(*i* − 1, *j* − 1)).

Das Ergebnis ist d(`source.Length`, `target.Length`). Verwende für die Rekursion eine private
Hilfsmethode mit den Parametern *i* und *j*. Teilstrings mit `Substring` brauchst du nicht.

Probiere die Methode mit kurzen Wörtern aus. Zähle dann mit einem Zähler in der Hilfsmethode,
wie oft sie für *haus* → *maus* und für *sonntag* → *montag* aufgerufen wird, und überlege, welche
Paare (*i*, *j*) dabei mehrfach vorkommen. Genau diese Beobachtung führt zu Teil 2.

## Teil 2: Mit Tabelle

`EditDistance.Table(source, target)` speichert jedes d(*i*, *j*) genau einmal in einem
zweidimensionalen Array:

- Das Array hat `source.Length + 1` Zeilen und `target.Length + 1` Spalten. Zeile 0 und Spalte 0
  gehören zum leeren Präfix.
- Die Zelle `[i, j]` enthält d(*i*, *j*), also die Distanz zwischen den ersten *i* Zeichen von
  `source` und den ersten *j* Zeichen von `target`.
- Du füllst zuerst Zeile 0 und Spalte 0 und danach die übrigen Zellen Zeile für Zeile von links
  nach rechts. So sind die drei Nachbarzellen links, oben und links oben immer schon berechnet,
  wenn du sie brauchst.
- Die Methode liefert die **ganze** Tabelle. Die Tests prüfen jede Zelle.

Die Tabelle für `Table("tor", "tier")` sieht so aus:

```text
         ""   t   i   e   r
    ""    0   1   2   3   4
    t     1   0   1   2   3
    o     2   1   1   2   3
    r     3   2   2   2   2
```

`EditDistance.Compute(source, target)` liefert die Zelle rechts unten. Ein Test berechnet damit
die Distanz zweier Strings mit über 400 Zeichen und bricht nach 10 Sekunden ab. Ruft `Compute`
die naive Variante auf, schlägt dieser Test deshalb fehl.

## Teil 3: Vorschläge

Die Klasse `SpellChecker` bekommt im Konstruktor eine Wortliste:

```c#
var checker = new SpellChecker(WordList.LoadGerman());

Console.WriteLine(checker.Contains("fahrrad"));   // True
foreach (var suggestion in checker.Suggest("fahrad", 2, 5))
{
    Console.WriteLine($"{suggestion.Word} ({suggestion.Distance})");
}
```

```text
True
fahrrad (1)
fahren (2)
fahrer (2)
fahrt (2)
```

### Konstruktor, Count und Contains

- Kommt ein Wort in der übergebenen Liste mehrfach vor, wird es nur einmal gespeichert. `Count`
  liefert die Anzahl der **verschiedenen** Wörter.
- Ändert der Aufrufer die übergebene Liste später, darf das den `SpellChecker` nicht verändern.
- `Contains(word)` prüft, ob das Wort in der Liste steht. Verglichen wird Zeichen für Zeichen,
  also mit Groß- und Kleinschreibung (`StringComparer.Ordinal`).

### Suggest

`Suggest(word, maxDistance, count)` liefert eine Liste von `Suggestion`-Objekten:

```c#
public sealed record Suggestion(string Word, int Distance);
```

- In die Liste kommen alle Wörter, deren Editierdistanz zu `word` **höchstens** `maxDistance`
  beträgt. Steht `word` selbst in der Liste, ist es mit der Distanz 0 dabei.
- Sortiert wird zuerst **aufsteigend nach der Distanz**. Bei gleicher Distanz entscheidet
  `string.CompareOrdinal`, also der Zeichencode Zeichen für Zeichen.
- Die Liste enthält **höchstens `count` Einträge**. Abgeschnitten wird erst nach dem Sortieren:
  Du bekommst die `count` besten Vorschläge.
- Gibt es keinen passenden Vorschlag, ist die Liste leer. Bei `count` = 0 ebenfalls.

Warum `CompareOrdinal`? Ein Vergleich nach deutschen Regeln hängt von der Kultur-Einstellung ab
und sortiert *ä* wie *a* oder *ae*. Das Ergebnis wäre je nach Rechner verschieden. Der Vergleich
nach Zeichencode liefert überall dasselbe Ergebnis. Dafür stehen die Umlaute hinten, weil ihre
Codes größer sind als die von `a` bis `z`:

| Aufruf | Ergebnis |
|---|---|
| `Suggest("uber", 1, 10)` | *aber* (1), *ufer* (1), *über* (1) |
| `Suggest("haus", 1, 10)` | *haus* (0), *aus* (1), *hals* (1), *haut* (1), *maus* (1) |
| `Suggest("strase", 2, 5)` | *strafe* (1), *straße* (1), *strahl* (2), *strand* (2) |
| `Suggest("lerer", 2, 5)` | *leder* (1), *leer* (1), *lehrer* (1), *leser* (1), *eher* (2) |

Beim letzten Beispiel haben 30 Wörter eine Distanz von höchstens 2. Die Liste wird nach dem
fünften Eintrag abgeschnitten.

### Fehler

| Fehler | Exception |
|---|---|
| `words` im Konstruktor ist `null` | `ArgumentNullException` |
| `words` enthält `null` | `ArgumentException` |
| `word` in `Contains` oder `Suggest` ist `null` | `ArgumentNullException` |
| `maxDistance` oder `count` ist negativ | `ArgumentOutOfRangeException` |

## Tests ausführen

Öffne ein Terminal im Ordner dieser README und starte die Tests:

```text
dotnet test
```

Am Anfang schlagen fast alle Tests fehl, weil die Stubs eine `NotImplementedException` werfen.
Nur die Tests für die Wortliste selbst laufen von Anfang an. Mit einem Filter führst du nur die
Tests einer Klasse aus:

| Testklasse | Was geprüft wird |
|---|---|
| `EditDistanceTests` | Bekannte Wortpaare, die Tabelle, gleiche Ergebnisse von `Naive` und `Compute` (Teil 1 und 2) |
| `SpellCheckerTests` | `Contains` und `Suggest` mit einer kleinen Wortliste (Teil 3) |
| `GermanWordListTests` | Die Wortliste und Vorschläge aus ihr (Teil 3) |

```text
dotnet test --filter "FullyQualifiedName~EditDistanceTests"
dotnet test --filter "FullyQualifiedName~SpellCheckerTests"
dotnet test --filter "FullyQualifiedName~GermanWordListTests"
```

## Erweiterungen (ohne Tests)

- Ein Wort, das um mehr als `maxDistance` Zeichen länger oder kürzer ist als `word`, kann nie
  in die Vorschlagsliste kommen. Überspringe solche Wörter, ohne ihre Distanz zu berechnen.
  Begründe, warum das stimmt.
- Gib zu zwei Wörtern die Operationen aus, die zur Distanz führen, zum Beispiel
  *sonntag* → *montag*: „s durch m ersetzen, n löschen“. Gehe dafür in der Tabelle von rechts
  unten zurück nach links oben.
- Bei der *Damerau-Levenshtein-Distanz* kostet auch das Vertauschen zweier benachbarter Zeichen
  nur 1 (*ab* → *ba*). Das ist einer der häufigsten Tippfehler. Erweitere die Tabelle um diesen
  Fall.
- Schreibe eine Konsolen-App, die einen Satz einliest, jedes Wort in Kleinbuchstaben umwandelt
  und für jedes unbekannte Wort drei Vorschläge ausgibt.
