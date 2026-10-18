namespace JsonParser.Application;

/// <summary>A token of the JSON input.</summary>
/// <param name="Type">Kind of the token.</param>
/// <param name="Text">
/// For <see cref="TokenType.String"/> the decoded content without quotes (escapes resolved),
/// for <see cref="TokenType.Number"/> the literal as written in the input,
/// for <see cref="TokenType.End"/> the empty string, otherwise the characters of the token (e.g. <c>{</c> or <c>true</c>).
/// </param>
/// <param name="Position">0-based index of the first character of the token.</param>
/// <param name="Line">1-based line of the first character.</param>
/// <param name="Column">1-based column of the first character.</param>
public sealed record Token(TokenType Type, string Text, int Position, int Line, int Column);
