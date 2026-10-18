using GraphDemo;

// The undirected example graph: one edge per line, optionally followed by a weight.
string exampleEdges = """
    A B
    A C
    B C
    B D
    C E
    D E
    """;
Graph<string> graph = Graph<string>.FromEdgeList(ParseEdges(exampleEdges, name => name), isDirected: false);

Console.WriteLine($"Ungerichteter Graph mit {graph.NodeCount} Knoten:");
foreach (string node in graph.Nodes)
{
    Console.WriteLine($"  {node}: {string.Join(", ", graph.Neighbors(node))}");
}
Console.WriteLine($"  Kante B-D? {graph.HasEdge("B", "D")}   Kante A-E? {graph.HasEdge("A", "E")}");
Console.WriteLine();

// A directed, weighted graph with int nodes: crossroads and one-way streets (length in metres).
string streetEdges = """
    1 2 120
    2 3 90
    3 1 150
    2 4 60
    4 3 70
    """;
Graph<int> streets = Graph<int>.FromEdgeList(ParseEdges(streetEdges, int.Parse), isDirected: true);

Console.WriteLine($"Gerichteter, gewichteter Graph mit {streets.NodeCount} Knoten:");
foreach (int node in streets.Nodes)
{
    Console.WriteLine($"  {node}: {string.Join(", ", streets.EdgesFrom(node))}");
}
Console.WriteLine($"  Kante 1->2? {streets.HasEdge(1, 2)}   Kante 2->1? {streets.HasEdge(2, 1)}");

// Reads an edge list with lines "From To" or "From To Weight". parseNode converts the node names.
static IEnumerable<Edge<TNode>> ParseEdges<TNode>(string text, Func<string, TNode> parseNode)
{
    foreach (string line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int weight = parts.Length > 2 ? int.Parse(parts[2]) : 1;
        yield return new Edge<TNode>(parseNode(parts[0]), parseNode(parts[1]), weight);
    }
}
