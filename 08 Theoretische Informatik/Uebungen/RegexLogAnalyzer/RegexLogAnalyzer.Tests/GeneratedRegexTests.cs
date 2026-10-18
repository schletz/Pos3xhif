using System.Reflection;
using System.Text.RegularExpressions;
using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Checks the requirement that every class uses <see cref="GeneratedRegexAttribute"/> with a timeout.
/// </summary>
public class GeneratedRegexTests
{
    private const int MaxTimeoutMilliseconds = 1000;

    [Theory]
    [InlineData(typeof(LogParser))]
    [InlineData(typeof(LicensePlateValidator))]
    [InlineData(typeof(IbanValidator))]
    [InlineData(typeof(EmailValidator))]
    public void Class_UsesGeneratedRegexWithTimeout(Type type)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var attributes = type.GetMembers(flags)
            .Select(m => m.GetCustomAttribute<GeneratedRegexAttribute>())
            .OfType<GeneratedRegexAttribute>()
            .ToList();

        Assert.True(attributes.Count > 0, $"{type.Name} declares no method or property with [GeneratedRegex].");
        foreach (var attribute in attributes)
        {
            Assert.True(
                attribute.MatchTimeoutMilliseconds is > 0 and <= MaxTimeoutMilliseconds,
                $"[GeneratedRegex(\"{attribute.Pattern}\")] in {type.Name} needs a timeout between 1 and " +
                $"{MaxTimeoutMilliseconds} ms (matchTimeoutMilliseconds), but has {attribute.MatchTimeoutMilliseconds}.");
        }
    }
}
