using System;
using System.Collections.Generic;
using System.Linq;
using Eventmanager.Application.Dtos;
using Eventmanager.Application.Infrastructure;
using Eventmanager.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace Eventmanager.Application.Services;

/// <summary>
/// Business logic of the event management system. The DbContext is passed in the constructor
/// (dependency injection), so the service can be tested with an in-memory database.
/// </summary>
public class EventService
{
    private readonly EventContext _db;

    public EventService(EventContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all shows between start (inclusive) and end (exclusive).
    /// The entities are only displayed, so change tracking is turned off.
    /// </summary>
    public List<Show> GetShowsInDateRange(DateTime start, DateTime end)
    {
        return _db.Shows
            .AsNoTracking()
            .Where(s => s.Date >= start && s.Date < end)
            .OrderBy(s => s.Date)
            .ToList();
    }

    /// <summary>
    /// Returns every event with the number of its shows.
    /// </summary>
    public List<EventWithShowCountDto> GetEventsWithShowCount()
    {
        return _db.Events
            .Select(e => new EventWithShowCountDto(e.Id, e.Name, e.Shows.Count))
            .ToList();
    }

    /// <summary>
    /// Returns an event with all of its shows or null if the id does not exist.
    /// </summary>
    public EventWithShowsDto? GetEventWithShows(int eventId)
    {
        return _db.Events
            .Where(e => e.Id == eventId)
            .Select(e => new EventWithShowsDto(
                e.Id, e.Name,
                e.Shows.OrderBy(s => s.Date).Select(s => new ShowDto(s.Id, s.Date)).ToList()))
            .FirstOrDefault();
    }

    /// <summary>
    /// Counts the sold and reserved persons (guest + pax) of a contingent.
    /// </summary>
    /// <exception cref="EventServiceException">The contingent does not exist.</exception>
    public ContingentStatisticsDto GetContingentStatistics(int contingentId)
    {
        var statistics = _db.Contingents
            .Where(c => c.Id == contingentId)
            .Select(c => new ContingentStatisticsDto(
                c.Id,
                c.Show.Date,
                c.Tickets.Where(t => t.TicketState == TicketState.Sold).Sum(t => t.Pax + 1),
                c.Tickets.Where(t => t.TicketState == TicketState.Reserved).Sum(t => t.Pax + 1)))
            .FirstOrDefault();
        if (statistics is null)
            throw new EventServiceException("Invalid contingent id.");
        return statistics;
    }

    /// <summary>
    /// Creates a reservation and returns the id of the new ticket.
    /// Rules: the ids must exist, the show must be at least 14 days after the reservation,
    /// a guest can only have one ticket per contingent and the contingent must not be sold out.
    /// </summary>
    /// <exception cref="EventServiceException">A business rule is violated.</exception>
    public int CreateReservation(int guestId, int contingentId, int pax, DateTime dateTime)
    {
        var contingent = _db.Contingents
            .Include(c => c.Show)
            .Include(c => c.Tickets)
            .FirstOrDefault(c => c.Id == contingentId);
        if (contingent is null)
            throw new EventServiceException("Invalid contingent id.");
        if (contingent.Show.Date < dateTime.AddDays(14))
            throw new EventServiceException("The show is too close in time.");

        var guest = _db.Guests.FirstOrDefault(g => g.Id == guestId);
        if (guest is null)
            throw new EventServiceException("Invalid guest id.");

        if (_db.Tickets.Any(t => t.Contingent.Id == contingentId && t.Guest.Id == guestId))
            throw new EventServiceException("A reservation or purchase has already been made for this contingent.");

        // Each ticket admits the guest plus the accompanying persons (pax).
        var soldOrReserved = contingent.Tickets.Sum(t => t.Pax + 1);
        if (soldOrReserved + pax + 1 > contingent.AvailableTickets)
            throw new EventServiceException("Show is sold out.");

        var ticket = new Ticket(guest, contingent, TicketState.Reserved, dateTime, pax);
        _db.Tickets.Add(ticket);
        _db.SaveChanges();
        return ticket.Id;
    }

    /// <summary>
    /// Changes a reserved ticket to sold.
    /// </summary>
    /// <exception cref="EventServiceException">The ticket does not exist or is already sold.</exception>
    public void SellTicket(int ticketId)
    {
        var ticket = _db.Tickets.FirstOrDefault(t => t.Id == ticketId);
        if (ticket is null)
            throw new EventServiceException("Invalid ticket id.");
        if (ticket.TicketState == TicketState.Sold)
            throw new EventServiceException("The ticket is already sold.");
        // The change tracker detects the change and SaveChanges sends an UPDATE.
        ticket.TicketState = TicketState.Sold;
        _db.SaveChanges();
    }

    /// <summary>
    /// Deletes all reservations (not sold tickets) made before the given point in time.
    /// Returns the number of deleted tickets.
    /// </summary>
    public int DeleteReservationsBefore(DateTime limit)
    {
        // ExecuteDelete sends DELETE directly to the database without loading the entities.
        return _db.Tickets
            .Where(t => t.TicketState == TicketState.Reserved && t.ReservationDateTime < limit)
            .ExecuteDelete();
    }
}
