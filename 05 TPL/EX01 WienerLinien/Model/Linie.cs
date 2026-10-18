using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;
using WienerLinien.Model;

namespace WienerLinien.Model
{
    [Table("Linie")]
    public class Linie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LinieId { get; set; }
        [Required, MaxLength(255)]
        public string Bezeichnung { get; set; }
        [Required, MaxLength(255)]
        public string Verkehrsmittel { get; set; }
        public List<Steig> Steige { get; set; }
    }
}