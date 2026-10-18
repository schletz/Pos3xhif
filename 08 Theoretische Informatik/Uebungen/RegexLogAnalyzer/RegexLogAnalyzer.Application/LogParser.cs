namespace RegexLogAnalyzer.Application;

/// <summary>
/// Parses lines of an Apache access log in Combined Log Format.
/// </summary>
public static partial class LogParser
{
    /// <summary>
    /// Parses one log line.
    /// </summary>
    /// <param name="line">A single line without line break.</param>
    /// <returns>The parsed entry, or <c>null</c> if the line is broken.</returns>
    public static LogEntry? ParseLine(string line) => throw new NotImplementedException();

    /// <summary>
    /// Parses all lines. Blank lines are skipped, broken lines are reported by their line number.
    /// </summary>
    /// <param name="lines">The lines of the log in their original order.</param>
    /// <returns>The valid entries and the 1-based numbers of the broken lines.</returns>
    public static LogParseResult ParseLines(IEnumerable<string> lines) => throw new NotImplementedException();

    /// <summary>
    /// Reads and parses a log file.
    /// </summary>
    /// <param name="path">Path of the log file (UTF-8).</param>
    /// <returns>The valid entries and the 1-based numbers of the broken lines.</returns>
    public static LogParseResult ParseFile(string path) => throw new NotImplementedException();
}
