namespace Routenplaner.Application;

/// <summary>
/// Part of a route that is travelled on one line without changing trains.
/// </summary>
/// <param name="Line">Name of the line.</param>
/// <param name="From">Station where you get on.</param>
/// <param name="To">Station where you get off.</param>
/// <param name="Stations">Number of rides from station to station (at least 1).</param>
/// <param name="Minutes">Sum of the travel times of these rides, without transfer time.</param>
public sealed record Leg(string Line, string From, string To, int Stations, int Minutes);
