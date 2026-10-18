namespace BusQueries.Application.Results;

/// <summary>
/// A stop of a line with its position and the travel time from the first stop.
/// </summary>
public record LineStopInfo(int Position, string StopName, string Municipality, int MinutesFromStart);
