using System;
using System.Collections.Generic;
using SmsGateway.Application;
using Xunit;

namespace SmsGateway.Tests;

public class SmsDispatcherRegisterSenderTests
{
    [Fact]
    public void RegisterSender_NewSender_AllowsSending()
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string> { "PizzaPronto" });

        dispatcher.RegisterSender("Paketblitz");
        dispatcher.Dispatch(new List<string> { "Paketblitz;+436501234567;Your parcel arrives today." });

        Assert.Single(dispatcher.SentMessages);
        Assert.Empty(dispatcher.BlockedMessages);
    }

    [Fact]
    public void RegisterSender_SameNameInOtherCase_HasNoEffect()
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string> { "PizzaPronto" });

        dispatcher.RegisterSender("PIZZAPRONTO");
        dispatcher.Dispatch(new List<string> { "pizzapronto;+436641234567;Hello" });

        Assert.Single(dispatcher.SentMessages);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("Hausarzt24X")]
    public void RegisterSender_NameAtLengthLimit_IsAccepted(string senderName)
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string>());

        dispatcher.RegisterSender(senderName);
        dispatcher.Dispatch(new List<string> { senderName + ";+436641234567;Hello" });

        Assert.Single(dispatcher.SentMessages);
    }

    [Theory]
    [InlineData("AB", "Sender name 'AB' must have 3 to 11 characters.")]
    [InlineData("Hausarzt24XY", "Sender name 'Hausarzt24XY' must have 3 to 11 characters.")]
    [InlineData("Pizza Pronto", "Sender name 'Pizza Pronto' must have 3 to 11 characters.")]
    [InlineData("Pizza-24", "Sender name 'Pizza-24' may only contain letters and digits.")]
    public void RegisterSender_InvalidName_ThrowsArgumentException(string senderName, string expectedMessage)
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string>());

        ArgumentException ex = Assert.Throws<ArgumentException>(() => dispatcher.RegisterSender(senderName));

        Assert.StartsWith(expectedMessage, ex.Message);
    }

    [Fact]
    public void Constructor_InvalidSenderName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SmsDispatcher(new List<string> { "PizzaPronto", "X" }));
    }
}
