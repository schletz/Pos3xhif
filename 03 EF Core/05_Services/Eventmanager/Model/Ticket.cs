using System;

namespace Eventmanager.Model
{
    public class Ticket
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Ticket()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Ticket(Guest guest, Contingent contingent, TicketState ticketState, DateTime reservationDateTime, int pax)
        {
            Guest = guest;
            Contingent = contingent;
            TicketState = ticketState;
            ReservationDateTime = reservationDateTime;
            Pax = pax;
        }

        public int Id { get; set; }
        public Guest Guest { get; set; }
        public Contingent Contingent { get; set; }
        public TicketState TicketState { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int Pax { get; set; }
    }
}