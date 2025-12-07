using Eventmanager.Infrastructure;
using Eventmanager.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventmanager.Services;

public record ContingentStatistics(int SoldTickets, int ReservedTickets, Show Show);

public class EventService
{
    private readonly EventContext _db;

    public EventService(EventContext db)
    {
        _db = db;
    }
    public List<Show> GetShowsInDateRange(DateTime start, DateTime end)
    {
        return _db.Shows
            .Where(s => s.Date >= start && s.Date <= end)
            .ToList();
    }


    public record EventWithShowCountDto(int EventId, string EventName, int ShowCount);
    public List<EventWithShowCountDto> GetEventsWithShowCount()
    {
        return _db.Events
            .Select(e => new EventWithShowCountDto(
                e.Id,
                e.Name,
                e.Shows.Count))
            .ToList();
    }

    public record EventWithShowsDto(int EventId, string EventName, List<ShowDto> Shows);
    public record ShowDto(int ShowId, DateTime Date);

    public EventWithShowsDto? GetEventsWithShows(int eventId)
    {
        return _db.Events
            .Where(e => e.Id == eventId)
            .Select(e => new EventWithShowsDto(
                e.Id, e.Name,
                e.Shows.Select(s => new ShowDto(s.Id, s.Date)).ToList()))
            .FirstOrDefault();
    }
    public ContingentStatistics CalcContingentStatistics(int contingentId)
    {
        var statistics = _db.Contingents
            .Where(c => c.Id == contingentId)
            .Select(c => new ContingentStatistics(
                c.Tickets.Where(t => t.TicketState == TicketState.Sold).Sum(t => t.Pax + 1),
                c.Tickets.Where(t => t.TicketState == TicketState.Reserved).Sum(t => t.Pax + 1),
                c.Show))
            .FirstOrDefault();
        if (statistics is null) { throw new ApplicationException("Invalid contingent id"); }
        return statistics;
    }

    public int CreateReservation(int guestId, int contingentId, int pax, DateTime dateTime)
    {
        var contingent = _db.Contingents
            .Include(c => c.Show).Include(c => c.Tickets).FirstOrDefault(c => c.Id == contingentId);
        if (contingent is null)
            throw new EventServiceException("Invalid contingent id.");
        if (contingent.Show.Date < dateTime.AddDays(14))
            throw new EventServiceException("The show is too close in time.");

        var guest = _db.Guests.FirstOrDefault(g => g.Id == guestId);
        if (guest is null)
            throw new EventServiceException("Invalid guest id.");

        if (_db.Tickets.Any(t => t.Contingent.Id == contingentId && t.Guest.Id == guestId && t.ReservationDateTime.Date == dateTime.Date))
            throw new EventServiceException("A reservation or purchase has already been made for this contingent.");

        var soldOrReserved = contingent.Tickets.Sum(t => t.Pax + 1);
        if (soldOrReserved + pax + 1 > contingent.AvailableTickets)
            throw new EventServiceException("Show is sold out.");
        var ticket = new Ticket(guest, contingent, TicketState.Reserved, dateTime, pax);
        _db.Tickets.Add(ticket);
        _db.SaveChanges();
        return ticket.Id;
    }
}