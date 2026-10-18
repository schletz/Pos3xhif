// *************************************************************************************************
// Tests for part B. DO NOT CHANGE THIS FILE.
// *************************************************************************************************
using System.Collections.Generic;
using BusLambdas.Application;
using Xunit;

namespace BusLambdas.Tests;

public class TripRulesTests
{
    private readonly List<Trip> _trips = TripData.Create();

    private Trip TripWithId(int id) => _trips[id - 1];

    [Fact(DisplayName = "IsCancelled is true only for cancelled trips.")]
    public void IsCancelledTest()
    {
        Assert.True(TripRules.IsCancelled(TripWithId(5)));
        Assert.False(TripRules.IsCancelled(TripWithId(6)));
    }

    [Fact(DisplayName = "HasDeparted is true if ActualDeparture is set.")]
    public void HasDepartedTest()
    {
        Assert.True(TripRules.HasDeparted(TripWithId(1)));
        Assert.False(TripRules.HasDeparted(TripWithId(6)));
    }

    [Fact(DisplayName = "IsDelayed is true for a delay of more than 2 minutes.")]
    public void IsDelayedTest()
    {
        Assert.True(TripRules.IsDelayed(TripWithId(2)));    // 6 minutes
        Assert.True(TripRules.IsDelayed(TripWithId(4)));    // 3 minutes
        Assert.False(TripRules.IsDelayed(TripWithId(7)));   // exactly 2 minutes
        Assert.False(TripRules.IsDelayed(TripWithId(3)));   // on time
        Assert.False(TripRules.IsDelayed(TripWithId(6)));   // not departed
    }

    [Fact(DisplayName = "PassengersOrZero returns 0 if nobody counted.")]
    public void PassengersOrZeroTest()
    {
        Assert.Equal(31, TripRules.PassengersOrZero(TripWithId(4)));
        Assert.Equal(0, TripRules.PassengersOrZero(TripWithId(7)));
    }

    [Fact(DisplayName = "Label returns line number and planned departure (HH:mm).")]
    public void LabelTest()
    {
        Assert.Equal("A1 07:00", TripRules.Label(TripWithId(1)));
        Assert.Equal("A3 10:00", TripRules.Label(TripWithId(7)));
    }

    [Fact(DisplayName = "Cancel returns a cancelled copy and does not change the original trip.")]
    public void CancelTest()
    {
        var original = TripWithId(6);

        var cancelled = TripRules.Cancel(original);

        Assert.True(cancelled.IsCancelled);
        Assert.False(original.IsCancelled);
        Assert.Equal(original with { IsCancelled = true }, cancelled);
    }

    [Fact(DisplayName = "SameVehicle compares the license plates of two trips.")]
    public void SameVehicleTest()
    {
        Assert.True(TripRules.SameVehicle(TripWithId(1), TripWithId(2)));
        Assert.False(TripRules.SameVehicle(TripWithId(1), TripWithId(3)));
    }

    [Fact(DisplayName = "IsOnLine returns a rule for the given line.")]
    public void IsOnLineTest()
    {
        var isOnA1 = TripRules.IsOnLine("A1");
        var isOnA3 = TripRules.IsOnLine("A3");

        Assert.True(isOnA1(TripWithId(1)));
        Assert.False(isOnA1(TripWithId(4)));
        Assert.True(isOnA3(TripWithId(4)));
    }

    [Fact(DisplayName = "LogTo returns an action that adds the label to the list.")]
    public void LogToTest()
    {
        var log = new List<string>();
        var logAction = TripRules.LogTo(log);

        logAction(TripWithId(1));
        logAction(TripWithId(3));

        Assert.Equal(new[] { "A1 07:00", "A2 07:15" }, log);
    }
}
