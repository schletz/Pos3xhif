using JsonParser.Application;
using JsonParser.Application.Values;

namespace JsonParser.Tests;

/// <summary>Tests for <see cref="Parser"/> with valid input: the resulting <see cref="JsonValue"/> tree.</summary>
public class ParserTests
{
    private static JsonObject Obj(params (string Key, JsonValue Value)[] members) =>
        new(members.ToDictionary(m => m.Key, m => m.Value));

    private static JsonArray Arr(params JsonValue[] items) => new(items);

    private static JsonNumber Num(decimal value) => new(value);

    public static TheoryData<string, JsonValue> Scalars => new()
    {
        { "true", new JsonBool(true) },
        { "false", new JsonBool(false) },
        { "null", new JsonNull() },
        { "\"Servus\"", new JsonString("Servus") },
        { "\"a\\\"b\\\\c\\nd\\te\"", new JsonString("a\"b\\c\nd\te") },
        { "42", Num(42) },
        { "0", Num(0) },
        { "-0", Num(0) },
        { "-0.5", Num(-0.5m) },
        { "  7 \r\n", Num(7) }
    };

    [Theory]
    [MemberData(nameof(Scalars))]
    public void Parse_Scalar_ReturnsMatchingValue(string json, JsonValue expected)
    {
        Assert.Equal(expected, Parser.Parse(json));
    }

    [Fact]
    public void Parse_EmptyObject_ReturnsObjectWithoutMembers()
    {
        var result = Assert.IsType<JsonObject>(Parser.Parse("{ }"));
        Assert.Empty(result.Members);
    }

    [Fact]
    public void Parse_EmptyArray_ReturnsArrayWithoutItems()
    {
        var result = Assert.IsType<JsonArray>(Parser.Parse("[\n]"));
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Parse_Array_KeepsOrderOfItems()
    {
        Assert.Equal(Arr(Num(3), Num(1), Num(2)), Parser.Parse("[3, 1, 2]"));
    }

    [Fact]
    public void Parse_MixedArray_ReturnsAllValueTypes()
    {
        var expected = Arr(Num(1), new JsonString("zwei"), new JsonBool(true), new JsonNull(), Arr(), Obj());
        Assert.Equal(expected, Parser.Parse("[1,\"zwei\",true,null,[],{}]"));
    }

    [Fact]
    public void Parse_NestedArrays_ReturnsTree()
    {
        Assert.Equal(Arr(Arr(Num(1)), Arr(Num(2), Arr(Num(3)))), Parser.Parse("[[1],[2,[3]]]"));
    }

    [Fact]
    public void Parse_NestedObject_ReturnsTree()
    {
        const string json = """
            {
              "name": "Anna",
              "noten": [1, 2, null],
              "aktiv": true,
              "adresse": { "plz": "1050", "ort": "Wien" }
            }
            """;
        var expected = Obj(
            ("name", new JsonString("Anna")),
            ("noten", Arr(Num(1), Num(2), new JsonNull())),
            ("aktiv", new JsonBool(true)),
            ("adresse", Obj(("plz", new JsonString("1050")), ("ort", new JsonString("Wien")))));
        Assert.Equal(expected, Parser.Parse(json));
    }

    [Fact]
    public void Parse_KeysWithEscapesAndEmptyKey_AreDecoded()
    {
        var expected = Obj(("", Num(1)), ("a\"b", Num(2)), ("x\ny", Num(3)));
        Assert.Equal(expected, Parser.Parse("{\"\":1, \"a\\\"b\":2, \"x\\ny\":3}"));
    }

    [Fact]
    public void Parse_SameKeyInDifferentObjects_IsAllowed()
    {
        var expected = Arr(Obj(("a", Num(1))), Obj(("a", Num(2))));
        Assert.Equal(expected, Parser.Parse("[{\"a\":1},{\"a\":2}]"));
    }

    [Theory]
    [InlineData("0.1", "0.1")]
    [InlineData("1.50", "1.5")]
    [InlineData("-273.15", "-273.15")]
    [InlineData("12345678901234567890", "12345678901234567890")]
    [InlineData("0.0000000001", "0.0000000001")]
    [InlineData("79228162514264337593543950335", "79228162514264337593543950335")]
    public void Parse_Number_IsExactDecimal(string json, string expected)
    {
        var number = Assert.IsType<JsonNumber>(Parser.Parse(json));
        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture), number.Value);
    }

    [Fact]
    public void Parse_DeepNesting_Works()
    {
        const int depth = 500;
        var json = new string('[', depth) + new string(']', depth);
        var value = Parser.Parse(json);
        for (var i = 1; i < depth; i++)
        {
            value = Assert.Single(Assert.IsType<JsonArray>(value).Items);
        }
        Assert.Empty(Assert.IsType<JsonArray>(value).Items);
    }

    [Fact]
    public void ParseDocument_WithTokensFromTokenizer_ReturnsValue()
    {
        var parser = new Parser(Tokenizer.Tokenize("{\"ok\": [true]}"));
        Assert.Equal(Obj(("ok", Arr(new JsonBool(true)))), parser.ParseDocument());
    }

    [Fact]
    public void Parse_ObjectsWithDifferentMemberOrder_AreEqual()
    {
        Assert.Equal(Parser.Parse("{\"a\":1,\"b\":[2]}"), Parser.Parse("{\"b\":[2],\"a\":1}"));
    }
}
