using System.Text.Json;
using JsonParser.Application.Values;

namespace JsonParser.Tests;

/// <summary>
/// Tests for the given value types and the comparison helper. These tests do not need the parser
/// and pass right from the start.
/// </summary>
public class JsonValueTests
{
    private static JsonObject Sample() => new(new Dictionary<string, JsonValue>
    {
        ["name"] = new JsonString("Tafelspitz"),
        ["preis"] = new JsonNumber(8.40m),
        ["allergene"] = new JsonArray([new JsonString("A"), new JsonString("C")]),
        ["aktion"] = new JsonNull()
    });

    [Fact]
    public void Equals_IndependentInstancesWithSameContent_AreEqual()
    {
        Assert.Equal(Sample(), Sample());
        Assert.Equal(Sample().GetHashCode(), Sample().GetHashCode());
    }

    [Fact]
    public void Equals_DifferentArrayOrder_AreNotEqual()
    {
        Assert.NotEqual(new JsonArray([new JsonNumber(1), new JsonNumber(2)]), new JsonArray([new JsonNumber(2), new JsonNumber(1)]));
    }

    [Fact]
    public void ToString_ReturnsCompactJson()
    {
        var value = new JsonArray([new JsonString("a\"b\\c\nd\te"), new JsonNumber(-0.5m), new JsonBool(false), Sample()]);
        Assert.Equal(
            "[\"a\\\"b\\\\c\\nd\\te\",-0.5,false,{\"name\":\"Tafelspitz\",\"preis\":8.40,\"allergene\":[\"A\",\"C\"],\"aktion\":null}]",
            value.ToString());
    }

    [Fact]
    public void JsonAssert_DifferentValue_FailsWithPath()
    {
        using var document = JsonDocument.Parse("{\"preis\": 8.40, \"allergene\": [\"A\", \"G\"], \"name\": \"Tafelspitz\", \"aktion\": null}");
        var ex = Assert.ThrowsAny<Exception>(() => JsonAssert.Equivalent(document.RootElement, Sample()));
        Assert.Contains("$.allergene[1]", ex.Message);
    }
}
