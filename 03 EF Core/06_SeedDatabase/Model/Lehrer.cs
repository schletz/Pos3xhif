using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BogusTest.Model
{
    public partial class Lehrer
    {
        public Lehrer()
        {
            Klasse = new HashSet<Klasse>();
            Sempruef = new HashSet<Sempruef>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(10)")]
        public string L_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string L_Zuname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string L_Vorname { get; set; }

        [InverseProperty("K_KVNavigation")]
        public virtual ICollection<Klasse> Klasse { get; set; }
        [InverseProperty("SP_LehrerNavigation")]
        public virtual ICollection<Sempruef> Sempruef { get; set; }
    }
}
