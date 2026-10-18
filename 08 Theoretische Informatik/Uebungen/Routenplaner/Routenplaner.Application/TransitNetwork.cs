namespace Routenplaner.Application;

/// <summary>
/// The network as a weighted graph with one node (<see cref="Stop"/>) per station and line.
/// Every connection becomes two ride edges (one per direction). At a station with several lines,
/// every stop has a transfer edge to every other stop of the station.
/// Instances are immutable.
/// </summary>
public sealed class TransitNetwork
{
    /// <summary>
    /// Builds the graph.
    /// </summary>
    /// <param name="connections">The sections of all lines.</param>
    /// <param name="transferMinutes">Fixed time for a transfer edge in minutes (at least 1).</param>
    /// <exception cref="ArgumentNullException"><paramref name="connections"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="transferMinutes"/> is less than 1.</exception>
    public TransitNetwork(IEnumerable<Connection> connections, int transferMinutes)
    {
        throw new NotImplementedException();
    }

    /// <summary>Time of every transfer edge in minutes.</summary>
    public int TransferMinutes => throw new NotImplementedException();

    /// <summary>All nodes of the graph, in any order.</summary>
    public IReadOnlyCollection<Stop> Stops => throw new NotImplementedException();

    /// <summary>The names of all stations, each exactly once, in any order.</summary>
    public IReadOnlyCollection<string> Stations => throw new NotImplementedException();

    /// <summary>
    /// Returns all stops of a station, sorted by line name (ordinal).
    /// </summary>
    /// <param name="station">Name of the station.</param>
    /// <returns>The stops, or an empty list if the station does not exist.</returns>
    public IReadOnlyList<Stop> StopsAt(string station)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all edges that start at <paramref name="stop"/>: the rides to the neighbouring
    /// stations and the transfers to the other lines of the station, in any order.
    /// </summary>
    /// <param name="stop">A node of the graph.</param>
    /// <exception cref="ArgumentException"><paramref name="stop"/> is not a node of the graph.</exception>
    public IReadOnlyList<Edge> EdgesFrom(Stop stop)
    {
        throw new NotImplementedException();
    }
}
