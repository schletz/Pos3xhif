using System.Text.Json;
using JsonParser.Application.Values;

namespace JsonParser.Tests;

/// <summary>
/// Compares a <see cref="JsonValue"/> tree with the result of <see cref="JsonDocument"/> (System.Text.Json),
/// which serves as reference implementation.
/// </summary>
/// <remarks>
/// Only the namespace <c>System.Text.Json</c> is imported. <c>System.Text.Json.Nodes</c> also contains types named
/// <c>JsonObject</c>, <c>JsonArray</c> and <c>JsonValue</c> and would clash with <c>JsonParser.Application.Values</c>.
/// </remarks>
internal static class JsonAssert
{
    /// <summary>
    /// Fails if <paramref name="actual"/> does not represent the same JSON value as <paramref name="expected"/>.
    /// The failure message contains the path to the first difference, e.g. <c>$.tage[2].gerichte</c>.
    /// </summary>
    public static void Equivalent(JsonElement expected, JsonValue actual, string path = "$")
    {
        switch (expected.ValueKind, actual)
        {
            case (JsonValueKind.Object, JsonObject obj):
                var properties = expected.EnumerateObject().ToList();
                if (properties.Count != obj.Members.Count)
                {
                    Fail(path, $"expected {properties.Count} members, actual {obj.Members.Count}");
                }
                foreach (var property in properties)
                {
                    if (!obj.Members.TryGetValue(property.Name, out var member))
                    {
                        Fail(path, $"member \"{property.Name}\" is missing");
                    }
                    Equivalent(property.Value, member!, $"{path}.{property.Name}");
                }
                break;
            case (JsonValueKind.Array, JsonArray array):
                var items = expected.EnumerateArray().ToList();
                if (items.Count != array.Items.Count)
                {
                    Fail(path, $"expected {items.Count} items, actual {array.Items.Count}");
                }
                for (var i = 0; i < items.Count; i++)
                {
                    Equivalent(items[i], array.Items[i], $"{path}[{i}]");
                }
                break;
            case (JsonValueKind.String, JsonString str):
                if (expected.GetString() != str.Value)
                {
                    Fail(path, $"expected \"{expected.GetString()}\", actual \"{str.Value}\"");
                }
                break;
            case (JsonValueKind.Number, JsonNumber number):
                if (expected.GetDecimal() != number.Value)
                {
                    Fail(path, $"expected {expected.GetDecimal()}, actual {number.Value}");
                }
                break;
            case (JsonValueKind.True, JsonBool { Value: true }):
            case (JsonValueKind.False, JsonBool { Value: false }):
            case (JsonValueKind.Null, JsonNull):
                break;
            default:
                Fail(path, $"expected {expected.ValueKind} {expected.GetRawText()}, actual {actual}");
                break;
        }
    }

    private static void Fail(string path, string message) => Assert.Fail($"Difference at {path}: {message}");
}
