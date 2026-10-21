# Übungen zum Kapitel Unittests

In dieser Übung schreibst du selbst Unit Tests mit xUnit. Du testest fertigen Code, den du nicht
ändern darfst, und findest mit deinen Tests die Fehler darin. Du brauchst dafür alle Kapitel von
*Warum testen?* bis *Testfälle finden*.

Die Übung ist ein 7z-Archiv mit einer Solution. Sie enthält ein Projekt mit dem Code und ein
Testprojekt, in das du deine Tests schreibst.

> [!TIP]
> **Die Angabe im Browser lesen**
>
> Die Angabe *README.md* ist eine Markdown-Datei. Mit einem Markdown Viewer siehst du sie im
> Browser formatiert, mit Überschriften, Code und Tabellen. Installiere dafür z. B. die Erweiterung
> [Markdown Viewer](https://chromewebstore.google.com/detail/markdown-viewer/ckkdlimhmcjmikdlpkmbgfkaikojcbjk)
> (für Chrome und Edge). **Wichtig:** Öffne danach *Erweiterung verwalten* und aktiviere die
> Option *Zugriff auf Datei-URLs zulassen*. Ohne diese Option darf die Erweiterung keine lokalen
> Dateien lesen. Danach ziehst du die *README.md* aus dem entpackten Ordner einfach in den Browser.

## BugHunt

Eine Parkgarage berechnet die Parkgebühr mit der Klasse *ParkingFeeCalculator*. Der vorhandene
Test ist grün, trotzdem stecken im Code genau 4 Fehler. Du stellst aus den Tarifregeln
Äquivalenzklassen und Grenzwerte auf und weist jeden Fehler mit einem roten Test nach. Du übst
*[Fact]*, *[Theory]* mit *[InlineData]*, *Assert.Throws* und das systematische Finden von
Testfällen.
Lade [BugHunt.7z](BugHunt.7z) herunter. Die Angabe steht im Archiv in der Datei *README.md*.
