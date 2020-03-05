# ML.NET Demo

```text
// *************************************************************************************************
// DEMOPROGRAMM FÜR DIE ORTSBESTIMMUNG ÜBER EINE FELDSTÄRKENMESSUNG
// *************************************************************************************************
// Unser virtuelles Stockwerk hat folgende Räume und Positionen der Accesspoints
//
//   AP1 (0|30)
//    +-------------------+
//    |                   |
//    | C3.01             |
//    | (0|20) - (10|30)  |
//    |                   |
//    +-------------------+ AP2 (10|20)
//    |                   |
//    | C3.02             |
//    | (0|10) - (10|20)  |
//    |                   |
//    +-------------------+
//    |                   |
//    | C3.03             |
//    | (0|0) - (10|10)   |
//    |                   |
//    +-------------------+
//   AP3 (0|0)
//
// Dieses Programm generiert verteilte Punkte in diesem Stockwerk und simuliert eine gemessene
// Feldstärke. Diese Feldstärke wird mit der Formel 100 * 1 / DIST berechnet, wobei DIST die
// Entfernung zum Accesspoint ist.
```


## Ergänzungen

**(1)** Im Programm wird das Modell durch die Zeile

```c#
var testData = trainData;
```

mit den Trainingsdaten getestet. Das soll natürlich nicht so sein. Generiere deshalb neue
Zufallsdaten (100). Dafür schreibe eine Methode *List<Measurement> GenerateMeasurements(int count)*.

**(2)** Zähle, wie viele der 100 generierten Testwerte korrekt klassifiziert wurden.

**(3)** Als Alternative verwende folgende Überlegung: Berechne pro Raum den Durchschnittswert pro
Accesspoint. Definiere danach dein Modell zur Raumfindung so: Für ein übergebenes Array an Messwerten
wird der nächste Wert aus dieser Durchschnittstabelle gesucht. Der nächste Wert ist der RMS Wert
der Differenzen für jeden Accesspoint.
