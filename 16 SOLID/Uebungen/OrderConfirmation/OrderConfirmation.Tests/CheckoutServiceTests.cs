using OrderConfirmation.Application;
using Xunit;

namespace OrderConfirmation.Tests;

public class CheckoutServiceTests
{
    // Successful payment

    [Fact]
    public void Checkout_WhenPaymentSucceeds_ReturnsTrue()
    {
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        bool success = checkoutService.Checkout(order);

        Assert.True(success);
    }

    [Fact]
    public void Checkout_WhenPaymentSucceeds_MarksOrderAsPaid()
    {
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        checkoutService.Checkout(order);

        Assert.True(order.IsPaid);
    }

    [Theory]
    [InlineData(250)]
    [InlineData(99)]
    public void Checkout_WhenPaymentSucceeds_PaysTotalAmountExactlyOnce(decimal totalAmount)
    {
        FakePaymentProvider paymentProvider = new FakePaymentProvider(result: true);
        CheckoutService checkoutService = new CheckoutService(paymentProvider, new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: totalAmount);

        checkoutService.Checkout(order);

        decimal payment = Assert.Single(paymentProvider.Payments);
        Assert.Equal(totalAmount, payment);
    }

    [Fact]
    public void Checkout_WhenPaymentSucceeds_SendsExactlyOneEmail()
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), emailSender);
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        checkoutService.Checkout(order);

        Assert.Single(emailSender.SentEmails);
    }

    [Theory]
    [InlineData("max@example.com")]
    [InlineData("anna@example.com")]
    public void Checkout_WhenPaymentSucceeds_SendsEmailToCustomer(string customerEmail)
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), emailSender);
        Order order = new Order(customerEmail: customerEmail, totalAmount: 250);

        checkoutService.Checkout(order);

        Email email = Assert.Single(emailSender.SentEmails);
        Assert.Equal(customerEmail, email.To);
    }

    [Fact]
    public void Checkout_WhenPaymentSucceeds_SendsEmailWithSubjectOrderConfirmation()
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), emailSender);
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        checkoutService.Checkout(order);

        Email email = Assert.Single(emailSender.SentEmails);
        Assert.Equal("Order confirmation", email.Subject);
    }

    [Theory]
    [InlineData(250, "250")]
    [InlineData(99, "99")]
    public void Checkout_WhenPaymentSucceeds_SendsEmailWithAmountInBody(decimal totalAmount, string expectedText)
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), emailSender);
        Order order = new Order(customerEmail: "max@example.com", totalAmount: totalAmount);

        checkoutService.Checkout(order);

        Email email = Assert.Single(emailSender.SentEmails);
        Assert.Contains(expectedText, email.Body);
    }

    // Declined payment

    [Fact]
    public void Checkout_WhenPaymentDeclined_ReturnsFalse()
    {
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: false), new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        bool success = checkoutService.Checkout(order);

        Assert.False(success);
    }

    [Fact]
    public void Checkout_WhenPaymentDeclined_DoesNotMarkOrderAsPaid()
    {
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: false), new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        checkoutService.Checkout(order);

        Assert.False(order.IsPaid);
    }

    [Fact]
    public void Checkout_WhenPaymentDeclined_SendsNoEmail()
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: false), emailSender);
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

        checkoutService.Checkout(order);

        Assert.Empty(emailSender.SentEmails);
    }

    // Order already paid

    [Fact]
    public void Checkout_WhenOrderAlreadyPaid_ReturnsFalse()
    {
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
        order.MarkAsPaid();

        bool success = checkoutService.Checkout(order);

        Assert.False(success);
    }

    [Fact]
    public void Checkout_WhenOrderAlreadyPaid_DoesNotCallPaymentProviderAgain()
    {
        FakePaymentProvider paymentProvider = new FakePaymentProvider(result: true);
        CheckoutService checkoutService = new CheckoutService(paymentProvider, new FakeEmailSender());
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
        order.MarkAsPaid();

        checkoutService.Checkout(order);

        Assert.Empty(paymentProvider.Payments);
    }

    [Fact]
    public void Checkout_WhenOrderAlreadyPaid_SendsNoFurtherEmail()
    {
        FakeEmailSender emailSender = new FakeEmailSender();
        CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: true), emailSender);
        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
        order.MarkAsPaid();

        checkoutService.Checkout(order);

        Assert.Empty(emailSender.SentEmails);
    }
}
