namespace JsonParser.Application;

/// <summary>
/// Thrown by <see cref="Tokenizer"/> and <see cref="Parser"/> when the input is not valid
/// according to the JSON subset of this exercise.
/// </summary>
/// <param name="message">Description of the error, without the position (it is appended automatically).</param>
/// <param name="position">0-based index of the error in the input string.</param>
/// <param name="line">1-based line number of <paramref name="position"/>.</param>
/// <param name="column">1-based column number of <paramref name="position"/>.</param>
public sealed class JsonParseException(string message, int position, int line, int column)
    : Exception($"{message} (line {line}, column {column})")
{
    /// <summary>0-based index of the error in the input string.</summary>
    public int Position { get; } = position;

    /// <summary>1-based line number. A new line starts after each <c>\n</c>.</summary>
    public int Line { get; } = line;

    /// <summary>1-based column number: number of characters since the last <c>\n</c> plus 1.</summary>
    public int Column { get; } = column;
}
