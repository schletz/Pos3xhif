namespace Dfa.Application;

/// <summary>
/// Builds the automaton for float literals as defined in the README.
/// </summary>
public static class FloatLiteral
{
    /// <summary>
    /// Creates a DFA that accepts float literals like <c>3.14</c>, <c>-0.5</c>, <c>1e10</c> or <c>2.5E-3</c>.
    /// </summary>
    public static Dfa<FloatLiteralState> Create()
    {
        throw new NotImplementedException();
    }
}
