using System;

namespace BusQueries.Application.Results;

/// <summary>
/// A trip that left later than planned. DelayMinutes is the delay in whole minutes.
/// </summary>
public record DelayedTrip(int TripId, string LineNumber, DateTime PlannedDeparture, int DelayMinutes);
