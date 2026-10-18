using System;
using System.Collections.Generic;
using BusLambdas.Application;

namespace BusLambdas.Tests;

/// <summary>
/// Sample trips of Monday, 5 October 2026. The expected values in the tests are calculated by hand
/// from this list.
/// </summary>
public static class TripData
{
    private static DateTime At(int hour, int minute) => new DateTime(2026, 10, 5, hour, minute, 0);

    public static List<Trip> Create() => new()
    {
        new Trip(1, "A1", "DO-AUTO1", At(7, 0), At(7, 1), 9, false),
        new Trip(2, "A1", "DO-AUTO1", At(7, 30), At(7, 36), 12, false),
        new Trip(3, "A2", "DO-AUTO2", At(7, 15), At(7, 15), 5, false),
        new Trip(4, "A3", "DO-BUS40", At(8, 0), At(8, 3), 31, false),
        new Trip(5, "A2", "DO-AUTO2", At(9, 15), null, null, true),
        new Trip(6, "A1", "DO-AUTO3", At(9, 30), null, null, false),
        new Trip(7, "A3", "DO-AUTO3", At(10, 0), At(10, 2), null, false),
    };
}
