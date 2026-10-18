using System.Collections.Generic;

namespace SmsGateway.Application;

/// <summary>
/// An SMS read from an input line. Long texts are split into several parts.
/// </summary>
public class SmsMessage
{
    /// <summary>
    /// Creates a message. The parts must already be split.
    /// </summary>
    public SmsMessage(string sender, string recipient, string text, IReadOnlyList<string> parts)
    {
        Sender = sender;
        Recipient = recipient;
        Text = text;
        Parts = parts;
    }

    /// <summary>The sender name as written in the input line.</summary>
    public string Sender { get; }

    /// <summary>The mobile number of the recipient, for example +436641234567.</summary>
    public string Recipient { get; }

    /// <summary>The complete text.</summary>
    public string Text { get; }

    /// <summary>The text split into SMS parts. A short text has exactly one part.</summary>
    public IReadOnlyList<string> Parts { get; }
}
