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
        private readonly SchoolDb _db = new SchoolDb();
        /// <summary>
        /// Wird aufgerufen, wenn das Binding aktualisiert werden soll. Das muss beim Schreiben
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Binding Property für alle Felder, die Daten der aktuellen Person ausgeben. Wichtig ist
        /// das Aufrufen von PropertyChanged im setter, da sonst die Bindings nicht aktualisiert
        /// werden!
        /// </summary>

        public List<Schoolclass> Classes => _db.Classes.OrderBy(c => c.ClassNr).ToList();
        public List<Gender> Genders => _db.Genders.OrderBy(g => g.Name).ToList();
        /// <summary>
        /// Binding für die Listview. Ist eine ObservableCollection, damit die Liste automatisch
        /// beim Hinzufügen oder Löschen aktualisiert wird.
        /// </summary>
        public ObservableCollection<Pupil> Pupils { get; } = new ObservableCollection<Pupil>();

        private Schoolclass _currentClass;
        public Schoolclass CurrentClass
        {
            get => _currentClass;
            set
            {
                _currentClass = value;
                // Entfernt alle alten Einträge aus der Pupils Collection und fügt die Schüler
                // der gewählten Klasse hinzu. Achtung: Pupils ist eine ObservableCollection, damit
                // die Anzeige aktualisiert wird darf sie nicht einfach neu gesetzt werden.
                Pupils.ReplaceAll(_currentClass?.Pupils);
            }
        }


        /// <summary>
        /// Aktuell angezeigte Person. Ist das Bindingfeld für die View.
        /// </summary>
        private Pupil _currentPupil;
        public Pupil CurrentPupil
        {
            get => _currentPupil;
            set
            {
                // Nur die Bindings aktualisieren, wenn sich etwas ändert.
                if (value != _currentPupil)
                {
                    _currentPupil = value;
                    // Achtung: PropertyChanged ist am Anfang NULL. Bei Initialisierungen im Konstruktor
                    // würde sonst eine Exception beim einfachen Aufruf geworfen.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPupil)));
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
            Pupils.AddRange(_db.Pupils);
            NewPupilCommand = new RelayCommand(
                () =>
                {
                    CurrentPupil = new Pupil
                    {
                        Schoolclass = CurrentClass
                    };
                });

            SavePupilCommand = new RelayCommand(
                () =>
                {
                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Wurde ein neuer Schüler angelegt, der daher nicht getrackt ist?
                            // Dann setzen wir den State auf Added, da wir ihn hinzufügen wollen.
                            if (_db.Entry(CurrentPupil).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                            {
                                _db.Entry(CurrentPupil).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                            }

                            // Hat der Schüler den State Added? Dann speichern wir ihn und fügen ihn
                            // zur ObservableCollection hinzu. Dies ist vom oberen Code getrennt, da
                            // beim Erfassen auch eine Exception auftreten kann. Dann soll der Schüler
                            // nach der Korrektur der Daten trotzdem gespeichert werden.
                            if (_db.Entry(CurrentPupil).State == Microsoft.EntityFrameworkCore.EntityState.Added)
                            {
                                _db.SaveChanges();
                                Pupils.Add(CurrentPupil);
                            }

                            // Wird der Schüler getrackt, dann speichern wir nur den Datensatz
                            // (also ein UPDATE)
                            else if(_db.Entry(CurrentPupil).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                            {
                                _db.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            _db.Entry(CurrentPupil).Reload();
                            MessageBox.Show("Der Schüler konnte nicht gespeichert werden." + e?.InnerException, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                },
                () => CurrentPupil != null);

            DeletePupilCommand = new RelayCommand(
                () =>
                {
                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            _db.Pupils.Remove(CurrentPupil);
                            // TODO: Was passiert, wenn der Schüler Semesterprüfungen hat?
                            _db.SaveChanges();
                            Pupils.Remove(CurrentPupil);
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            // Da wir den Schüler schon aus unserer lokalen Collection gelöscht haben,
                            // müssen wir ihn wieder herstellen. Sonst wird beim nächsten SaveChanges()
                            // die Löschoperation nochmals versucht.
                            transaction.Rollback();
                            _db.Entry(CurrentPupil).Reload();
                            MessageBox.Show("Der Schüler konnte nicht gelöscht werden." + e?.InnerException, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                },
                () => CurrentPupil != null);
        }
        public ICommand NewPupilCommand { get; }
        public ICommand SavePupilCommand { get; }
        public ICommand DeletePupilCommand { get; }
    }
}


