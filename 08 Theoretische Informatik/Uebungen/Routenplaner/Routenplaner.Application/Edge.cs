namespace Routenplaner.Application;

/// <summary>
/// A directed edge of the graph. It is either a ride to the neighbouring station on the same line
/// or a transfer to another line at the same station.
/// </summary>
/// <param name="From">Stop where the edge starts.</param>
/// <param name="To">Stop where the edge ends.</param>
/// <param name="Minutes">Travel time or transfer time in minutes.</param>
public sealed record Edge(Stop From, Stop To, int Minutes)
{
    /// <summary>
    /// <c>true</c> for a transfer (same station, different line), <c>false</c> for a ride
    /// between two stations.
    /// </summary>
    public bool IsTransfer => From.Station == To.Station;

    /// <summary>Returns the edge in the form <c>Karlsplatz (U1) -> Stephansplatz (U1), 2 min</c>.</summary>
    public override string ToString() => $"{From} -> {To}, {Minutes} min";
}
