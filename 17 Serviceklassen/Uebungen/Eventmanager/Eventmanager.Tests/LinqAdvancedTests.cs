using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eventmanager.Tests;

/// <summary>
/// Examples of the chapter "Erweiterte LINQ Techniken": GroupBy, SelectMany, Join and Aggregate.
/// </summary>
public class LinqAdvancedTests : DatabaseTest
{
    /// <summary>
    /// A public holiday. In a real application the list comes from a file or a web API,
    /// so there is no foreign key to the shows.
    /// </summary>
    private record Holiday(DateOnly Date, string Name);

    private static readonly List<Holiday> _holidays = new()
    {
        new Holiday(new DateOnly(2023, 8, 15), "Assumption Day"),
        new Holiday(new DateOnly(2023, 10, 26), "National Day"),
        new Holiday(new DateOnly(2023, 12, 8), "Immaculate Conception"),
        new Holiday(new DateOnly(2024, 1, 1), "New Year"),
        new Holiday(new DateOnly(2024, 1, 6), "Epiphany"),
        new Holiday(new DateOnly(2024, 5, 1), "Labour Day"),
    };

    [Fact]
    public void GroupByWithAggregate()
    {
        var personsPerType = _db.Tickets
            .GroupBy(t => t.Contingent.ContingentType)
            .Select(g => new { ContingentType = g.Key, Tickets = g.Count(), Persons = g.Sum(t => t.Pax + 1) })
            .ToList();
        personsPerType.ForEach(Print);
        Assert.Equal(3, personsPerType.Count);
    }

    [Fact]
    public void GroupByTwoKeys()
    {
        var showsPerMonth = _db.Shows
            .GroupBy(s => new { s.Date.Year, s.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToList();
        showsPerMonth.ForEach(Print);
        Assert.Equal(_db.Shows.Count(), showsPerMonth.Sum(x => x.Count));
    }

    [Fact]
    public void NavigationInsteadOfGroupBy()
    {
        // Same information as GROUP BY, but events without shows are included.
        var showsPerEvent = _db.Events
            .Select(e => new { e.Name, Count = e.Shows.Count })
            .ToList();
        showsPerEvent.ForEach(Print);
        Assert.Equal(_db.Events.Count(), showsPerEvent.Count);
    }

    [Fact]
    public void SelectManyFlattensLists()
    {
        List<string> guestNames = _db.Events
            .Where(e => e.Id == 1)
            .SelectMany(e => e.Shows)
            .SelectMany(s => s.Contingents)
            .SelectMany(c => c.Tickets)
            .Select(t => t.Guest.Lastname)
            .Distinct()
            .OrderBy(n => n)
            .ToList();
        guestNames.ForEach(Print);
        Assert.NotEmpty(guestNames);
    }

    [Fact]
    public void JoinWithoutNavigation()
    {
        var shows = _db.Shows
            .Select(s => new { s.Id, s.Date, EventName = s.Event.Name })
            .ToList();

        var showsOnHolidays = shows
            .Join(_holidays,
                s => DateOnly.FromDateTime(s.Date),
                h => h.Date,
                (s, h) => new { s.EventName, s.Date, Holiday = h.Name })
            .ToList();
        showsOnHolidays.ForEach(Print);

        var allShows = shows
            .LeftJoin(_holidays,
                s => DateOnly.FromDateTime(s.Date),
                h => h.Date,
                (s, h) => new { s.EventName, s.Date, Holiday = h?.Name })
            .ToList();
        Assert.Equal(shows.Count, allShows.Count);
    }

    [Fact]
    public void AggregateReducesAList()
    {
        List<string> eventNames = _db.Events.OrderBy(e => e.Name).Select(e => e.Name).ToList();

        string text = eventNames.Aggregate((result, name) => result + ", " + name);
        int letters = eventNames.Aggregate(0, (sum, name) => sum + name.Length);
        Print(text);
        Print(letters);

        Assert.Equal(string.Join(", ", eventNames), text);
        Assert.Equal(eventNames.Sum(n => n.Length), letters);
    }

    [Fact]
    public void AggregateCannotBeTranslated()
    {
        Assert.Throws<InvalidOperationException>(
            () => _db.Events.Select(e => e.Name).Aggregate((result, name) => result + ", " + name));
    }
}
