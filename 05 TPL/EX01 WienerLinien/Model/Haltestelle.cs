using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WienerLinien.Model
{
    [Table("Haltestelle")]
    public class Haltestelle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int HaltestelleId { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        public List<Steig> Steige { get; set; }
    }
}