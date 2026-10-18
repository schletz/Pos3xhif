# Das Command Interface

Bisher konnten wir zwar über Binding Daten aus Properties des Viewmodels darstellen bzw. Eingaben
im Viewmodel speichern, die Buttons hatten jedoch noch klassische Eventhandler:

```c#
private void PrevButton_Click(object sender, RoutedEventArgs e)
{
    if (DataContext is MainViewModel vm) { vm.PrevPerson(); }
}
```

In XAML gibt es auch die Möglichkeit, bei einem Button die Eigenschaft *Command* mit einem
Binding zu versehen:

```xml
<Button Width="32" Margin="0 0 30 0" Command="{Binding PrevCommand}" Content="Prev." />
```

Natürlich kann dieses Feld *PrevCommand* kein "normales" Property (also string, int, ...) sein.
Es muss nämlich die Methode beinhalten, die beim Klicken auf den Button ausgeführt wird.

Konkret muss das Property im Viewmodel vom Typ *ICommand* sein. Es wird in [MainViewModel](ViewModels/MainViewModel.cs)
als public property definiert:

```c#
public ICommand NextCommand { get; }
```

*ICommand* ist - wie der Buchstabe I schon andeutet - ein Interface. Die Definition zeigt 2 Methoden,
die beim Implementieren des Interfaces geschrieben werden müssen: *CanExecute* und *Execute*.

```c#
public interface ICommand
{
    event EventHandler CanExecuteChanged;
    bool CanExecute(object parameter);
    void Execute(object parameter);
}
```

Möchten wir jetzt für unseren *PrevButton* eine Methode definieren, die gestartet werden soll, müssten
wir eine eigene Klasse von *ICommand* ableiten:

```c#
public class PrevCommandMethods : ICommand
{
    // Button ist immer aktiviert.
    bool CanExecute(object parameter) => true;
    void Execute(object parameter)
    {
        // Do something
    }
}
```

Das hat allerdings mehrere Nachteile:

- Für jeden Button muss eine eigene Klasse geschrieben werden, was natürlich mühsam ist.
- Innerhalb dieser Implementierung gibt es kein Zugriff auf das ViewModel.

## *RelayCommand* als generische Implementierung von *ICommand*

Die Implementierung ist in der Datei [RelayCommand.cs](ViewModels/RelayCommand.cs) und arbeitet mit
folgender Idee: Es wird einfach ein Action Parameter (meist als Lambda Expression) übergeben. Dieser
Code wird dann in der Methode *Execute()* ausgeführt. Dadurch gewinnt man gleich mehrere Vorteile:

- Es kann eine Funktion übergeben werden, die ausgeführt werden soll. Die Klasse muss dabei
  nicht immer neu geschrieben werden.
- In dieser Funktion ist es möglich, auf die Felder des ViewModels zuzugreifen.

Konkret wird im Konstruktor von [MainViewModel](ViewModels/MainViewModel.cs) eine Instanz von
*RelayCommand* erzeugt. Der erste Parameter ist vom Typ *Action* und erhöht im Fall von *NextCommand*
einfach den Index im Viewmodel. Der zweite Parameter gibt an, wann der Button aktiv sein soll. Das
ist in diesem Fall dann, wenn das letzte Element noch nicht erreicht ist.

```c#
public MainViewModel()
{
    Persons = personDb.Persons.ToList();
    NextCommand = new RelayCommand(
        // Action für den Klick
        () =>
        {
            CurrentIndex++;
        },
        // Gibt an, wann der Button aktiv sein soll.
        () => CurrentIndex < Persons.Count - 1
    );
}
```

## Übung

Im vorigen Beispiel [ViewModelDemoApp](../02_ViewModelDemoApp) sollte eine Suchfunktion eingebaut
werden. Dies wurde noch auf Basis von code behind und Eventhandlern umgesetzt. Nun führe **auf Basis
der Lösung des vorigen Beispiels** (Link ist unter der vorigen Übungsaufgabe) folgende
Verbesserungen durch:

- Der Suchbutton soll an ein Command im Viewmodel gebunden werden.
- Der Text des Buttons (Suche oder Filter entfernen) soll über ein Binding aus dem Viewmodel geladen
  werden.
- Der Text der Statusbar soll auch über ein Binding aus dem Viewmodel geladen werden.

Im Endeffekt darf kein code behind mehr in der Applikation zu finden sein.