using System;
using System.Collections.Generic;

namespace BusQueries.Application.Model;

/// <summary>
/// One trip of a vehicle on a line, starting at the first stop. ActualDeparture is null
/// as long as the bus has not left. PassengerCount is null if nobody counted.
/// </summary>
public class Trip
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Trip() { }
#pragma warning restore CS8618

    public Trip(Line line, Vehicle vehicle, SafetyOperator? safetyOperator, DateTime plannedDeparture)
    {
        Line = line;
        Vehicle = vehicle;
        SafetyOperator = safetyOperator;
        PlannedDeparture = plannedDeparture;
    }

    public int Id { get; set; }
    public Line Line { get; set; }
    public Vehicle Vehicle { get; set; }
    public SafetyOperator? SafetyOperator { get; set; }
    public DateTime PlannedDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public bool IsCancelled { get; set; }
    public int? PassengerCount { get; set; }
    public List<Intervention> Interventions { get; } = new();
}
