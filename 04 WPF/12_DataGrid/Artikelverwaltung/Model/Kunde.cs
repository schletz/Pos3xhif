using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artikelverwaltung.Model
{
    [Table("Kunde")]
    public class Kunde
    {
        public int KundeId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Vorname { get; set; }
        [Required]
        [MaxLength(255)]
        public string Zuname { get; set; }
        [Required]
        [MaxLength(255)]
        public string Adresse { get; set; }
        public int Plz { get; set; }
        [Required]
        [MaxLength(255)]
        public string Ort { get; set; }
        public virtual List<Bestellung> Bestellungen { get; set; }
    }
}
