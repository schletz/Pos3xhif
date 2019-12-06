<!-- markdownlint-disable MD045 -->
# Join und Gruppierungen

## Join über Navigation Properties

In Datenbanken ist es üblich, mehrere Tabellen zu verknüpfen. Möchten wir z. B. eine Liste aller
Schüler mit ihrem Klassenvorstand und der Abteilung ausgeben, so wird dies in SQL mit der *JOIN*
Klausel ausgeführt:

```sql
SELECT C_Department, C_ID, C_ClassTeacher, P_Lastname, P_Firstname
FROM Schoolclass INNER JOIN Pupil ON (C_ID = P_Class)
ORDER BY C_Department, C_ID, P_Lastname, P_Firstname;
```

Das Ergebnis ist eine flache Tabelle:

| C_Department | C_ID  | C_ClassTeacher | P_Lastname  | P_Firstname  |
| ------------ | ----- | -------------- | ----------- | ------------ |
| HIF          | 1AHIF | NIJ            | Aktas       | Nilsu        |
| HIF          | 1AHIF | NIJ            | Beslic      | Laura        |
| HIF          | 1AHIF | NIJ            | Bice        | Melek        |
| HIF          | 1AHIF | NIJ            | Bleier      | Sarah        |
| HIF          | 1AHIF | NIJ            | Breitwieser | Michelle-Lea |
| HIF          | 1AHIF | NIJ            | Brodkorb    | Ilaria       |
| HIF          | 1AHIF | NIJ            | El Ibrahim  | Jasmin       |

In LINQ könnten wir folgendes Statement schreiben:

```c#
from p in data.Pupil
select new
{
    p.P_ClassNavigation.C_Department,
    p.P_ClassNavigation.C_ID,
    p.P_ClassNavigation.C_ClassTeacher,
    p.P_Lastname,
    p.P_Firstname
};
```

Dies setzt allerdings voraus, dass eine Navigation zwischen *Pupil* und *Schoolclass* (hier
*P_ClassNavigation* existiert). Falls wir allerdings nur eine einseitige Navigation in Form einer
Liste zwischen den Klassen haben (*Pupils*), können wir unser LINQ Statement auch so schreiben:

```c#
from c in data.Schoolclass
from p in c.Pupils
select new
{
    c.C_Department,
    c.C_ID,
    c.C_ClassTeacher,
    p.P_Lastname,
    p.P_Firstname
};
```

Wichtig ist hier die Verwendung von *c.Pupils* in der zweiten *from* Klausel. Durch diesen Ausdruck
wird das Join Kriterium in SQL realisiert. Es entspricht zwei geschachtelten *foreach* Schleifen:

```c#
foreach (Schoolclass c in data.Schoolclass)
    foreach(Pupil p in c.Pupils)
    { /* Do something */}
```

Ein häufiger Fehler ist folgendes Konstrukt:

```c#
from c in data.Schoolclass
from p in data.Pupil          // Hier werden alle Schüler abgefragt
select new
{
    c.C_Department,
    c.C_ID,
    c.C_ClassTeacher,
    p.P_Lastname,
    p.P_Firstname
};
```

Hier wird für jede Klasse jeder Schüler, und zwar unabhängig seiner Klasse, zurückgegeben. Es
entspricht dem *CROSS JOIN* und gibt n x m Datensätze zurück.

### Der händische Join in LINQ

Wir haben bis jetzt Navigations benutzt, um die Instanzen zu verknüpfen. Falls keine Navigations
vorhanden sind, gibt es in LINQ auch den Join Ausdruck. Etwas gewöhnungsbedürftig ist hier das
Verwenden von equals statt dem C# Vergleichsoperator (*==*).

```c#
from c in data.Schoolclass
join p in data.Pupil on c.C_ID equals p.P_Class
select new
{
    c.C_Department,
    c.C_ID,
    c.C_ClassTeacher,
    p.P_Lastname,
    p.P_Firstname
};
```

## Gruppierungen

### Gruppierung in SQL

