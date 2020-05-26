using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Documents;

namespace Artikelverwaltung.Model
{
    [Table("Artikel")]
    public class Artikel
    {
        public int ArtikelId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Ean { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Column(TypeName ="DECIMAL(9,4)")]
        public decimal Preis { get; set; }
        [MaxLength(255)]
        public string Hersteller { get; set; }
        public DateTime ProduziertAb { get; set; }
        public DateTime? EingestelltAb { get; set; }
        [Required]
        public virtual Kategorie Kategorie { get; set; }
        public virtual List<Bestellung> Bestellungen { get; set; }
    }
}
