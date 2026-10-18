namespace RegexLogAnalyzer.Application;

/// <summary>
/// Result of parsing a whole log: the valid entries and the line numbers of the broken lines.
/// </summary>
/// <param name="Entries">All valid entries in the order of the input.</param>
/// <param name="BrokenLineNumbers">1-based line numbers of all broken lines, ascending.</param>
public record LogParseResult(IReadOnlyList<LogEntry> Entries, IReadOnlyList<int> BrokenLineNumbers)
{
    /// <summary>Number of broken lines.</summary>
    public int BrokenLineCount => BrokenLineNumbers.Count;
}
