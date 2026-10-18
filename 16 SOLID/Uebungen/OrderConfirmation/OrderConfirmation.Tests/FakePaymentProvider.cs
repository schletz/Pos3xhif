using System.Collections.Generic;
using OrderConfirmation.Application;

namespace OrderConfirmation.Tests;

/// <summary>
/// Payment provider for tests: does not pay, returns a fixed result and records every payment.
/// </summary>
public class FakePaymentProvider : IPaymentProvider
{
    private readonly bool _result;
    private readonly List<decimal> _payments = new();

    public FakePaymentProvider(bool result)
    {
        _result = result;
    }

    /// <summary>All amounts passed to <see cref="Pay"/>, in calling order.</summary>
    public IReadOnlyList<decimal> Payments => _payments;

    public bool Pay(decimal amount)
    {
        _payments.Add(amount);
        return _result;
    }
}
