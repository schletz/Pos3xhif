namespace JsonParser.Application.Values;

/// <summary>
/// A JSON object. Two objects are equal if they have the same keys with equal values.
/// The order of the members does not matter for equality.
/// </summary>
public sealed record JsonObject(IReadOnlyDictionary<string, JsonValue> Members) : JsonValue
{
    /// <summary>Compares the members key by key instead of comparing the dictionary references.</summary>
    public bool Equals(JsonObject? other) =>
        other is not null
        && Members.Count == other.Members.Count
        && Members.All(m => other.Members.TryGetValue(m.Key, out var value) && m.Value.Equals(value));

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        // XOR is order independent, so objects with the same members in a different order get the same hash.
        var hash = 0;
        foreach (var (key, value) in Members) { hash ^= HashCode.Combine(key, value); }
        return hash;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "{" + string.Join(",", Members.Select(m => JsonString.Quote(m.Key) + ":" + m.Value)) + "}";
}
