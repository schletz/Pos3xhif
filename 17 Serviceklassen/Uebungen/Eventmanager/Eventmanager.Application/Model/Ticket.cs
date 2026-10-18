using System;

namespace Eventmanager.Application.Model;

/// <summary>
/// A reservation or purchase of a guest for a contingent.
/// Pax is the number of accompanying persons, so a ticket admits Pax + 1 persons.
/// </summary>
public class Ticket
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Ticket() { }
#pragma warning restore CS8618

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
