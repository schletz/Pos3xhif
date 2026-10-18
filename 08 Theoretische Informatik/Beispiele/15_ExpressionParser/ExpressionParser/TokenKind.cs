namespace ExpressionParser;

/// <summary>
/// The kinds of tokens the <see cref="Tokenizer"/> produces.
/// </summary>
public enum TokenKind
{
    /// <summary>A number literal such as <c>42</c> or <c>3.5</c>.</summary>
    Number,
    /// <summary>The symbol <c>+</c>.</summary>
    Plus,
    /// <summary>The symbol <c>-</c> (subtraction or negation).</summary>
    Minus,
    /// <summary>The symbol <c>*</c>.</summary>
    Star,
    /// <summary>The symbol <c>/</c>.</summary>
    Slash,
    /// <summary>The symbol <c>(</c>.</summary>
    LeftParen,
    /// <summary>The symbol <c>)</c>.</summary>
    RightParen,
    /// <summary>Marks the end of the input. Every token list ends with exactly one such token.</summary>
    End
}
