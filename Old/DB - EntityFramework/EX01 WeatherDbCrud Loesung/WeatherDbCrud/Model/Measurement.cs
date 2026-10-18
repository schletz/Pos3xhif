namespace WeatherDbCrud.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Measurement")]
    public partial class Measurement
    {
        [Key]
        [Column(Order = 0)]
        public DateTime M_Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int M_Station { get; set; }

        public decimal? M_Temperature { get; set; }

        public virtual Station Station { get; set; }
    }
}
