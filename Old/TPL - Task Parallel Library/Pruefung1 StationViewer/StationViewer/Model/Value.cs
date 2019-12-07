namespace StationViewer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Value")]
    public partial class Value
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int STATION { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime TIME { get; set; }

        public double TT { get; set; }

        public virtual Station Station1 { get; set; }
    }
}
