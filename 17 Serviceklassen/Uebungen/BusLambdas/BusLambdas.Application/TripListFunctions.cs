using System;
using System.Collections.Generic;

namespace BusLambdas.Application;

/// <summary>
/// Part A: methods that receive a lambda as parameter. Do not use LINQ here,
/// write a foreach loop and call the lambda yourself.
/// </summary>
public static class TripListFunctions
{
    /// <summary>
    /// Returns a new list with the trips for which predicate returns true (same order).
    /// </summary>
    public static List<Trip> Filter(List<Trip> trips, Func<Trip, bool> predicate)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a new list with the result of projection for every trip (same order).
    /// </summary>
    public static List<string> Map(List<Trip> trips, Func<Trip, string> projection)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Calls action for every trip.
    /// </summary>
    public static void ForEach(List<Trip> trips, Action<Trip> action)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns true if predicate returns true for at least one trip.
    /// </summary>
    public static bool Exists(List<Trip> trips, Func<Trip, bool> predicate)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the number of trips for which predicate returns true.
    /// </summary>
    public static int Count(List<Trip> trips, Func<Trip, bool> predicate)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the first trip for which predicate returns true, or null if there is none.
    /// </summary>
    public static Trip? FindFirst(List<Trip> trips, Func<Trip, bool> predicate)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the sum of selector over all trips.
    /// </summary>
    public static int Sum(List<Trip> trips, Func<Trip, int> selector)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }
}
