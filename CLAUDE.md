# Lehrunterlagen C# – HTL Spengergasse

## Spezifikationen und Vorlagen

| Datei                        | Bedeutung                                                                                                                                    |
|------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| specs/chapter_spec.md        | Spezifikation zum Aufbau der einzelnen Kapitel.                                                                                              |
| specs/lehrplan_informatik.md | Lehrplan der Höheren Abteilung für Informatik. Der Gegenstand Programmieren und Software Engineering befindet sich in den Zeilen 469 - 642   |
| specs/unittests.md           | Aufbau von Unittests für Übungen aus dem Bereich EF Core und Serviceklassen, die den Studenten in der Vorlage zur Verfügung gestellt werden. |
| dotnet_book.adoc             | AsciiDoc Sammeldokument für den PDF Export aller Kapitel.                                                                                    |
| 16 SOLID/02_DRY.adoc         | Beispiel für ein Kapitel                                                                                                                     |
| 16 SOLID/02_DRY.html         | Beispiel für interaktives HTML Zusatzmaterial                                                                                                |

## Zielgruppe

Lehrunterlagen für den Programmierunterricht der HTL Spengergasse, Spengergasse 20, Wien (www.spengergasse.at).
Zielgruppe: Schüler und Studierende.
Vermittelt wird Praxiswissen mit theoretischem Hintergrund, keine Uni-Vorlesung.

### Vorwissen der Studenten

Die Studenten haben zuvor Java gelernt.
Sie können OOP im Code umsetzen, haben aber noch keine Architektur gelernt.
Die Studenten beherrschen auch SQL.

## Ordnerstruktur

- Die Hauptkapitel sind Ordner in diesem Repo (01 Types, 03 EF Core, ...)
- Darin sind die adoc Dateien mit den Kapiteln. Auf sie wird in der README.md verlinkt.
- HTML Zusatzmaterialien heißen wie die adoc Datei mit der Endung `html`.
- In den Ordner der Hauptkapitel gibt es einen Unterordner `Uebungen`.
    - In diesen Ordner sind die einzelnen Solutions, die den Studenten als Vorlage der Übung dienen.
    - Ein Hook erstellt aus der Vorlage eine 7z Datei, damit die Studenten die Solution herunterladen können.

## Sprache

- Deutsch, Anrede „du“. Viele Leser haben nicht Deutsch als Muttersprache: kurze Sätze, keine
  Redewendungen, Fachbegriffe beim ersten Auftreten erklären. Gängige englische Fachbegriffe
  (Property, Interface, Lambda) bleiben englisch.
- Code ist konsequent Englisch: Bezeichner, Kommentare, Ausgabetexte, Assert-Messages.

## Angaben und sonstige Dateien: als Markdown

- Regel: Alles, das nicht in `dotnet_book.adoc` referenziert wird, wird als Markdown-File erstellt.
  Dies umfasst Übungsangaben, Inhaltsverzeichnisse des Kurses, etc.
- Verwende GitHub Flavored Markdown, da die Markdowns auf github betrachtet werden.

## C#-Projekte

.NET 10. Application als `classlib`, nur in `01 Types` als `console` ohne Testprojekt (dort ist
xUnit noch unbekannt, die Konsolenausgabe prüft die Lösung). Unter macOS `rm -rf`/`mkdir`/`mv`.

```text
rd /S /Q DemoProject
mkdir DemoProject
cd DemoProject
mkdir DemoProject.Application
cd DemoProject.Application
dotnet new classlib
cd ..
mkdir DemoProject.Tests
cd DemoProject.Tests
dotnet new install xunit.v3.templates
dotnet new xunit3 -f net10.0
move global.json ..
dotnet reference add ..\DemoProject.Application
cd ..
dotnet new sln -f sln
dotnet sln add DemoProject.Application
dotnet sln add DemoProject.Tests
start DemoProject.sln
```

Jede csproj: `ImplicitUsings` entfernen (man sieht, woher ein Typ kommt), dazu
`<Nullable>enable</Nullable>` und `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.
Konsolen-Apps zusätzlich `<OutputType>Exe</OutputType>`.

xUnit v3 (`xunit.v3.mtp-v2`, nicht das SDK-Template `xunit`, das noch v2 erzeugt):
- Testprojekt ist eine Exe: `<OutputType>Exe</OutputType>` bleibt drin.
- `-f net10.0` angeben, das Template setzt sonst net8.0.
- `global.json` (`"test": {"runner": "Microsoft.Testing.Platform"}`) gehört in den
  Solution-Ordner. `dotnet test` sucht sie ab dem aktuellen Ordner aufwärts; fehlt sie, bricht es
  unter .NET 10 mit „Testing with VSTest target is no longer supported“ ab.
- `<Using Include="Xunit" />` entfernen, stattdessen `using Xunit;` in den Testdateien.
- `dotnet run` im Testprojekt führt die Tests ebenfalls aus.

Nach `01 Types` aktuelle Sprachfeatures nutzen, wenn sie den Code lesbarer machen. Ziel: Die
Absolventen verstehen jeden C#-Code auf GitHub oder im Unternehmen.

## Automatisches Erstellen von Archiven der Übungen

- Jeder Ordner `<Kapitel>/Uebungen/<Übung>` wird beim Commit automatisch als
  `<Kapitel>/Uebungen/<Übung>.7z` gepackt (siehe *Prüfen und Git*). Die Studierenden laden die
  Übung als 7z herunter, ohne das Repo zu klonen. Kapitel verlinken daher auf das 7z, mit dem
  Hinweis „Die Angabe steht im Archiv in der Datei *README.md*.“
- Die README.md einer Übung muss deshalb für sich allein funktionieren: **keine relativen Links
  auf übergeordnete Ordner** (`../`), sie zeigen im entpackten Archiv ins Leere. Auf Kapitel ohne
  Link verweisen, z. B. „im Kapitel *Endliche Automaten* im Kurs“.


## HTML-Zusatzmaterial

- Interaktive Präsentation: Folien mit Animation (Schritt, Play/Pause), danach Übungsteil mit
  sofortigem Feedback.
- Läuft per Doppelklick über `file://`: CSS/JS inline, kein CDN, keine Web-Fonts, keine
  ES-Module, kein `fetch`.
- Heißt wie das zugehörige Markdown und liegt daneben. Hell/Dunkel über `prefers-color-scheme`
  (im Dark Mode CI-Grundfarben direkt, Rot als `#f5b4c7`). localStorage nur in `try/catch`.
- Link im Markdown auf GitHub Pages, nicht auf die Repo-Datei:
  `https://die-spengergasse.github.io/course-pos-csharp_basics/<Pfad mit %20>`.

## Prüfen und Git

- Vor „fertig“: `dotnet build` ohne Warnungen; `dotnet test` mit einer temporären eigenen
  Lösung grün, danach zurück auf die Stubs. HTML mit playwright-cli über `file://` testen.
- Commit und Push nur auf ausdrücklichen Wunsch. `git push` schreibt auf beide Remotes
  (Die-Spengergasse, schletz/Pos3xhif). `.github/workflows/pages.yml` veröffentlicht bei jedem
  Push auf `master` alle HTML-Files.
- Pre-commit Hook in `.githooks/` (nach dem Klonen einmal `activate_hook.cmd` ausführen): packt
  jede Übung, die der Commit ändert, aus dem Index (nicht aus dem Working Tree) neu und stagt das
  7z. Braucht Python und 7-Zip unter `C:\Program Files\7-Zip`. Alle Archive neu bauen:
  `python .githooks/build_exercise_archives.py --all`.
