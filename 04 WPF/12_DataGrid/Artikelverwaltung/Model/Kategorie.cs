using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Documents;

namespace Artikelverwaltung.Model
{
    [Table("Kategorie")]
    public class Kategorie
    {
        public int KategorieId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public virtual List<Artikel> Artikel { get; set; }
    }
}
