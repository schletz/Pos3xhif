using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Searches routes in the Vienna network (transfer time 4 minutes). For every query in this class
/// there is exactly one best route.
/// </summary>
public class ViennaRouteTests
{
    private readonly TransitNetwork _network;
    private readonly RoutePlanner _planner;

    /// <summary>xUnit creates a new instance for every test, so every test loads the network itself.</summary>
    public ViennaRouteTests()
    {
        _network = ViennaMetro.Load();
        _planner = new RoutePlanner(_network);
    }

    [Theory]
    [InlineData("Stephansplatz", "Reumannplatz", "U1: Stephansplatz -> Reumannplatz (5, 8)", 8)]
    [InlineData("Karlsplatz", "Volkstheater", "U2: Karlsplatz -> Volkstheater (2, 3)", 3)]
    [InlineData("Volkstheater", "Karlsplatz", "U2: Volkstheater -> Karlsplatz (2, 3)", 3)]
    [InlineData("Westbahnhof", "Schwedenplatz", "U3: Westbahnhof -> Stephansplatz (5, 7) | U1: Stephansplatz -> Schwedenplatz (1, 1)", 12)]
    [InlineData("Hauptbahnhof", "Westbahnhof", "U1: Hauptbahnhof -> Karlsplatz (2, 3) | U2: Karlsplatz -> Volkstheater (2, 3) | U3: Volkstheater -> Westbahnhof (3, 5)", 19)]
    [InlineData("Oberlaa", "Ottakring", "U1: Oberlaa -> Karlsplatz (9, 14) | U2: Karlsplatz -> Volkstheater (2, 3) | U3: Volkstheater -> Ottakring (8, 12)", 37)]
    [InlineData("Erdberg", "Rathaus", "U3: Erdberg -> Landstraße (4, 6) | U4: Landstraße -> Schottenring (2, 3) | U2: Schottenring -> Rathaus (2, 3)", 20)]
    [InlineData("Floridsdorf", "Oberlaa", "U6: Floridsdorf -> Spittelau (5, 9) | U4: Spittelau -> Schwedenplatz (4, 6) | U1: Schwedenplatz -> Oberlaa (11, 17)", 40)]
    [InlineData("Seestadt", "Hütteldorf", "U2: Seestadt -> Praterstern (12, 22) | U1: Praterstern -> Karlsplatz (4, 6) | U4: Karlsplatz -> Hütteldorf (11, 17)", 53)]
    [InlineData("Siebenhirten", "Seestadt", "U6: Siebenhirten -> Längenfeldgasse (8, 13) | U4: Längenfeldgasse -> Karlsplatz (4, 6) | U1: Karlsplatz -> Praterstern (4, 6) | U2: Praterstern -> Seestadt (12, 22)", 59)]
    public void FewestStations_ReturnsTheUniqueBestRoute(string from, string to, string expected, int minutes)
    {
        var route = _planner.FewestStations(from, to);

        Assert.NotNull(route);
        Assert.True(RouteFormat.Compact(route) == expected,
            $"{from} -> {to}\nexpected: {expected}\nactual:   {RouteFormat.Compact(route)}");
        Assert.True(route.Minutes == minutes, $"{from} -> {to}: expected {minutes} min, actual {route.Minutes} min.");
    }

