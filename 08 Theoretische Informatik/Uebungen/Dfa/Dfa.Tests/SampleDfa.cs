using Dfa.Application;

namespace Dfa.Tests;

/// <summary>
/// A small partial DFA over Σ = {a, b} for the language a b* a, used as test fixture.
/// δ(0, a) = 1, δ(1, b) = 1, δ(1, a) = 2, F = {2}. All other transitions are missing.
/// </summary>
internal static class SampleDfa
{
    public static readonly int[] States = [0, 1, 2];
    public const string Alphabet = "ab";
    public const int Start = 0;
    public static readonly int[] FinalStates = [2];

    public static Dictionary<(int, char), int> CreateTransitions() => new()
    {
        [(0, 'a')] = 1,
        [(1, 'b')] = 1,
        [(1, 'a')] = 2
    };

    public static Dfa<int> Create() =>
        new(States, Alphabet, CreateTransitions(), Start, FinalStates);
}
