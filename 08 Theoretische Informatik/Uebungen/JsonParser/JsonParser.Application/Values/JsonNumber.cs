using System.Globalization;

namespace JsonParser.Application.Values;

/// <summary>
/// A JSON number. The value is stored as <see cref="decimal"/>, so decimal fractions like 0.1 are exact.
/// <c>1.5</c> and <c>1.50</c> are equal because <see cref="decimal"/> compares numerically.
/// </summary>
public sealed record JsonNumber(decimal Value) : JsonValue
{
    /// <inheritdoc/>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
