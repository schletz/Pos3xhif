using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModelDemoApp.Model;

namespace ViewModelDemoApp.ViewModels
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
        /// Wird aufgerufen, wenn das Binding aktualisiert werden soll. Das muss beim Schreiben
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Binding Property für alle Felder, die Daten der aktuellen Person ausgeben. Wichtig ist
        /// das Aufrufen von PropertyChanged im setter, da sonst die Bindings nicht aktualisiert
        /// werden!
        /// </summary>

        /// <summary>
        /// Aktuell angezeigter Index der Personenliste. Setzt auch die aktuell angezeigte Person.
        /// </summary>
        private int currentIndex;
        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                // Damit der Index nicht außerhalb von 0 ... Count-1 ist, begrenzen wir ihn mit
                // Max und Min.
                currentIndex = Math.Max(0, Math.Min(Persons.Count - 1, value));
                CurrentPerson = Persons[currentIndex];
            }
        }
        /// <summary>
        /// Aktuell angezeigte Person. Ist das Bindingfeld für die View.
        /// </summary>
        private Person? currentPerson;

        /// <summary>
        /// Liste der gefundenen Personen aus der Datenbank.
        /// </summary>
        private List<Person> persons = new();
        public List<Person> Persons
        {
            get => persons;
            set
            {
                persons = value;
                // Wenn die Liste neu zugewiesen wird, positionieren wir uns immer auf das erste
                // Element.
                CurrentIndex = 0;
            }
        }
        public Person? CurrentPerson
        {
            get => currentPerson;
            private set
            {
                // Nur die Bindings aktualisieren, wenn sich etwas ändert.
                if (value != currentPerson)
                {
                    currentPerson = value;
                    // Achtung: PropertyChanged ist am Anfang NULL. Bei Initialisierungen im Konstruktor
                    // würde sonst eine Exception beim einfachen Aufruf geworfen.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPerson)));
                }
            }
        }

        /// <summary>
        /// Konstruktor mit Initialisierungen.
        /// Initialisiert die Command Properties für die Buttons. Hier kann die Action, die durchgeführt
        /// werden soll, direkt mitgegeben werden. Bei längeren Methoden sollte aber die Methode
        /// im View Model als private definiert werden und es wird hier einfach der Methodenname
        /// übergeben.
        /// Das Erzeugen des RelayCommand sollte nicht direkt im getter des Binding Properties
        /// geschehen, da sonst immer eine neue Instanz erzeugt wird. Deswegen initialisieren wir
        /// hier vorher.
        /// </summary>
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

            PrevCommand = new RelayCommand(
                () =>
                {
                    CurrentIndex--;
                },
                () => CurrentIndex > 0);
        }

        /// <summary>
        /// Lädt die vorige Person für die Anzeige.
        /// </summary>
        public void NextPerson() => CurrentIndex++;
        /// <summary>
        /// Lädt die nächste Person für die Anzeige.
        /// </summary>
        public void PrevPerson() => CurrentIndex--;

        /// <summary>
        /// Binding Property für den Next Button.
        /// </summary>
        public ICommand NextCommand { get; }
        /// <summary>
        /// Binding Property für den Previous Button
        /// </summary>
        public ICommand PrevCommand { get; }
    }
}



