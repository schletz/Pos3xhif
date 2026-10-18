using System.Collections.Generic;
using SmsGateway.Application;
using Xunit;

namespace SmsGateway.Tests;

public class SmsDispatcherDispatchTests
{
    private static SmsDispatcher CreateDispatcher()
    {
        return new SmsDispatcher(new List<string> { "PizzaPronto", "Hausarzt24" });
    }

    [Fact]
    public void Dispatch_ValidLine_SendsMessage()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;Your pizza is on the way." });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal("PizzaPronto", message.Sender);
        Assert.Equal("+436641234567", message.Recipient);
        Assert.Equal("Your pizza is on the way.", message.Text);
        Assert.Equal(new List<string> { "Your pizza is on the way." }, message.Parts);
    }

    [Fact]
    public void Dispatch_SpacesAroundSenderAndRecipient_TrimsBoth()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { " PizzaPronto ; +436641234567 ;Hello" });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal("PizzaPronto", message.Sender);
        Assert.Equal("+436641234567", message.Recipient);
    }

    [Fact]
    public void Dispatch_TextContainsSemicolon_KeepsSemicolonInText()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "Hausarzt24;+436769876543;Appointment: Monday; 9:00" });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal("Appointment: Monday; 9:00", message.Text);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(160, 1)]
    [InlineData(161, 2)]
    [InlineData(306, 2)]
    [InlineData(307, 3)]
    [InlineData(459, 3)]
    public void Dispatch_TextLength_SendsExpectedNumberOfParts(int textLength, int expectedParts)
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;" + new string('x', textLength) });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal(expectedParts, message.Parts.Count);
    }

    [Fact]
    public void Dispatch_TextWith200Characters_SplitsAfter153Characters()
    {
        SmsDispatcher dispatcher = CreateDispatcher();
        string text = new string('a', 153) + new string('b', 47);

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;" + text });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal(new List<string> { new string('a', 153), new string('b', 47) }, message.Parts);
        Assert.Equal(text, message.Text);
    }

    [Fact]
    public void Dispatch_TextWith460Characters_ReportsError()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;" + new string('x', 460) });

        Assert.Empty(dispatcher.SentMessages);
        Assert.Equal(new List<string> { "Line 1: text is longer than 459 characters." }, dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_UnregisteredSender_BlocksMessage()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "GewinnJetzt;+436601112223;You won! Click here." });

        Assert.Empty(dispatcher.SentMessages);
        SmsMessage blocked = Assert.Single(dispatcher.BlockedMessages);
        Assert.Equal("GewinnJetzt", blocked.Sender);
        Assert.Empty(dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_SenderInOtherCase_SendsMessage()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PIZZAPRONTO;+436641234567;Your pizza is on the way." });

        SmsMessage message = Assert.Single(dispatcher.SentMessages);
        Assert.Equal("PIZZAPRONTO", message.Sender);
    }

    [Theory]
    [InlineData("PizzaPronto;+436641234567")]
    [InlineData("PizzaPronto")]
    public void Dispatch_TooFewFields_ReportsError(string line)
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { line });

        Assert.Equal(new List<string> { "Line 1: expected 3 fields separated by ';'." }, dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_EmptySender_ReportsError()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { " ;+436641234567;Hello" });

        Assert.Equal(new List<string> { "Line 1: sender is missing." }, dispatcher.Errors);
    }

    [Theory]
    [InlineData("+4312345678")]
    [InlineData("+43123456789012")]
    public void Dispatch_RecipientAtLengthLimit_SendsMessage(string recipient)
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;" + recipient + ";Hello" });

        Assert.Single(dispatcher.SentMessages);
    }

    [Theory]
    [InlineData("06641234567")]
    [InlineData("+431234567")]
    [InlineData("+431234567890123")]
    [InlineData("+43 664 1234567")]
    [InlineData("+4915112345678")]
    [InlineData("+43664123456a")]
    public void Dispatch_InvalidRecipient_ReportsError(string recipient)
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;" + recipient + ";Hello" });

        Assert.Empty(dispatcher.SentMessages);
        Assert.Equal(new List<string> { "Line 1: invalid recipient '" + recipient + "'." }, dispatcher.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Dispatch_EmptyText_ReportsError(string text)
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;" + text });

        Assert.Equal(new List<string> { "Line 1: text is empty." }, dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_InvalidLineOfUnregisteredSender_ReportsErrorAndDoesNotBlock()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "GewinnJetzt;12345;You won!" });

        Assert.Empty(dispatcher.BlockedMessages);
        Assert.Equal(new List<string> { "Line 1: invalid recipient '12345'." }, dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_BlankLines_AreSkippedButCounted()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string>
        {
            "PizzaPronto;+436641234567;Hello",
            "",
            "   ",
            "PizzaPronto;+436641234567;"
        });

        Assert.Single(dispatcher.SentMessages);
        Assert.Equal(new List<string> { "Line 4: text is empty." }, dispatcher.Errors);
    }

    [Fact]
    public void Dispatch_CalledTwice_KeepsResultsAndRestartsLineNumbers()
    {
        SmsDispatcher dispatcher = CreateDispatcher();

        dispatcher.Dispatch(new List<string> { "PizzaPronto;+436641234567;Hello", "PizzaPronto" });
        dispatcher.Dispatch(new List<string> { "Hausarzt24" });

        Assert.Single(dispatcher.SentMessages);
        Assert.Equal(
            new List<string>
            {
                "Line 2: expected 3 fields separated by ';'.",
                "Line 1: expected 3 fields separated by ';'."
            },
            dispatcher.Errors);
    }
}
