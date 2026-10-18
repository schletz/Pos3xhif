namespace RegexLogAnalyzer.Application;

/// <summary>
/// Evaluations over parsed log entries.
/// </summary>
public static class LogAnalyzer
{
    /// <summary>
    /// Returns the most frequently requested paths.
    /// </summary>
    /// <param name="entries">The parsed log entries.</param>
    /// <param name="count">Maximum number of paths to return (at least 1).</param>
    /// <returns>Paths sorted by count descending, ties sorted by path (ordinal) ascending.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 1.</exception>
    public static IReadOnlyList<PathCount> TopPaths(IEnumerable<LogEntry> entries, int count) =>
        throw new NotImplementedException();

    /// <summary>
    /// Computes the error rate (status 400 or higher) for every hour that contains requests.
    /// </summary>
    /// <param name="entries">The parsed log entries.</param>
    /// <returns>One element per hour with requests, sorted by hour ascending.</returns>
    public static IReadOnlyList<HourlyErrorRate> ErrorRatesByHour(IEnumerable<LogEntry> entries) =>
        throw new NotImplementedException();
}
