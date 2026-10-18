using System;
using System.Collections.Generic;
using System.Linq;
using Eventmanager.Application.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Eventmanager.Tests;

/// <summary>
/// Examples of the chapter "LINQ: Where, Select und Any". Run a test and look at its output:
/// it shows the SQL statement EF Core generated from the LINQ query.
/// Add your own tests here to try out queries.
/// </summary>
public class LinqPlaygroundTests : DatabaseTest
{
    [Fact]
    public void WhereFiltersShows()
    {
        var start = new DateTime(2024, 2, 1);
        List<Show> shows = _db.Shows
            .Where(s => s.Date >= start && s.Date < start.AddMonths(2))
            .ToList();
        Assert.NotEmpty(shows);
    }

    [Fact]
    public void SelectProjectsGuests()
    {
        List<string> names = _db.Guests
            .Where(g => g.BirthDate.Year >= 2003)
            .Select(g => g.Firstname + " " + g.Lastname)
            .ToList();
        names.ForEach(Print);
        Assert.NotEmpty(names);
    }

    [Fact]
    public void AnyCreatesExists()
    {
        List<Event> events = _db.Events
            .Where(e => e.Shows.Any(s => s.Date.Year == 2024))
            .ToList();
        Assert.NotEmpty(events);
    }

    [Fact]
    public void NotAnyCreatesNotExists()
    {
        List<Guest> guestsWithoutPurchase = _db.Guests
            .Where(g => !g.Tickets.Any(t => t.TicketState == TicketState.Sold))
            .ToList();
        Print(guestsWithoutPurchase.Count);
    }

    [Fact]
    public void AllOnDatabase()
    {
        bool allInTheEvening = _db.Shows.All(s => s.Date.Hour >= 18);
        Assert.True(allInTheEvening);
    }

    [Fact]
    public void ToQueryStringShowsSql()
    {
        IQueryable<DateTime> query = _db.Shows
            .Where(s => s.Event.Name == "Funk")
            .Select(s => s.Date);
        Print(query.ToQueryString());
    }

    [Fact]
    public void TimeSpanCannotBeTranslated()
    {
        // SQLite has no data type for time spans. EF Core cannot translate the condition.
        Assert.Throws<InvalidOperationException>(() => _db.Tickets
            .Where(t => (t.Contingent.Show.Date - t.ReservationDateTime).TotalDays > 20)
            .ToList());
    }

    [Fact]
    public void ToListThenFilterInMemory()
    {
        // Filter as much as possible in the database, then continue in memory.
        var tickets = _db.Tickets
            .Where(t => t.TicketState == TicketState.Reserved)
            .Select(t => new { t.Id, t.ReservationDateTime, ShowDate = t.Contingent.Show.Date })
            .ToList()
            .Where(t => (t.ShowDate - t.ReservationDateTime).TotalDays > 20)
            .ToList();
        Print(tickets.Count);
        Assert.NotEmpty(tickets);
    }

    [Fact]
    public void RewrittenConditionCanBeTranslated()
    {
        // The same condition without TimeSpan: SQLite can calculate AddDays.
        var count = _db.Tickets
            .Count(t => t.TicketState == TicketState.Reserved
                && t.ReservationDateTime < t.Contingent.Show.Date.AddDays(-20));
        Print(count);
        Assert.True(count > 0);
    }
}
