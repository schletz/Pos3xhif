namespace BusQueries.Application.Model;

/// <summary>
/// Why the safety operator had to take over control of an autonomous bus.
/// </summary>
public enum InterventionReason
{ Obstacle, Weather, SensorFailure, Passenger, Traffic }
