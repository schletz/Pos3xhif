using System.Text.RegularExpressions;
namespace Preprocessor;

/// <summary>
/// Bildet die Attributes des Makros ab. Der Zugriff erfolgt
/// über einen numerischen Key, um Attributes nach Position zu erhalten oder
/// über einen string Key, um Attributes nach Namen zu erhalten
/// </summary>
public class Attributes
{
    // Erkennt , als Trennzeichen, ignoriert aber das , wenn es in Anführungszeichen steht
    private static readonly Regex _attributesRegex = new(@",(?=(?:[^\""]*\""[^\""]*\"")*[^\""]*$)", RegexOptions.Compiled);
    // Trennt paramName="value" in paramName und value auf
    private static readonly Regex _namedAttributeRegex = new(@"(?<name>[a-z0-9-_]+)=""?(?<value>[^""]*)", RegexOptions.Compiled);
    public string[] AttributesArray { get; }
    public Dictionary<string, string> NamedAttributes { get; }

    public Attributes(string attributes)
    {
        AttributesArray = _attributesRegex.Split(attributes).Select(RemoveQuotes).ToArray();
        NamedAttributes = AttributesArray.SelectMany(a => _namedAttributeRegex.Matches(a))
            .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value);
    }
    private string RemoveQuotes(string value) => value.StartsWith('"') && value.EndsWith('"') ? value[1..^1] : value;
    public string this[int index] => AttributesArray[index];
    public string this[string name] => NamedAttributes.TryGetValue(name, out var val) ? val : string.Empty;
}
