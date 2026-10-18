namespace Rechtschreibhilfe.Application;

/// <summary>
/// Checks words against a word list and suggests similar words based on the edit distance.
/// Words are compared exactly (ordinal, case sensitive).
/// </summary>
public sealed class SpellChecker
{
    /// <summary>
    /// Creates a spell checker for the given words. Duplicates are stored only once.
    /// </summary>
    /// <param name="words">The word list, e.g. from <see cref="WordList.LoadGerman"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="words"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="words"/> contains <c>null</c>.</exception>
    public SpellChecker(IEnumerable<string> words)
    {
        throw new NotImplementedException();
    }

    /// <summary>Number of different words in the word list.</summary>
    public int Count => throw new NotImplementedException();

    /// <summary>
    /// Checks whether <paramref name="word"/> is in the word list.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
    public bool Contains(string word)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the words of the list whose edit distance to <paramref name="word"/> is at most
    /// <paramref name="maxDistance"/>. The result is sorted by distance and, for equal distances,
    /// by <see cref="string.CompareOrdinal(string, string)"/>. It contains at most <paramref name="count"/> entries.
    /// If <paramref name="word"/> itself is in the list, it is the first entry (distance 0).
    /// </summary>
    /// <param name="word">The word to look up.</param>
    /// <param name="maxDistance">Largest edit distance that is accepted (0 or more).</param>
    /// <param name="count">Maximum number of suggestions (0 or more).</param>
    /// <returns>The suggestions, possibly an empty list.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxDistance"/> or <paramref name="count"/> is negative.</exception>
    public IReadOnlyList<Suggestion> Suggest(string word, int maxDistance, int count)
    {
        throw new NotImplementedException();
    }
}
