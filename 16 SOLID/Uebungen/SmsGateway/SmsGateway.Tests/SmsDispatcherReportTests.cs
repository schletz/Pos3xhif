using System.Collections.Generic;
using SmsGateway.Application;
using Xunit;

namespace SmsGateway.Tests;

public class SmsDispatcherReportTests
{
    /// <summary>
    /// 3 messages sent with 1 + 2 + 3 = 6 parts, 3 messages of 2 senders blocked, 2 invalid lines, 1 blank line.
    /// </summary>
    private static SmsDispatcher CreateDispatcherWithMixedInput()
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string> { "PizzaPronto", "Hausarzt24", "Paketblitz" });
        dispatcher.Dispatch(new List<string>
        {
            "PizzaPronto;+436641234567;Your pizza is on the way.",
            "Hausarzt24;+436769876543;" + new string('x', 200),
            "GewinnJetzt;+436601112223;You won! Click here.",
            "Paketblitz;0664 1234567;Your parcel arrives today.",
            "",
            "PizzaPronto;+436641234567",
            "paketblitz;+436501234567;" + new string('y', 400),
            "BankAlarm;+436601112224;Your account is locked.",
            "GewinnJetzt;+436601112225;Last chance!"
        });
        return dispatcher;
    }

    [Fact]
    public void CreateReport_MixedInput_ContainsCounters()
    {
        SmsDispatcher dispatcher = CreateDispatcherWithMixedInput();

        string report = dispatcher.CreateReport();

        Assert.Contains("Sent messages: 3", report);
        Assert.Contains("Sent parts: 6", report);
        Assert.Contains("Blocked messages: 3", report);
        Assert.Contains("Invalid lines: 2", report);
    }

    [Fact]
    public void CreateReport_MixedInput_ContainsTotalCostOfAllParts()
    {
        SmsDispatcher dispatcher = CreateDispatcherWithMixedInput();

        string report = dispatcher.CreateReport();

        Assert.Contains("Total cost: 0.54 EUR", report);
    }

    [Fact]
    public void CreateReport_MixedInput_ListsEveryBlockedSenderOnceSorted()
    {
        SmsDispatcher dispatcher = CreateDispatcherWithMixedInput();

        string report = dispatcher.CreateReport();

        Assert.Contains("Blocked senders: BankAlarm, GewinnJetzt", report);
    }

    [Fact]
    public void CreateReport_NothingDispatched_ReportsZeros()
    {
        SmsDispatcher dispatcher = new SmsDispatcher(new List<string> { "PizzaPronto" });

        string report = dispatcher.CreateReport();

        Assert.Contains("Sent messages: 0", report);
        Assert.Contains("Sent parts: 0", report);
        Assert.Contains("Blocked messages: 0", report);
        Assert.Contains("Invalid lines: 0", report);
        Assert.Contains("Total cost: 0.00 EUR", report);
        Assert.DoesNotContain("Blocked senders", report);
    }

    [Fact]
    public void CreateReport_MixedInput_StartsWithTitle()
    {
        SmsDispatcher dispatcher = CreateDispatcherWithMixedInput();

        string report = dispatcher.CreateReport();

        Assert.StartsWith("SMS dispatch report", report);
    }
}
