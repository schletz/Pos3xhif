namespace JsonParser.Application.Values;

/// <summary>
/// Base type of every value in a parsed JSON document.
/// The concrete types are <see cref="JsonObject"/>, <see cref="JsonArray"/>, <see cref="JsonString"/>,
/// <see cref="JsonNumber"/>, <see cref="JsonBool"/> and <see cref="JsonNull"/>.
/// </summary>
/// <remarks>
/// All types have value equality, so two independently parsed documents with the same content are equal.
/// <see cref="ToString"/> returns the value as compact JSON, which keeps test failure messages readable.
/// </remarks>
public abstract record JsonValue
{
    /// <summary>Returns the value as compact JSON text (no whitespace between tokens).</summary>
    public abstract override string ToString();
}
