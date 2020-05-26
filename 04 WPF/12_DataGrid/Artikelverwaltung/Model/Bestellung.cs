using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artikelverwaltung.Model
{
    [Table("Bestellung")]
    public class Bestellung
    {
        public int BestellungId { get; set; }
        [Required]
        public virtual Artikel Artikel { get; set; }
        [Required]
        public virtual Kunde Kunde { get; set; }
        public DateTime Datum { get; set; }
        public DateTime? BezahltAm { get; set; }
        public int Menge { get; set; }
    }
}
