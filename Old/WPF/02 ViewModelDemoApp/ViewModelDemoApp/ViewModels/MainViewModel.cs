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
            CurrentPerson = Persons[currentIndex];
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
        /// Binding Property für den Next Button.
        /// </summary>
        public ICommand NextCommand { get; private set; }
        /// <summary>
        /// Liest alle Personen aus der Datenbank in den Speicher. Sonst können wir nicht mit dem
        /// Index arbeiten, da Collections keinen Indexzugriff ermöglichen.
        /// </summary>
        public IList<Person> Persons => personDb.Persons.ToList();
        /// <summary>
        /// Binding Property für den Previous Button
        /// </summary>
        public ICommand PrevCommand { get; private set; }
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
        }
    }
}
