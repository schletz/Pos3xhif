namespace RegexLogAnalyzer.Application;

/// <summary>
/// Number of requests for one path.
/// </summary>
/// <param name="Path">Requested path without the query string.</param>
/// <param name="Count">Number of requests for this path.</param>
public record PathCount(string Path, int Count);
