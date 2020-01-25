using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BogusTest.Model
{
    public partial class Fach
    {
        public Fach()
        {
            Sempruef = new HashSet<Sempruef>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(10)")]
        public string F_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string F_Name { get; set; }

        [InverseProperty("SP_FachNavigation")]
        public virtual ICollection<Sempruef> Sempruef { get; set; }
    }
}
