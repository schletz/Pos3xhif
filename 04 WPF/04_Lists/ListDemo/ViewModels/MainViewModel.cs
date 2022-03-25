using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ListDemo.Model;
using ListDemo.Extensions;
using ListDemo.Dto;
using System.Windows;

namespace ListDemo.ViewModels
{
    /// <summary>
    /// Viewmodel für MainWindow
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Instanz der Personendatenbank.
        /// </summary>
        private readonly SchoolDb _db = SchoolDb.FromMockup();
        /// <summary>
        /// Wird aufgerufen, wenn das Binding aktualisiert werden soll. Das muss beim Schreiben
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Binding Property für alle Felder, die Daten der aktuellen Person ausgeben. Wichtig ist
        /// das Aufrufen von PropertyChanged im setter, da sonst die Bindings nicht aktualisiert
        /// werden!
        /// </summary>

        public ICommand NewPupilCommand { get; }
        public ICommand SavePupilCommand { get; }

        public List<Schoolclass> Classes => _db.Classes;
        public List<Gender> Genders => _db.Genders;
        /// <summary>
        /// Binding für die Listview. Ist eine ObservableCollection, damit die Liste automatisch
        /// beim Hinzufügen oder Löschen aktualisiert wird.
        /// </summary>
        public ObservableCollection<StudentDto> Pupils { get; } = new ObservableCollection<StudentDto>();

        private Schoolclass? _currentClass;
        public Schoolclass? CurrentClass
        {
            get => _currentClass;
            set
            {
                _currentClass = value;
                // Entfernt alle alten Einträge aus der Pupils Collection und fügt die Schüler
                // der gewählten Klasse hinzu. Achtung: Pupils ist eine ObservableCollection, damit
                // die Anzeige aktualisiert wird darf sie nicht einfach neu gesetzt werden.
                if (_currentClass is null)
                {
                    Pupils.Clear();
                    return;
                }
                var students = _db.Pupils.Where(p => p.Schoolclass.Name == _currentClass.Name).OrderBy(p => p.Lastname).ThenBy(p => p.Firstname);
                // Wir verwenden Automapper (Konfiguration in App.xaml.cs), um aus der Liste der Students
                // eine Liste von StudentDTO Klassen zu erstellen.
                Pupils.ReplaceAll(App.Mapper.Map<IEnumerable<StudentDto>>(students));
            }
        }


        /// <summary>
        /// Aktuell angezeigte Person. Ist das Bindingfeld für die View.
        /// </summary>
        private StudentDto? _currentStudent;
        public StudentDto? CurrentStudent
        {
            get => _currentStudent;
            set
            {
                // Nur die Bindings aktualisieren, wenn sich etwas ändert.
                if (value != _currentStudent)
                {
                    _currentStudent = value;
                    // Achtung: PropertyChanged ist am Anfang NULL. Bei Initialisierungen im Konstruktor
                    // würde sonst eine Exception beim einfachen Aufruf geworfen.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentStudent)));
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
            NewPupilCommand = new RelayCommand(
                () =>
                {
                    CurrentStudent = new StudentDto()
                    {
                        Schoolclass = _currentClass
                    };
                });
            SavePupilCommand = new RelayCommand(
                () =>
                {
                    if (CurrentStudent is null) { return; }
                    if (string.IsNullOrEmpty(CurrentStudent.Firstname))
                    {
                        MessageBox.Show("Der Vorname ist leer.", "Ungültige Daten", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    if (string.IsNullOrEmpty(CurrentStudent.Lastname))
                    {
                        MessageBox.Show("Der Nachname ist leer.", "Ungültige Daten", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    if (CurrentStudent.Gender is null)
                    {
                        MessageBox.Show("Das Geschlecht ist leer.", "Ungültige Daten", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    if (CurrentStudent.Schoolclass is null)
                    {
                        MessageBox.Show("Die Klasse leer.", "Ungültige Daten", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    var student = App.Mapper.Map<Student>(CurrentStudent);
                    var existing = _db.Pupils.FirstOrDefault(p => p.Id == student.Id);
                    if (existing is not null)
                    {
                        _db.Pupils.Remove(existing);
                    }
                    _db.Pupils.Add(student);
                    // Wir sorgen wieder für eine konsistente Darstellung, indem wir die Klasse neu "einlesen".
                    var students = _db.Pupils.Where(p => p.Schoolclass.Name == _currentClass?.Name).OrderBy(p => p.Lastname).ThenBy(p => p.Firstname);
                    Pupils.ReplaceAll(App.Mapper.Map<IEnumerable<StudentDto>>(students));
                    CurrentStudent = null;
                });
        }
    }
}


