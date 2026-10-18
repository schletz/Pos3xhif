using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests with the sample log <c>Data/access.log</c> (200 lines, 13 broken, 1 blank).
/// </summary>
public class LogFileTests
{
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, "Data", "access.log");

    private static LogParseResult Parse()
    {
        Assert.True(File.Exists(LogPath), $"Sample log not found: {LogPath}");
        return LogParser.ParseFile(LogPath);
    }

    [Fact]
    public void ParseFile_SampleLog_Returns186Entries()
    {
        Assert.Equal(186, Parse().Entries.Count);
    }

    [Fact]
    public void ParseFile_SampleLog_ReportsBrokenLineNumbers()
    {
        var result = Parse();

        Assert.Equal([9, 23, 38, 52, 67, 81, 96, 110, 124, 139, 153, 167, 181], result.BrokenLineNumbers);
        Assert.Equal(13, result.BrokenLineCount);
    }

    [Fact]
    public void ParseFile_SampleLog_SumOfBytesTreatsDashAsZero()
    {
        Assert.Equal(1_271_588, Parse().Entries.Sum(e => e.Bytes));
    }

    [Fact]
    public void ParseFile_SampleLog_FirstEntryIsCorrect()
    {
        var first = Parse().Entries[0];

        Assert.Equal(
            new LogEntry("192.168.1.5", new DateTimeOffset(2026, 10, 17, 8, 2, 4, TimeSpan.FromHours(2)),
                "GET", "/search", 200, 2017),
            first);
    }

    [Fact]
    public void TopPaths_SampleLog_ReturnsTopFive()
    {
        var top = LogAnalyzer.TopPaths(Parse().Entries, 5);

        Assert.Equal(
            [
                new PathCount("/", 34),
                new PathCount("/stundenplan", 22),
                new PathCount("/news", 21),
                new PathCount("/css/site.css", 19),
                new PathCount("/js/app.js", 13),
            ],
            top);
    }

    [Fact]
    public void TopPaths_SampleLog_TieOnPlaceTenIsSortedByPath()
    {
        var top = LogAnalyzer.TopPaths(Parse().Entries, 11);

        Assert.Equal(new PathCount("/search", 12), top[5]);
        Assert.Equal(new PathCount("/login", 8), top[9]);
        Assert.Equal(new PathCount("/news/2026/herbstfest", 8), top[10]);
    }

    [Fact]
    public void ErrorRatesByHour_SampleLog_ReturnsSixHours()
    {
        var rates = LogAnalyzer.ErrorRatesByHour(Parse().Entries);
        var offset = TimeSpan.FromHours(2);

        Assert.Equal(
            [
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 8, 0, 0, offset), 28, 0),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 9, 0, 0, offset), 34, 5),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 10, 0, 0, offset), 39, 4),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 11, 0, 0, offset), 32, 9),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 12, 0, 0, offset), 28, 5),
                new HourlyErrorRate(new DateTimeOffset(2026, 10, 17, 13, 0, 0, offset), 25, 2),
            ],
            rates);
        Assert.Equal(0.28125, rates[3].ErrorRate, 6);
    }
}
