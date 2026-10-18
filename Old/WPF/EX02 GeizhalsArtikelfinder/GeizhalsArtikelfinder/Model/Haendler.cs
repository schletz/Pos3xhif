using System.Collections.Generic;

namespace GeizhalsArtikelfinder.Model
{
    public class Haendler
    {
        public long Uid { get; set; }
        public string Name { get; set; }
        public string Land { get; set; }
        public string Url { get; set; }
        public ICollection<Angebot> Angebote { get; set; }

    }
}
