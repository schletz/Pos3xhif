using System;
using System.Collections.Generic;

namespace FlightMonitor.Domain
{
    public partial class Departure
    {
        public long Idx { get; set; }
        public string Fn { get; set; }
        public DateTimeOffset Scheduledatetime { get; set; }
        public DateTimeOffset Schedule { get; set; }
        public DateTimeOffset Actualdatetime { get; set; }
        public DateTimeOffset Actual { get; set; }
        public string Gate { get; set; }
        public string GateArea { get; set; }
        public Status Status { get; set; }
        public List<Codeshare> Codeshares { get; set; }
        public Aircraft Aircraft { get; set; }
        public Airline Airline { get; set; }
        public List<Destination> Destinations { get; set; }
        public Checkin Checkin { get; set; }
    }

}
