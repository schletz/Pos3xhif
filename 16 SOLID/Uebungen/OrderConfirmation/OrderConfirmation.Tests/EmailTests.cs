using OrderConfirmation.Application;
using Xunit;

namespace OrderConfirmation.Tests;

public class EmailTests
{
    [Fact]
    public void Constructor_WithValues_SetsAllProperties()
    {
        Email email = new Email(to: "max@example.com", subject: "Hello", body: "Text");

        Assert.Equivalent(new { To = "max@example.com", Subject = "Hello", Body = "Text" }, email);
    }
}
