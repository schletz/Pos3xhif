using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests for <see cref="LogParser.ParseLine"/> and <see cref="LogParser.ParseLines"/> with hand-written lines.
/// </summary>
public class LogParserTests
{
    private const string ValidLine =
        "203.0.113.5 - anna [17/Oct/2026:08:15:02 +0200] \"GET /search?q=regex&page=2 HTTP/1.1\" 200 2326 " +
        "\"https://www.example.at/\" \"Mozilla/5.0 (Windows NT 10.0; Win64; x64)\"";

    [Fact]
    public void ParseLine_ValidLine_ReturnsAllFields()
    {
        var entry = LogParser.ParseLine(ValidLine);

        Assert.NotNull(entry);
        Assert.Equal("203.0.113.5", entry.IpAddress);
        Assert.Equal(new DateTimeOffset(2026, 10, 17, 8, 15, 2, TimeSpan.FromHours(2)), entry.Timestamp);
        Assert.Equal(TimeSpan.FromHours(2), entry.Timestamp.Offset);
        Assert.Equal("GET", entry.Method);
        Assert.Equal("/search", entry.Path);
        Assert.Equal(200, entry.Status);
        Assert.Equal(2326, entry.Bytes);
    }

    [Fact]
    public void ParseLine_DashAsBytes_ReturnsZeroBytes()
    {
        var entry = LogParser.ParseLine(
            "10.0.0.1 - - [17/Oct/2026:09:00:00 +0200] \"GET /css/site.css HTTP/1.1\" 304 - \"-\" \"curl/8.15.0\"");

        Assert.NotNull(entry);
        Assert.Equal(304, entry.Status);
        Assert.Equal(0, entry.Bytes);
    }

    [Fact]
    public void ParseLine_NegativeOffset_KeepsOffset()
    {
        var entry = LogParser.ParseLine(
            "10.0.0.1 - - [10/Oct/2000:13:55:36 -0700] \"GET /apache_pb.gif HTTP/1.0\" 200 2326 \"-\" \"-\"");

        Assert.NotNull(entry);
        Assert.Equal(new DateTimeOffset(2000, 10, 10, 13, 55, 36, TimeSpan.FromHours(-7)), entry.Timestamp);
        Assert.Equal(TimeSpan.FromHours(-7), entry.Timestamp.Offset);
    }

    [Theory]
    [InlineData("0.0.0.0 - - [17/Oct/2026:10:00:00 +0200] \"GET / HTTP/1.1\" 200 10 \"-\" \"-\"", "0.0.0.0", "/")]
    [InlineData("255.255.255.255 - - [17/Oct/2026:10:00:00 +0200] \"HEAD / HTTP/1.1\" 200 - \"-\" \"-\"", "255.255.255.255", "/")]
    [InlineData("192.0.2.250 frank c.huber [17/Oct/2026:10:00:00 +0200] \"DELETE /api/rooms/7 HTTP/2.0\" 204 - \"\" \"\"", "192.0.2.250", "/api/rooms/7")]
    [InlineData("198.51.100.199 - - [01/Jan/2026:00:00:00 +0000] \"POST /login? HTTP/1.0\" 302 0 \"-\" \"x [y] (z; w)\"", "198.51.100.199", "/login")]
    [InlineData("100.64.12.8 - - [31/Dec/2026:23:59:59 +0100] \"GET /news/2026/herbstfest HTTP/1.1\" 599 123456789 \"-\" \"-\"", "100.64.12.8", "/news/2026/herbstfest")]
    [InlineData("10.10.10.10 - - [17/Oct/2026:10:00:00 +0200] \"OPTIONS /a-b_c.d~e HTTP/1.1\" 100 0 \"-\" \"-\"", "10.10.10.10", "/a-b_c.d~e")]
    public void ParseLine_ValidVariants_ReturnsEntry(string line, string expectedIp, string expectedPath)
    {
        var entry = LogParser.ParseLine(line);

        Assert.True(entry is not null, $"Line should be valid: {line}");
        Assert.Equal(expectedIp, entry.IpAddress);
        Assert.Equal(expectedPath, entry.Path);
    }

    [Theory]
    [InlineData("", "empty line")]
    [InlineData("203.0.113.5 - anna [17/Oct/2026:08:15:02 +0200] \"GET /sea", "truncated line")]
    [InlineData("256.10.3.7 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "octet greater than 255")]
    [InlineData("10.0.12 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "only three octets")]
    [InlineData("10.0.0.1.2 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "five octets")]
    [InlineData("192.168.01.5 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "octet with leading zero")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 2OO 1 \"-\" \"-\"", "letter O in status")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" - 1 \"-\" \"-\"", "status missing")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 600 1 \"-\" \"-\"", "status 600")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 20 1 \"-\" \"-\"", "status with two digits")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1.5 \"-\" \"-\"", "bytes not an integer")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] GET / HTTP/1.1 200 1 \"-\" \"-\"", "request without quotes")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"curl/8.15.0", "user agent without closing quote")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1", "common log format without referer and user agent")]
    [InlineData("10.0.0.1 - - [17/Okt/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "German month name")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:24:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "hour 24")]
    [InlineData("10.0.0.1 - - [31/Sep/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "31 September")]
    [InlineData("10.0.0.1 - - [7/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "day with one digit")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "offset missing")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"get / HTTP/1.1\" 200 1 \"-\" \"-\"", "lower case method")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET index.html HTTP/1.1\" 200 1 \"-\" \"-\"", "path without leading slash")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / FTP/1.1\" 200 1 \"-\" \"-\"", "protocol is not HTTP")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\" extra", "text after the user agent")]
    [InlineData(" 10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"", "leading space")]
    [InlineData("10.0.0.1 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"\n", "trailing line break")]
    public void ParseLine_BrokenLine_ReturnsNull(string line, string reason)
    {
        Assert.True(LogParser.ParseLine(line) is null, $"Line should be broken ({reason}): {line}");
    }

    [Fact]
    public void ParseLines_SkipsBlankLinesAndReportsBrokenLineNumbers()
    {
        string[] lines =
        [
            ValidLine,
            "",
            "garbage",
            "   ",
            ValidLine,
            "10.0.12 - - [17/Oct/2026:08:15:02 +0200] \"GET / HTTP/1.1\" 200 1 \"-\" \"-\"",
        ];

        var result = LogParser.ParseLines(lines);

        Assert.Equal(2, result.Entries.Count);
        Assert.Equal([3, 6], result.BrokenLineNumbers);
        Assert.Equal(2, result.BrokenLineCount);
    }

    [Fact]
    public void ParseLines_EmptyInput_ReturnsEmptyResult()
    {
        var result = LogParser.ParseLines([]);

        Assert.Empty(result.Entries);
        Assert.Empty(result.BrokenLineNumbers);
    }
}
