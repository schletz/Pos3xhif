# Listen in XAML

![](screenshot.png)

## Grundsätzliches

Oft sollen Collections in der Benutzeroberfläche dargestellt werden. Dies entsteht oft bei folgenden
Situationen:

- Eine Dropdownliste zur Filterung soll angezeigt werden.
- Eine Liste von Objekten soll zur Navigation angezeigt werden.
- Eine Liste von möglichen Werten soll in einem Eingabeformular angezeigt werden (z. B. männlich/weiblich).

Für die Darstellung von Listen gibt es in WPF mehrere Controls:

![List Types](ListTypes.png)
*<sup>Quelle: http://www.sws.bfh.ch/~amrhein/Skripten/Info2/, Kapitel 8: WPF Listen und Tabellen</sup>*

## Klassendiagramm der Model Klassen

Zur Demonstration werden Musterdaten generiert, die Schulklassen samt Schüler und Prüfungen
beinhalten:

![](classdiagram3.png)

## Warum DTO (data transfer object) Klassen?

Da wir schon mit dem *nullable reference types* von C# 8 arbeiten, haben unsere Modelklassen
alle Konstruktoren:

```c#
public class Student
{
    public Student(string firstname, string lastname,  Gender gender, Schoolclass schoolclass, DateTime? dateOfBirth = null)
    {
        Id = ++lastId;
        Firstname = firstname;
        Lastname = lastname;
        // ...
    }

    public int Id { get; set; }
    public string Firstname { get; set; }
    // ...
}
```

Beim Anlegen eines *neuen Schülers* ergibt sich im Zusammenspiel mit dem Binding
Mechanismus ein Problem: Unsere Eingabefelder für Vorname, Nachname, ... binden sich an eine
Instanz eines Schülers. Aber wie erzeugen wir diese Instanz? Werte haben wir ja noch keine.
Daher müssen wir im Code *new Student()* schreiben, was allerdings nicht möglich ist.

Um das Problem zu lösen, erstellen wir einen Ordner *Dto* und schreiben eine Klasse
*StudentDto*:

```c#
public class StudentDto
{
    public int? Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public Schoolclass? Schoolclass { get; set; }
}
```

Dies ist eine sogenannte POCO (plain old clr object) Klasse. Alle Felder sind *nullable*,
das bedeutet wir können mit *new StudentDto()* einen leeren Student für das Binding
erstellen.

> *Hinweis:* Gerade in der Webentwicklung sind DTO Klassen sehr wichtig. Sie dienen dazu,
> Formulardaten über einen POST Request zu serialisieren. Dafür wird nie das Model selbst
> verwendet, damit Felder, die nicht gesetzt werden dürfen (z. B. interne Rollen, ...) auch
> nie vom User überschrieben werden können.

### Mappings

Da wir die DTO Klasse für die Anzeige der Schüler verwenden, müssen wir zuerst aus einer
Instanz von *Student* eine Instanz von *StudentDto* erstellen. Das kann natürlich händisch
erfolgen, indem wir folgenden Code schreiben:

```c#
new StudentDto() 
{
    Firstname = student.Firstname,
    Lastname = student.Lastname,
    // ...
}
```