In SQL gibt es mit der *GROUP BY* Klausel die Möglichkeit, die Daten in Gruppen zu unterteilen.
Betrachten wir unsere Tabelle *Lesson*. Wir können nach verschiedenen Kriterien gruppieren:

![](grouping01.png)

Der Zweck von Gruppierungen ist meist das Aggregieren von Daten. In SQL stehen uns die
Aggregatfunktionen *COUNT()*, *MAX()*, *MIN()*, ... zur Verfügung. Diese werden dann pro Gruppe von der
Datenbank ausgewertet:

![](grouping02b.png)

> **Hinweis:** Natürlich kann nach einer Gruppierung nicht mehr auf die einzelnen Werte zugegriffen
> werden, die nicht Teil der Gruppierung sind. Viele versuchen verzweifelt, bei einer Gruppierung
> nach der Klasse noch den Gegenstand auszugeben. Das kann natürlich nicht funktionieren, da alle
> Datensätze der Klasse bereits zu einem Wert zusammengefasst wurden. Werte, nach denen nicht gruppiert
> wird, müssen immer in einer Aggregatsfunktion stehen.

### Gruppierung in LINQ

Nach diesem Ausflug in SQL sehen wir uns die Implementierung in LINQ an.

#### Technische Umsetzung mit IGrouping

Im Namespace *System.Linq* ist ein Interface *IGrouping* definiert, welches folgende Eigenschaften
hat:

```c#
public interface IGrouping<out TKey, out TElement> : IEnumerable<TElement>, IEnumerable
{
    TKey Key { get; }
}
```

Dieses Interface erweitert offensichtlich eine normale Collection (*IEnumerable*) um ein Property *Key*.
In diesem Property ist der Wert, nach dem gruppiert wird gespeichert. Ein konkretes Beispiel erklärt
den Einsatz am Besten:

```c#
from l in data.Lesson
group l by l.L_Class into g
select new
{
    Class   = g.Key,
    Count   = g.Count(),
    MaxHour = g.Max(x => x.L_Hour)
};
```

Verwirrend ist hier der Einsatz der Gruppierungsvariable *g*. Sie ist eine Instanz von *IGrouping* und
somit kann über das Property *Key* auf den Gruppierungsschlüssel zugegriffen werden. Möchte man auf
die Werte in der Gruppe zugreifen, wird einfach *g* verwendet. Dadurch funktioniert der Ausdruck
*g.Max(x => x.L_Hour)* und liefert die letzte Stunde pro Klasse.

![](igrouping.png)

Natürlich kann auch hier nicht auf einzelne Werte zugegriffen werden. Da *g* eine Collection ist,
funktioniert der Zugriff auf *g.L_Teacher* so wie auf *data.Lessons.L_Teacher* syntaktisch nicht
(eine Collection hat kein Property *L_Teacher*).

Möchte man nach mehreren Spalten gruppieren, wird mit *new* ein neuer Typ erzeugt:

```c#
from l in data.Lesson
group l by new { l.L_Class, l.L_Subject } into g
select new
{
    Class   = g.Key.L_Class,
    Subject = g.Key.L_Subject,
    Count   = g.Count(),
    MaxHour = g.Max(x => x.L_Hour)
};
```

## Übungen

Löse die Aufgaben in der Datei ![Program.cs](Program.cs) und schreibe deine Abfragen statt `(object)null;`
in die Variablen. Die korrekten Ausgaben sind unter dem Klassendiagramm aufgelistet.

![](classdiagram.png)

