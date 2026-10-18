using JsonParser.Application.Values;

namespace JsonParser.Application;

/// <summary>
/// Recursive descent parser for the JSON subset. Builds a <see cref="JsonValue"/> tree from a list of tokens.
/// </summary>
/// <param name="tokens">Tokens from <see cref="Tokenizer.Tokenize"/>. The last token must be <see cref="TokenType.End"/>.</param>
public sealed class Parser(IReadOnlyList<Token> tokens)
{
    private readonly IReadOnlyList<Token> _tokens = tokens;

    /// <summary>
    /// Tokenizes and parses a JSON text.
    /// </summary>
    /// <param name="json">The JSON text.</param>
    /// <returns>The value of the document.</returns>
    /// <exception cref="JsonParseException">The text is not valid according to the subset.</exception>
    public static JsonValue Parse(string json)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parses exactly one value followed by the end of the input.
    /// </summary>
    /// <returns>The value of the document.</returns>
    /// <exception cref="JsonParseException">The tokens do not form a valid document.</exception>
    public JsonValue ParseDocument()
    {
        throw new NotImplementedException();
    }
}
