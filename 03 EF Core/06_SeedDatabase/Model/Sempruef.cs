using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BogusTest.Model
{
    public partial class Sempruef
    {
        [Key]
        public long SP_Id { get; set; }
        [Column(TypeName = "SMALLINT")]
        public long? SP_Schueler { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string SP_Fach { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string SP_Lehrer { get; set; }
        [Column(TypeName = "SMALLINT")]
        public long? SP_Note { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime? SP_Datum { get; set; }

        [ForeignKey(nameof(SP_Fach))]
        [InverseProperty(nameof(Fach.Sempruef))]
        public virtual Fach SP_FachNavigation { get; set; }
        [ForeignKey(nameof(SP_Lehrer))]
        [InverseProperty(nameof(Lehrer.Sempruef))]
        public virtual Lehrer SP_LehrerNavigation { get; set; }
        [ForeignKey(nameof(SP_Schueler))]
        [InverseProperty(nameof(Schueler.Sempruef))]
        public virtual Schueler SP_SchuelerNavigation { get; set; }
    }
}
