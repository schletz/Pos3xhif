namespace Dfa.Application;

/// <summary>
/// A deterministic finite automaton (DFA) M = (Q, Σ, δ, q₀, F) over characters.
/// The transition function δ may be partial: a missing transition means that the word is rejected.
/// Instances are immutable; later changes to the collections passed to the constructor have no effect.
/// </summary>
/// <typeparam name="TState">Type of the states, e.g. an enum. Equality is determined by
/// <see cref="EqualityComparer{T}.Default"/>.</typeparam>
public sealed class Dfa<TState> where TState : notnull
{
    /// <summary>
    /// Creates a DFA and validates its configuration.
    /// </summary>
    /// <param name="states">The set of states Q. Every state must occur exactly once.</param>
    /// <param name="alphabet">The alphabet Σ. Every character must occur exactly once.</param>
    /// <param name="transitions">The transition function δ: (state, symbol) → next state.</param>
    /// <param name="start">The start state q₀; must be an element of <paramref name="states"/>.</param>
    /// <param name="finalStates">The set of final states F ⊆ Q. Every state must occur exactly once.</param>
    /// <exception cref="ArgumentNullException">An argument is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The configuration is invalid (see README).</exception>
    public Dfa(
        IEnumerable<TState> states,
        IEnumerable<char> alphabet,
        IReadOnlyDictionary<(TState State, char Symbol), TState> transitions,
        TState start,
        IEnumerable<TState> finalStates)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks whether the automaton accepts <paramref name="word"/>.
    /// </summary>
    /// <returns><c>true</c> if every character could be read and the automaton ends in a final state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
    public bool Accepts(string word)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the sequence of states the automaton passes through while reading <paramref name="word"/>.
    /// The first element is always the start state. If a transition is missing, the sequence ends
    /// with the state in which the automaton got stuck.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
    public IReadOnlyList<TState> Trace(string word)
    {
        throw new NotImplementedException();
    }
}
