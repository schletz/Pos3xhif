using System.Reflection;

namespace Rechtschreibhilfe.Application;

/// <summary>
/// Provides the German word list from <c>Data/woerter.txt</c>, which is embedded into the assembly.
/// </summary>
public static class WordList
{
    private const string ResourceName = "Rechtschreibhilfe.Application.woerter.txt";

    /// <summary>
    /// Loads the German word list: one lowercase word per line, UTF-8, without duplicates,
    /// sorted with <see cref="StringComparer.Ordinal"/>.
    /// </summary>
    /// <returns>The words in the order of the file.</returns>
    public static IReadOnlyList<string> LoadGerman()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Resource {ResourceName} not found.");
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var words = new List<string>();
        while (reader.ReadLine() is { } line)
        {
            if (line.Length > 0) words.Add(line);
        }
        return words;
    }
}
