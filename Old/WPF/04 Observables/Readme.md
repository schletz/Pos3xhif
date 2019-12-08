# Observable Collections
![View Model Demo App Ui](ViewModelDemoApp2Ui.png)

Dieses Beispiel ist eine Ergänzung zum Beispiel 3 (Listen). In diesem Ansatz wird im ViewModel eine 
**eigene unabhäbngige Liste** definiert, die die Personenobjekte speichert. Im vorigen Beispiel mussten 
wir beim Hinzufügen oder Löschen folgende Schritte ausführen:
- Hinzufügen bzw. Löschen des Personenobjektes im Model.
- Aufruf von *PropertyChanged()*, welches die gesamte Liste neu lädt.

Der letzte Punkt - das erneute Laden der Liste - kann bei Webservices sehr zeitintensiv sein. Schließlich werden
dann die Daten über das Internet neu geladen. Da wir jetzt im ViewModel eine eigene Liste führen, können wir
unseren Ablauf beim Hinzufügen oder Löschen von Personenobjekten ändern:
- Hinzufügen bzw. Löschen des Personenobjektes im ViewModel.
- Synchronisation mit dem Model im Hintergrund.

Für die Liste im ViewModel verwenden wir eine *ObservableCollection*. Sie feuert beim Hinzufügen oder 
Löschen von Elementen das Event *CollectionChanged*, auf das wir zentral reagieren können. Der Aufruf 
von *PropertyChanged()* entfällt, da die ObservableCollection das Interface *INotifyPropertyChanged* 
implementiert.

Fügen wir nun eine Person durch die Logik in *GeneratePersonCommand* zur ObservableCollection im ViewModel
hinzu, wird zwar die GUI ohne unser Zutun aktualisiert, die Person wird aber nicht im Model gespeichert. 
Es entsteht folgende Situation:
![Untracked Objects](UntrackedObjects.png)

Das neue Objekt wird zwar  zur ObservableCollection hinzugefügt, das Model weiß aber nichts davon. Es 
ist ein *"untracked object"*. Diesen Fall kann auf 2 Arten gelöst werden.
**Ansatz 1:** Im ViewModel wird der Event *CollectionChanged* abonniert. Im Eventhandler wird mit folgendem Code
  die Person im Model eingetragen oder gelöscht (je nach Änderung der Liste):
```c#
private void PersonObservable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
{
    foreach (Person p in e.NewItems?.Cast<Person>() ?? Enumerable.Empty<Person>())
    {
        personDb.Person.Add(p);
    }
    foreach (Person p in e.OldItems?.Cast<Person>() ?? Enumerable.Empty<Person>())
    {
        personDb.Person.Remove(p);
    }
}
```

**Ansatz 2:** Wir leiten von *ObservableCollection&lt;T&gt;* eine eigene Klasse *SynchronizedObservable&lt;T&gt;*
ab. Diese Klasse wird im Beispiel verwendet und kann nun für alle Basiscollections, in die zurückgeschrieben
werden soll, verwendet werden.
```c#
public class SynchronizedObservable<T> : ObservableCollection<T>
{
    private readonly ICollection<T> sourceCollection;

    public SynchronizedObservable(ICollection<T> sourceCollection) : base(sourceCollection)
    {
        this.sourceCollection = sourceCollection;
        CollectionChanged += SynchronizedObservable_CollectionChanged;
    }

    private void SynchronizedObservable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (T p in e.NewItems?.Cast<T>() ?? Enumerable.Empty<T>())
        {
            sourceCollection.Add(p);
        }
        foreach (T p in e.OldItems?.Cast<T>() ?? Enumerable.Empty<T>())
        {
            sourceCollection.Remove(p);
        }
    }
}
```

**Was ist besser? Der vorige Ansatz mit der "normalen" Liste oder die ObservableCollection?**
Diese Frage kann nicht pauschal beantwortet werden. Wenn das Model die Daten schnell bereitstellen kann,
wie es z. B. bei einem OR Mapper mit Proxy der Fall ist, bietet der Zugang aus Beispiel 3 sicher eine
einfachere Möglichkeit der Anzeige. Wenn eine Synchronisation mit dem Model ausprogrammiert werden muss,
dann ist unsere Klasse *SynchronizedObservable<T>* ein geeigneter Ort, diesen Code unterzubringen.

