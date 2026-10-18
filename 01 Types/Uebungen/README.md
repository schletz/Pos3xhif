# Übungen zum Kapitel Types

In diesen Übungen setzt du ein Klassenmodell in C# um. Das Ziel ist ein objektorientiertes Design,
bei dem das Typsystem Fehler gar nicht erst zulässt: read-only Properties, abstrakte Klassen,
Interfaces und Collections, die man von außen nicht verändern kann. Du brauchst dafür alle Kapitel
von *Wertetypen* bis *Interfaces*.

Jede Übung ist ein 7z-Archiv mit einer Solution. Sie enthält ein Testprogramm (*Program.cs*), das
deine Implementierung prüft und dir am Ende die erreichten Punkte zeigt. Musterlösungen gibt es
bewusst nicht: Das Testprogramm sagt dir, ob deine Lösung stimmt.

> [!TIP]
> **Die Angabe im Browser lesen**
>
> Die Angabe *README.md* ist eine Markdown-Datei. Mit einem Markdown Viewer siehst du sie im
> Browser formatiert, mit Überschriften, Code und Klassendiagramm. Installiere dafür z. B. die Erweiterung
> [Markdown Viewer](https://chromewebstore.google.com/detail/markdown-viewer/ckkdlimhmcjmikdlpkmbgfkaikojcbjk)
> (für Chrome und Edge). **Wichtig:** Öffne danach *Erweiterung verwalten* und aktiviere die
> Option *Zugriff auf Datei-URLs zulassen*. Ohne diese Option darf die Erweiterung keine lokalen
> Dateien lesen. Danach ziehst du die *README.md* aus dem entpackten Ordner einfach in den Browser.

## Anmeldesystem

Bewerber für einen Schulplatz werden mit ihren Noten erfasst. Je nach Abteilung prüft ein
austauschbarer *Grade Checker* (ein Interface mit zwei Implementierungen), ob die Noten für die
Aufnahme reichen. Ein aufgenommener Bewerber wird durch eine abgeleitete Klasse ersetzt, die
über einen *protected* Copy-Konstruktor entsteht. Du übst Interfaces, Vererbung und Dictionaries.
Lade [Anmeldesystem.7z](Anmeldesystem.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.

## Blog

Ein kleines Blogsystem mit Text- und Bildposts. Die abstrakte Klasse *Post* gibt ein abstraktes
Property *Html* vor, das jede Unterklasse anders umsetzt. Posts verwalten Kommentare in einer
*IReadOnlyList* und erlauben pro E-Mail-Adresse nur ein Rating. Du übst abstrakte Klassen,
berechnete Properties und nullable Werttypen (*decimal?*).
Lade [Blog.7z](Blog.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.

## CarRentalService

Ein Autoverleih vermietet Motorräder und Autos. Der Mietpreis wird je nach Fahrzeugart anders
berechnet: Motorräder nur nach Tagen, Autos zusätzlich nach Kilometern. Ein Mietvorgang ist offen,
solange Ende und Kilometerstand *null* sind. Du übst abstrakte Methoden, Polymorphismus und den
Umgang mit *DateTime?* und *int?*.
Lade [CarRentalService.7z](CarRentalService.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.

## HealthChecker

Beschäftigte legen Nachweise vor: Test, Genesung oder Impfung („3G am Arbeitsplatz“ aus dem Jahr
2021). Jeder Nachweis implementiert das Interface *ISafetyCheck* und hat eigene Regeln für die
Gültigkeit. Das Datum der zweiten Impfung darf nur einmal und nur mit gültigem Wert gesetzt
werden. Du übst Interfaces, Datumsberechnungen und Properties mit Logik im Setter.
Lade [HealthChecker.7z](HealthChecker.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.

## Kalender

Ein Kalendersystem mit privaten Terminen und Meetings. Beim Eintragen eines Termines wird auf
Überschneidungen geprüft. Zu Meetings kann man andere User einladen, und beim Absagen muss das
Meeting aus mehreren Kalendern verschwinden. Du übst Vererbung, gegenseitige Referenzen zwischen
Objekten und den Parametertyp als Schutz vor falschen Aufrufen.
Lade [Kalender.7z](Kalender.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.

## LibraryManager

Eine Bibliothek verleiht Bücher und Zeitschriften mit unterschiedlichen Fristen. Das späteste
Rückgabedatum wird aus dem Werk berechnet, und ein Kunde darf nur ein Werk gleichzeitig
entleihen. Am Ende listest du alle überfälligen Reservierungen auf. Du übst abstrakte Properties,
berechnete Properties und das Filtern von Listen.
Lade [LibraryManager.7z](LibraryManager.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.
