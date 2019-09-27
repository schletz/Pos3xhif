# Projektionen, anonyme Typen und var


Liefere eine Liste aller Prüfungsfächer als *IEnumerable&lt;string&gt;*. Mit *Select()* kann der zurückgegebene
Typ jedes Listenelements definiert werden. Select nennt man deswegen auch "Projektion".
```c#
// Liste beinhaltet D, E, E, AM, D, AM, ...
IEnumerable<string> uebung1 = db.Pruefungen.Select(p => p.Fach);
// Liste beinhaltet D, E, AM, POS, DBI (jedes Fach nur 1x)
IEnumerable<string> uebung2 = db.Pruefungen.Select(p => p.Fach).Distinct();
```

Liefere eine Liste aller Schüler mit der Anzahl der Prüfungen als Objekt Name, Vorname, Anzahl
Der Compiler legt eine anonyme Klasse an:
```
class A {
  string Name {get;set;}
  string Vorname {get;set;}
  int Anzahl {get;set;}
}
```

Da dieser Typ keinen Namen hat, wird das Schlüsselwort *var* verwendet. Der Compiler weist der Variable
beim kompilieren (nicht zur Laufzeit!) den Typ zu. Der Typ der Variable kann danach nicht mehr geändert
werden.
```c#
var uebung3 = db.Schuelers.Select(s => new
{
      s.Name,
      s.Vorname,
      Anzahl = s.Pruefungen.Count   Propertynamen festlegen
}).OrderBy(x => x.Anzahl).ThenBy(x => x.Name);
// Funktioniert nicht:
// uebung3 = "Ein String".
```

Liefere ein JSON Objekt mit folgendem Aufbau:
```
{
   Name: Mustermann,
   Vorname: Max,
   Pruefer: [KY, FAV]
},...
```
```c#
var uebung4 = db.Schuelers.Select(s => new
{
      s.Name,
      s.Vorname,
      Pruefer = s.Pruefungen.Select(p => p.Pruefer).Distinct()
});
WriteJson(uebung4, "Beispiel 5 - Schüler mit Prüfer", true);
```

Liefere ein JSON Objekt mit folgendem Aufbau:
```
{
   Name: "Mustermann,"
   Vorname: "Max",
   db.Pruefungen: [{"Pruefer"="KY", "Fach"="AM"}, ...]
},...
```

```c#
var uebung5 = db.Schuelers.Select(s => new
{
      s.Name,
      s.Vorname,
      Pruefungen = s.Pruefungen.Select(p => new
      {
         p.Pruefer,
         p.Fach
      })
});
WriteJson(uebung5, "Beispiel 6 - Schüler mit Prüfungen", true);
```

## Übung
Öffne das Projekt in *LinqUebung2/LinqUebung2.App.csproj*. Die Angaben sind in der Datei *Program.cs*,
beim Ausführen lautet die korrekte Ausgabe in der Konsole:
```
Die Prüfungsfächer sind D,AM,DBI,POS,E
Die schlechteste E Note ist 4
Beispiel 3
   3CHIF: Calladine Clémence hat 3 Prüfungen.
   3CHIF: Clearley Åsa hat 4 Prüfungen.
   3CHIF: Curtin Maëline hat 4 Prüfungen.
   3CHIF: Cuseick Cléa hat 3 Prüfungen.
   3CHIF: Dibden Maéna hat 6 Prüfungen.
   3CHIF: Domanek Noémie hat 6 Prüfungen.
   3CHIF: Kynge Valérie hat 4 Prüfungen.
   3CHIF: McComiskey Léa hat 3 Prüfungen.
   3CHIF: Minerdo Laurélie hat 2 Prüfungen.
   3CHIF: Santori Céline hat 0 Prüfungen.
   3CHIF: Spurnier Stéphanie hat 2 Prüfungen.
   3CHIF: Wilton Lèi hat 1 Prüfungen.
   3CHIF: Works Styrbjörn hat 2 Prüfungen.
Beispiel 4
   3BHIF: Nayshe Eliès hat D,DBI,E,POS
   3BHIF: Avramovitz Chloé hat AM,D,DBI
   3BHIF: Pinder Jú hat D
   3BHIF: Billson Eléa hat E
   3BHIF: Gianulli Léonie hat
   3BHIF: Dixon Personnalisée hat E,POS
   3BHIF: Jeandin Maïté hat AM,D,DBI,POS
Beispiel 5
   Negative Prüfungen von Elt Célia:
   Negative Prüfungen von Mattack Loïca:
      POS am 28.11.2017
   Negative Prüfungen von Riseborough Lauréna:
   Negative Prüfungen von Stiff Maéna:
      POS am 18.06.2018
   Negative Prüfungen von Elbourn Josée:
   Negative Prüfungen von Fosdike Kallisté:
   Negative Prüfungen von Dunstall Lyséa:
      POS am 24.10.2017
   Negative Prüfungen von Sharpe Béatrice:
   Negative Prüfungen von Browne Esbjörn:
      AM am 09.06.2018
   Negative Prüfungen von Castellan Léa:
      POS am 28.09.2017
Beispiel 6
   KY gibt Noten von 1 bis 5
   FZ gibt Noten von 1 bis 5
   SZ gibt Noten von 1 bis 5
   FAV gibt Noten von 1 bis 4
   NAI gibt Noten von 1 bis 5
```