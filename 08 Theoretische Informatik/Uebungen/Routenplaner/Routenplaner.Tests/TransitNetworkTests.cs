using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Tests the graph construction of <see cref="TransitNetwork"/> with <see cref="MiniNetwork"/>.
/// Collections that may come in any order are sorted before comparing.
/// </summary>
public class TransitNetworkTests
{
    private static List<string> Sorted<T>(IEnumerable<T> items) =>
        items.Select(item => item!.ToString()!).Order(StringComparer.Ordinal).ToList();

    [Fact]
    public void Stops_ContainsOneStopPerStationAndLine()
    {
        var network = MiniNetwork.Create();

        Stop[] expected =
        [
            new("Alpha", "A"), new("Alpha", "D"), new("Beta", "A"), new("Beta", "B"), new("Gamma", "B"),
            new("Gamma", "C"), new("Omega", "C"), new("Omega", "D"), new("Delta", "D"), new("Epsilon", "D"),
            new("Zeta", "D"), new("Insel", "E"), new("Strand", "E")
        ];
        Assert.Equal(Sorted(expected), Sorted(network.Stops));
    }

    [Fact]
    public void Stations_ContainsEveryStationOnce()
    {
        var network = MiniNetwork.Create();

        string[] expected = ["Alpha", "Beta", "Gamma", "Omega", "Delta", "Epsilon", "Zeta", "Insel", "Strand"];
        Assert.Equal(Sorted(expected), Sorted(network.Stations));
    }

    [Fact]
    public void TransferMinutes_IsTheValueFromTheConstructor()
    {
        Assert.Equal(7, MiniNetwork.Create(7).TransferMinutes);
    }

    [Fact]
    public void StopsAt_ReturnsAllLinesOfTheStationSortedByLine()
    {
        var network = MiniNetwork.Create();

        Assert.Equal([new Stop("Alpha", "A"), new Stop("Alpha", "D")], network.StopsAt("Alpha"));
        Assert.Equal([new Stop("Delta", "D")], network.StopsAt("Delta"));
    }

    [Fact]
    public void StopsAt_UnknownStation_ReturnsEmptyList()
    {
        Assert.Empty(MiniNetwork.Create().StopsAt("Nirgendwo"));
    }

    [Fact]
    public void EdgesFrom_StopInTheMiddleOfALine_HasRidesInBothDirections()
    {
        var network = MiniNetwork.Create();
        var delta = new Stop("Delta", "D");

        Edge[] expected = [new(delta, new Stop("Alpha", "D"), 3), new(delta, new Stop("Epsilon", "D"), 3)];
        Assert.Equal(Sorted(expected), Sorted(network.EdgesFrom(delta)));
    }

    [Fact]
    public void EdgesFrom_StopAtTheEndOfALine_HasOneRide()
    {
        var network = MiniNetwork.Create();
        var insel = new Stop("Insel", "E");

        Assert.Equal([new Edge(insel, new Stop("Strand", "E"), 1)], network.EdgesFrom(insel));
    }

    [Fact]
    public void EdgesFrom_StationWithTwoLines_HasTransferWithTransferMinutes()
    {
        var network = MiniNetwork.Create(5);
        var alpha = new Stop("Alpha", "A");

        Edge[] expected = [new(alpha, new Stop("Beta", "A"), 2), new(alpha, new Stop("Alpha", "D"), 5)];
        Assert.Equal(Sorted(expected), Sorted(network.EdgesFrom(alpha)));
    }

    [Fact]
    public void EdgesFrom_StationWithThreeLines_HasTransfersToBothOtherLines()
    {
        var network = new TransitNetwork(
            [new("X", "Kreuz", "West", 1), new("Y", "Kreuz", "Nord", 2), new("Z", "Kreuz", "Ost", 3)], 4);
        var kreuzY = new Stop("Kreuz", "Y");

        Edge[] expected =
        [
            new(kreuzY, new Stop("Nord", "Y"), 2), new(kreuzY, new Stop("Kreuz", "X"), 4), new(kreuzY, new Stop("Kreuz", "Z"), 4)
        ];
        Assert.Equal(Sorted(expected), Sorted(network.EdgesFrom(kreuzY)));
    }

    [Fact]
    public void EdgesFrom_UnknownStop_ThrowsArgumentException()
    {
        var network = MiniNetwork.Create();

        Assert.Throws<ArgumentException>(() => network.EdgesFrom(new Stop("Nirgendwo", "A")));
        // The station exists, but line B does not stop there.
        Assert.Throws<ArgumentException>(() => network.EdgesFrom(new Stop("Alpha", "B")));
    }

    [Fact]
    public void Constructor_InvalidArguments_Throw()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new TransitNetwork(null!, 4));
        Assert.Equal("connections", ex.ParamName);

        var range = Assert.Throws<ArgumentOutOfRangeException>(() => new TransitNetwork(MiniNetwork.Connections, 0));
        Assert.Equal("transferMinutes", range.ParamName);
    }

    [Fact]
    public void Constructor_CopiesTheConnections()
    {
        var connections = new List<Connection>(MiniNetwork.Connections);
        var network = new TransitNetwork(connections, 4);

        connections.Add(new Connection("F", "Omega", "Hafen", 2));

        Assert.True(network.Stops.Count == 13, "Changing the list after the constructor must not change the network.");
        Assert.Empty(network.StopsAt("Hafen"));
    }
}
