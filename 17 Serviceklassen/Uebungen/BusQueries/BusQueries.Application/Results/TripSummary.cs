using System;

namespace BusQueries.Application.Results;

/// <summary>
/// Overview of a trip. OperatorName is "FirstName LastName" or null for conventional buses.
/// </summary>
public record TripSummary(
    int TripId, string LineNumber, string LicensePlate, string? OperatorName,
    DateTime PlannedDeparture, bool IsCancelled, int InterventionCount);
