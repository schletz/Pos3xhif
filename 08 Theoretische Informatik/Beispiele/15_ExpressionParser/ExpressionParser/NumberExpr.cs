using System.Globalization;

namespace ExpressionParser;

/// <summary>
/// A leaf of the AST that holds a number.
/// </summary>
/// <param name="Value">The value of the number literal.</param>
public sealed record NumberExpr(double Value) : Expr
{
    /// <summary>
    /// Returns the number with a decimal point, independent of the current culture.
    /// </summary>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
