using Artikelverwaltung.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Artikelverwaltung.ViewModel
{
    public class ArtikelViewModel : BaseViewModel
    {
        private Kategorie selectedKategorie;

        /// <summary>
        /// Binding für die Combobox, die die Kategorien zur Filterung anzeigt.
        /// </summary>
        public List<Kategorie> Kategorien => _db.Kategorien.OrderBy(k => k.Name).ToList();

        /// <summary>
        /// Bindingfeld für die Combobox, wenn eine Kategorie ausgewählt wurde. Es wird dann die
        /// Collection der Artikel mit den entsprechenden Artikel der Kategorie befüllt.
        /// </summary>
        public Kategorie SelectedKategorie
        {
            get => selectedKategorie;
            set
            {
                selectedKategorie = value;
                // Die Observable Collection wird geleert und danach mit den Artikeln befüllt.
                Artikel.Clear();
                foreach (var a in selectedKategorie.Artikel.OrderBy(a => a.Name))
                {
                    Artikel.Add(a);
                }
            }
        }

        /// <summary>
        /// Observable Collection für das DataGrid. Es darf keine neue Collection im Programmablauf
        /// erstellt werden, deswegen ein read only Property.
        /// </summary>
        public ObservableCollection<Artikel> Artikel { get; } = new ObservableCollection<Artikel>();

        /// <summary>
        /// Command für den Löschbutton in jeder Zeile des DataGrids
        /// </summary>
        public ICommand DeleteCommand { get; }
        /// <summary>
        /// Command für den Speichernbutton
        /// </summary>
        public ICommand SaveCommand { get; }
        public ArtikelViewModel()
        {
            DeleteCommand = new RelayCommand((selectedObject) =>
            {
                var artikel = selectedObject as Artikel;
                // Wenn in einer leeren Zeile auf löschen geklickt wurde, wird dies ignoriert.
                if (artikel == null) { return; }
                // Der aktuell ausgewählte Artikel wird aus der Datenbank gelöscht.
                _db.Entry(artikel).State = EntityState.Deleted;
                if (TrySaveChanges())
                {
                    Artikel.Remove(artikel);
                }
                else
                {
                    MessageBox.Show("Fehler beim Löschen des Datensatzes.", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            SaveCommand = new RelayCommand(() =>
            {
                // Um herauszufinden, welche Artikel über das DataGrid neu eingegeben wurden, wird
                // der EntityState jedes Datensatzes geprüft. Ist er Detached - also vom OR Mapper
                // nicht verwaltet - so ist dieser neu eingegeben und wird hinzugefügt.
                foreach (var a in Artikel)
                {
                    if (_db.Entry(a).State == EntityState.Detached)
                    {
                        // Falls der User im Grid keine Kategorie angibt, setzen wir die aktuell
                        // ausgewählte Kategorie.
                        a.Kategorie = a.Kategorie ?? SelectedKategorie;
                        _db.Entry(a).State = EntityState.Added;
                    }
                }
                if (!TrySaveChanges())
                {
                    MessageBox.Show("Fehler beim Speichern der Daten.", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Lädt die Artikel aus der Kategorie neu, da hier der setter nochmals durchlaufen wird.
                SelectedKategorie = SelectedKategorie;
            });
        }

    }
}
