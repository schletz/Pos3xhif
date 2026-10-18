using System;
using System.Linq;
using Eventmanager.Application.Model;
using Eventmanager.Application.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Eventmanager.Tests;

/// <summary>
/// Tests of the service methods shown in the chapter "Serviceklassen mit EF Core".
/// </summary>
public class EventServiceTests : DatabaseTest
{
    private readonly EventService _service;

    public EventServiceTests()
    {
        _service = new EventService(_db);
    }

    [Fact]
    public void GetShowsInDateRange_ReturnsOnlyShowsInRange()
    {
        var start = new DateTime(2024, 2, 1);
        var end = new DateTime(2024, 4, 1);

        var shows = _service.GetShowsInDateRange(start, end);

        Assert.NotEmpty(shows);
        Assert.All(shows, s => Assert.InRange(s.Date, start, end.AddTicks(-1)));
    }

    [Fact]
    public void GetEventsWithShowCount_CountsAllShows()
    {
        var result = _service.GetEventsWithShowCount();

        Assert.Equal(_db.Events.Count(), result.Count);
        Assert.Equal(_db.Shows.Count(), result.Sum(r => r.ShowCount));
    }

    [Fact]
    public void GetEventWithShows_ReturnsNullForUnknownId()
    {
        Assert.Null(_service.GetEventWithShows(9999));
    }

    [Fact]
    public void CreateReservation_AddsTicket()
    {
        // A guest without a ticket in this contingent and a show far enough in the future.
        var contingent = _db.Contingents
            .Include(c => c.Show)
            .Include(c => c.Tickets).ThenInclude(t => t.Guest)
            .AsEnumerable()
            .First(c => c.Tickets.Sum(t => t.Pax + 1) + 1 <= c.AvailableTickets
                && c.Tickets.Select(t => t.Guest.Id).Distinct().Count() < _db.Guests.Count());
        var guest = _db.Guests.First(g => !g.Tickets.Any(t => t.Contingent.Id == contingent.Id));

        var ticketId = _service.CreateReservation(guest.Id, contingent.Id, 0, contingent.Show.Date.AddDays(-20));

        _db.ChangeTracker.Clear();
        var ticket = _db.Tickets.Include(t => t.Guest).First(t => t.Id == ticketId);
        Assert.Equal(TicketState.Reserved, ticket.TicketState);
        Assert.Equal(guest.Id, ticket.Guest.Id);
    }

    [Fact]
    public void CreateReservation_ThrowsIfShowIsTooClose()
    {
        var contingent = _db.Contingents.Include(c => c.Show).First();
        var guest = _db.Guests.First();

        var e = Assert.Throws<EventServiceException>(
            () => _service.CreateReservation(guest.Id, contingent.Id, 0, contingent.Show.Date.AddDays(-3)));
        Assert.Equal("The show is too close in time.", e.Message);
    }

    [Fact]
    public void SellTicket_ChangesStateToSold()
    {
        var ticketId = _db.Tickets.First(t => t.TicketState == TicketState.Reserved).Id;

        _service.SellTicket(ticketId);

        _db.ChangeTracker.Clear();
        Assert.Equal(TicketState.Sold, _db.Tickets.First(t => t.Id == ticketId).TicketState);
    }

    [Fact]
    public void DeleteReservationsBefore_DeletesOnlyReservations()
    {
        var limit = new DateTime(2024, 1, 1);
        var expected = _db.Tickets.Count(t => t.TicketState == TicketState.Reserved && t.ReservationDateTime < limit);
        var soldBefore = _db.Tickets.Count(t => t.TicketState == TicketState.Sold);

        var deleted = _service.DeleteReservationsBefore(limit);

        Assert.True(expected > 0);
        Assert.Equal(expected, deleted);
        Assert.Equal(soldBefore, _db.Tickets.Count(t => t.TicketState == TicketState.Sold));
    }
}
