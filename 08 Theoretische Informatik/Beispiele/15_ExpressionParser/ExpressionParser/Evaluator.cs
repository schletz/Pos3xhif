namespace ExpressionParser;

/// <summary>
/// Computes the value of an AST.
/// </summary>
public static class Evaluator
{
    /// <summary>
    /// Evaluates the tree recursively: first the children, then the node itself.
    /// Division by zero follows the rules of <see cref="double"/> and yields infinity or NaN.
    /// </summary>
    /// <param name="expr">The root of the tree.</param>
    /// <returns>The value of the expression.</returns>
    public static double Evaluate(Expr expr) => expr switch
    {
        NumberExpr(var value) => value,
        NegateExpr(var operand) => -Evaluate(operand),
        BinaryExpr(var left, BinaryOperator.Add, var right) => Evaluate(left) + Evaluate(right),
        BinaryExpr(var left, BinaryOperator.Subtract, var right) => Evaluate(left) - Evaluate(right),
        BinaryExpr(var left, BinaryOperator.Multiply, var right) => Evaluate(left) * Evaluate(right),
        BinaryExpr(var left, BinaryOperator.Divide, var right) => Evaluate(left) / Evaluate(right),
        _ => throw new ArgumentException($"Unknown node {expr.GetType().Name}", nameof(expr))
    };
}
