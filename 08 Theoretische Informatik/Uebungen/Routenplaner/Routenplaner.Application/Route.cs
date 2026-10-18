namespace Routenplaner.Application;

/// <summary>
/// A connection from one station to another, split into legs. Between two legs you change
/// trains.
/// </summary>
/// <param name="Legs">The legs in travel order (at least one).</param>
/// <param name="Minutes">Total time: all rides plus all transfers.</param>
public sealed record Route(IReadOnlyList<Leg> Legs, int Minutes)
{
    /// <summary>Station where the route starts.</summary>
    public string From => Legs[0].From;

    /// <summary>Station where the route ends.</summary>
    public string To => Legs[^1].To;

    /// <summary>Number of rides from station to station over all legs. Transfers do not count.</summary>
    public int Stations => Legs.Sum(leg => leg.Stations);

    /// <summary>Number of transfers.</summary>
    public int Transfers => Legs.Count - 1;

    /// <summary>
    /// Builds a route from a path in the graph. Consecutive rides on the same line form one leg.
    /// </summary>
    /// <param name="edges">The edges of the path in travel order.</param>
    /// <returns>The route with its legs and total time.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="edges"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// The path is empty, not connected, starts or ends with a transfer or contains two transfers in a row.
    /// </exception>
    public static Route FromEdges(IReadOnlyList<Edge> edges)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the route as readable text, one line per leg and transfer (format see README).
    /// Lines are separated by <c>\n</c>.
    /// </summary>
    public string Describe()
    {
        throw new NotImplementedException();
    }
}
