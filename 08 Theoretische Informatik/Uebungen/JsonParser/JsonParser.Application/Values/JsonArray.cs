namespace JsonParser.Application.Values;

/// <summary>
/// A JSON array. Two arrays are equal if they contain equal items in the same order.
/// </summary>
public sealed record JsonArray(IReadOnlyList<JsonValue> Items) : JsonValue
{
    /// <summary>Compares the items element by element instead of comparing the list references.</summary>
    public bool Equals(JsonArray? other) =>
        other is not null && Items.SequenceEqual(other.Items);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var item in Items) { hash.Add(item); }
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString() => "[" + string.Join(",", Items) + "]";
}
