namespace Routenplaner.Application;

/// <summary>
/// Thrown when a line of the network file does not follow the format described in the README.
/// </summary>
/// <param name="message">Description of the problem.</param>
/// <param name="lineNumber">1-based number of the faulty line. Comment and empty lines are counted too.</param>
public sealed class CsvFormatException(string message, int lineNumber)
    : FormatException($"Line {lineNumber}: {message}")
{
    /// <summary>1-based number of the faulty line in the input.</summary>
    public int LineNumber { get; } = lineNumber;
}
