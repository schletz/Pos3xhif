using JsonParser.Application;

namespace JsonParser.Tests;

/// <summary>Tests for <see cref="Tokenizer.Tokenize"/>: token types, decoded text, positions and lexical errors.</summary>
public class TokenizerTests
{
    private static TokenType[] Types(string input) => [.. Tokenizer.Tokenize(input).Select(t => t.Type)];

    [Fact]
    public void Tokenize_Punctuation_ReturnsTypesInOrder()
    {
        TokenType[] expected =
        [
            TokenType.LeftBrace, TokenType.RightBrace, TokenType.LeftBracket, TokenType.RightBracket,
            TokenType.Colon, TokenType.Comma, TokenType.End
        ];
        Assert.Equal(expected, Types("{}[]:,"));
    }

    [Fact]
    public void Tokenize_EmptyInput_ReturnsOnlyEndToken()
    {
        var tokens = Tokenizer.Tokenize("");
        Assert.Equal([new Token(TokenType.End, "", 0, 1, 1)], tokens);
    }

    [Fact]
    public void Tokenize_Literals_ReturnsLiteralTokens()
    {
        var tokens = Tokenizer.Tokenize("true false null");
        Token[] expected =
        [
            new(TokenType.True, "true", 0, 1, 1),
            new(TokenType.False, "false", 5, 1, 6),
            new(TokenType.Null, "null", 11, 1, 12),
            new(TokenType.End, "", 15, 1, 16)
        ];
        Assert.Equal(expected, tokens);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-0")]
    [InlineData("42")]
    [InlineData("-17")]
    [InlineData("3.14")]
    [InlineData("-0.5")]
    [InlineData("10.250")]
    [InlineData("1234567890123")]
    public void Tokenize_ValidNumber_ReturnsNumberTokenWithLiteralText(string input)
    {
        var tokens = Tokenizer.Tokenize(input);
        Assert.Equal([new Token(TokenType.Number, input, 0, 1, 1), new Token(TokenType.End, "", input.Length, 1, input.Length + 1)], tokens);
    }

    [Theory]
    [InlineData("\"abc\"", "abc")]
    [InlineData("\"\"", "")]
    [InlineData("\"a\\\"b\"", "a\"b")]
    [InlineData("\"a\\\\b\"", "a\\b")]
    [InlineData("\"line1\\nline2\"", "line1\nline2")]
    [InlineData("\"a\\tb\"", "a\tb")]
    [InlineData("\"\\\\n\"", "\\n")]
    [InlineData("\"Grüße aus Graz €\"", "Grüße aus Graz €")]
    [InlineData("\"{[:,]} true 1\"", "{[:,]} true 1")]
    public void Tokenize_String_ReturnsDecodedText(string input, string expectedText)
    {
        var tokens = Tokenizer.Tokenize(input);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(new Token(TokenType.String, expectedText, 0, 1, 1), tokens[0]);
        Assert.Equal(new Token(TokenType.End, "", input.Length, 1, input.Length + 1), tokens[1]);
    }

    [Fact]
    public void Tokenize_TokensWithoutWhitespace_AreSeparatedCorrectly()
    {
        var tokens = Tokenizer.Tokenize("{\"a\":[-1.5,true]}");
        Token[] expected =
        [
            new(TokenType.LeftBrace, "{", 0, 1, 1),
            new(TokenType.String, "a", 1, 1, 2),
            new(TokenType.Colon, ":", 4, 1, 5),
            new(TokenType.LeftBracket, "[", 5, 1, 6),
            new(TokenType.Number, "-1.5", 6, 1, 7),
            new(TokenType.Comma, ",", 10, 1, 11),
            new(TokenType.True, "true", 11, 1, 12),
            new(TokenType.RightBracket, "]", 15, 1, 16),
            new(TokenType.RightBrace, "}", 16, 1, 17),
            new(TokenType.End, "", 17, 1, 18)
        ];
        Assert.Equal(expected, tokens);
    }

    [Fact]
    public void Tokenize_MultipleLines_ComputesLineAndColumn()
    {
        // \r\n counts as one line break, \t counts as one column.
        var tokens = Tokenizer.Tokenize("{\n  \"a\": [1,\r\n\ttrue]\n}");
        Token[] expected =
        [
            new(TokenType.LeftBrace, "{", 0, 1, 1),
            new(TokenType.String, "a", 4, 2, 3),
            new(TokenType.Colon, ":", 7, 2, 6),
            new(TokenType.LeftBracket, "[", 9, 2, 8),
            new(TokenType.Number, "1", 10, 2, 9),
            new(TokenType.Comma, ",", 11, 2, 10),
            new(TokenType.True, "true", 15, 3, 2),
            new(TokenType.RightBracket, "]", 19, 3, 6),
            new(TokenType.RightBrace, "}", 21, 4, 1),
            new(TokenType.End, "", 22, 4, 2)
        ];
        Assert.Equal(expected, tokens);
    }

    [Fact]
    public void Tokenize_TrailingWhitespace_EndTokenIsAtInputLength()
    {
        var tokens = Tokenizer.Tokenize("[1]  \n ");
        Assert.Equal(new Token(TokenType.End, "", 7, 2, 2), tokens[^1]);
    }

    [Theory]
    [InlineData("@", 0, 1, 1)]
    [InlineData("[1, @]", 4, 1, 5)]
    [InlineData("'text'", 0, 1, 1)]
    [InlineData("/* comment */ 1", 0, 1, 1)]
    [InlineData("\"abc", 0, 1, 1)]
    [InlineData("[ \"abc", 2, 1, 3)]
    [InlineData("\"abc\\", 0, 1, 1)]
    [InlineData("\"a\\x\"", 2, 1, 3)]
    [InlineData("\"a\\u0041\"", 2, 1, 3)]
    [InlineData("\"a\\/b\"", 2, 1, 3)]
    [InlineData("\"a\\rb\"", 2, 1, 3)]
    [InlineData("\"a\nb\"", 2, 1, 3)]
    [InlineData("\"a\tb\"", 2, 1, 3)]
    [InlineData("01", 0, 1, 1)]
    [InlineData("-01", 0, 1, 1)]
    [InlineData("[00]", 1, 1, 2)]
    [InlineData("-", 0, 1, 1)]
    [InlineData("-x", 0, 1, 1)]
    [InlineData("1.", 0, 1, 1)]
    [InlineData("[1.]", 1, 1, 2)]
    [InlineData("1.x", 0, 1, 1)]
    [InlineData(".5", 0, 1, 1)]
    [InlineData("+1", 0, 1, 1)]
    [InlineData("1e5", 1, 1, 2)]
    [InlineData("1.5.2", 3, 1, 4)]
    [InlineData("True", 0, 1, 1)]
    [InlineData("nul", 0, 1, 1)]
    [InlineData("nullx", 0, 1, 1)]
    [InlineData("[true, fals]", 7, 1, 8)]
    [InlineData("[\n  1,\n  @\n]", 9, 3, 3)]
    public void Tokenize_InvalidInput_ThrowsWithPosition(string input, int position, int line, int column)
    {
        var ex = Assert.Throws<JsonParseException>(() => Tokenizer.Tokenize(input));
        Assert.Equal((position, line, column), (ex.Position, ex.Line, ex.Column));
    }

    [Fact]
    public void Tokenize_NonBreakingSpace_IsNotWhitespace()
    {
        var input = "[1," + (char)0xA0 + "2]";
        var ex = Assert.Throws<JsonParseException>(() => Tokenizer.Tokenize(input));
        Assert.Equal((3, 1, 4), (ex.Position, ex.Line, ex.Column));
    }
}
