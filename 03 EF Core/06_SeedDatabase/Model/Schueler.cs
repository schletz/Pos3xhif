using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BogusTest.Model
{
    public partial class Schueler
    {
        public Schueler()
        {
            Sempruef = new HashSet<Sempruef>();
        }

        [Key]
        [Column(TypeName = "SMALLINT")]
        public long S_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string S_Zuname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string S_Vorname { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string S_Geschl { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string S_Klasse { get; set; }

        public DateTime? S_Gebdat { get; set; }

        [ForeignKey(nameof(S_Klasse))]
        [InverseProperty(nameof(Klasse.Schueler))]
        public virtual Klasse S_KlasseNavigation { get; set; }
        [InverseProperty("SP_SchuelerNavigation")]
        public virtual ICollection<Sempruef> Sempruef { get; set; }
    }
}
