# Erste MVVM Applikation
![View Model Demo App Ui](ViewModelDemoAppUi.png)

Diese Applikation benutzt alle 3 Teile der MVVM (Model View ViewModel) Architektur: Ein **Model** in Form einer Personen-Datenbank.
Ein **ViewModel**, welches die Binding Properties bereitstellt. Die *View*, die mittels Binding auf 
das ViewModel zugreift.

Folgende Themen werden im Quellcode behandelt:

- Setzen des *DataContext* in XAML auf das ViewModel.
- Bindingoptionen (StringFormat, Multibinding, Converter)
- RelayCommand und Command Bindings

Das Zusammenspiel der Komponenten wird bei folgender Grafik deutlich:

![View Model Approach](ViewModelApproach.png)

Grafik erstellt mit [draw.io].

[draw.io]: https://www.draw.io

Hinweis: Um eine Methode aus dem ViewModel aufzurufen, wenn ein Button angeklickt wurde, kann
ein Eventhandler im code behind verwendet werden. Er kann so aussehen:

```c#
private void DeleteTrack_Click(object sender, RoutedEventArgs e)
{
    var vm = DataContext as MainViewModel;
    vm.DeleteSelectedTrack();
}
```

## Übung

**(1)** Baue über den Formularfeldern ein Suchfeld mit einem Button ein, mit dessen Hilfe Personen
gesucht werden können, dessen Nachname mit der Eingabe beginnt. Nach Klick auf den Button
soll die Suche durchgeführt werden.

**(2)** Nach dem Klick auf die Suche soll der Button die Beschriftung "Filter entfernen" haben. Beim
erneuten Klick darauf sollen wieder alle Personen angezeigt werden.

**(3)** Füge eine Statusbar zur App hinzu. Eine Anleitung dazu ist auf [www.wpf-tutorial.com](https://www.wpf-tutorial.com/de/53/einfache-steuerelemente/die-wpf-statusbar/).
Wenn nach Personen gesucht wurde, soll der Text "x Personen gefunden" erscheinen. Hinweis:
Definiere für die Anzahl der gefundenen Personen einen Textblock und über Binding soll die
Anzahl eingefügt werden.

**(4)** Wenn das Suchergebnis angezeigt wird, sollen die Buttons für die Navigation natürlich nur
die gefundenen Personen anzeigen.

Achte stets auf den State der Applikation, sodass die Ausgaben keine falschen Werte liefern (z. B.
stehengebliebene Statusmeldungen, ...).

[Lösung zur Übung](Loesung_ViewModelDemoApp.zip)
