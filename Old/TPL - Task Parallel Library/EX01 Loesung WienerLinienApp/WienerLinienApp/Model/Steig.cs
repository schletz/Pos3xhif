namespace WienerLinienApp.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Steig")]
    public partial class Steig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int S_ID { get; set; }

        public int S_Linie { get; set; }

        public int S_Haltestelle { get; set; }

        [StringLength(10)]
        public string S_Steig { get; set; }

        [Required]
        [StringLength(1)]
        public string S_Richtung { get; set; }

        public int S_Reihenfolge { get; set; }

        public virtual Haltestelle Haltestelle { get; set; }

        public virtual Linie Linie { get; set; }
    }
}
