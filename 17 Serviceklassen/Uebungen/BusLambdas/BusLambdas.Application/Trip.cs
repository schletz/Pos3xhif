using System;

namespace BusLambdas.Application;

/// <summary>
/// A trip of an autonomous bus. ActualDeparture is null as long as the bus has not left,
/// PassengerCount is null if nobody counted.
/// </summary>
public record Trip(
    int Id, string LineNumber, string LicensePlate, DateTime PlannedDeparture,
    DateTime? ActualDeparture, int? PassengerCount, bool IsCancelled);
