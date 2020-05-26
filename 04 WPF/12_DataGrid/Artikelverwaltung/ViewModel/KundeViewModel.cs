using Artikelverwaltung.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Artikelverwaltung.ViewModel
{
    public class KundeViewModel : BaseViewModel
    {
        /// <summary>
        /// Observable Collection für das DataGrid
        /// </summary>
        public ObservableCollection<Kunde> Kunden { get; } = new ObservableCollection<Kunde>();

        /// <summary>
        /// Konstruktor. Liest alle Kunden aus der Datenbank und schreibt sie in die Observable
        /// Collection.
        /// </summary>
        public KundeViewModel()
        {
            Kunden.Clear();
            foreach (var k in _db.Kunden.OrderBy(k => k.Zuname).ThenBy(k=>k.Vorname))
            {
                Kunden.Add(k);
            }
        }
    }
}