    [Theory]
    [InlineData("Stephansplatz", "Reumannplatz", "U1: Stephansplatz -> Reumannplatz (5, 8)", 8)]
    [InlineData("Karlsplatz", "Volkstheater", "U2: Karlsplatz -> Volkstheater (2, 3)", 3)]
    [InlineData("Westbahnhof", "Schwedenplatz", "U3: Westbahnhof -> Stephansplatz (5, 7) | U1: Stephansplatz -> Schwedenplatz (1, 1)", 12)]
    [InlineData("Hauptbahnhof", "Westbahnhof", "U1: Hauptbahnhof -> Stephansplatz (3, 5) | U3: Stephansplatz -> Westbahnhof (5, 7)", 16)]
    [InlineData("Westbahnhof", "Hauptbahnhof", "U3: Westbahnhof -> Stephansplatz (5, 7) | U1: Stephansplatz -> Hauptbahnhof (3, 5)", 16)]
    [InlineData("Oberlaa", "Ottakring", "U1: Oberlaa -> Stephansplatz (10, 16) | U3: Stephansplatz -> Ottakring (10, 14)", 34)]
    [InlineData("Erdberg", "Rathaus", "U3: Erdberg -> Volkstheater (8, 11) | U2: Volkstheater -> Rathaus (1, 2)", 17)]
    [InlineData("Ottakring", "Seestadt", "U3: Ottakring -> Volkstheater (8, 12) | U2: Volkstheater -> Seestadt (17, 30)", 46)]
    [InlineData("Hütteldorf", "Leopoldau", "U4: Hütteldorf -> Karlsplatz (11, 17) | U1: Karlsplatz -> Leopoldau (14, 23)", 44)]
    [InlineData("Simmering", "Floridsdorf", "U3: Simmering -> Landstraße (8, 12) | U4: Landstraße -> Spittelau (5, 8) | U6: Spittelau -> Floridsdorf (5, 9)", 37)]
    [InlineData("Seestadt", "Hütteldorf", "U2: Seestadt -> Schottenring (14, 25) | U4: Schottenring -> Hütteldorf (15, 23)", 52)]
    [InlineData("Siebenhirten", "Seestadt", "U6: Siebenhirten -> Längenfeldgasse (8, 13) | U4: Längenfeldgasse -> Schottenring (8, 12) | U2: Schottenring -> Seestadt (14, 25)", 58)]
    public void Fastest_ReturnsTheUniqueBestRoute(string from, string to, string expected, int minutes)
    {
        var route = _planner.Fastest(from, to);

        Assert.NotNull(route);
        Assert.True(RouteFormat.Compact(route) == expected,
            $"{from} -> {to}\nexpected: {expected}\nactual:   {RouteFormat.Compact(route)}");
        Assert.True(route.Minutes == minutes, $"{from} -> {to}: expected {minutes} min, actual {route.Minutes} min.");
    }

    [Fact]
    public void Describe_FastestFromHauptbahnhofToWestbahnhof()
    {
        var route = _planner.Fastest("Hauptbahnhof", "Westbahnhof");

        Assert.NotNull(route);
        Assert.Equal(
            "Von Hauptbahnhof nach Westbahnhof\n" +
            "U1 bis Stephansplatz (3 Stationen, 5 min)\n" +
            "umsteigen in U3\n" +
            "U3 bis Westbahnhof (5 Stationen, 7 min)\n" +
            "Gesamt: 8 Stationen, 1 Umstieg, 16 min",
            route.Describe());
    }

    /// <summary>
    /// Compares both searches for every pair of stations: the fastest route is never slower and
    /// the route with the fewest stations never has more stations. Both directions take the same time.
    /// </summary>
    [Fact]
    public void AllPairs_SearchesAreConsistent()
    {
        var stations = _network.Stations.Order(StringComparer.Ordinal).ToList();
        foreach (var from in stations)
        {
            foreach (var to in stations.Where(s => s != from))
            {
                var fewest = _planner.FewestStations(from, to);
                var fastest = _planner.Fastest(from, to);
                var back = _planner.Fastest(to, from);

                Assert.True(fewest is not null && fastest is not null && back is not null, $"{from} -> {to} must be reachable.");
                Assert.True((fewest.From, fewest.To, fastest.From, fastest.To) == (from, to, from, to),
                    $"{from} -> {to}: a route must start at {from} and end at {to}.");
                Assert.True(fastest.Minutes <= fewest.Minutes,
                    $"{from} -> {to}: Fastest takes {fastest.Minutes} min, FewestStations only {fewest.Minutes} min.");
                Assert.True(fewest.Stations <= fastest.Stations,
                    $"{from} -> {to}: FewestStations has {fewest.Stations} stations, Fastest only {fastest.Stations}.");
                Assert.True(fastest.Minutes == back.Minutes,
                    $"{from} -> {to} takes {fastest.Minutes} min, the way back {back.Minutes} min.");
            }
        }
    }
}
