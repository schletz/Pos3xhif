using System;
using System.Collections.Generic;

namespace BusLambdas.Application;

/// <summary>
/// Part B: rules written as lambdas. Replace every throw expression with a lambda,
/// e.g. <c>public static Func&lt;Trip, bool&gt; IsCancelled =&gt; t =&gt; ...;</c>
/// </summary>
public static class TripRules
{
    /// <summary>
    /// True if the trip is cancelled.
    /// </summary>
    public static Func<Trip, bool> IsCancelled => throw new NotImplementedException();

    /// <summary>
    /// True if the bus has left (ActualDeparture is not null).
    /// </summary>
    public static Func<Trip, bool> HasDeparted => throw new NotImplementedException();

    /// <summary>
    /// True if the bus left more than 2 minutes after the planned departure.
    /// </summary>
    public static Func<Trip, bool> IsDelayed => throw new NotImplementedException();

    /// <summary>
    /// The number of passengers, 0 if nobody counted.
    /// </summary>
    public static Func<Trip, int> PassengersOrZero => throw new NotImplementedException();

    /// <summary>
    /// A label like "A1 07:30" (line number, space, planned departure as HH:mm).
    /// </summary>
    public static Func<Trip, string> Label => throw new NotImplementedException();

    /// <summary>
    /// A copy of the trip with IsCancelled set to true.
    /// </summary>
    public static Func<Trip, Trip> Cancel => throw new NotImplementedException();

    /// <summary>
    /// True if both trips use the same vehicle (same license plate).
    /// </summary>
    public static Func<Trip, Trip, bool> SameVehicle => throw new NotImplementedException();

    /// <summary>
    /// Returns a rule that is true if the trip belongs to the given line.
    /// </summary>
    public static Func<Trip, bool> IsOnLine(string lineNumber) => throw new NotImplementedException();

    /// <summary>
    /// Returns an action that adds the label of a trip (see <see cref="Label"/>) to the log.
    /// </summary>
    public static Action<Trip> LogTo(List<string> log) => throw new NotImplementedException();
}
