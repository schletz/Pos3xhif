using System.Globalization;

namespace ExpressionParser;

/// <summary>
/// Recursive descent parser for arithmetic expressions. Each nonterminal of the grammar
/// has its own method:
/// <code>
/// expr   = term { ("+" | "-") term }
/// term   = factor { ("*" | "/") factor }
/// factor = number | "(" expr ")" | "-" factor
/// </code>
/// </summary>
public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;

    /// <summary>
    /// Creates a parser for a token list as produced by <see cref="Tokenizer.Tokenize"/>.
    /// </summary>
    /// <param name="tokens">The tokens. The last one has to be <see cref="TokenKind.End"/>.</param>
    public Parser(List<Token> tokens)
    {
        if (tokens.Count == 0 || tokens[^1].Kind != TokenKind.End)
        {
            throw new ArgumentException("The token list has to end with an End token.", nameof(tokens));
        }
        _tokens = tokens;
    }

    /// <summary>
    /// Tokenizes and parses the text and returns the root of the AST.
    /// </summary>
    /// <param name="source">An arithmetic expression such as <c>2 * (3 + 4)</c>.</param>
    /// <exception cref="ParseException">The text is not a valid expression.</exception>
    public static Expr Parse(string source)
    {
        var parser = new Parser(new Tokenizer(source).Tokenize());
        return parser.ParseAll();
    }

    /// <summary>
    /// Parses the whole token list. After the expression only the End token may follow.
    /// </summary>
    /// <exception cref="ParseException">The tokens do not form a valid expression.</exception>
    public Expr ParseAll()
    {
        _pos = 0;
        Expr expr = ParseExpr();
        Expect(TokenKind.End, "end of input");
        return expr;
    }

    // expr = term { ("+" | "-") term }
    private Expr ParseExpr()
    {
        Expr left = ParseTerm();
        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)    // (1)
        {
            Token op = Advance();
            Expr right = ParseTerm();
            left = op.Kind == TokenKind.Plus                         // (2)
                ? new BinaryExpr(left, BinaryOperator.Add, right)
                : new BinaryExpr(left, BinaryOperator.Subtract, right);
        }
        return left;
    }

    // term = factor { ("*" | "/") factor }
    private Expr ParseTerm()
    {
        Expr left = ParseFactor();
        while (Current.Kind is TokenKind.Star or TokenKind.Slash)
        {
            Token op = Advance();
            Expr right = ParseFactor();
            left = op.Kind == TokenKind.Star
                ? new BinaryExpr(left, BinaryOperator.Multiply, right)
                : new BinaryExpr(left, BinaryOperator.Divide, right);
        }
        return left;
    }

    // factor = number | "(" expr ")" | "-" factor
    private Expr ParseFactor()
    {
        Token token = Current;
        switch (token.Kind)                                          // (3)
        {
            case TokenKind.Number:
                Advance();
                return new NumberExpr(double.Parse(token.Text, CultureInfo.InvariantCulture));
            case TokenKind.LeftParen:
                Advance();
                Expr inner = ParseExpr();                            // (4)
                Expect(TokenKind.RightParen, "')'");
                return inner;
            case TokenKind.Minus:
                Advance();
                return new NegateExpr(ParseFactor());                // (5)
            default:
                throw new ParseException(
                    $"Expected number, '(' or '-' but found {Describe(token)}", token.Position);
        }
    }

    /// <summary>The token that is read next. The parser never moves past the End token.</summary>
    private Token Current => _tokens[_pos];

    /// <summary>Consumes the current token and returns it.</summary>
    private Token Advance()
    {
        Token token = Current;
        if (token.Kind != TokenKind.End)
        {
            _pos++;
        }
        return token;
    }

    /// <summary>Consumes the current token if it has the expected kind, otherwise reports an error.</summary>
    private Token Expect(TokenKind kind, string description)
    {
        if (Current.Kind != kind)
        {
            throw new ParseException(
                $"Expected {description} but found {Describe(Current)}", Current.Position);
        }
        return Advance();
    }

    private static string Describe(Token token) =>
        token.Kind == TokenKind.End ? "end of input" : $"'{token.Text}'";
}
