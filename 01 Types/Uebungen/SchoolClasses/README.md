# Schulklassen und Schüler

Eine Schule verwaltet ihre Klassen. Jede Klasse hat eine Liste von Schülern, und jeder Schüler
kennt seine Klasse. Die Referenz zeigt also in beide Richtungen. Deine Klassen müssen dafür sorgen,
dass beide Seiten immer zusammenpassen: Ein Schüler sitzt in genau einer Klasse oder in keiner.

## Klassenbeschreibung

Private Felder, die du für die Implementierung brauchst, legst du selbst an.

#### Klasse SchoolClass

- **SchoolClass(string name, string classTeacher)** setzt *Name* und *ClassTeacher*. Beide
  Properties sind read-only.
- **Students** liefert die Schüler als `IReadOnlyList<Student>`, in der Reihenfolge, in der sie
  hinzugefügt wurden. Speichere die Schüler intern in einer `private readonly` Liste und gib sie mit
  `AsReadOnly()` zurück. Von außen darf niemand Schüler hinzufügen oder entfernen, ohne
  *AddStudent()* oder *RemoveStudent()* aufzurufen.
- **Cities** ist read-only und liefert ein neues `HashSet<string>` mit den Wohnorten aller
  Schüler. Jeder Ort kommt nur einmal vor. Ein Schüler kann umziehen (*City* ändert sich). Berechne
  das HashSet daher bei jedem Aufruf neu, statt es zu speichern.
- **AddStudent(Student student)** fügt den Schüler am Ende der Liste ein und setzt
  *student.SchoolClass* auf diese Klasse.
  - Ist der Schüler schon in dieser Klasse, passiert nichts.
  - Ist der Schüler in einer anderen Klasse, entfernt ihn die Methode vorher aus der alten Klasse.
- **RemoveStudent(Student student)** entfernt den Schüler, setzt *student.SchoolClass* auf
  *null* und liefert *true*. Ist der Schüler nicht in dieser Klasse, ändert die Methode nichts und
  liefert *false*. Tipp: Sieh dir den Rückgabewert von `List.Remove()` an.

#### Klasse Student

- **Student(int id, string firstname, string lastname, string city)** setzt die 4 Properties.
  *Id*, *Firstname* und *Lastname* sind read-only. *City* hat eine public set Methode.
- **SchoolClass** zeigt auf die Klasse des Schülers. Ein neuer Schüler ist in keiner Klasse, das
  Property hat dann den Wert *null*. Überlege dir den Datentyp.
  - Die Klasse *SchoolClass* muss das Property setzen können, andere Projekte sollen es nur
    lesen. Verwende dafür `internal set`. *internal* bedeutet: Nur Code im selben Projekt darf die
    set Methode aufrufen.
  - Schreibe `[JsonIgnore]` (Namespace *System.Text.Json.Serialization*) über das Property.
    Sonst kann man die Schüler nicht als JSON ausgeben: Der Serializer würde vom Schüler zur
    Klasse, von der Klasse zu ihren Schülern und wieder zur Klasse gehen, ohne Ende.
- **ChangeClass(SchoolClass newClass)** wechselt in die neue Klasse. Danach ist der Schüler nicht
  mehr in der alten Klasse, aber am Ende der Liste der neuen Klasse. Überlege, welche Methode diese
  Arbeit schon erledigt.

## Durchzuführende Arbeiten

Öffne die Solution *SchoolClasses.sln* in diesem Ordner. Lege jede Klasse in einer eigenen
Datei an und verwende den Namespace *SchoolClasses.Application*. Die Dateien *Program.cs* und
*TestHelpers.cs* darfst du nicht ändern.

*Program.cs* ist ein Testprogramm. Es kompiliert erst, wenn alle Klassen mit allen Properties und
Methoden existieren. Lege daher zuerst alle Klassen an und lass die Methoden eine
*NotImplementedException* werfen. Danach startest du das Programm im Ordner
*SchoolClasses.Application* mit `dotnet run`. Es zeigt dir bei jeder Prüfung *OK* oder *FAILED*.
Wirft eine Prüfung eine Exception, gilt sie als nicht erfüllt.

Eine vollständige Lösung liefert folgende Ausgabe:

```text
Checking class implementation
   (1) OK: SchoolClass does not have a default constructor.
   (2) OK: Student does not have a default constructor.
   (3) OK: SchoolClass.Name and SchoolClass.ClassTeacher are read-only.
   (4) OK: Student.Id, Firstname and Lastname are read-only, City can be changed.
   (5) OK: SchoolClass.Students is IReadOnlyList<Student> and uses AsReadOnly().
   (6) OK: SchoolClass.Cities is a read-only property of type HashSet<string>.
   (7) OK: Student.SchoolClass has an internal set method.
Checking constructors
   (8) OK: Constructor of SchoolClass sets the values, a new class has no students.
   (9) OK: Constructor of Student sets the values, a new student has no class.
Checking AddStudent and RemoveStudent
   (10) OK: AddStudent adds the students in the correct order and sets SchoolClass.
   (11) OK: AddStudent ignores a student who is already in this class.
   (12) OK: Cities returns every city once and is up to date after a change of City.
   (13) OK: RemoveStudent removes the student and sets SchoolClass to null.
   (14) OK: RemoveStudent does not change a student of another class.
Checking ChangeClass
   (15) OK: ChangeClass moves the student to the end of the new class.
   (16) OK: ChangeClass works for a student without a class.
   (17) OK: AddStudent removes the student from the old class first.
   (18) OK: Students can be serialized to JSON without the class ([JsonIgnore]).

Result of the program in C:\...\SchoolClasses.Application:
18 of 18 checks passed.
23 of 23 points. Grade: Sehr gut.
```
