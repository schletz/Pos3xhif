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
