using ObservablesDemo.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ObservablesDemo.ViewModels
{
    /// <summary>
    /// Viewmodel für MainWindow
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Instanz der Personendatenbank.
        /// </summary>
        private readonly PersonDb personDb = PersonDb.FromMockup();
        /// <summary>
        /// Aktuell angezeigter Index der Personenliste.
        /// </summary>
        private int currentIndex = 0;
        /// <summary>
        /// Aktuelle Person.
        /// </summary>
        private Person currentPerson;

        /// <summary>
        /// Konstruktor mit Initialisierungen.
        /// </summary>
        public MainViewModel()
        {
            // Zuweisung des get Properties Persons ist möglich, da readonly
            // im Konstruktur initialisiert werden kann.
            // Zur Synchronisation mit dem Model verwenden wir eine eigene Klasse (SynchronizedObervable).
            // Natürlich kann die "normale" ObservableCollection verwendet werden, allerdings muss dann
            // das CollectionChanged Event implementiert werden, damit die Änderung im Model übertragen
            // wird. Der Code könnte so aussehen:
            // Persons.CollectionChanged += (source, e) =>
            // {
            //     foreach (Person p in e.NewItems.Cast<Person>())
            //     {
            //         personDb.Persons.Add(p);
            //     }
            //     foreach (Person p in e.OldItems.Cast<Person>())
            //     {
            //         personDb.Persons.Remove(p);
            //     }
            // };
            Persons = new SynchronizedObservable<Person>(personDb.Persons);
            currentPerson = Persons[0];
            InitCommands();
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Binding aktualisiert werden soll. Das muss beim Schreiben
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Binding Property für alle Felder, die Daten der aktuellen Person ausgeben. Wichtig ist
        /// das Aufrufen von PropertyChanged im setter, da sonst die Bindings nicht aktualisiert
        /// werden!
        /// </summary>
        public Person CurrentPerson
        {
            get => currentPerson;
            // set muss wegen der Liste public sein!
            set
            {
                // Nur die Bindings aktualisieren, wenn sich etwas ändert.
                if (value != currentPerson)
                {
                    currentPerson = value;
                    // Damit die Textfelder neu eingelesen werden, müssen wir mitteilen, dass
                    // CurrentPerson sich geändert hat.
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPerson)));
                }
            }
        }
        /// <summary>
        /// Binding Property für den Next Button.
        /// </summary>
        public ICommand NextCommand { get; private set; }
        /// <summary>
        /// Liest alle Personen aus der Datenbank in den Speicher. Sonst können wir nicht mit dem
        /// Index arbeiten, da Collections keinen Indexzugriff ermöglichen.
        /// Durch die ObservableColleciton wird automatisch beim Hinzufügen PropertyChanged geworfen,
        /// da ObservableCollection das Interface INotifyPropertyChanged implementiert.
        /// Natürlich kann man auch eine ICollection verwenden und PropertyChanged beim
        /// Ändern von Elementen aufrufen.
        /// </summary>
        public ObservableCollection<Person> Persons { get; }
        /// <summary>
        /// Binding Property für den Previous Button
        /// </summary>
        public ICommand PrevCommand { get; private set; }
        /// <summary>
        /// Binding Property für den Generate Person Button.
        /// </summary>
        public ICommand GeneratePersonCommand { get; private set; }
        /// <summary>
        /// Binding Property für den Delete Person Button.
        /// </summary>
        public ICommand DeletePersonCommand { get; private set; }
        /// <summary>
        /// Initialisiert die Properties für die Buttons. Hier kann die Action, die durchgeführt
        /// werden soll, direkt mitgegeben werden. Bei längeren Methoden sollte aber die Methode
        /// im View Model als private definiert werden und es wird hier einfach der Methodenname
        /// übergeben.
        /// Das Erzeugen des RelayCommand sollte nicht direkt im getter des Binding Properties
        /// geschehen, da sonst immer eine neue Instanz erzeugt wird. Deswegen initialisieren wir
        /// hier vorher.
        /// </summary>
        private void InitCommands()
        {
            NextCommand = new RelayCommand(
                // Action für den Klick
                () =>
                {
                    CurrentPerson = Persons[++currentIndex];
                },
                // Gibt an, wann der Button aktiv sein soll.
                () => currentIndex < Persons.Count - 1
                );

            PrevCommand = new RelayCommand(
                () =>
                {
                    CurrentPerson = Persons[--currentIndex];
                },
                () => currentIndex > 0);

            GeneratePersonCommand = new RelayCommand(
                () =>
                {
                    Random rnd = new Random();
                    Persons.Add(new Person
                    {
                        Firstname = $"Vorname{rnd.Next(1000, 9999 + 1)}",
                        Lastname = $"Zuname{rnd.Next(1000, 9999 + 1)}",
                        Sex = rnd.Next(0, 2) == 0 ? Sex.Male : Sex.Female,
                        DateOfBirth = DateTime.Now.AddDays(-rnd.Next(18 * 365, 25 * 365))
                    });
                });

            DeletePersonCommand = new RelayCommand(() => Persons.Remove(CurrentPerson));
        }
    }
}
