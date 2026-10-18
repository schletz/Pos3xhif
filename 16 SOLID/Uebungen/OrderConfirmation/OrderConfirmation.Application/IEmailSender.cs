namespace OrderConfirmation.Application;

/// <summary>
/// Sends emails, e.g. via a mail server.
/// </summary>
public interface IEmailSender
{
    void Send(Email email);
}
