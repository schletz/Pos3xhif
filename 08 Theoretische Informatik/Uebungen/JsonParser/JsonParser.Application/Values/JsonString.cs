namespace JsonParser.Application.Values;

/// <summary>A JSON string. <see cref="Value"/> holds the decoded text (escape sequences already resolved).</summary>
public sealed record JsonString(string Value) : JsonValue
{
    /// <inheritdoc/>
    public override string ToString() => Quote(Value);

    /// <summary>Encodes a text as JSON string literal using the escapes of the subset.</summary>
    internal static string Quote(string text) =>
        "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t") + "\"";
}
