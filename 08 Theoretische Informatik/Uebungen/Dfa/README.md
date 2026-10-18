# Ein generischer DEA

Im Kapitel *Endliche Automaten* im Kurs hast du einen DEA mit einem `enum` und
einem `switch`-Ausdruck programmiert. Für jeden neuen Automaten brauchst du dort neuen Code. In
dieser Übung schreibst du eine generische Klasse `Dfa<TState>`, die den Automaten aus **Daten**
baut: Die Übergangstabelle wird zu einem Dictionary. Danach baust du damit drei Automaten.

## Aufbau der Solution

Öffne die Solution *Dfa.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *Dfa.Application* | Class Library mit den Stubs. Hier programmierst du. |
| *Dfa.Tests* | xUnit-Tests. Die Tests darfst du nicht ändern. |

In *Dfa.Application* findest du folgende Dateien:

| Datei | Was du tun musst |
|---|---|
| *Dfa.cs* | Konstruktor, `Accepts` und `Trace` implementieren (Teil 1). |
| *DivisibleByThree.cs* | `Create` implementieren (Teil 2). |
| *DivisibleByThreeState.cs* | Vorgegeben, nicht ändern. |
| *FloatLiteral.cs* | `Create` implementieren (Teil 3). |
| *FloatLiteralState.cs* | Die Zustände selbst festlegen (Teil 3). |
| *TrafficLight.cs* | `Create` und `Encode` implementieren (Teil 4). |
| *TrafficLightState.cs*, *TrafficLightEvent.cs* | Vorgegeben, nicht ändern. |

Du darfst private Felder und private Hilfsmethoden ergänzen. Die öffentlichen Signaturen bleiben
so, wie sie sind.

## Teil 1: Die Klasse Dfa&lt;TState&gt;

Ein DEA ist ein 5-Tupel M = (Q, Σ, δ, q₀, F). Der Konstruktor bekommt genau diese fünf Teile:

```c#
public Dfa(
    IEnumerable<TState> states,                                     // Q
    IEnumerable<char> alphabet,                                     // Σ
    IReadOnlyDictionary<(TState State, char Symbol), TState> transitions, // δ
    TState start,                                                   // q₀
    IEnumerable<TState> finalStates)                                // F
```

So legst du zum Beispiel einen Automaten an, der eine gerade Anzahl von Einsen akzeptiert:

```c#
var dfa = new Dfa<string>(
    states: ["even", "odd"],
    alphabet: "1",
    transitions: new Dictionary<(string, char), string>
    {
        [("even", '1')] = "odd",
        [("odd", '1')] = "even"
    },
    start: "even",
    finalStates: ["even"]);

Console.WriteLine(dfa.Accepts("1111"));                 // True
Console.WriteLine(string.Join(" → ", dfa.Trace("11"))); // even → odd → even
```

- `TState` ist der Typ der Zustände, meistens ein `enum`. Gleichheit gilt wie bei
  `Dictionary` und `HashSet` (`EqualityComparer<TState>.Default`). Wegen `where TState : notnull`
  darf ein Zustand nicht `null` sein, denn er ist Teil eines Dictionary-Schlüssels.
- Da `string` das Interface `IEnumerable<char>` implementiert, kannst du das Alphabet als
  String angeben (`"01"`).
- Der Schlüssel `(Zustand, Zeichen)` kommt in einem Dictionary nur einmal vor. Zu einem Zustand
  und einem Zeichen gibt es also höchstens einen Folgezustand. Der **Determinismus** ist damit
  schon durch den Typ garantiert, doppelte Übergänge kann es nicht geben.
- δ darf **partiell** sein: Es müssen nicht alle Paare (Zustand, Zeichen) einen Übergang haben.
  Ein fehlender Übergang spielt dieselbe Rolle wie ein Fehlerzustand, den du nicht zeichnest.
- Ein `Dfa`-Objekt ist **unveränderlich**. Ändert der Aufrufer die übergebene Liste oder das
  Dictionary später, darf das den Automaten nicht verändern.

