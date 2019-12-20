using System;
using System.Collections.Generic;

namespace TankstellenVw
{
    public class Verkauf
    {
        public int Id { get; set; }
        public int Tankstelle { get; set; }
        public DateTime Tag { get; set; }
        public decimal? VerkaufBenzinLiter { get; set; }
        public decimal? VerkaufDieselLiter { get; set; }
        public decimal? VerkaufShopEuro { get; set; }
        public Tagespreis _Tagespreis { get; set; }
    }
}
