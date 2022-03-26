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
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

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
        private readonly SchoolDb _db = new SchoolDb();
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

        public List<Schoolclass> Classes => _db.Classes.ToList();
        public List<Gender> Genders => _db.Genders.ToList();
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
                ReadStudents();
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
                    // Wir weisen den neuen Schüler gleich die gewählte Klasse zu.
                    CurrentStudent = new StudentDto()
                    {
                        Schoolclass = _currentClass
                    };
                });
            SavePupilCommand = new RelayCommand(
                () =>
                {
                    if (CurrentStudent is null) { return; }
                    try
                    {
                        // Gibt es den Schüler schon?
                        var studentDb = _db.Pupils.FirstOrDefault(p => p.Id == CurrentStudent.Id);
                        // Wenn nein, machen wir ein INSERT.
                        if (studentDb is null)
                        {
                            var student = App.Mapper.Map<Student>(CurrentStudent);
                            _db.Pupils.Add(student);
                        }
                        // Ansonsten ein UPDATE
                        else
                        {
                            // Automapper kann eine existierende Instanz (studentDb) ergänzen.
                            App.Mapper.Map(CurrentStudent, studentDb);
                        }
                        _db.SaveChanges();
                        ReadStudents();
                        // Die Felder für die Schülerbearbeitung werden wieder geleert.
                        CurrentStudent = null;
                    }
                    catch (ApplicationException e)
                    {
                        MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    catch (DbUpdateException e)
                    {
                        MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                }, () => CurrentStudent is not null);
        }

        /// <summary>
        /// Liest alle Schüler einer Klasse in die ObservableCollection
        /// </summary>
        private void ReadStudents()
        {
            if (CurrentClass is null)
            {
                Pupils.Clear();
                return;
            }
            // Wir lesen alle Students der Klasse (der Name ist der PK) und sortieren nach dem Namen.
            var students = _db.Pupils
                .Include(p => p.Gender)
                .Where(p => p.Schoolclass.Name == CurrentClass.Name)
                .OrderBy(p => p.Lastname).ThenBy(p => p.Firstname);
            // Wir verwenden Automapper (Konfiguration in App.xaml.cs), um aus der Liste der Students
            // eine Liste von StudentDTO Klassen zu erstellen.
            Pupils.ReplaceAll(students.ProjectTo<StudentDto>(App.Mapper.ConfigurationProvider));
        }
    }
}


