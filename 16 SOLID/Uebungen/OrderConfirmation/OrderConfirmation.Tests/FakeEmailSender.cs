using System;
using System.Collections.Generic;
using OrderConfirmation.Application;

namespace OrderConfirmation.Tests;

/// <summary>
/// Email sender for tests: does not send anything, but stores every email passed to
/// <see cref="Send"/>.
/// </summary>
public class FakeEmailSender : IEmailSender
{
    /// <summary>All emails passed to <see cref="Send"/>, in calling order.</summary>
    public IReadOnlyList<Email> SentEmails => throw new NotImplementedException();

    public void Send(Email email)
    {
        throw new NotImplementedException();
    }
}
