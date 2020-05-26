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

        public List<Kategorie> Kategorien => _db.Kategorien.OrderBy(k => k.Name).ToList();
        public Kategorie SelectedKategorie
        {
            get => selectedKategorie;
            set
            {
                selectedKategorie = value;
                Artikel.Clear();
                foreach (var a in selectedKategorie.Artikel.OrderBy(a => a.Name))
                {
                    Artikel.Add(a);
                }
            }
        }
        public ObservableCollection<Artikel> Artikel { get; } = new ObservableCollection<Artikel>();
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ArtikelViewModel()
        {
            DeleteCommand = new RelayCommand((selectedObject) =>
            {
                var artikel = selectedObject as Artikel;
                if (artikel == null) { return; }
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
                foreach (var a in Artikel)
                {
                    if (_db.Entry(a).State == EntityState.Detached)
                    {
                        a.Kategorie = a.Kategorie ?? SelectedKategorie;
                        _db.Entry(a).State = EntityState.Added;
                    }
                }
                if (!TrySaveChanges())
                {
                    MessageBox.Show("Fehler beim Speichern der Daten.", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SelectedKategorie = SelectedKategorie;
            });
        }

    }
}
