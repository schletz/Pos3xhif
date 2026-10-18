namespace ExpressionParser;

/// <summary>
/// A token is the smallest meaningful unit of the input, e.g. a number or an operator.
/// </summary>
/// <param name="Kind">What kind of token this is.</param>
/// <param name="Text">The characters of the input that form this token.</param>
/// <param name="Position">Zero-based index of the first character in the input.</param>
public record Token(TokenKind Kind, string Text, int Position)
{
    /// <summary>
    /// Returns a compact form like <c>Number '42' at 0</c> for debugging output.
    /// </summary>
    public override string ToString() => $"{Kind} '{Text}' at {Position}";
}
