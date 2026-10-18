using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Writes the legs of a route in a compact form, so that a test can compare a whole route
/// with one string and a failing test shows the complete route.
/// </summary>
public static class RouteFormat
{
    /// <summary>
    /// Returns the legs in the form <c>U1: Hauptbahnhof -> Stephansplatz (3, 5) | U3: ...</c>,
    /// where the numbers are stations and minutes of the leg.
    /// </summary>
    public static string Compact(Route route) =>
        string.Join(" | ", route.Legs.Select(leg => $"{leg.Line}: {leg.From} -> {leg.To} ({leg.Stations}, {leg.Minutes})"));
}
