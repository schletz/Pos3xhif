namespace ExpressionParser;

/// <summary>
/// Splits the input text into a list of <see cref="Token"/>s (lexical analysis).
/// Whitespace separates tokens and is skipped.
/// </summary>
/// <param name="source">The text to split.</param>
public class Tokenizer(string source)
{
    private int _pos;

    /// <summary>
    /// Reads the whole input and returns all tokens. The last token is always <see cref="TokenKind.End"/>.
    /// </summary>
    /// <exception cref="ParseException">The input contains a character that starts no token.</exception>
    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        _pos = 0;
        while (_pos < source.Length)
        {
            char c = source[_pos];
            if (char.IsWhiteSpace(c))
            {
                _pos++;
                continue;
            }
            if (char.IsAsciiDigit(c))
            {
                tokens.Add(ReadNumber());
                continue;
            }
            TokenKind kind = c switch
            {
                '+' => TokenKind.Plus,
                '-' => TokenKind.Minus,
                '*' => TokenKind.Star,
                '/' => TokenKind.Slash,
                '(' => TokenKind.LeftParen,
                ')' => TokenKind.RightParen,
                _ => throw new ParseException($"Unexpected character '{c}'", _pos)
            };
            tokens.Add(new Token(kind, c.ToString(), _pos));
            _pos++;
        }
        tokens.Add(new Token(TokenKind.End, "", source.Length));
        return tokens;
    }

    /// <summary>
    /// Reads a number literal: <c>number = digit { digit } [ "." digit { digit } ]</c>.
    /// </summary>
    private Token ReadNumber()
    {
        int start = _pos;
        SkipDigits();
        if (_pos < source.Length && source[_pos] == '.')
        {
            _pos++;
            // "3." is not a number: at least one digit has to follow the decimal point.
            if (_pos >= source.Length || !char.IsAsciiDigit(source[_pos]))
            {
                throw new ParseException("Expected digit after '.'", _pos);
            }
            SkipDigits();
        }
        return new Token(TokenKind.Number, source[start.._pos], start);
    }

    private void SkipDigits()
    {
        while (_pos < source.Length && char.IsAsciiDigit(source[_pos]))
        {
            _pos++;
        }
    }
}
