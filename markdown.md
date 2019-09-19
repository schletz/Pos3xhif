# Dokumente erstellen mit Markdown
Markdown ist eine Auszeichnungssprache, die mit minimalen Formatierungsanweisungen auskommt. Sie ist
primär für die Ausgabe im Browser (also HTML) gedacht. Daher fehlen seitenbasierende Anweisungen wie
Seitennummern, Umbrüche, Abstandssteuerungen, etc.

Als Programmierer ist dies die "Hauptsprache", wenn es um formatierte Dokumente geht. Codeblöcke können
mit der entsprechenden Programmiersprache gekennzeichnet werden. Dadurch wird automatisch eine Syntaxhervorhebung
entsprechend dieser Sprache generiert.

Markdown ist nicht geeignet, wenn in die Formatierung durch die Angabe von Schriftgrößen, Abständen, etc.
eingegriffen werden soll. Es ist für all jene gedacht, die sich nicht viel mit Formatierungen auseinander
setzen wollen und denen das Ergebnis der generierten Darstellung genügt.

## Die Markdown Syntax
Für einen Informatiker ist sie einfach zu lernen, mit dem PDF von https://guides.github.com/pdfs/markdown-cheatsheet-online.pdf
weiß man innerhalb weniger Minuten bescheid.

## Tools zur Anzeige: Browserextensions
In Github werden md Dokumente gerendert dargestellt. Um md Dokumente von der Festplatte unter Windows 
anzeigen zu können, gibt es diverse Extensions. Diese Extensions unterscheiden sich aber in ihren Markdown 
Features. Für Google Chrome bietet der [Markdown Reader](https://chrome.google.com/webstore/detail/markdown-reader/gpoigdifkoadgajcincpilkjmejcaanc)
ein gutes Syntax Highlighting und einen Index an. Außerdem wird die Darstellung bei jedem Speichervorgang
automatisch aktualisiert

Wichtig ist die Konfiguration der Extension. Sie muss auf lokale Datei-URLs zugreifen können, um die
Dateien von der Festplatte darstellen zu können. In Chrome kann das unter *Einstellungen verwalten* im
Menü der Markdown Reader Extension eingestellt werden (Punkt *Zugriff auf Datei-URLs zulassen*).

## Erstellen mit Visual Studio Code
In Visual Studio Code können md Dokumente ohne Extensions erstellt und angezeigt werden. Klickt man
bei einer md Datei mit der rechten Maustaste auf den Reiter der Datei, kann mittels *Open Preview*
(oder *CTRL+SHIFT+V*) eine Vorschau angezeigt werden.

## Exportieren in ein PDF
Mit der Software [Pandoc](https://pandoc.org/installing.html) können md Dokumente in PDF (über LaTeX),
Word oder HTML umgewandelt werden. Um aus Visual Studio Code diese Dokumente erzeugen zu können, gibt
es die Extension *vscode-pandoc*. Nun kann in einem md Dokument mit dem Shortcut *CTRL+K* und danach *P*
die Umwandlung in PDF, HTML oder Word gestartet werden.

### Konfigurieren der Extension vscode-pandoc
In Visual Studio Code können mittels *File* - *Preferences* - *Settings* die Einstellungen verändert
werden. Gibt man in das Suchfeld *pandoc* ein, werden die speziellen Optionen für Pandoc angezeigt.
Für *Pandoc: Pdf Opt String* kann folgendes eingefügt werden:
```
-V documentclass=article -V geometry:a4paper -V geometry:margin=2.5cm --highlight=tango
```

Dies aktiviert die LaTeX Klasse article (geeignet für kleine Dokumente), setzt die Papiergröße auf
A4, die Ränder auf 2.5cm und verwendet für die Syntaxhervorhebung das Farbschema tango (graußer Hintergrund).

Gibt man in das Suchfeld der Einstellungen *ruler* ein, kann in der Datei settings.json mit 
```javascript
"editor.rulers": [ 100 ]
```
ein rechter Rand nach 100 Zeichen angezeigt werden.

In md Dokumenten kann am Anfang der Datei mittels yaml noch gesteuert werden, welche Werte für Titel,
Autor und Datum von LaTeX verwendet werden:
```
---
title: Titel des Dokumentes
author: Max Mustermann
date: 1. Jänner 2019
---
```
