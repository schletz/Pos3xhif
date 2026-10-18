// *************************************************************************************************
// Tests for part A. DO NOT CHANGE THIS FILE.
// *************************************************************************************************
using System.Collections.Generic;
using System.Linq;
using BusLambdas.Application;
using Xunit;

namespace BusLambdas.Tests;

public class TripListFunctionsTests
{
    private readonly List<Trip> _trips = TripData.Create();

    [Fact(DisplayName = "Filter returns a new list with the matching trips in the same order.")]
    public void FilterTest()
    {
        var a2Trips = TripListFunctions.Filter(_trips, t => t.LineNumber == "A2");
        var none = TripListFunctions.Filter(_trips, t => t.Id > 100);

        Assert.Equal(new[] { 3, 5 }, a2Trips.Select(t => t.Id));
        Assert.Empty(none);
        Assert.Equal(7, _trips.Count);
    }

    [Fact(DisplayName = "Map returns the projected value of every trip in the same order.")]
    public void MapTest()
    {
        var plates = TripListFunctions.Map(_trips, t => t.LicensePlate);

        Assert.Equal(
            new[] { "DO-AUTO1", "DO-AUTO1", "DO-AUTO2", "DO-BUS40", "DO-AUTO2", "DO-AUTO3", "DO-AUTO3" },
            plates);
    }

    [Fact(DisplayName = "ForEach calls the action for every trip.")]
    public void ForEachTest()
    {
        int idSum = 0;
        var visited = new List<int>();

        TripListFunctions.ForEach(_trips, t => idSum += t.Id);
        TripListFunctions.ForEach(_trips, t => visited.Add(t.Id));

        Assert.Equal(28, idSum);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, visited);
    }

    [Fact(DisplayName = "Exists returns true if at least one trip matches.")]
    public void ExistsTest()
    {
        bool hasA3 = TripListFunctions.Exists(_trips, t => t.LineNumber == "A3");
        bool hasN1 = TripListFunctions.Exists(_trips, t => t.LineNumber == "N1");

        Assert.True(hasA3);
        Assert.False(hasN1);
    }

    [Fact(DisplayName = "Count returns the number of matching trips.")]
    public void CountTest()
    {
        Assert.Equal(1, TripListFunctions.Count(_trips, t => t.IsCancelled));
        Assert.Equal(3, TripListFunctions.Count(_trips, t => t.PassengerCount == null));
        Assert.Equal(0, TripListFunctions.Count(_trips, t => t.Id > 100));
    }

    [Fact(DisplayName = "FindFirst returns the first matching trip or null.")]
    public void FindFirstTest()
    {
        var first = TripListFunctions.FindFirst(_trips, t => t.LicensePlate == "DO-AUTO3");
        var none = TripListFunctions.FindFirst(_trips, t => t.LicensePlate == "W-12345");

        Assert.Equal(6, first?.Id);
        Assert.Null(none);
    }

    [Fact(DisplayName = "Sum adds up the selected values.")]
    public void SumTest()
    {
        Assert.Equal(28, TripListFunctions.Sum(_trips, t => t.Id));
        Assert.Equal(0, TripListFunctions.Sum(new List<Trip>(), t => t.Id));
    }
}
