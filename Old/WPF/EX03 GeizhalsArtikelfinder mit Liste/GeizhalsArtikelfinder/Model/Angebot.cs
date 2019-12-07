using System;

namespace GeizhalsArtikelfinder.Model
{
    public partial class Angebot
    {
        public int Id { get; set; }

        public long Artikel { get; set; }

        public long Haendler { get; set; }

        public DateTime Datum { get; set; }

        public decimal Preis { get; set; }
        public int AnzVerkaeufe { get; set; }

        public string Url { get; set; }
        public Artikel _Artikel { get; set; }

        public Haendler _Haendler { get; set; }
    }
}
