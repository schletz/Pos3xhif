using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Tests <see cref="Route.FromEdges"/> and <see cref="Route.Describe"/> with hand-made paths.
/// </summary>
public class RouteTests
{
    private static Edge Ride(string from, string to, string line, int minutes) =>
        new(new Stop(from, line), new Stop(to, line), minutes);

    private static Edge Transfer(string station, string fromLine, string toLine, int minutes) =>
        new(new Stop(station, fromLine), new Stop(station, toLine), minutes);

    [Fact]
    public void FromEdges_SingleRide_ReturnsOneLeg()
    {
        var route = Route.FromEdges([Ride("Alpha", "Beta", "A", 2)]);

        Assert.Equal([new Leg("A", "Alpha", "Beta", 1, 2)], route.Legs);
        Assert.Equal(2, route.Minutes);
        Assert.Equal(("Alpha", "Beta", 1, 0), (route.From, route.To, route.Stations, route.Transfers));
    }

    [Fact]
    public void FromEdges_RidesOnTheSameLine_FormOneLeg()
    {
        var route = Route.FromEdges([Ride("Alpha", "Delta", "D", 3), Ride("Delta", "Epsilon", "D", 3), Ride("Epsilon", "Zeta", "D", 2)]);

        Assert.Equal([new Leg("D", "Alpha", "Zeta", 3, 8)], route.Legs);
        Assert.Equal(8, route.Minutes);
    }

    [Fact]
    public void FromEdges_Transfers_SplitLegsAndCountInMinutes()
    {
        var route = Route.FromEdges(
        [
            Ride("Alpha", "Beta", "A", 2),
            Transfer("Beta", "A", "B", 4),
            Ride("Beta", "Gamma", "B", 2),
            Transfer("Gamma", "B", "C", 4),
            Ride("Gamma", "Delta", "C", 1),
            Ride("Delta", "Omega", "C", 2)
        ]);

        Assert.Equal(
            [new Leg("A", "Alpha", "Beta", 1, 2), new Leg("B", "Beta", "Gamma", 1, 2), new Leg("C", "Gamma", "Omega", 2, 3)],
            route.Legs);
        Assert.True(route.Minutes == 15, $"7 minutes of rides plus 2 transfers of 4 minutes are 15 minutes, not {route.Minutes}.");
        Assert.Equal((4, 2), (route.Stations, route.Transfers));
    }

    [Fact]
    public void FromEdges_InvalidPath_ThrowsArgumentException()
    {
        var ride = Ride("Alpha", "Beta", "A", 2);
        var transfer = Transfer("Beta", "A", "B", 4);

        Assert.Throws<ArgumentException>(() => Route.FromEdges([]));
        Assert.Throws<ArgumentException>(() => Route.FromEdges([Transfer("Alpha", "D", "A", 4), ride]));
        Assert.Throws<ArgumentException>(() => Route.FromEdges([ride, transfer]));
        Assert.Throws<ArgumentException>(() => Route.FromEdges(
            [ride, transfer, Transfer("Beta", "B", "C", 4), Ride("Beta", "Gamma", "C", 2)]));
        // Not connected: the second edge does not start where the first one ends.
        Assert.Throws<ArgumentException>(() => Route.FromEdges([ride, Ride("Gamma", "Omega", "A", 2)]));
        Assert.Throws<ArgumentException>(() => Route.FromEdges([ride, Ride("Beta", "Gamma", "B", 2)]));
    }

    [Fact]
    public void FromEdges_Null_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Route.FromEdges(null!));
        Assert.Equal("edges", ex.ParamName);
    }

    [Fact]
    public void Describe_RouteWithoutTransfer_UsesSingularForms()
    {
        var route = new Route([new Leg("U1", "Karlsplatz", "Stephansplatz", 1, 2)], 2);

        Assert.Equal(
            "Von Karlsplatz nach Stephansplatz\n" +
            "U1 bis Stephansplatz (1 Station, 2 min)\n" +
            "Gesamt: 1 Station, 0 Umstiege, 2 min",
            route.Describe());
    }

    [Fact]
    public void Describe_RouteWithOneTransfer()
    {
        var route = new Route(
            [new Leg("U1", "Hauptbahnhof", "Stephansplatz", 3, 5), new Leg("U3", "Stephansplatz", "Westbahnhof", 5, 7)], 16);

        Assert.Equal(
            "Von Hauptbahnhof nach Westbahnhof\n" +
            "U1 bis Stephansplatz (3 Stationen, 5 min)\n" +
            "umsteigen in U3\n" +
            "U3 bis Westbahnhof (5 Stationen, 7 min)\n" +
            "Gesamt: 8 Stationen, 1 Umstieg, 16 min",
            route.Describe());
    }

    [Fact]
    public void Describe_RouteWithTwoTransfers()
    {
        var route = new Route(
        [
            new Leg("U1", "Hauptbahnhof", "Karlsplatz", 2, 3),
            new Leg("U2", "Karlsplatz", "Volkstheater", 2, 3),
            new Leg("U3", "Volkstheater", "Westbahnhof", 3, 5)
        ], 19);

        Assert.Equal(
            "Von Hauptbahnhof nach Westbahnhof\n" +
            "U1 bis Karlsplatz (2 Stationen, 3 min)\n" +
            "umsteigen in U2\n" +
            "U2 bis Volkstheater (2 Stationen, 3 min)\n" +
            "umsteigen in U3\n" +
            "U3 bis Westbahnhof (3 Stationen, 5 min)\n" +
            "Gesamt: 7 Stationen, 2 Umstiege, 19 min",
            route.Describe());
    }
}
