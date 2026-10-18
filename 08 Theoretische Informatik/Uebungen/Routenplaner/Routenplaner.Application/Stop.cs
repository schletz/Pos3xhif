namespace Routenplaner.Application;

/// <summary>
/// A node of the graph: a station on one particular line. A station served by three lines
/// (e.g. Karlsplatz with U1, U2 and U4) is represented by three stops.
/// </summary>
/// <param name="Station">Name of the station.</param>
/// <param name="Line">Name of the line.</param>
public sealed record Stop(string Station, string Line)
{
    /// <summary>Returns the stop in the form <c>Karlsplatz (U1)</c>.</summary>
    public override string ToString() => $"{Station} ({Line})";
}
