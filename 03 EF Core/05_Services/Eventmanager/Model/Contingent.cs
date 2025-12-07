using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Eventmanager.Model
{
    public class Contingent
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Contingent()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Contingent(Show show, ContingentType contingentType, int availableTickets)
        {
            Show = show;
            ContingentType = contingentType;
            AvailableTickets = availableTickets;
        }

        public int Id { get; set; }
        public Show Show { get; set; }
        public ContingentType ContingentType { get; set; }
        public int AvailableTickets { get; set; }
        [JsonIgnore]
        public List<Ticket> Tickets { get; set; } = new();
    }
}