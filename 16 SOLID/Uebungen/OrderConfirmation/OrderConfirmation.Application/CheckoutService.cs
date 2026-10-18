using System;

namespace OrderConfirmation.Application;

/// <summary>
/// Pays an order and sends an order confirmation to the customer.
/// </summary>
public class CheckoutService
{
    public CheckoutService(IPaymentProvider paymentProvider, IEmailSender emailSender)
    {
        throw new NotImplementedException();
    }

    /// <summary>Optional logger. If it is null, nothing is logged.</summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Pays the order. After a successful payment the customer gets exactly one email with the
    /// subject "Order confirmation". The body contains the total amount.
    /// </summary>
    /// <returns>
    /// True if the payment succeeded. False if the order was already paid or the payment was
    /// declined. In both cases no email is sent.
    /// </returns>
    public bool Checkout(Order order)
    {
        throw new NotImplementedException();
    }
}
