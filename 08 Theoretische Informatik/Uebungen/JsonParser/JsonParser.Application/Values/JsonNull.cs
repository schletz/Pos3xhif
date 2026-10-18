namespace JsonParser.Application.Values;

/// <summary>The JSON literal <c>null</c>. All instances are equal.</summary>
public sealed record JsonNull : JsonValue
{
    /// <inheritdoc/>
    public override string ToString() => "null";
}
