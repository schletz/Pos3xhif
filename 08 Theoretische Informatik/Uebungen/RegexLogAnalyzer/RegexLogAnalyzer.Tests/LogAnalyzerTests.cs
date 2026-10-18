using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests for <see cref="LogAnalyzer"/> with hand-made entries.
/// </summary>
public class LogAnalyzerTests
{
    private static readonly TimeSpan Cest = TimeSpan.FromHours(2);

    private static LogEntry Entry(string path, int status = 200, int hour = 8, int minute = 0, int second = 0) =>
        new("10.0.0.1", new DateTimeOffset(2026, 10, 17, hour, minute, second, Cest), "GET", path, status, 100);

    [Fact]
    public void TopPaths_SortsByCountDescending()
    {
        LogEntry[] entries = [Entry("/b"), Entry("/a"), Entry("/b"), Entry("/c"), Entry("/b"), Entry("/c")];

        var top = LogAnalyzer.TopPaths(entries, 3);

        Assert.Equal([new PathCount("/b", 3), new PathCount("/c", 2), new PathCount("/a", 1)], top);
    }

    [Fact]
    public void TopPaths_TieIsSortedByOrdinalPath()
    {
        // Ordinal comparison: upper case letters come before lower case letters.
        LogEntry[] entries = [Entry("/news"), Entry("/apple"), Entry("/Zebra"), Entry("/news")];

        var top = LogAnalyzer.TopPaths(entries, 3);

        Assert.Equal([new PathCount("/news", 2), new PathCount("/Zebra", 1), new PathCount("/apple", 1)], top);
    }

    [Fact]
    public void TopPaths_PathsDifferingInCaseAreDifferentPaths()
    {
        LogEntry[] entries = [Entry("/News"), Entry("/news"), Entry("/news")];

        var top = LogAnalyzer.TopPaths(entries, 5);

        Assert.Equal([new PathCount("/news", 2), new PathCount("/News", 1)], top);
    }

    [Fact]
    public void TopPaths_CountLargerThanNumberOfPaths_ReturnsAllPaths()
    {
        LogEntry[] entries = [Entry("/a"), Entry("/b")];

        Assert.Equal(2, LogAnalyzer.TopPaths(entries, 10).Count);
    }

    [Fact]
    public void TopPaths_NoEntries_ReturnsEmptyList()
    {
        Assert.Empty(LogAnalyzer.TopPaths([], 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TopPaths_CountLessThanOne_Throws(int count)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LogAnalyzer.TopPaths([Entry("/")], count));
    }

    [Fact]
    public void ErrorRatesByHour_Status400AndAboveIsError()
    {
        LogEntry[] entries = [Entry("/", 200), Entry("/", 302), Entry("/", 399), Entry("/", 400), Entry("/", 404), Entry("/", 503)];

        var rate = Assert.Single(LogAnalyzer.ErrorRatesByHour(entries));

        Assert.Equal(6, rate.Requests);
        Assert.Equal(3, rate.Errors);
        Assert.Equal(0.5, rate.ErrorRate, 6);
    }

    [Fact]
    public void ErrorRatesByHour_GroupsByFullHourAndSortsAscending()
    {
        LogEntry[] entries =
        [
            Entry("/", 500, hour: 10, minute: 0, second: 0),
            Entry("/", 200, hour: 8, minute: 59, second: 59),
            Entry("/", 404, hour: 9, minute: 0, second: 0),
            Entry("/", 200, hour: 8, minute: 0, second: 0),
            Entry("/", 200, hour: 9, minute: 30, second: 0),
        ];

        var rates = LogAnalyzer.ErrorRatesByHour(entries);

        Assert.Equal(
            [
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 8, 0, 0, Cest), 2, 0),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 9, 0, 0, Cest), 2, 1),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 10, 0, 0, Cest), 1, 1),
            ],
            rates);
    }

    [Fact]
    public void ErrorRatesByHour_HourWithoutRequestsIsMissing()
    {
        LogEntry[] entries = [Entry("/", hour: 8), Entry("/", hour: 11)];

        var rates = LogAnalyzer.ErrorRatesByHour(entries);

        Assert.Equal([8, 11], rates.Select(r => r.Hour.Hour));
    }

    [Fact]
    public void ErrorRatesByHour_HourKeepsOffsetOfLog()
    {
        var rate = Assert.Single(LogAnalyzer.ErrorRatesByHour([Entry("/", hour: 14, minute: 7)]));

        Assert.Equal(Cest, rate.Hour.Offset);
        Assert.Equal(14, rate.Hour.Hour);
        Assert.Equal(0, rate.Hour.Minute);
        Assert.Equal(0, rate.Hour.Second);
    }

    [Fact]
    public void ErrorRatesByHour_NoEntries_ReturnsEmptyList()
    {
        Assert.Empty(LogAnalyzer.ErrorRatesByHour([]));
    }
}
