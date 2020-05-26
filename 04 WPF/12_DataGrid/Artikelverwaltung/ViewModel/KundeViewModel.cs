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
        public ObservableCollection<Kunde> Kunden { get; } = new ObservableCollection<Kunde>();

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
