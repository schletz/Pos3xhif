# AsciiDoc Productivity

## 1. Quellcode aus der Zwischenablage einfügen (Insert as source block)
Kopiere einen beliebigen Code. Mache einen Rechtsklick in dein AsciiDoc-Dokument und wähle _Insert as source block_. 
Es öffnet sich oben eine Eingabezeile, in der du die Programmiersprache (z. B. _csharp_, _java_, _python_) eintippen kannst. Der Code wird dann perfekt formatiert als AsciiDoc-Source-Block eingefügt.

## 2. TSV-Tabellen einfügen (Insert as tsv table)
Kopiere tabellenartige Daten (z. B. direkt aus Excel) in deine Zwischenablage. Wähle im Rechtsklick-Menü _Insert as tsv table_. 
Die Extension erkennt automatisch die Anzahl der Spalten (anhand der Tabulatoren) und generiert einen fertigen AsciiDoc-Tabellen-Block im TSV-Format.

## 3. Bild aus lokaler Datei einfügen (Insert image from file)
Du möchtest ein Bild einbinden, das auf deiner Festplatte liegt? Wähle _Insert image from file_. 
Ein Dialogfenster öffnet sich im aktuellen Ordner. Wähle dein Bild aus. Die Extension berechnet vollautomatisch den _relativen Pfad_ von deinem Dokument zum Bild und fügt die korrekte _image::pfad/zum/bild.png[]_ Syntax ein.
Hinweis: Damit der Pfad zur adoc Datei passt, muss du sie zuerst speichern.

## 4. Bild aus der Zwischenablage speichern (Insert image from clipboard)
Du hast einen Screenshot gemacht und er liegt in deiner unsichtbaren Zwischenablage? 
Wähle _Insert image from clipboard_. Ein Speichern-Dialog öffnet sich. Gib dem Bild einen Namen. Die Extension speichert das Bild aus dem Arbeitsspeicher auf deine Festplatte und fügt den Code mit dem relativen Pfad sofort ins Dokument ein.
Hinweis: Damit der Pfad zur adoc Datei passt, muss du sie zuerst speichern.

## 5. Bild aus dem Internet herunterladen (Insert image URL)
Kopiere die URL eines Bildes (z. B. _https://beispiel.de/bild.png_) in die Zwischenablage. Wähle _Insert image URL_. 
Die Extension lädt das Bild aus dem Internet herunter, fragt dich, wo du es lokal abspeichern möchtest, und fügt es dann mit der Angabe der ursprünglichen Quelle ins Dokument ein. So gehen keine Bilder verloren, falls die Website später offline geht.
Hinweis: Damit der Pfad zur adoc Datei passt, muss du sie zuerst speichern.

## 6. AsciiDoc Tabelle als TSV in die Zwischenablage kopieren
Markiere eine Tabelle in AsciiDoc mit dem Start- und Endzeichen (_|===_).
Im Kontextmenü gibt es den Punkt _Copy AsciiDoc Table as TSV to clipboard_.
Er kopiert die Tabelle als Tab getrennte Tabelle in die Zwischenablage.
Diese Daten können dann z. B. in Excel eingefügt werden.

## 7. Datei direkt als Code-Block importieren (File Explorer Feature)
Das ist das mächtigste Feature für Programmierer: 
Gehe in der _linken Dateibaum-Ansicht_ von VS Code (File Explorer) auf eine Code-Datei (z. B. _.cs_, _.java_, _.py_). Mache einen Rechtsklick _auf die Datei_ und wähle _Insert as source block_. 
Die Extension liest die komplette Datei ein, erkennt die Programmiersprache automatisch, berechnet den relativen Pfad und fügt in deinem aktuell geöffneten AsciiDoc-Dokument einen klickbaren Link zur Datei samt dem Quellcode ein.
Hinweis: Damit der Pfad zur adoc Datei passt, muss du sie zuerst speichern.

## 8. Dateien eines Verzeichnisses in die Zwischenablage kopieren
Gerade für KI Prompts wird der Source Code im Contextwindow benötigt.
Beim Klicken auf ein *Verzeichnis* im File Explorer erscheint ein Menüpunkt _Copy sources to clipboard_.
Wenn du den aktuellen Ordner kopieren möchtest, kannst du im Explorer auf den Button neben dem Verzeichnisnamen klicken (siehe Screenshot).
Achte beim Prompten, ob auch der ganze Code kopiert wurde.
Gerade im Free Plan ist das Contextwindow nur sehr begrenzt.


### Konfiguration

Vor jedem Start fragt die App, welche Erweiterungen berücksichtigt werden sollen.
Die Vorbelegung wird aus der Datei _settings.json_ gelesen (_includeExtensions_).

Für die Erweiterungen _docx_ und _pdf_ ist ein Extraktor vorhanden (_mammoth_ für Worddateien, _pdfreader_ für PDF Dateien).
Um auch diese Dateien zu kopieren, musst du die Erweiterungen _docx_ und _pdf_ in den Einstellungen hinzufügen bzw. vor dem Kopieren eingeben.

Es werden keine Dateien gelesen, die über 10 MB groß sind.

Die folgenden Einstellungen können in der _settings.json_ Datei gesetzt werden (Beispiele):

```json
"asciidoc-productivity.includeExtensions": "cs|csproj|java|rb|json|js|ts|jsx|tsx|py|txt|xml|adoc|md|cmd|sh|sql|yaml|puml",
"asciidoc-productivity.excludeDirectories": ["bin", "obj", "node_modules", "TestResults"],
"asciidoc-productivity.excludeFiles": ["package-lock.json"],
```

## Erweitern und Erstellen der VSIX Datei

Im auf https://github.com/Die-Spengergasse/course-pos-csharp_basics/tree/master/07%20Asciidoctor/asciidoc-productivity befindet sich der Quelltext.

**package.json:**
Definiert im Key _contributes_ die Menüeintrage und verweist auf die Methoden in der Extension.

**src/extension.ts:**
Die eigentliche Extension.
Sie wird zu Beginn geladen.
Bei einem Klick auf das Menü wird die entsprechende Methode aufgerufen.

**src/EditorService.ts:**
Um die Methoden des Editors zusammenzufassen, gibt es hier eine klassische Servicedatei.

**src/ConfigurationService.ts:**
Liest die Konfiguration aus der Datei _settings.json_ und stellt diese der Applikation bereit.

## Testen

Wenn du mit _Open Folder_ das Verzeichnis der Extension öffnest, kannst du einfach mit _F5_ oder _Run -> Start Debugging_ ein VS Code Fenster öffnen und deine Extension zuerst einmal testen.

## Exportieren in eine VSIX Datei

Um die Extension selbst zu bauen und in Visual Studio Code (VS Code) zu installieren, wird das globale Node.js Tool _@vscode/vsce_ benötigt.
Du kannst es mit _npm install -g @vscode/vsce_ in der Konsole installieren.
Gehe nun in den Ordner der Extension (dort, wo die _package.json_ liegt).
Führe dann folgenden Befehl aus, um die fertige Installationsdatei zu generieren:

```bash
vsce package --allow-missing-repository
```

Nach dem Durchlauf findest du in deinem Ordner eine neue Datei mit der Endung _.vsix_ (z. B. _asciidoc-productivity-1.0.0.vsix_).
Diese kannst du nun in VS Code installieren.