Das ist natürlich mühsam. Außerdem ist der Code sehr mechanisch: Er weist einfach Properties
mit dem selben Namen 1:1 zu. Abhilfe gibt es in NuGet. Das Paket 
**[Automapper](https://www.nuget.org/packages/AutoMapper/)** ist eines der bekanntesten
Pakete im .NET Bereich. Wir können es über die Paketverwaltung (*Manage NuGet Pakates* im
Kontextmenü des Projektes) oder direkt durch Editieren der Projektdatei
[ListDemo.csproj](ListDemo/ListDemo.csproj) installieren:

```xml
<ItemGroup>
    <PackageReference Include="AutoMapper" Version="11.*" />
</ItemGroup>
```

#### Konfigurieren von Automapper

Nun erstellen wir eine Klasse *DtoMappingProfile* im Ordner *Dto*. Diese Klasse muss
sich von der Klasse *Profile* (im Automapper Paket enthalten) ableiten.

```c#
class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<StudentDto, Student>()
            .BeforeMap((src, dst) =>
            {
                if (string.IsNullOrEmpty(src.Firstname)) { throw new ApplicationException("Invalid firstname."); }
                if (string.IsNullOrEmpty(src.Lastname)) { throw new ApplicationException("Invalid lastname."); }
                if (src.Gender is null) { throw new ApplicationException("Invalid gender."); }
                if (src.Schoolclass is null) { throw new ApplicationException("Invalid schoolclass."); }
            });
    }
}
```

Im Konstruktor definieren wir die gewünschten Mapping mit *CreateMap*. Die Typparameter
haben den Aufbau *\<source, destination\>*. So bedeutet
*CreateMap\<Student, StudentDto\>()* dass wir aus einer *Student* Klasse eine *StudentDto*
Klasse erstellen können. In der Standardkonfiguration weist Automapper einfach Properties
mit dem gleichen Namen zu.

Der umgekehrte Weg (von StudentDto zu Student) ist komplizierter. Schließlich dürfen in
der Klasse Student einige Felder nicht *null* sein. Daher müssen wir dies mit der Methode
*BeforeMap* prüfen. Hier kann jeder beliebige Code hineingeschrieben werden, es sollte
allerdings keine Businesslogik dort untergebracht werden.

In unserem Fall werfen wir eine *ApplicationException*, wenn die Daten unpassend sind. Diese
Exception kann dann im Code abgefangen werden.

> *Hinweis:* Strenggenommen wird hier eine *Validierung* durchgeführt. Heutige Frameworks
> wie ASP.NET Core setzen dafür ausgereiftere Konzepte um. Für WPF ist diese Methode
> aber ausreichend.

#### Registrieren des Mappers

Wenn unser Programm startet, müssen wir eine Instanz von Automapper erstellen. Dafür
verwenden wir die Klasse *App*, da sie gleich zu Beginn geladen wird.

```c#
public partial class App : Application
{
    public static readonly IMapper Mapper = 
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(DtoMappingProfile)))
        .CreateMapper();
}
```

Wir sehen eine etwas eigenartige Vorgehensweise: Wir definieren ein *statisches Feld*
(kein Property!) dafür. Die Ursache ist folgende: Statische Felder werden nur 1x
beim Start des Programmes initialisiert. Automapper muss über Reflection zuerst die
Properties der zu mappenden Typen auslesen. Das kostet recht viel Zeit und darf daher
nicht bei jedem Mapping gemacht werden. Durch das statische Feld wird die Zuordnungstabelle
gespeichert und kann schnell (ohne Reflection) abgerufen werden.

#### Die Map Methode

Der eigentliche Mappingvorgang ist recht einfach. Mit der Methode *Map\<DestType\>(src)*
wird das übergebene Objekt in den Typ, der als Typparameter angegeben wird, umgewandelt.

Die folgende Codezeile erstellt aus *studentDto* (eine Instanz von *StudentDto*) eine
Instanz von *Student*:

```c#
var student = App.Mapper.Map<Student>(studentDto);
```

Möchten wir eine ganze Liste umwandeln, können wir mit *IEnumerable* auch eine Liste
des Zieltyps erzeugen:

```c#
var studentDtos = App.Mapper.Map<IEnumerable<StudentDto>>(students)
```

## Die Combo Box (Dropdownliste)

In einer Combobox kann der Benutzer einen Wert auswählen. Dabei kann ein Standardwert gesetzt werden,
der beim Starten der Applikation gesetzt wird.

### Binding mit *ItemsSource* und *SelectedValue*

Die Combobox bezieht ihre Werte über die Eigenschaft *ItemsSource*. Sie verweist auf eine Liste im
ViewModel. Dort muss diese Liste als public Property vom Typ *IEnumerable\<T\>* (oder ein
abgeleiteter Typ wie *ICollection* oder *List*) definiert sein. Da meist komplexe Datentypen
verwendet werden
(Entityklassen wie *Pupil*, *Taecher*, ...) muss mit der Eigenschaft *DisplayMemberPath* festgelegt
werden, welches Property dieser Klasse dem Benutzer angezeigt wird.

Wählt der Benutzer nun ein Element aus, wird das Property, welches über die Eigenschaft
*SelectedValue* gebunden ist, gesetzt. Dies muss im ViewModel natürlich einen public setter haben
und vom gleichen Typ wie die Listenelemente sein. Soll nur ein Property des Listenelementes
zurückgeschrieben werden, gibt es mit der Eigenschaft *SelectedValuePath* noch die Möglichkeit,
das zurückgeschriebene Property des Listenelements zu definieren.

Mit *SelectedIndex* kann z. B. das erste Element beim Start ausgewählt werden. Das Standardverhalten
ist kein ausgewählter Wert (leerer Eintrag).

### Definition in XAML

In unserem Musterprogramm ist die ComboBox, die die Liste aller Klassen für die Filterung darstellt,
so definiert:

```xml
<ComboBox x:Name="Classlist" SelectedIndex="0" DockPanel.Dock="Top"
          SelectedValue="{Binding CurrentClass}" ItemsSource="{Binding Classes}"
          DisplayMemberPath="Name"/>
```

Da *Schoolclass* ein komplexer Typ ist, bestimmen wir mit dem Attribut *DisplayMemberPath* welches
Property zur Anzeige verwendet wird. Sonst würde die *ToString()* Methode verwendet werden,
die einfach den Typnamen anzeigt.

In [MainViewModel](ListDemo/ViewModels/MainViewModel.cs) wird das Property *Classes* vom Typ
`List<Schoolclass>` definiert. Das Property *CurrentClass* hat dementsprechend den Typ *Schoolclass*,
damit es das gewählte Listenelement aufnehmen kann. Der setter regelt dann die Aktualisierung der
angezeigten Schüler.

```c#
public List<Schoolclass> Classes => _db.Classes;

private Schoolclass? _currentClass;
public Schoolclass? CurrentClass
{
    get => _currentClass;
    set
    {
        _currentClass = value;
        if (_currentClass is null)
        {
            Pupils.Clear();
            return;
        }
        var students = _db.Pupils.Where(p => p.Schoolclass.Name == _currentClass.Name).OrderBy(p => p.Lastname).ThenBy(p => p.Firstname);
        Pupils.ReplaceAll(App.Mapper.Map<IEnumerable<StudentDto>>(students));
    }
}
```

## Die List Box

Für die Auflistung der Schüler wird in unserem Musterprogramm eine ListBox definiert. Eine ListBox
ist durch ihre Features ein sehr vielseitiges und daher häufig verwendetes Control:

- Sie kann horizontal oder vertikal wachsen.
- Die Darstellung der einzelnen Zelle kann durch ein Data Template beliebig definiert werden.
- Sie kann in Verbindung mit einer *ObservableCollection* automatisch aktualisiert werden.

### Binding mit *ItemsSource* und *SelectedValue*

Auch eine ListBox hat die für das Binding an das ViewModel notwendigen Eigenschaften: *ItemsSource*
und *SelectedValue*. Ihre Bedeutung ist wie bei der ComboBox.

### Definition in XAML mit Data Templates

Eine Liste wird in der Regel für die Anzeige von Collections komplexer Typen (Entities) verwendet.
Da die Liste selbst nicht weiß, wie sie z. B. die Klasse *Student* darstellen soll, wird ohne unser
Zutun die *ToString()* Methode aufgerufen. Die liefert allerdings nur den Typnamen, was wenig sinnvoll
ist.

Wollen wir steuern, wie der einzelne Schüler in unserem Musterprogramm dargestellt wird, können wir
das über ein *DataTemplate* definieren.

Eine vereinfachte Definition der Liste samt Data Template aus unserem Programm zeigt der folgende
Code:

```xml
<ListBox MinWidth="140" ItemsSource="{Binding Pupils}" SelectedValue="{Binding CurrentPupil}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <!-- Alle Bindings gelten für das aktuelle Pupil Objekt. -->
            <StackPanel>
                <TextBlock Text="{Binding Firstname}" />
                <TextBlock FontWeight="Bold" Text="{Binding Lastname}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

Beachte, dass innerhalb des Data Templates direkt auf die Properties von *Pupil* zugegriffen werden
kann. Es wirkt quasi wie eine *foreach* Schleife, die durch die einzelnen Elemente iteriert und das
aktuelle Element in einer Variablen bereitstellt.

#### Zusatzinfo: Buttons in Listen

Soll ein Button in eine Liste eingebaut werden, der ein Command des ViewModels aufruft,
muss zuerst dem Window mit *x:Name* ein Name gegeben werden (z. B. *MainWindowPage*). Dann kann im Data
Template ein Button definiert werden, der das Command *DeleteItem* aufruft und den aktuellen Datensatz
übergibt:

```xml
<Button Content="Delete"
        Command="{Binding Source={x:Reference MainWindowPage}, Path=BindingContext.DeleteItem}"
        CommandParameter="{Binding .}" />
```

### Verwendung einer *ObservableCollection*

Wird bei einer normalen Liste ein Element hinzugefügt oder gelöscht, bekommt die ListBox nichts
davon mit. Wird hingegen eine *ObservableCollection* verwendet, wird beim Aufrufen der *Add()* oder
*Remove()* Methode die ListBox automatisch aktualisiert.

Das ist deswegen notwendig, da sich die Liste beim Anlegen oder Löschen eines Schülers verändert.
Außerdem wird beim Wechsel der Klasse die Liste neu befüllt.

> **Wichtig:** Observable Collections arbeiten nur richtig, wenn mit der gleichen Instanz gearbeitet
> wird. Ein häufiger Fehler ist das Erstellen einer neuen Observable Collection mit *new* um
> z. B. alle Werte zu ändern.

In [MainViewModel](ListDemo/ViewModels/MainViewModel.cs) werden die Properties *Pupils* und
*CurrentClass* so definiert:

```c#
using System.Collections.ObjectModel;
using ListDemo.Extensions;
...

public ObservableCollection<StudentDto> Pupils { get; } = new ObservableCollection<StudentDto>();
private Schoolclass? _currentClass;
public Schoolclass? CurrentClass
{
    get => _currentClass;
    set
    {
        _currentClass = value;
        if (_currentClass is null)
        {
            Pupils.Clear();
            return;
        }
        var students = _db.Pupils.Where(p => p.Schoolclass.Name == _currentClass.Name).OrderBy(p => p.Lastname).ThenBy(p => p.Firstname);
        Pupils.ReplaceAll(App.Mapper.Map<IEnumerable<StudentDto>>(students));
    }
}
```

Die Methoden *AddRange()* und *ReplaceAll()* gibt es nicht im Framework, sie wurden als Extension
Methoden in [ObservableCollectionExtensions](ListDemo/Extensions/ObservableCollectionExtensions.cs)
selbst hinzugefügt.

## Übung

Erweitere die bestehende Solution [ListDemo](ListDemo/ListDemo.sln) um die folgenden Punkte:

![](angabe.png)

1. Zeige in der Liste nicht nur den Schülernamen, sondern auch den Namen des Klassenvorstandes an.
2. Der Button *Schüler löschen* ist noch ohne Funktion. Beim Drücken darauf soll der aktuelle Schüler
   entfernt werden. Achte darauf, dass er aus der ListBox, aus der *Pupils* Collection der Datenbank
   und der *Pupils* Collection der aktuellen Klassen entfernt wird.
3. In XAML Code haben die Prüfungen noch statische Werte zur Demonstration. Gestalte die ListBox
   so, dass der Prüfungsgegenstand, das Datum der Prüfung, das Lehrerkürzel und die Note nett
   aufbereitet ausgegeben werden.
4. Der Bereich *Neue Prüfung* besteht im Moment noch aus Textboxen ohne Binding. Um eine
   Prüfung anlegen zu können, muss eine DTO Klasse *ExamDto* definiert werden. Vergiss nicht,
   das Mappingprofil zu definieren. Die Liste der Prüfungen bindet sich dann an eine Collection
   vom Typ *ExamDto*. Erzeuge die
   entsprechenden Bindings und die benötigten Properties im ViewModel.

   (a) Der Prüfer soll aus einer Liste von Lehrern gewählt werden, die über die Datenbank
   bereitgestellt wird.
5. Beim Klicken auf Prüfung speichern soll die Prüfung zur Liste der Prüfungen des Schülers
   hinzugefügt werden. Achte auch darauf, dass sich die Liste der Prüfungen sofort aktualisiert.
