namespace RegexLogAnalyzer.Application;

/// <summary>
/// Requests and errors within one hour of the log.
/// </summary>
/// <param name="Hour">Start of the hour (minutes and seconds are 0) with the offset from the log.</param>
/// <param name="Requests">Number of valid requests in this hour (at least 1).</param>
/// <param name="Errors">Number of requests with status 400 or higher.</param>
public record HourlyErrorRate(DateTimeOffset Hour, int Requests, int Errors)
{
    /// <summary>Share of errors between 0.0 and 1.0.</summary>
    public double ErrorRate => (double)Errors / Requests;
}
