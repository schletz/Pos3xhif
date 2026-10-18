namespace Routenplaner.Application;

/// <summary>
/// Searches connections between two stations of a <see cref="TransitNetwork"/>.
/// The search may start on any line of the start station and ends as soon as the destination
/// station is reached on any line. Transfers at the start or at the destination never occur.
/// </summary>
/// <param name="network">The network to search in.</param>
/// <exception cref="ArgumentNullException"><paramref name="network"/> is <c>null</c>.</exception>
public sealed class RoutePlanner(TransitNetwork network)
{
    private readonly TransitNetwork _network = network ?? throw new ArgumentNullException(nameof(network));

    /// <summary>
    /// Finds the connection with the fewest stations using breadth-first search.
    /// Transfers do not count as a station.
    /// </summary>
    /// <param name="from">Name of the start station.</param>
    /// <param name="to">Name of the destination station.</param>
    /// <returns>The route, or <c>null</c> if the destination cannot be reached.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="from"/> or <paramref name="to"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// A station does not exist, or <paramref name="from"/> and <paramref name="to"/> are equal.
    /// </exception>
    public Route? FewestStations(string from, string to)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Finds the fastest connection (rides plus transfers) using Dijkstra's algorithm.
    /// </summary>
    /// <param name="from">Name of the start station.</param>
    /// <param name="to">Name of the destination station.</param>
    /// <returns>The route, or <c>null</c> if the destination cannot be reached.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="from"/> or <paramref name="to"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// A station does not exist, or <paramref name="from"/> and <paramref name="to"/> are equal.
    /// </exception>
    public Route? Fastest(string from, string to)
    {
        throw new NotImplementedException();
    }
}
