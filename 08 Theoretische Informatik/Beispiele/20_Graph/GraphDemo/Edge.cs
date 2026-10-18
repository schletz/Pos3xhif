namespace GraphDemo;

/// <summary>
/// An edge from <paramref name="From"/> to <paramref name="To"/>. In an undirected graph the
/// graph stores one edge for each direction.
/// </summary>
/// <typeparam name="TNode">Type of the nodes, e.g. <see cref="string"/> or <see cref="int"/>.</typeparam>
/// <param name="From">Start node of the edge.</param>
/// <param name="To">End node of the edge.</param>
/// <param name="Weight">Weight of the edge (distance, time, cost). Unweighted edges have the weight 1.</param>
public record Edge<TNode>(TNode From, TNode To, int Weight = 1)
{
    /// <summary>Returns the edge as <c>From -> To</c>, followed by the weight if it is not 1.</summary>
    public override string ToString() => Weight == 1 ? $"{From} -> {To}" : $"{From} -> {To} ({Weight})";
}
