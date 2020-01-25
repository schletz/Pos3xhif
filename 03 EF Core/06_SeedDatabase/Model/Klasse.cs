using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BogusTest.Model
{
    public partial class Klasse
    {
        public Klasse()
        {
            Schueler = new HashSet<Schueler>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(10)")]
        public string K_Nr { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string K_KV { get; set; }

        [ForeignKey(nameof(K_KV))]
        [InverseProperty(nameof(Lehrer.Klasse))]
        public virtual Lehrer K_KVNavigation { get; set; }
        [InverseProperty("S_KlasseNavigation")]
        public virtual ICollection<Schueler> Schueler { get; set; }
    }
}
