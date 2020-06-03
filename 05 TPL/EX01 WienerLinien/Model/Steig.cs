using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WienerLinien.Model
{
    [Table("Steig")]
    public class Steig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Haltestelle Haltestelle { get; set; }
        [Required, Column(TypeName = "CHAR(1)")]
        public string Richtung { get; set; }
        public int Reihenfolge { get; set; }
        public int SteigId { get; set; }
        public int LinieId { get; set; }
        public Linie Linie { get; set; }
        public int HaltestelleId { get; set; }
    }
}