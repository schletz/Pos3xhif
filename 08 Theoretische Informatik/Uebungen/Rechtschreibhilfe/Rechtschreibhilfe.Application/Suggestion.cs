namespace Rechtschreibhilfe.Application;

/// <summary>
/// A word from the word list together with its edit distance to the word that was looked up.
/// </summary>
/// <param name="Word">The suggested word.</param>
/// <param name="Distance">Edit distance between the looked up word and <paramref name="Word"/>.</param>
public sealed record Suggestion(string Word, int Distance);
