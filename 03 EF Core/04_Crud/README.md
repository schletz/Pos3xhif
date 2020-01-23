# CRUD (Create, Read, Update, Delete)

> **Wichtiger Hinweis für alle Punkte:** Ohne *SaveChanges()* werden keine Änderungen geschrieben!

## Einfügen einzelner Datensätze

Folgende Beispiele fügen einen neuen Schüler zur 3BHIF, die schon in der Datenbank steht, hinzu.

### Durch Einhängen in die Liste der 1:n Beziehung

```c#
try
{
    // Beachte: Keine Zuweisung der Klasse.
    Pupil newPupil = new Pupil { P_Account = "ZZZ9999", P_Firstname = "XXX", P_Lastname = "YYY" };

    // Da pupil sonst leer ist, müssen wir nachladen.
    // Wenn wir danach alle Schüler ausgeben wollen, ist es sinnvoll, Pupil auch zu laden:
    // var myClass = context.Schoolclass.Include(c => c.Pupil).Single(c => c.C_ID == "3BHIF");
    Schoolclass myClass = context.Schoolclass.Find("3BHIF");

    // Hier passiert noch nichts in der Datenbank
    myClass.Pupil.Add(newPupil);

    // INSERT INTO "Pupil" ("P_Account", "P_Class", "P_Firstname", "P_Lastname")
    // VALUES (@p0, @p1, @p2, @p3);
    // SELECT "P_ID"
    // FROM "Pupil"
    // WHERE changes() = 1 AND "rowid" = last_insert_rowid();
    context.SaveChanges();

    // newPupil.P_ClassNavigation zeigt jetzt auf die Klasse
    Console.WriteLine($"Ich bin in der {newPupil.P_ClassNavigation.C_ID}");
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

Weitere Beispiele (vor allem das Einfügen ganzer Strukturen) sind auf
[Microsoft Docs - Saving Related Data](https://docs.microsoft.com/en-us/ef/core/saving/related-data)
zu finden.

### Durch Setzen des EntityState

```c#
try
{
    Pupil newPupil = new Pupil { P_Account = "ZZZ9999", P_Firstname = "XXX", P_Lastname = "YYY" };

    // Wichtig: Befülle die Navigation, nicht das Fremdschlüsselfeld. P_ClassNavigation wäre dann
    // auch nach dem Speichern null.
    newPupil.P_ClassNavigation = context.Schoolclass.Find("3BHIF");

    // newPupil ist dem OR Mapper nicht bekannt. Ohne diese Anweisung würde newPupil nicht beachtet
    // werden, da er wie eine beliebige andere Variable einmal im Speicher liegt.
    context.Entry(newPupil).State = EntityState.Added;

    // Hier wird newPupil.P_Class (das Schlüsselfeld) auf "3BHIF" gesetzt.
    context.SaveChanges();
    Console.WriteLine($"Ich bin in der {newPupil.P_ClassNavigation.C_ID}");
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

## Aktualisieren vorhandener Daten

### Ändern von normalen Properties

Es wird einfach das Objekt über eine Abfrage gesucht und dem gewünschten Property wird der neue
Wert zugewiesen. Mit *SaveChanges()* wird die Änderung mittels *UPDATE* gespeichert.

```c#
try
{
    Pupil editPupil = context.Pupil.SingleOrDefault(p => p.P_Account == "ZZZ9999");
    // editPupil ist dem OR Mapper bekannt, daher können wir Properties ändern.
    editPupil.P_Account = "YYY0000";
    // Nicht vergessen!
    context.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

### Ändern von Navigationen

Soll die Beziehung geändert werden, so wird - wie beim Einfügen des Datensatzes nach Variante 2 -
der primäre Teil der Beziehung gesucht (hier die Klasse, da 1 Klasse n Schüler hat). Danach wird
die Navigation einfach auf diesen Wert gesetzt. Der Schüler wird automatisch aus der Liste der
alten Klasse entfernt.

```c#
try
{
    Pupil editPupil = context.Pupil.SingleOrDefault(p => p.P_Account == "ZZZ9999");
    // Ändern der Navigation auf die neue Klasse.
    editPupil.P_ClassNavigation = context.Schoolclass.Find("3AHIF");
    // Nicht vergessen!
    context.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

## Löschen vorhandener Daten

Natürlich müssen vor dem Löschen alle Einträge, wo das Element als Fremdschlüssel vorkommt, ebenfalls
entfernt werden. Es kann keine Klasse gelöscht werden, wo noch Schüler in der Tabelle *Pupil* darauf
verweisen.

### Remove

Soll ein Element gelöscht werden, so wird es gesucht und mit *context.Table.Remove()* entfernt.

```c#
try
{
    Pupil deletePupil = context.Pupil.SingleOrDefault(p => p.P_Account == "ZZZ9999");
    context.Pupil.Remove(deletePupil);
    context.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

## RemoveRange

```c#
try
{
    Schoolclass classToDelete = context.Schoolclass.Include(c => c.Pupil).SingleOrDefault(c => c.C_ID == "3BHIF");
    context.Pupil.RemoveRange(classToDelete.Pupil);
    context.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
{
    Console.Error.WriteLine(e.Message);
}
```

## Übung

In diesem Ordner gibt es eine SQLite Datenbank [Sempruef.db](Sempruef.db). Sie hat folgendes
Modell:

![](datenmodell_sempruef.png)

Alle auf *Id* endenden Spalten sind AUTOINCREMENT Werte.

**(1)** Erstelle eine neue Appliktation *SempruefCrud* und generiere die Modelklassen in den Ordner
        Model:

```text
md SempruefCrud
cd SempruefCrud
REM Die DB in diesen Ordner kopieren.
dotnet new console
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet ef dbcontext scaffold "DataSource=Sempruef.db" Microsoft.EntityFrameworkCore.Sqlite --output-dir Model --use-database-names --force --data-annotations
```

Achte darauf, dass der Autoincrement Wert *SP_Id* auch in den Modelklassen als serverseitig generierter
Wert definiert ist. Außerdem ist der korrekte Datentyp für den DATETIME Wert bei *SP_Datum*
(darf NULL sein) richtig einzugragen.

Vergiss nicht, dass die SQLite Datenbank mit *Copy if newer* in das Ausgabeverzeichnis kopiert wird.

**(2)** Bilde in der Datei *Program.cs* die folgenden Geschäftsfälle ab. Bei jedem Geschäftsfall ist
        der Datenbankcontext mit *using* neu zu öffnen:

```c#
using (SempruefContext db = new SempruefContext())
{
    // ...
}
```

1. Lege in der Tabelle *Fach* ein neues Fach *DBI* mit dem Fachnamen *Datenbanken* an.
2. Lege in der Tabelle Lehrer einen neuen Lehrer *SZ* (Michael Schletz) an.
3. Lege alle Semesterprüfungen, die noch nicht oder mit 5 beurteilt sind, mit Datum 24. Feb 2020
   ohne Note aber mit den selben Daten (Fach, Prüfer, Schüler) erneut an.
4. Aktualisiere alle Schüler ohne Geschlecht auf den Wert *m*. Hinweis: Am Besten löse dies mit
   einer klassischen *foreach* Schleife, die Filterung soll allerdings vorher geschehen.
5. Lösche die Klasse 3CHIF samt aller Daten aus dem System. Was ist zu Beachten, wenn bereits
   Semesterprüfungen eingetragen wurden?
6. Lege die Klasse 4AHIF mit dem Klassenvorstand SZ und folgenden Schülern an. Verwende dabei
   gleich eine Klasse mit einer fertig definierten Liste von Schülern, damit nur ein *Add* Befehl
   nötig ist. Details sind auf
   [Microsoft Docs - Saving Related Data](https://docs.microsoft.com/en-us/ef/core/saving/related-data)
   zu finden. Beachte, dass die Schülernummer kein Autowert ist. Überlege dir daher selbst eine Nummer.

| S_Nr | S_Zuname | S_Vorname    | S_Geschl |
| ---- | -------- | ------------ | -------- |
| 2001 | Quint    | Leggen       | m        |
| 2002 | Bobine   | O'Brallaghan | w        |
| 2003 | Ethelin  | Ferrini      | w        |
| 2004 | Andrej   | Osgardby     | m        |

7. Zum Testen einer Applikation sollen zufällige Semesterprüfungen für das Sommersemester eingetragen
   werden. Erstelle dafür 20 Datensätze. Gehe dabei so vor:

   - Für jeden Eintrag wähle zufällig einen Lehrer, ein Fach und einen Schüler aus den entsprechenden
     Tabellen aus.
   - Weise als Note eine Zufallszahl zwischen 1 und 5 zu, wobei in 20 % der Fälle die Note leer sein
     muss. Das kann mit `rnd.Next(0, 100) < 20 ? /* NULL */ : /* Note */` gelöst werden, wobei *rnd*
     die Instanz der *Random* Klasse ist.
   - Das Datum soll ab 10. Feb 2020 generiert werden. Zähle am Besten zu diesem Datum eine zufällige
     Anzahl an Tagen zwischen 0 und 100 dazu. Auch hier sollen 20 % der Datumswerte NULL sein.
   - Damit die Funktion deterministisch arbeitet (also immer die selben Daten liefert), initialisiere
     den Zufallszahlengenerator mit einem fixen Wert im Konstruktor.  

**(3)** Um die Datenbank zu prüfen, gib am Ende alle Tabellen mit den folgenden Statements als JSON aus.
        Öffne hierfür auch einen neuen Context:

```c#
using (SempruefContext db = new SempruefContext())
{
    Console.WriteLine("TABELLE Fach");
    Console.WriteLine(JsonSerializer.Serialize(db.Fach.ToList()));  // using System.Text.Json
    Console.WriteLine("TABELLE Klasse");
    Console.WriteLine(JsonSerializer.Serialize(db.Klasse.ToList()));
    Console.WriteLine("TABELLE Lehrer");
    Console.WriteLine(JsonSerializer.Serialize(db.Lehrer.ToList()));
    Console.WriteLine("TABELLE Schueler");
    Console.WriteLine(JsonSerializer.Serialize(db.Schueler.ToList()));
    Console.WriteLine("TABELLE Sempruef");
    Console.WriteLine(JsonSerializer.Serialize(db.Sempruef.ToList()));
}
```
