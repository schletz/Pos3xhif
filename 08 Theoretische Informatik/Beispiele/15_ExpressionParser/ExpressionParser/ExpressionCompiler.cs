using System.Linq.Expressions;

namespace ExpressionParser;

/// <summary>
/// Translates our own AST into a <see cref="System.Linq.Expressions.Expression"/> tree.
/// .NET can compile such a tree into a delegate that runs as IL code.
/// </summary>
public static class ExpressionCompiler
{
    /// <summary>
    /// Builds the expression tree and compiles it into a delegate without parameters.
    /// </summary>
    /// <param name="expr">The root of our AST.</param>
    /// <returns>A delegate that computes the value of the expression.</returns>
    public static Func<double> Compile(Expr expr)
    {
        Expression body = ToExpression(expr);
        return Expression.Lambda<Func<double>>(body).Compile();
    }

    /// <summary>
    /// Translates the tree node by node: every node of our AST becomes exactly one node of the
    /// expression tree.
    /// </summary>
    /// <param name="expr">The root of our AST.</param>
    /// <returns>The root of the expression tree.</returns>
    public static Expression ToExpression(Expr expr) => expr switch
    {
        NumberExpr(var value) => Expression.Constant(value),
        NegateExpr(var operand) => Expression.Negate(ToExpression(operand)),
        BinaryExpr(var left, var op, var right) => Combine(op, ToExpression(left), ToExpression(right)),
        _ => throw new ArgumentException($"Unknown node {expr.GetType().Name}", nameof(expr))
    };

    private static BinaryExpression Combine(BinaryOperator op, Expression left, Expression right) => op switch
    {
        BinaryOperator.Add => Expression.Add(left, right),
        BinaryOperator.Subtract => Expression.Subtract(left, right),
        BinaryOperator.Multiply => Expression.Multiply(left, right),
        BinaryOperator.Divide => Expression.Divide(left, right),
        _ => throw new ArgumentException($"Unknown operator {op}", nameof(op))
    };
}
