# Seeden einer Datenbank mit Musterdaten

In diesem Programm wird die Datenbank mit den Semesterprüfungen automatisch mit Werten befüllt.
Dabei wird das Paket [Bogus](https://github.com/bchavez/Bogus) verwendet. Es ist eine .NET Portierung
von faker.js und beinhaltet Datasets für Namen, Vornamen, Adressen, ...

In der Solution wird die Semesterprüfungs Datenbank mit folgendem Schema erstellt:

![](datenmodell_sempruef.png)

Die Erklärung und die Befüllung von Klassen und Schülern ist in den Kommentaren der Datei
[Program.cs](Program.cs) enthalten.

## Übung

Befülle die restlichen Tabellen (*Lehrer*, *Fach* und *Sempruef*) mit Werten. Die Achte darauf, dass für
Felder die NULL werte enthalten können, auch solche Werte in z. B. 20 % der Fälle generiert werden.
