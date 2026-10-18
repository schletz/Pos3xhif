using System.Collections.Generic;

namespace GeizhalsArtikelfinder.Model
{
    public partial class Artikel
    {

        public long Ean { get; set; }
        public string Name { get; set; }
        public string Kategorie { get; set; }
        public ICollection<Angebot> Angebote { get; set; }
    }
}
