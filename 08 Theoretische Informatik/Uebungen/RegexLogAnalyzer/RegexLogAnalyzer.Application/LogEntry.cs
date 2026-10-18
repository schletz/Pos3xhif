namespace RegexLogAnalyzer.Application;

/// <summary>
/// One successfully parsed request from an Apache access log.
/// </summary>
/// <param name="IpAddress">IPv4 address of the client, e.g. <c>203.0.113.5</c>.</param>
/// <param name="Timestamp">Time of the request including the UTC offset written in the log.</param>
/// <param name="Method">HTTP method, e.g. <c>GET</c>.</param>
/// <param name="Path">Requested path without the query string, e.g. <c>/search</c>.</param>
/// <param name="Status">HTTP status code, e.g. <c>404</c>.</param>
/// <param name="Bytes">Size of the response body in bytes; <c>-</c> in the log is stored as 0.</param>
public record LogEntry(
    string IpAddress,
    DateTimeOffset Timestamp,
    string Method,
    string Path,
    int Status,
    long Bytes);
