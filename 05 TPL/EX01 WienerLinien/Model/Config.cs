using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WienerLinien.Model
{
    [Table("Config")]
    public class Config
    {
        public int ConfigId { get; set; }
        public DateTime? LineUpdateAt { get; set; }
        public DateTime? StationUpdateAt { get; set; }
    }
}
