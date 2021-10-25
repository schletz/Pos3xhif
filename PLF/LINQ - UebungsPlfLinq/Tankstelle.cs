using System;
using System.Collections.Generic;

namespace TankstellenVw
{
    public class Tankstelle
    {
        public int Id { get; set; }
        public string Bundesland { get; set; }
        public int PLZ { get; set; }
        public string Betreiber { get; set; }
        public virtual ICollection<Tagespreis> Tagespreise { get; set; }
    }
}
