using JsonParser.Application;

namespace JsonParser.Tests;

/// <summary>Tests for <see cref="Parser"/> with invalid input: every error must be reported at the defined position.</summary>
public class ParserErrorTests
{
    [Theory]
    [InlineData("", 0, 1, 1)]
    [InlineData("   ", 3, 1, 4)]
    [InlineData("[1,]", 3, 1, 4)]
    [InlineData("{\"a\":1,}", 7, 1, 8)]
    [InlineData("[1 2]", 3, 1, 4)]
    [InlineData("{\"a\" 1}", 5, 1, 6)]
    [InlineData("{1:2}", 1, 1, 2)]
    [InlineData("{true:2}", 1, 1, 2)]
    [InlineData("{\"a\":}", 5, 1, 6)]
    [InlineData("[", 1, 1, 2)]
    [InlineData("{\"a\":1", 6, 1, 7)]
    [InlineData("1 2", 2, 1, 3)]
    [InlineData("{\"a\":1}{", 7, 1, 8)]
    [InlineData("]", 0, 1, 1)]
    [InlineData(":", 0, 1, 1)]
    [InlineData("[,1]", 1, 1, 2)]
    [InlineData("{,}", 1, 1, 2)]
    [InlineData("[1,,2]", 3, 1, 4)]
    [InlineData("[1}", 2, 1, 3)]
    [InlineData("{\"a\":1]", 6, 1, 7)]
    [InlineData("{\n  \"a\": 1,\n}", 12, 3, 1)]
    public void Parse_SyntaxError_ThrowsAtUnexpectedToken(string input, int position, int line, int column)
    {
        AssertError(input, position, line, column);
    }

    [Theory]
    [InlineData("{\"a\":1,\"a\":2}", 7, 1, 8)]
    [InlineData("{\"a\":{\"b\":1,\"b\":2}}", 12, 1, 13)]
    [InlineData("{\n\"x\": 1,\n\"y\": 2,\n\"x\": 3\n}", 18, 4, 1)]
    public void Parse_DuplicateKey_ThrowsAtSecondKey(string input, int position, int line, int column)
    {
        AssertError(input, position, line, column);
    }

    [Theory]
    [InlineData("99999999999999999999999999999999", 0, 1, 1)]
    [InlineData("[1, -79228162514264337593543950336]", 4, 1, 5)]
    public void Parse_NumberOutOfDecimalRange_ThrowsAtNumber(string input, int position, int line, int column)
    {
        AssertError(input, position, line, column);
    }

    [Theory]
    [InlineData("[1, 2, @]", 7, 1, 8)]
    [InlineData("{\"text\": \"a\\u0041\"}", 11, 1, 12)]
    [InlineData("[1, 1e5]", 5, 1, 6)]
    public void Parse_LexicalError_IsReportedByParse(string input, int position, int line, int column)
    {
        AssertError(input, position, line, column);
    }

    [Fact]
    public void Parse_Error_MessageContainsLineAndColumn()
    {
        var ex = Assert.Throws<JsonParseException>(() => Parser.Parse("[\n  1,\n]"));
        Assert.Contains("(line 3, column 1)", ex.Message);
    }

    private static void AssertError(string input, int position, int line, int column)
    {
        var ex = Assert.Throws<JsonParseException>(() => Parser.Parse(input));
        Assert.Equal((position, line, column), (ex.Position, ex.Line, ex.Column));
    }
}
