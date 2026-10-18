using Dfa.Application;
using static Dfa.Application.TrafficLightState;

namespace Dfa.Tests;

/// <summary>
/// Tests the traffic light automaton and the encoding of events.
/// </summary>
public class TrafficLightTests
{
    private readonly Dfa<TrafficLightState> _dfa = TrafficLight.Create();

    [Fact]
    public void EncodeMapsEveryEventToItsCharacter()
    {
        Assert.Equal("tfr", TrafficLight.Encode(TrafficLightEvent.Timer, TrafficLightEvent.Fault, TrafficLightEvent.Repair));
    }

    [Fact]
    public void EncodeOfNoEventsIsTheEmptyWord()
    {
        Assert.Equal("", TrafficLight.Encode());
    }

    [Fact]
    public void EncodeAcceptsAnyEnumerable()
    {
        List<TrafficLightEvent> events = [TrafficLightEvent.Timer, TrafficLightEvent.Timer, TrafficLightEvent.Fault];
        Assert.Equal("ttf", TrafficLight.Encode(events));
    }

    [Fact]
    public void EncodeOfUndefinedEventThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TrafficLight.Encode(TrafficLightEvent.Timer, (TrafficLightEvent)42));
    }

    [Fact]
    public void FullCycleGoesThroughAllPhases()
    {
        var word = TrafficLight.Encode(Enumerable.Repeat(TrafficLightEvent.Timer, 5));
        Assert.Equal([Red, RedYellow, Green, GreenBlinking, Yellow, Red], _dfa.Trace(word));
    }

    [Theory]
    [InlineData("")]            // the light starts red
    [InlineData("ttttt")]       // one full cycle
    [InlineData("tttttttttt")]  // two full cycles
    [InlineData("fr")]          // fault in red, then repair
    [InlineData("ttfr")]        // fault in green, then repair
    [InlineData("tfttfr")]      // timer and further faults are ignored while blinking
    [InlineData("frttttt")]     // after a repair the normal cycle starts again
    public void AcceptsValidSequencesEndingInRed(string word)
    {
        Assert.True(_dfa.Accepts(word), $"'{word}' is a valid event sequence that ends in red.");
    }

    [Theory]
    [InlineData("t")]           // red-yellow
    [InlineData("tt")]          // green
    [InlineData("tttt")]        // yellow
    [InlineData("f")]           // still blinking
    [InlineData("ttft")]        // still blinking
    [InlineData("frt")]         // red-yellow after the repair
    public void RejectsValidSequencesNotEndingInRed(string word)
    {
        Assert.False(_dfa.Accepts(word), $"'{word}' does not end in red.");
    }

    [Theory]
    [InlineData("r")]           // repair without fault
    [InlineData("ttr")]
    [InlineData("frr")]         // second repair
    [InlineData("x")]           // not an event
    [InlineData("T")]
    public void RejectsInvalidSequences(string word)
    {
        Assert.False(_dfa.Accepts(word), $"'{word}' contains an event that is not allowed.");
    }

    [Fact]
    public void FaultLeadsToYellowBlinkingFromEveryPhase()
    {
        Assert.Equal([Red, RedYellow, Green, YellowBlinking], _dfa.Trace("ttf"));
        Assert.Equal([Red, RedYellow, Green, GreenBlinking, Yellow, YellowBlinking], _dfa.Trace("ttttf"));
    }

    [Fact]
    public void TraceStopsAtRepairWithoutFault()
    {
        Assert.Equal([Red, RedYellow], _dfa.Trace("trtt"));
    }
}
