using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SmsGateway.Application;

/// <summary>
/// Dispatches SMS messages from text lines in the format "sender;recipient;text".
/// Only senders from the sender register may send messages.
/// </summary>
public class SmsDispatcher
{
    private const int SinglePartLength = 160;
    private const int MultiPartLength = 153;
    private const int MaxParts = 3;
    private const decimal CostPerPart = 0.09m;

    private readonly List<string> registeredSenders = new List<string>();
    private readonly List<SmsMessage> sentMessages = new List<SmsMessage>();
    private readonly List<SmsMessage> blockedMessages = new List<SmsMessage>();
    private readonly List<string> errors = new List<string>();
    private int sentPartCount = 0;

    /// <summary>
    /// Creates a dispatcher with the given registered sender names.
    /// </summary>
    /// <exception cref="ArgumentException">A sender name is invalid.</exception>
    public SmsDispatcher(IEnumerable<string> registeredSenders)
    {
        foreach (string sender in registeredSenders)
        {
            RegisterSender(sender);
        }
    }

    /// <summary>Messages that were sent, in the order of the input lines.</summary>
    public IReadOnlyList<SmsMessage> SentMessages => sentMessages;

    /// <summary>Valid messages that were not sent because the sender is not registered.</summary>
    public IReadOnlyList<SmsMessage> BlockedMessages => blockedMessages;

    /// <summary>One error text per invalid line, for example "Line 2: sender is missing.".</summary>
    public IReadOnlyList<string> Errors => errors;

    /// <summary>
    /// Adds a sender name to the register. A name has 3 to 11 characters and contains
    /// only letters and digits. Registering a name twice has no effect.
    /// </summary>
    /// <exception cref="ArgumentException">The name is invalid.</exception>
    public void RegisterSender(string senderName)
    {
        if (senderName.Length < 3 || senderName.Length > 11)
        {
            throw new ArgumentException($"Sender name '{senderName}' must have 3 to 11 characters.", nameof(senderName));
        }
        foreach (char c in senderName)
        {
            if (!char.IsLetterOrDigit(c))
            {
                throw new ArgumentException($"Sender name '{senderName}' may only contain letters and digits.", nameof(senderName));
            }
        }

        // Store the names in upper case, so the check ignores upper and lower case.
        string normalizedName = senderName.ToUpperInvariant();
        if (!registeredSenders.Contains(normalizedName))
        {
            registeredSenders.Add(normalizedName);
        }
    }

    /// <summary>
    /// Reads the lines, validates them and sends every valid message of a registered sender.
    /// Blank lines are skipped. Line numbers start at 1 for every call.
    /// </summary>
    public void Dispatch(IEnumerable<string> lines)
    {
        int lineNumber = 0;
        foreach (string line in lines)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Split into sender, recipient and text. The text itself may contain ';'.
            string[] fields = line.Split(';', 3);
            if (fields.Length != 3)
            {
                errors.Add($"Line {lineNumber}: expected 3 fields separated by ';'.");
                continue;
            }
            string sender = fields[0].Trim();
            string recipient = fields[1].Trim();
            string text = fields[2];

            if (sender.Length == 0)
            {
                errors.Add($"Line {lineNumber}: sender is missing.");
                continue;
            }

            // An Austrian mobile number: +43 followed by 8 to 12 digits.
            bool recipientIsValid = recipient.StartsWith("+43") && recipient.Length >= 11 && recipient.Length <= 15;
            for (int i = 1; i < recipient.Length && recipientIsValid; i++)
            {
                if (!char.IsDigit(recipient[i]))
                {
                    recipientIsValid = false;
                }
            }
            if (!recipientIsValid)
            {
                errors.Add($"Line {lineNumber}: invalid recipient '{recipient}'.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                errors.Add($"Line {lineNumber}: text is empty.");
                continue;
            }
            if (text.Length > MultiPartLength * MaxParts)
            {
                errors.Add($"Line {lineNumber}: text is longer than {MultiPartLength * MaxParts} characters.");
                continue;
            }

            // A text with up to 160 characters fits into one SMS.
            // Longer texts are split into parts of 153 characters.
            List<string> parts = new List<string>();
            if (text.Length <= SinglePartLength)
            {
                parts.Add(text);
            }
            else
            {
                for (int start = 0; start < text.Length; start += MultiPartLength)
                {
                    int length = Math.Min(MultiPartLength, text.Length - start);
                    parts.Add(text.Substring(start, length));
                }
            }
            SmsMessage message = new SmsMessage(sender, recipient, text, parts);

            // Only registered senders may send.
            if (!registeredSenders.Contains(sender.ToUpperInvariant()))
            {
                blockedMessages.Add(message);
                continue;
            }

            // Send the message. In this version, sending means adding it to the outbox list.
            sentMessages.Add(message);
            sentPartCount += parts.Count;
        }
    }

    /// <summary>
    /// Creates a text report with the counters, the total cost and the blocked senders.
    /// </summary>
    public string CreateReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("SMS dispatch report");
        sb.AppendLine($"Sent messages: {sentMessages.Count}");
        sb.AppendLine($"Sent parts: {sentPartCount}");
        sb.AppendLine($"Blocked messages: {blockedMessages.Count}");
        sb.AppendLine($"Invalid lines: {errors.Count}");
        decimal totalCost = sentPartCount * CostPerPart;
        sb.AppendLine($"Total cost: {totalCost.ToString("0.00", CultureInfo.InvariantCulture)} EUR");
        if (blockedMessages.Count > 0)
        {
            // Every blocked sender appears once, sorted by name.
            List<string> senders = blockedMessages
                .Select(m => m.Sender)
                .Distinct()
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToList();
            sb.AppendLine($"Blocked senders: {string.Join(", ", senders)}");
        }
        return sb.ToString();
    }
}
