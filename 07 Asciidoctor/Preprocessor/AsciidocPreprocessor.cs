using System.Text.RegularExpressions;
namespace Preprocessor;

// *************************************************************************************************
/// <summary>
/// Sucht nach Makros und ruft die entsprechenden Prozessoren auf.
/// Der Rückgabewert des Prozessors wird anstelle des Makros im Text eingefügt.
/// Ein Makro hat den folgenden Aufbau und muss am Zeilenanfang stehen:
/// makro_name::target[attr1,attr2,attr3=value,attr4=value2]
/// makro_name::[attr1,attr2,attr3=value,attr4=value2]
/// </summary>
public class AsciidocPreprocessor
{
    private static readonly Regex _variableRegex = new(@"^:(?<name>[^:]+):\s*(?<value>.*)", RegexOptions.Compiled | RegexOptions.Multiline);
    private static readonly Regex _macroRegex = new(@"^(?<name>[a-zA-Z0-9-_]+)::(?<target>[^[]+)?\[(?<attributes>[^\]]*)\]",
        RegexOptions.Compiled | RegexOptions.Multiline);
    private readonly Dictionary<string, Func<string, Attributes, Dictionary<string, string>, string>> _macroProcessors = new();
    private readonly Dictionary<string, Func<string, Attributes, Dictionary<string, string>, Task<string>>> _macroAsyncProcessors = new();
    private string _content;
    private Dictionary<string, string> _globalVariables;

    public static AsciidocPreprocessor FromFile(string filename)
    {
        if (!File.Exists(filename))
            throw new ServiceException($"Eingabedatei {filename} wurde nicht gefunden.");
        var content = File.ReadAllText(filename, new System.Text.UTF8Encoding(false));
        return new(content);
    }

    public AsciidocPreprocessor(string content)
    {
        _content = content;
        _globalVariables = _variableRegex
            .Matches(content)
            .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value.Trim());
    }

    public void AddMacroProcessor(string name, Func<string, Attributes, Dictionary<string, string>, string> processor) =>
        _macroProcessors[name] = processor;

    public void AddAsyncMacroProcessor(string name, Func<string, Attributes, Dictionary<string, string>, Task<string>> processor) =>
        _macroAsyncProcessors[name] = processor;

    /// <summary>
    /// Ruft für jedes gefundene Makro den entsprechenden Prozessor auf.
    /// Der Rückgabewert des Prozessors wird anstelle des Makros im Text eingefügt.
    /// </summary>
    public async Task<string> Process()
    {
        var matches = _macroRegex.Matches(_content);
        var result = _content;
        foreach (Match match in matches)
        {
            var name = match.Groups["name"].Value;
            var target = match.Groups["target"].Value;
            var attributes = new Attributes(match.Groups["attributes"].Value);
            try
            {
                Logger.LogInfo($"Processing macro {name}::{target}");
                if (_macroAsyncProcessors.TryGetValue(name, out var asyncProcessor))
                {
                    var replacement = await asyncProcessor(target, attributes, _globalVariables);
                    result = result.Replace(match.Value, replacement);
                }
                if (_macroProcessors.TryGetValue(name, out var processor))
                {
                    var replacement = processor(target, attributes, _globalVariables);
                    result = result.Replace(match.Value, replacement);
                }
            }
            catch (Exception e)
            {
                result = result.Replace(match.Value, @$"
[.error]
----
Fehler im Makro {name}::{target}

{e.InnerException?.Message ?? e.Message};
Global Variables: {System.Text.Json.JsonSerializer.Serialize(_globalVariables)}
Attributes: {System.Text.Json.JsonSerializer.Serialize(attributes.AttributesArray)}
Named Attributes: {System.Text.Json.JsonSerializer.Serialize(attributes.NamedAttributes)}
----
");
            }
        }
        return result;
    }
}
