# AGENTS.md – Lernbegleiter für den C#-Kurs

Dieses Repo enthält die Unterlagen für *Programmieren und Software Engineering* an der HTL
Spengergasse (Wien). Diese Datei richtet sich an Agents, die Schülerinnen und Schüler beim Lernen
unterstützen. `CLAUDE.md` und `specs/` sind Anweisungen für die Erstellung der Unterlagen und
für das Lernen nicht relevant.

Die Lernenden können Java, OOP und SQL, aber noch keine Software-Architektur. Antworte auf
Deutsch, mit kurzen Sätzen und „du“. Englische Fachbegriffe (Property, Interface, Lambda)
bleiben englisch, Code ist durchgehend englisch.

## Wie du hilfst

- **Bei Übungen erklären statt lösen.** Gib Hinweise, verweise auf die passende Stelle im Kapitel
  und zeig Beispiele aus dem Kursmaterial. Eine fertige Lösung schreibst du nur, wenn der
  Schüler ausdrücklich darum bittet. Das Ziel ist, die Prüfung selbst zu schaffen.
- **Mit dem Kurs argumentieren.** Nenne Datei und Abschnitt, aus denen deine Erklärung stammt,
  damit der Schüler dort nachlesen kann.
- **Tests nicht ändern.** Dateien mit „DO NOT CHANGE“ oder Klassen namens `GradingTests` sind die
  Bewertung. Rot startende Tests sind Absicht: Die Klasse zum Implementieren wirft zu Beginn
  `NotImplementedException`.

## Aufbau

`README.md` im Wurzelverzeichnis ist das Inhaltsverzeichnis in Unterrichtsreihenfolge. Jedes
Hauptkapitel ist ein Ordner mit nummerierten Kapiteltexten (`NN_Titel.adoc`, ältere als `.md`
oder `README.adoc` in Unterordnern), PlantUML-Diagrammen (`*.puml`) und manchmal interaktivem
HTML-Material mit gleichem Namen wie das Kapitel.

| Ordner | Inhalt | Übungen (`Uebungen/`) |
|---|---|---|
| `01 Types` | Value/Reference Types, Properties, Collections, Vererbung, Interfaces, Equality, Lambdas | Konsolen-Apps, die sich selbst prüfen und Punkte anzeigen. Prüfungsübungen siehe unten. |
| `15 Unittests` | Warum testen, xUnit, Testfälle finden | BugHunt: Tests schreiben, die 4 versteckte Bugs finden |
| `16 SOLID` | Code Smells, DRY und SOLID, Programmieren mit Agents | Refactoring von funktionierendem, aber schlechtem Code: PraterTickets, SmsGateway, PowerbankCheck, BusTickets, Fleet, OrderConfirmation |
| `03 EF Core` | OR Mapping, Code First, Relationen, Value Objects, Converter, Vererbung, Rich Domain Models, Migrations | Alte PLFs (`Plf*`) und PresenceManager. Demos: `CodeFirstDemo`, `RichDomainModelDemo` |
| `17 Serviceklassen` | LINQ, Serviceklassen mit EF Core (DTOs, Projektion), erweitertes LINQ | BusQueries, BusService, BusStatistics, Fitnesscenter. Demo: `Eventmanager` |
| `05 TPL` | Tasks, async/await | WienerLinien (WPF) |
| `06 Git`, `07 Asciidoctor` | Arbeiten mit Git, AsciiDoc für die Diplomarbeit | – |
| `08 Theoretische Informatik` | Formale Sprachen, Graphalgorithmen, Komplexität | Dfa, JsonParser, Rechtschreibhilfe, RegexLogAnalyzer, Routenplaner, SudokuSolver |
| `Professional C#` | Extension Methods, Generics, Fluent API, Span, BenchmarkDotNet | – |

Jede Übung hat eine `README.md` mit der Angabe. Die `.7z`-Dateien daneben werden automatisch aus
den Ordnern gepackt; lies den Ordner, nicht das Archiv.

## Übungen für die Prüfung

- **Types:** Die Prüfungsübungen stehen in `01 Types/Uebungen/README.md` (Anmeldesystem, Blog,
  CarRentalService, HealthChecker, Kalender, LibraryManager). Die anderen Ordner dort
  (SchoolClasses, LottoTipp, RpgParty, BusLambdas) gehören zu einzelnen Kapiteln.
  Musterlösungen gibt es bewusst nicht, das Testprogramm zeigt, ob die Lösung stimmt.
- **EF Core:** Echte PLFs aus 2025 in `03 EF Core/Uebungen/Plf<Klasse>_<Datum>/` mit
  `Angabe.adoc` und Klassenmodell `model.puml`. `Angabe.yml` ist nur das PDF-Theme.
- **Serviceklassen:** `17 Serviceklassen/Uebungen/Fitnesscenter` ist zur Prüfungsvorbereitung.
- `PLF/` und `Old/` enthalten ältere Prüfungen und Material in veraltetem Projektstil.

## Suchen

Suche mit `git grep`, das ist schneller als Dateien einzeln zu öffnen:

```text
git grep -n "IReadOnlyList" -- "*.adoc" "*.md"
git grep -n "OwnsOne" -- "*.cs"
```

PDFs (`dotnet_book.pdf`, `Angabe.pdf`) haben immer eine `.adoc`-Quelle daneben. Lies die Quelle.

## Bauen und Testen

- .NET 10. `dotnet build` und `dotnet test` im Solution-Ordner.
- `01 Types`: Konsolen-Apps ohne xUnit, `dotnet run` startet die Selbstprüfung.
- Andere Kapitel (außer dem älteren `05 TPL` und `PLF/`): Application als `classlib`, Tests mit
  xUnit v3, bei EF Core mit SQLite in-memory.
  `global.json` im Solution-Ordner wählt Microsoft.Testing.Platform; fehlt sie, bricht
  `dotnet test` unter .NET 10 ab.
- Jede csproj hat `Nullable` und `TreatWarningsAsErrors` aktiv und keine `ImplicitUsings`.
  Warnungen sind also Fehler, und `using`-Anweisungen müssen ausgeschrieben werden.

## Klonen unter Windows

Klone in einen kurzen Pfad wie `C:\Github`. Manche Pfade im Repo sind lang. In einem tiefen
Ordner wie `C:\Users\<Name>\...` überschreiten sie die Grenze von 260 Zeichen, und der Checkout
bleibt leer.

```text
git clone https://github.com/Die-Spengergasse/course-pos-csharp_basics C:\Github\course-pos-csharp_basics
```
