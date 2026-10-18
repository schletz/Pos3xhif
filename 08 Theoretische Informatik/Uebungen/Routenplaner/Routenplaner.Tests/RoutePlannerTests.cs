using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Tests <see cref="RoutePlanner"/> with <see cref="MiniNetwork"/>. Every query has exactly one best route.
/// </summary>
public class RoutePlannerTests
{
    private const string AlphaToOmegaWithTransfers = "A: Alpha -> Beta (1, 2) | B: Beta -> Gamma (1, 2) | C: Gamma -> Omega (1, 2)";
    private const string AlphaToOmegaOnLineD = "D: Alpha -> Omega (4, 12)";

    private readonly RoutePlanner _planner = new(MiniNetwork.Create(4));

    [Fact]
    public void FewestStations_TransfersDoNotCount()
    {
        var route = _planner.FewestStations("Alpha", "Omega");

        Assert.NotNull(route);
        Assert.True(RouteFormat.Compact(route) == AlphaToOmegaWithTransfers,
            $"3 stations with 2 transfers are fewer than 4 stations on line D, but the route was {RouteFormat.Compact(route)}.");
        Assert.True(route.Minutes == 14, $"6 minutes of rides plus 2 transfers of 4 minutes are 14 minutes, not {route.Minutes}.");
    }

    [Fact]
    public void Fastest_LongTransfers_StaysOnOneLine()
    {
        var route = _planner.Fastest("Alpha", "Omega");

        Assert.NotNull(route);
        Assert.True(RouteFormat.Compact(route) == AlphaToOmegaOnLineD,
            $"With 4 minutes per transfer, line D (12 min) is faster than 14 min with transfers, but the route was {RouteFormat.Compact(route)}.");
        Assert.Equal(12, route.Minutes);
    }

    [Fact]
    public void Fastest_ShortTransfers_ChangesTrains()
    {
        var route = new RoutePlanner(MiniNetwork.Create(2)).Fastest("Alpha", "Omega");

        Assert.NotNull(route);
        Assert.True(RouteFormat.Compact(route) == AlphaToOmegaWithTransfers,
            $"With 2 minutes per transfer, the route with transfers (10 min) is faster than line D (12 min), but the route was {RouteFormat.Compact(route)}.");
        Assert.Equal(10, route.Minutes);
    }

    [Fact]
    public void FewestStations_DoesNotDependOnTransferMinutes()
    {
        var route = new RoutePlanner(MiniNetwork.Create(20)).FewestStations("Alpha", "Omega");

        Assert.NotNull(route);
        Assert.Equal(AlphaToOmegaWithTransfers, RouteFormat.Compact(route));
        Assert.Equal(46, route.Minutes);
    }

    [Fact]
    public void BothSearches_StartStationWithTwoLines_NoTransferAtTheStart()
    {
        // Beta has the lines A and B. The route starts directly on line B.
        const string expected = "B: Beta -> Gamma (1, 2) | C: Gamma -> Omega (1, 2)";

        foreach (var route in new[] { _planner.FewestStations("Beta", "Omega"), _planner.Fastest("Beta", "Omega") })
        {
            Assert.NotNull(route);
            Assert.Equal(expected, RouteFormat.Compact(route));
            Assert.True(route.Minutes == 8, $"2 + 4 + 2 minutes, a transfer at the start does not count, but the route took {route.Minutes} minutes.");
        }
    }

    [Fact]
    public void BothSearches_DestinationWithTwoLines_NoTransferAtTheEnd()
    {
        // Beta has the lines A and B. The route ends on line B.
        const string expected = "C: Omega -> Gamma (1, 2) | B: Gamma -> Beta (1, 2)";

        foreach (var route in new[] { _planner.FewestStations("Omega", "Beta"), _planner.Fastest("Omega", "Beta") })
        {
            Assert.NotNull(route);
            Assert.Equal(expected, RouteFormat.Compact(route));
            Assert.True(route.Minutes == 8, $"2 + 4 + 2 minutes, a transfer at the end does not count, but the route took {route.Minutes} minutes.");
        }
    }

    [Fact]
    public void BothSearches_NeighbouringStations_ReturnOneLegWithOneStation()
    {
        Assert.Equal("D: Delta -> Epsilon (1, 3)", RouteFormat.Compact(_planner.FewestStations("Delta", "Epsilon")!));
        Assert.Equal("D: Epsilon -> Delta (1, 3)", RouteFormat.Compact(_planner.Fastest("Epsilon", "Delta")!));
    }

    [Fact]
    public void BothSearches_UnreachableDestination_ReturnNull()
    {
        Assert.Null(_planner.FewestStations("Alpha", "Strand"));
        Assert.Null(_planner.Fastest("Insel", "Omega"));
    }

    [Theory]
    [InlineData("Nirgendwo", "Omega", "from")]
    [InlineData("Alpha", "Nirgendwo", "to")]
    [InlineData("Alpha", "Alpha", "to")]
    public void BothSearches_InvalidStations_ThrowArgumentException(string from, string to, string paramName)
    {
        var ex1 = Assert.Throws<ArgumentException>(() => _planner.FewestStations(from, to));
        var ex2 = Assert.Throws<ArgumentException>(() => _planner.Fastest(from, to));
        Assert.Equal(paramName, ex1.ParamName);
        Assert.Equal(paramName, ex2.ParamName);
    }

    [Fact]
    public void BothSearches_Null_ThrowArgumentNullException()
    {
        Assert.Equal("from", Assert.Throws<ArgumentNullException>(() => _planner.FewestStations(null!, "Omega")).ParamName);
        Assert.Equal("to", Assert.Throws<ArgumentNullException>(() => _planner.Fastest("Alpha", null!)).ParamName);
    }

    [Fact]
    public void Constructor_Null_ThrowsArgumentNullException()
    {
        Assert.Equal("network", Assert.Throws<ArgumentNullException>(() => new RoutePlanner(null!)).ParamName);
    }
}
