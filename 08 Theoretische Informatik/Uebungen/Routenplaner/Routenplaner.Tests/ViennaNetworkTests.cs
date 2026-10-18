using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Reads <c>Data/ubahn_wien.csv</c> and checks the graph of the Vienna network.
/// </summary>
public class ViennaNetworkTests
{
    [Fact]
    public void ReadFile_ReadsAllConnections()
    {
        var connections = ConnectionReader.ReadFile(ViennaMetro.DataFile);

        Assert.True(connections.Count == 104, $"The file has 104 sections, but {connections.Count} were read.");
        Assert.Equal(new Connection("U1", "Oberlaa", "Neulaa", 2), connections[0]);
        Assert.Equal(new Connection("U2", "Karlsplatz", "Museumsquartier", 2), connections[23]);
        Assert.Equal(new Connection("U6", "Neue Donau", "Floridsdorf", 2), connections[^1]);
    }

    [Fact]
    public void Load_CreatesAllStopsAndStations()
    {
        var network = ViennaMetro.Load();

        Assert.True(network.Stops.Count == 109, $"U1 to U6 have 109 stops (station and line), not {network.Stops.Count}.");
        Assert.True(network.Stations.Count == 98, $"U1 to U6 have 98 stations, not {network.Stations.Count}.");
        Assert.Equal(["U1", "U2", "U3", "U4", "U6"], network.Stops.Select(s => s.Line).Distinct().Order(StringComparer.Ordinal));
        Assert.Equal(ViennaMetro.TransferMinutes, network.TransferMinutes);
    }

    [Fact]
    public void Load_FindsAllTransferStations()
    {
        var network = ViennaMetro.Load();

        var transferStations = network.Stations
            .Where(station => network.StopsAt(station).Count > 1)
            .Order(StringComparer.Ordinal);

        Assert.Equal(
            ["Karlsplatz", "Landstraße", "Längenfeldgasse", "Praterstern", "Schottenring",
             "Schwedenplatz", "Spittelau", "Stephansplatz", "Volkstheater", "Westbahnhof"],
            transferStations);
        Assert.Equal(["U1", "U2", "U4"], network.StopsAt("Karlsplatz").Select(s => s.Line));
    }

    [Fact]
    public void EdgesFrom_KarlsplatzU1_HasTwoRidesAndTwoTransfers()
    {
        var network = ViennaMetro.Load();
        var karlsplatz = new Stop("Karlsplatz", "U1");

        string[] expected =
        [
            "Karlsplatz (U1) -> Karlsplatz (U2), 4 min",
            "Karlsplatz (U1) -> Karlsplatz (U4), 4 min",
            "Karlsplatz (U1) -> Stephansplatz (U1), 2 min",
            "Karlsplatz (U1) -> Taubstummengasse (U1), 1 min"
        ];
        Assert.Equal(expected, network.EdgesFrom(karlsplatz).Select(e => e.ToString()).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void EdgesFrom_EndOfLine_HasOneRide()
    {
        var network = ViennaMetro.Load();
        var oberlaa = new Stop("Oberlaa", "U1");

        Assert.Equal([new Edge(oberlaa, new Stop("Neulaa", "U1"), 2)], network.EdgesFrom(oberlaa));
    }
}
