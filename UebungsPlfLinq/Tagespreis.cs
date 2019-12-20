using System;
using System.Collections.Generic;

namespace TankstellenVw
{
    public class Tagespreis
    {
        public int Tankstelle { get; set; }
        public DateTime Tag { get; set; }
        public decimal PreisBenzin { get; set; }
        public decimal PreisDiesel { get; set; }
        public ICollection<Verkauf> Verkaeufe { get; set; }
        public Tankstelle _Tankstelle { get; set; }
    }
}
