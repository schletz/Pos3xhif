using System;

namespace OrderConfirmation.Application;

/// <summary>
/// An email with recipient, subject and body. All properties are read-only.
/// </summary>
public class Email
{
    public Email(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }

    public string To => throw new NotImplementedException();
    public string Subject => throw new NotImplementedException();
    public string Body => throw new NotImplementedException();
}
