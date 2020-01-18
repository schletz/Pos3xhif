using System;

namespace FlightMonitor.Domain
{
    public partial class Destination
    {
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NameDe { get; set; }
        public string NameEn10 { get; set; }
        public string NameDe10 { get; set; }
        public string IataCode { get; set; }
        public Uri WebSite { get; set; }
        public City City { get; set; }
    }

}
