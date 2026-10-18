namespace Rechtschreibhilfe.Application;

/// <summary>
/// Computes the edit distance (Levenshtein distance) of two strings: the smallest number of
/// insertions, deletions and substitutions of single characters that turn <c>source</c> into <c>target</c>.
/// Each operation costs 1. Characters are compared exactly (ordinal), so <c>H</c> and <c>h</c> are different.
/// </summary>
public static class EditDistance
{
    /// <summary>
    /// Computes the edit distance with plain recursion, without storing intermediate results.
    /// Only suitable for short strings.
    /// </summary>
    /// <param name="source">The string to transform.</param>
    /// <param name="target">The string to transform into.</param>
    /// <returns>The edit distance.</returns>
    /// <exception cref="ArgumentNullException">An argument is <c>null</c>.</exception>
    public static int Naive(string source, string target)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Fills the dynamic programming table. The table has <c>source.Length + 1</c> rows and
    /// <c>target.Length + 1</c> columns. The cell <c>[i, j]</c> contains the edit distance between the first
    /// <c>i</c> characters of <paramref name="source"/> and the first <c>j</c> characters of <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The string to transform.</param>
    /// <param name="target">The string to transform into.</param>
    /// <returns>The completely filled table.</returns>
    /// <exception cref="ArgumentNullException">An argument is <c>null</c>.</exception>
    public static int[,] Table(string source, string target)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the edit distance with dynamic programming (see <see cref="Table"/>).
    /// </summary>
    /// <param name="source">The string to transform.</param>
    /// <param name="target">The string to transform into.</param>
    /// <returns>The edit distance.</returns>
    /// <exception cref="ArgumentNullException">An argument is <c>null</c>.</exception>
    public static int Compute(string source, string target)
    {
        throw new NotImplementedException();
    }
}
