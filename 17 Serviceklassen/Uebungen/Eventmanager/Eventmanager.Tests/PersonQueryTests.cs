using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eventmanager.Tests;

/// <summary>
/// LINQ on a list in memory (LINQ to Objects). The lambdas run in C#, no database is involved.
/// </summary>
public class PersonQueryTests
{
    private readonly List<Person> _persons = new()
    {
        new Person("Anna", "Huber", "Wien", 17),
        new Person("Ben", "Gruber", "Graz", 24),
        new Person("Clara", "Bauer", "Wien", 31),
        new Person("David", "Wagner", "Linz", 16),
        new Person("Emma", "Pichler", "Wien", 45),
    };

    [Fact]
    public void WhereAndSelect()
    {
        List<string> names = _persons
            .Where(p => p.City == "Wien")
            .Select(p => p.FirstName)
            .ToList();

        Assert.Equal(new[] { "Anna", "Clara", "Emma" }, names);
    }

    [Fact]
    public void AnyAndAll()
    {
        bool hasMinors = _persons.Any(p => p.Age < 18);
        bool allAdults = _persons.All(p => p.Age >= 18);
        // All on an empty sequence is true: there is no element that violates the condition.
        bool allAdultsInEmptyList = new List<Person>().All(p => p.Age >= 18);

        Assert.True(hasMinors);
        Assert.False(allAdults);
        Assert.True(allAdultsInEmptyList);
    }
}