```text
RESULT1
[{"Department":"FIT","Count":7},
{"Department":"HBG","Count":9},
{"Department":"HIF","Count":23},
{"Department":"HKU","Count":5},
{"Department":"HMN","Count":10},
{"Department":"HWI","Count":11},
{"Department":"O","Count":1},
{"Department":"VIF","Count":5},
{"Department":"AIF","Count":12},
{"Department":"BIF","Count":9},
{"Department":"CIF","Count":6},
{"Department":"CMN","Count":6},
{"Department":"KIF","Count":6},
{"Department":"KKU","Count":4},
{"Department":"BKU","Count":2}]

RESULT2
[{"Department":"HIF","Count":23},
{"Department":"HWI","Count":11},
{"Department":"AIF","Count":12}]

RESULT3
[{"Teacher":"AH","Subject":"OPS","LastTest":"2020-04-30T00:00:00"},
{"Teacher":"BH","Subject":"NVS1","LastTest":"2020-07-02T00:00:00"},
{"Teacher":"BH","Subject":"OPS","LastTest":"2020-05-26T00:00:00"},
{"Teacher":"GC","Subject":"BWM1","LastTest":"2020-01-08T00:00:00"},
{"Teacher":"GC","Subject":"BWM2","LastTest":"2019-09-05T00:00:00"},
{"Teacher":"GC","Subject":"BWM3","LastTest":"2020-06-14T00:00:00"},
{"Teacher":"GRJ","Subject":"BAP","LastTest":"2019-11-11T00:00:00"},
{"Teacher":"GRJ","Subject":"POS1","LastTest":"2020-02-15T00:00:00"},
{"Teacher":"HAF","Subject":"BAP","LastTest":"2020-06-24T00:00:00"},
{"Teacher":"HIK","Subject":"DBI1","LastTest":"2020-02-15T00:00:00"},
{"Teacher":"HW","Subject":"IOT","LastTest":"2019-12-13T00:00:00"},
{"Teacher":"KSR","Subject":"GAD","LastTest":"2019-09-29T00:00:00"},
{"Teacher":"KUE","Subject":"PRE","LastTest":"2019-09-30T00:00:00"},
{"Teacher":"NAI","Subject":"Dx","LastTest":"2019-11-24T00:00:00"},
{"Teacher":"NAI","Subject":"Dy","LastTest":"2020-04-24T00:00:00"},
{"Teacher":"PC","Subject":"AMx","LastTest":"2020-04-01T00:00:00"},
{"Teacher":"PC","Subject":"AMy","LastTest":"2020-04-01T00:00:00"},
{"Teacher":"PUA","Subject":"IOT","LastTest":"2019-11-03T00:00:00"},
{"Teacher":"RA","Subject":"DBI1","LastTest":"2019-12-26T00:00:00"},
{"Teacher":"SH","Subject":"PRE","LastTest":"2020-04-07T00:00:00"},
{"Teacher":"SK","Subject":"GAD","LastTest":"2019-12-04T00:00:00"},
{"Teacher":"SO","Subject":"E1x","LastTest":"2020-01-01T00:00:00"},
{"Teacher":"SO","Subject":"E1y","LastTest":"2020-04-08T00:00:00"},
{"Teacher":"SWH","Subject":"NVS1","LastTest":"2020-01-16T00:00:00"},
{"Teacher":"SZ","Subject":"POS1","LastTest":"2020-02-22T00:00:00"},
{"Teacher":"TT","Subject":"PRE","LastTest":"2019-12-21T00:00:00"}]

RESULT4
[{"Room":"C2.09","Day":1,"Hour":16,"Teacher":"PUC","Class":"5BBIF"},
{"Room":"C2.09","Day":1,"Hour":16,"Teacher":"MAY","Class":"5BBIF"},
{"Room":"C2.10","Day":1,"Hour":16,"Teacher":"BIG","Class":"5ACIF"},
{"Room":"C2.06","Day":1,"Hour":10,"Teacher":"SO","Class":"4CHIF"},
{"Room":"C2.07","Day":1,"Hour":16,"Teacher":"HIK","Class":"7ACIF"},
{"Room":"C2.07","Day":1,"Hour":16,"Teacher":"BAM","Class":"7ACIF"},
{"Room":"C2.11","Day":1,"Hour":16,"Teacher":"HAU","Class":"7CBIF"},
{"Room":"C2.08","Day":1,"Hour":8,"Teacher":"KUM","Class":"1BHBGM"},
{"Room":"C2.14","Day":1,"Hour":16,"Teacher":"SB","Class":"1EVIF"}]
```