### Accepts und Trace

- `Accepts(word)` startet in q₀ und liest das Wort Zeichen für Zeichen. Gibt es für den
  aktuellen Zustand und das gelesene Zeichen keinen Übergang, bleibt der Automat stehen und das
  Wort wird **abgelehnt**. Das gilt auch für Zeichen, die nicht in Σ liegen. Es wird keine
  Exception geworfen. Das Wort wird genau dann akzeptiert, wenn alle Zeichen gelesen wurden und
  der Automat danach in einem Endzustand ist.
- `Trace(word)` liefert die Folge der Zustände, die der Automat durchläuft:
  - Das erste Element ist immer q₀. Für das leere Wort ε ist das Ergebnis also `[q₀]`.
  - Kann der Automat das ganze Wort lesen, enthält die Liste |w| + 1 Zustände.
  - Fehlt ein Übergang, endet die Liste mit dem Zustand, in dem der Automat stecken geblieben
    ist. Die restlichen Zeichen werden nicht mehr gelesen.
- Ist `word` gleich `null`, werfen beide Methoden eine `ArgumentNullException`.

Ein Beispiel mit einem Automaten für die Sprache a b* a über Σ = {a, b}. Die Übergänge sind
δ(0, a) = 1, δ(1, b) = 1 und δ(1, a) = 2. Startzustand ist 0, F = {2}. Alle anderen Übergänge
fehlen.

| Wort | `Trace` | `Accepts` | Warum |
|---|---|---|---|
| ε | `[0]` | false | 0 ist kein Endzustand. |
| `abba` | `[0, 1, 1, 1, 2]` | true | Ganzes Wort gelesen, 2 ∈ F. |
| `ab` | `[0, 1, 1]` | false | Ganzes Wort gelesen, aber 1 ∉ F. |
| `ba` | `[0]` | false | δ(0, b) fehlt. |
| `aaab` | `[0, 1, 2]` | false | δ(2, a) fehlt, obwohl 2 ein Endzustand ist. |
| `abxa` | `[0, 1, 1]` | false | `x` liegt nicht in Σ. |

### Ungültige Konfigurationen

Der Konstruktor prüft die Angaben und wirft bei einem Fehler eine Exception. `ParamName` ist
jeweils der Name des Parameters, in dem der Fehler steckt. Die Tests prüfen den genauen Typ der
Exception und `ParamName`.

| Fehler | Exception | `ParamName` |
|---|---|---|
| Ein Argument ist `null` | `ArgumentNullException` | Name des Arguments |
| Ein Zustand kommt in `states` doppelt vor | `ArgumentException` | `states` |
| Ein Zeichen kommt in `alphabet` doppelt vor | `ArgumentException` | `alphabet` |
| `start` ist nicht in `states` (auch bei leerem `states`) | `ArgumentException` | `start` |
| Ein Endzustand ist nicht in `states` | `ArgumentException` | `finalStates` |
| Ein Endzustand kommt doppelt vor | `ArgumentException` | `finalStates` |
| Ein Übergang beginnt oder endet in einem Zustand, der nicht in `states` ist | `ArgumentException` | `transitions` |
| Ein Übergang verwendet ein Zeichen, das nicht in `alphabet` ist | `ArgumentException` | `transitions` |

Erlaubt sind dagegen ein leeres Alphabet, eine leere Übergangstabelle und eine leere Menge von
Endzuständen. Solche Automaten sind zwar langweilig, aber korrekt.

Gib in der Fehlermeldung an, welcher Zustand oder welches Zeichen das Problem ist. Wer später
einen Automaten mit 20 Zuständen anlegt, ist dafür dankbar.

## Teil 2: Binärzahlen, die durch 3 teilbar sind

`DivisibleByThree.Create()` liefert einen `Dfa<DivisibleByThreeState>` mit folgender Sprache:

- Σ = {`0`, `1`}.
- Ein Wort ist eine **nicht leere** Folge von Binärziffern. Führende Nullen sind erlaubt
  (`0011` ist die Zahl 3). Das leere Wort ε ist keine Zahl und wird abgelehnt.
- Akzeptiert wird genau dann, wenn der Wert der Zahl durch 3 teilbar ist: `0`, `11`, `110`,
  `1001`, … werden akzeptiert, `1`, `10`, `100`, … abgelehnt.

Die Zustände sind im `enum` `DivisibleByThreeState` vorgegeben. `Start` heißt „noch keine Ziffer
gelesen“, `RemainderN` heißt „die bisher gelesenen Ziffern ergeben eine Zahl mit Rest N bei
Division durch 3“. Die Tests prüfen mit `Trace`, ob dein Automat die Zustände in dieser
Bedeutung verwendet. Der Automat soll beliebig lange Zahlen verarbeiten können, du darfst den
Wert der Zahl also nicht ausrechnen.

## Teil 3: Gleitkommaliterale

`FloatLiteral.Create()` liefert einen `Dfa<FloatLiteralState>`, der Gleitkommaliterale nach
dieser Regel (EBNF) akzeptiert:

```text
float    = [ sign ] digits ( fraction [ exponent ] | exponent )
fraction = "." digits
exponent = ( "e" | "E" ) [ sign ] digits
sign     = "+" | "-"
digits   = digit { digit }
digit    = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
```

Das Alphabet ist Σ = {`0`, …, `9`, `+`, `-`, `.`, `e`, `E`}. In Worten:

- Vorne steht optional **ein** Vorzeichen.
- Danach kommt mindestens eine Ziffer. Führende Nullen sind erlaubt (`007.5`), wie in C#.
- Ein Gleitkommaliteral braucht einen **Nachkommateil oder einen Exponenten** (oder beides). Eine
  reine Ganzzahl wie `42` ist kein Gleitkommaliteral.
- Der Nachkommateil ist ein Punkt mit **mindestens einer Ziffer davor und danach**.
- Der Exponent ist `e` oder `E`, optional ein Vorzeichen und mindestens eine Ziffer. Der
  Exponent ist eine Ganzzahl.
- Leerzeichen, Komma, Ziffern-Trennzeichen (`1_000.0`) und Typ-Suffixe (`1.5f`) sind nicht
  erlaubt.

| Akzeptiert | Abgelehnt |
|---|---|
| `3.14`, `-0.5`, `+2.0`, `0.0` | ε, `42`, `-7` (kein Nachkommateil, kein Exponent) |
| `1e10`, `1E0`, `2.5E-3`, `6.022e+23` | `.5`, `5.`, `1.e5` (Ziffer vor und nach dem Punkt fehlt) |
| `007.50`, `-1e-007` | `1e`, `1e+`, `e5` (Exponent unvollständig) |
| | `--1.0`, `+-1.0`, `1.0.0`, `1e5.0`, `1e5e5` |

Die Zustände legst du selbst im `enum` `FloatLiteralState` fest. Zeichne den Automaten zuerst
auf Papier und gib ihn als Übergangstabelle an, bevor du programmierst. Die Tests verwenden nur
`Accepts`, deine Zustände sind also frei wählbar.

## Teil 4: Ampelsteuerung

Ein DEA muss keine Zeichenketten im üblichen Sinn prüfen. Hier ist Σ eine Menge von
**Ereignissen**, und ein Wort ist eine Folge von Ereignissen. Die Ereignisse sind im `enum`
`TrafficLightEvent` vorgegeben, die Zustände im `enum` `TrafficLightState`.

### Kodierung der Ereignisse

`Dfa<TState>` arbeitet mit `char`. Deshalb wird jedes Ereignis als ein Zeichen kodiert:

| Ereignis | Zeichen | Bedeutung |
|---|---|---|
| `TrafficLightEvent.Timer` | `t` | Die Zeit der aktuellen Phase ist abgelaufen. |
| `TrafficLightEvent.Fault` | `f` | Eine Störung wurde erkannt. |
| `TrafficLightEvent.Repair` | `r` | Ein Techniker hat die Ampel repariert. |

