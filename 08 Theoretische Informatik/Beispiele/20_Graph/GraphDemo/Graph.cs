namespace GraphDemo;

/// <summary>
/// A directed or undirected graph, stored as an adjacency list: every node maps to the list of
/// edges that start at this node.
/// </summary>
/// <typeparam name="TNode">Type of the nodes. Nodes are compared with <c>Equals</c> and <c>GetHashCode</c>.</typeparam>
/// <param name="isDirected"><c>true</c> for a directed graph, <c>false</c> for an undirected graph.</param>
public class Graph<TNode>(bool isDirected) where TNode : notnull
{
    private readonly Dictionary<TNode, List<Edge<TNode>>> _adjacency = [];

    /// <summary><c>true</c> if the edges have a direction.</summary>
    public bool IsDirected => isDirected;

    /// <summary>All nodes in the order in which they were added.</summary>
    public IEnumerable<TNode> Nodes => _adjacency.Keys;

    /// <summary>Number of nodes.</summary>
    public int NodeCount => _adjacency.Count;

    /// <summary>Adds a node without edges. Adding an existing node again has no effect.</summary>
    public void AddNode(TNode node) => _adjacency.TryAdd(node, []);

    /// <summary>
    /// Adds an edge and creates missing nodes. In an undirected graph the edge is stored in both
    /// directions. Duplicate edges are not detected.
    /// </summary>
    /// <param name="from">Start node.</param>
    /// <param name="to">End node.</param>
    /// <param name="weight">Weight of the edge, 1 for unweighted graphs.</param>
    public void AddEdge(TNode from, TNode to, int weight = 1)
    {
        AddNode(from);
        AddNode(to);
        _adjacency[from].Add(new Edge<TNode>(from, to, weight));
        if (!isDirected)
        {
            _adjacency[to].Add(new Edge<TNode>(to, from, weight));
        }
    }

    /// <summary>Returns all edges that start at <paramref name="node"/>.</summary>
    /// <exception cref="KeyNotFoundException">The node is not part of the graph.</exception>
    public IReadOnlyList<Edge<TNode>> EdgesFrom(TNode node) =>
        _adjacency.TryGetValue(node, out List<Edge<TNode>>? edges)
            ? edges
            : throw new KeyNotFoundException($"Unknown node {node}.");

    /// <summary>Returns the nodes that can be reached from <paramref name="node"/> over one edge.</summary>
    /// <exception cref="KeyNotFoundException">The node is not part of the graph.</exception>
    public IEnumerable<TNode> Neighbors(TNode node) => EdgesFrom(node).Select(e => e.To);

    /// <summary>Returns <c>true</c> if there is an edge from <paramref name="from"/> to <paramref name="to"/>.</summary>
    public bool HasEdge(TNode from, TNode to) =>
        _adjacency.TryGetValue(from, out List<Edge<TNode>>? edges) && edges.Any(e => e.To.Equals(to));

    /// <summary>Builds a graph from an edge list.</summary>
    /// <param name="edges">The edges. Every node that occurs in an edge becomes a node of the graph.</param>
    /// <param name="isDirected"><c>true</c> for a directed graph.</param>
    public static Graph<TNode> FromEdgeList(IEnumerable<Edge<TNode>> edges, bool isDirected)
    {
        var graph = new Graph<TNode>(isDirected);
        foreach (Edge<TNode> edge in edges)
        {
            graph.AddEdge(edge.From, edge.To, edge.Weight);
        }
        return graph;
    }
}
