namespace JsonParser.Application;

/// <summary>Kinds of tokens the <see cref="Tokenizer"/> produces.</summary>
public enum TokenType
{
    /// <summary><c>{</c></summary>
    LeftBrace,
    /// <summary><c>}</c></summary>
    RightBrace,
    /// <summary><c>[</c></summary>
    LeftBracket,
    /// <summary><c>]</c></summary>
    RightBracket,
    /// <summary><c>:</c></summary>
    Colon,
    /// <summary><c>,</c></summary>
    Comma,
    /// <summary>A string literal including its quotes.</summary>
    String,
    /// <summary>A number literal.</summary>
    Number,
    /// <summary><c>true</c></summary>
    True,
    /// <summary><c>false</c></summary>
    False,
    /// <summary><c>null</c></summary>
    Null,
    /// <summary>End of input. Always the last token.</summary>
    End
}