`TrafficLight.Encode(...)` setzt diese Tabelle um und macht aus einer Folge von Ereignissen ein
Wort. So schreibt der aufrufende Code typsicher mit dem `enum` und muss die Zeichen nicht
kennen. Die Zuordnung steht an genau einer Stelle:

```c#
var dfa = TrafficLight.Create();
var word = TrafficLight.Encode(TrafficLightEvent.Timer, TrafficLightEvent.Fault, TrafficLightEvent.Repair);
Console.WriteLine(word);              // tfr
Console.WriteLine(dfa.Accepts(word)); // True
```

- `Encode` nimmt beliebig viele Ereignisse (`params IEnumerable<TrafficLightEvent>`), also auch
  eine Liste. Ohne Ereignisse liefert es das leere Wort.
- Ein Wert, der im `enum` nicht definiert ist (z. B. `(TrafficLightEvent)42`), führt zu einer
  `ArgumentOutOfRangeException`, `null` zu einer `ArgumentNullException`.
- Schreib `TrafficLightEvent.Timer` immer aus. Durch die Implicit Usings ist auch
  `System.Threading.Timer` sichtbar, ein `using static` auf das `enum` führt deshalb zu einem
  Namenskonflikt.

### Verhalten der Ampel

- Die Ampel startet auf `Red`.
- Bei `t` wechselt sie in die nächste Phase:
  `Red` → `RedYellow` → `Green` → `GreenBlinking` → `Yellow` → `Red`.
- Bei `f` geht sie aus **jedem** Zustand nach `YellowBlinking`, auch wenn sie dort schon ist.
- In `YellowBlinking` ändert `t` nichts. Erst `r` setzt die Ampel wieder auf `Red`.
- `r` ist nur in `YellowBlinking` erlaubt. In allen anderen Zuständen gibt es keinen Übergang
  für `r`.
- Endzustand ist nur `Red`. Akzeptiert wird also eine **gültige** Folge von Ereignissen, nach der
  die Ampel auf Rot steht.

Beispiele: ε, `ttttt`, `fr` und `ttfr` werden akzeptiert. `tt` (Ampel ist grün), `ttft` (Ampel
blinkt noch) und `ttr` (Reparatur ohne Störung) werden abgelehnt.

## Tests ausführen

Öffne ein Terminal im Ordner dieser README und starte die Tests:

```text
dotnet test
```

Am Anfang schlagen alle Tests fehl, weil die Stubs eine `NotImplementedException` werfen. Gehe
am besten in der Reihenfolge der Teile vor, denn die Automaten aus Teil 2–4 brauchen eine
funktionierende Klasse `Dfa<TState>`. Mit einem Filter führst du nur die Tests einer Klasse aus:

```text
dotnet test --filter "FullyQualifiedName~DfaConstructorTests"
dotnet test --filter "FullyQualifiedName~DfaRunTests"
dotnet test --filter "FullyQualifiedName~DivisibleByThreeTests"
dotnet test --filter "FullyQualifiedName~FloatLiteralTests"
dotnet test --filter "FullyQualifiedName~TrafficLightTests"
```

Lies bei einem Fehler die Meldung des Tests. Sie sagt dir, welches Wort falsch behandelt wurde.

## Erweiterung (ohne Tests)

- Ergänze in `Dfa<TState>` eine Methode, die die Übergangstabelle als Text ausgibt (Zustände als
  Zeilen, Zeichen als Spalten, Endzustände markiert).
- Statt die Ereignisse als `char` zu kodieren, könnte die Klasse auch ein generisches Alphabet
  haben: `Dfa<TState, TSymbol>` mit `Accepts(IEnumerable<TSymbol>)`. Überlege, was sich am
  Konstruktor ändert und ob `Dfa<TState>` dann als Spezialfall übrig bleibt.
