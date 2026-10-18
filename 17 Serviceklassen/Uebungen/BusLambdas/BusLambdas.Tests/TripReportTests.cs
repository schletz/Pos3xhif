// *************************************************************************************************
// Tests for part C. DO NOT CHANGE THIS FILE.
// *************************************************************************************************
using System.Collections.Generic;
using BusLambdas.Application;
using Xunit;

namespace BusLambdas.Tests;

public class TripReportTests
{
    private readonly List<Trip> _trips = TripData.Create();

    [Fact(DisplayName = "GetLabelsOfDelayedTrips returns the labels of the delayed trips.")]
    public void GetLabelsOfDelayedTripsTest()
    {
        Assert.Equal(new[] { "A1 07:30", "A3 08:00" }, TripReport.GetLabelsOfDelayedTrips(_trips));
    }

    [Fact(DisplayName = "CountPassengersOnLine adds up the passengers of a line (not counted = 0).")]
    public void CountPassengersOnLineTest()
    {
        Assert.Equal(21, TripReport.CountPassengersOnLine(_trips, "A1"));
        Assert.Equal(31, TripReport.CountPassengersOnLine(_trips, "A3"));
        Assert.Equal(0, TripReport.CountPassengersOnLine(_trips, "N1"));
    }

    [Fact(DisplayName = "GetLicensePlatesOfWaitingTrips returns the vehicles of trips that are neither cancelled nor departed.")]
    public void GetLicensePlatesOfWaitingTripsTest()
    {
        Assert.Equal(new[] { "DO-AUTO3" }, TripReport.GetLicensePlatesOfWaitingTrips(_trips));
    }
}
