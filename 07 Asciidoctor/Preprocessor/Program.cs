using System.Text.RegularExpressions;
namespace Preprocessor;

public class Program
{

    public static void Main(string[] args)
    {
        var preprocessor = AsciidocPreprocessor.FromFile(args[0]);
        preprocessor.AddMacroProcessor("html_image", htmlImageProcessor);
        var newContent = preprocessor.Process();
        Console.WriteLine(newContent);
    }
    static string htmlImageProcessor(string target, Attributes attributes)
    {
        return @$"<img src=""{target}"" width=""{attributes[0]}"" style=""width: {attributes["css_width"]}"">";
    }
}

// *************************************************************************************************
/// <summary>
/// Sucht nach Makros und ruft die entsprechenden Prozessoren auf.
/// </summary>
public class AsciidocPreprocessor
{
    private readonly Regex _macroRegex = new(@"^(?<name>[a-zA-Z0-9-_]+)::(?<target>[^[]+)?\[(?<attributes>[^\]]*)\]",
        RegexOptions.Compiled | RegexOptions.Multiline);
    private readonly Dictionary<string, Func<string, Attributes, string>> _macroProcessors = new();
    private readonly string _content;

    public static AsciidocPreprocessor FromFile(string filename) =>
        new(File.ReadAllText(filename, new System.Text.UTF8Encoding(false)));

    public AsciidocPreprocessor(string content) => _content = content;

    public void AddMacroProcessor(string name, Func<string, Attributes, string> processor) =>
        _macroProcessors[name] = processor;

    public string Process() => _macroRegex.Replace(_content, match =>
    {
        var name = match.Groups["name"].Value;
        var target = match.Groups["target"].Value;
        var attributes = new Attributes(match.Groups["attributes"].Value);
        return _macroProcessors.TryGetValue(name, out var processor) ? processor(target, attributes) : match.Value;
    });
}

/// <summary>
/// Bildet die Attributes des Makros ab. Der Zugriff erfolgt
/// über einen numerischen Key, um Attributes nach Position zu erhalten oder
/// über einen string Key, um Attributes nach Namen zu erhalten
/// </summary>
public class Attributes
{
    private static readonly Regex _namedAttributeRegex = new(@"(?<name>[a-z0-9_-]+)=(?:""(?<value>[^""]+)""|(?<value>[^,\]]+))",
        RegexOptions.Compiled);
    private readonly string[] _attributes;
    private readonly Dictionary<string, string> _namedAttributes;

    public Attributes(string attributes) => (_attributes, _namedAttributes) =
        (attributes.Split(','), _namedAttributeRegex
            .Matches(attributes)
            .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value));

    public string this[int index] => _attributes[index];
    public string this[string name] => _namedAttributes.TryGetValue(name, out var val) ? val : string.Empty;
}
