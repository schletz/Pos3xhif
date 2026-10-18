namespace ExpressionParser;

/// <summary>
/// Thrown by <see cref="Tokenizer"/> and <see cref="Parser"/> when the input is not a valid expression.
/// </summary>
/// <param name="message">Describes what was expected and what was found.</param>
/// <param name="position">Zero-based index in the input where the error was detected.</param>
public class ParseException(string message, int position) : Exception(message)
{
    /// <summary>
    /// Zero-based index in the input where the error was detected.
    /// </summary>
    public int Position { get; } = position;
}
