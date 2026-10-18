namespace ExpressionParser;

/// <summary>
/// An inner node of the AST for the unary minus, e.g. <c>-x</c>.
/// </summary>
/// <param name="Operand">The expression that is negated.</param>
public sealed record NegateExpr(Expr Operand) : Expr
{
    /// <summary>
    /// Returns the subtree in prefix notation, e.g. <c>(neg 5)</c>.
    /// </summary>
    public override string ToString() => $"(neg {Operand})";
}
