namespace Routenplaner.Application;

/// <summary>
/// Reads the network file (format <c>Linie;Von;Nach;Minuten</c>, see README).
/// </summary>
public static class ConnectionReader
{
    /// <summary>The header that must be the first line which is neither empty nor a comment.</summary>
    public const string Header = "Linie;Von;Nach;Minuten";

    /// <summary>
    /// Parses the lines of a network file. Empty lines and lines starting with <c>#</c> are skipped,
    /// the first remaining line must be <see cref="Header"/>.
    /// </summary>
    /// <param name="lines">The lines of the file without line breaks.</param>
    /// <returns>One <see cref="Connection"/> per data line, in the order of the input.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lines"/> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">A line does not follow the format.</exception>
    public static IReadOnlyList<Connection> Parse(IEnumerable<string> lines)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a network file (UTF-8) and parses it like <see cref="Parse"/>.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns>One <see cref="Connection"/> per data line, in the order of the file.</returns>
    /// <exception cref="CsvFormatException">A line does not follow the format.</exception>
    public static IReadOnlyList<Connection> ReadFile(string path)
    {
        throw new NotImplementedException();
    }
}
