namespace OrderConfirmation.Application;

/// <summary>
/// Pays an amount, e.g. with a credit card.
/// </summary>
public interface IPaymentProvider
{
    /// <summary>Returns true if the payment succeeded, false if it was declined.</summary>
    bool Pay(decimal amount);
}
