namespace JsonParser.Application.Values;

/// <summary>The JSON literals <c>true</c> and <c>false</c>.</summary>
public sealed record JsonBool(bool Value) : JsonValue
{
    /// <inheritdoc/>
    public override string ToString() => Value ? "true" : "false";
}
