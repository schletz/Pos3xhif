namespace JsonParser.Application;

/// <summary>
/// Splits a JSON text into tokens (lexical analysis). Whitespace between tokens is skipped.
/// </summary>
public static class Tokenizer
{
    /// <summary>
    /// Reads the whole input and returns its tokens. The last token is always <see cref="TokenType.End"/>
    /// at position <c>input.Length</c>.
    /// </summary>
    /// <param name="input">The JSON text.</param>
    /// <returns>All tokens in the order of the input.</returns>
    /// <exception cref="JsonParseException">The input contains a character sequence that is not a valid token.</exception>
    public static IReadOnlyList<Token> Tokenize(string input)
    {
        throw new NotImplementedException();
    }
}
