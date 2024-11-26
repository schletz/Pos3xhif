using System.Text.RegularExpressions;
namespace Preprocessor;

// *************************************************************************************************
/// <summary>
/// Sucht nach Makros und ruft die entsprechenden Prozessoren auf.
/// </summary>
public class AsciidocPreprocessor
{
    private readonly Regex _variableRegex = new(@"^:(?<name>[^:]+):\s*(?<value>.*)", RegexOptions.Compiled | RegexOptions.Multiline);
    private readonly Regex _macroRegex = new(@"^(?<name>[a-zA-Z0-9-_]+)::(?<target>[^[]+)?\[(?<attributes>[^\]]*)\]",
        RegexOptions.Compiled | RegexOptions.Multiline);
    private readonly Dictionary<string, Func<string, Attributes, Dictionary<string, string>, Task<string>>> _macroProcessors = new();
    private readonly string _content;
    private readonly Dictionary<string, string> _globalVariables;
    public static AsciidocPreprocessor FromFile(string filename)
    {
        if (!File.Exists(filename))
            throw new Exception($"Eingabedatei {filename} wurde nicht gefunden.");
        var content = File.ReadAllText(filename, new System.Text.UTF8Encoding(false));
        return new(content);

    }

    public AsciidocPreprocessor(string content)
    {
        _content = content;
        _globalVariables = _variableRegex.Matches(content)
            .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value.Trim());
    }

    public void AddMacroProcessor(string name, Func<string, Attributes, Dictionary<string, string>, Task<string>> processor) =>
        _macroProcessors[name] = processor;

    public async Task<string> Process()
    {
        var matches = _macroRegex.Matches(_content);
        var result = _content;
        foreach (Match match in matches)
        {
            var name = match.Groups["name"].Value;
            var target = match.Groups["target"].Value;
            var attributes = new Attributes(match.Groups["attributes"].Value);
            if (_macroProcessors.TryGetValue(name, out var processor))
            {
                try
                {
                    var replacement = await processor(target, attributes, _globalVariables);
                    result = result.Replace(match.Value, replacement);
                }
                catch (Exception e)
                {
                    result = result.Replace(match.Value, $"Fehler bei {name}: {e.InnerException?.Message ?? e.Message}");
                }
            }
        }
        return result;
    }
}
