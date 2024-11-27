using System.Text.RegularExpressions;
namespace Preprocessor;

/// <summary>
/// Bildet die Attributes des Makros ab. Der Zugriff erfolgt
/// über einen numerischen Key, um Attributes nach Position zu erhalten oder
/// über einen string Key, um Attributes nach Namen zu erhalten.
/// Es kann auch AttributesArray und NamedAttributes verwendet werden.
/// Beispiel: macro::target[attr1,attr2,attr3=value3
///           attributes[0] -> attr1 (auch mit attributes.AttributesArray[0])
///           attributes[1] -> attr2 (auch mit attributes.AttributesArray[1])
///           attributes[2] -> attr3=value3 (auch mit attributes.AttributesArray[2])
///           attributes["attr3"] -> value3 (auch mit attributes.NamedAttributes["attr3"])
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
        AttributesArray = _attributesRegex.Split(attributes)
            .Select(RemoveQuotes).ToArray();
        NamedAttributes = AttributesArray.SelectMany(a => _namedAttributeRegex.Matches(a))
            .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value);
    }
    private string RemoveQuotes(string value) => value.StartsWith('"') && value.EndsWith('"') ? value[1..^1] : value;
    public string this[int index] => index < AttributesArray.Length ? AttributesArray[index] : string.Empty;
    public string this[string name] => NamedAttributes.TryGetValue(name, out var val) ? val : string.Empty;
}
