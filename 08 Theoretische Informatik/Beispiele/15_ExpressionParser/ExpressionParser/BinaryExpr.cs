namespace ExpressionParser;

/// <summary>
/// An inner node of the AST that combines two subexpressions with an operator.
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Operator">The operator.</param>
/// <param name="Right">The right operand.</param>
public sealed record BinaryExpr(Expr Left, BinaryOperator Operator, Expr Right) : Expr
{
    /// <summary>
    /// Returns the subtree in prefix notation, e.g. <c>(* 2 (+ 3 4))</c>.
    /// </summary>
    public override string ToString()
    {
        char symbol = Operator switch
        {
            BinaryOperator.Add => '+',
            BinaryOperator.Subtract => '-',
            BinaryOperator.Multiply => '*',
            BinaryOperator.Divide => '/',
            _ => throw new InvalidOperationException($"Unknown operator {Operator}")
        };
        return $"({symbol} {Left} {Right})";
    }
}
