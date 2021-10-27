using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Füge mit
///     using TestHelpers;
///     using static TestHelpers.ProgramChecker;
/// die Extension- und Helper Methoden zur Datei Program.cs hinzu.
/// </summary>
namespace TestHelpers
{
    /// <summary>
    /// Erweitert die Typeklasse fürdie Prüfung der Implementierung einer Klasse
    /// </summary>
    public static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type) =>
            type.GetConstructor(Type.EmptyTypes) is not null;

        public static bool PropertyHasType<Ttarget>(this Type type, string propertyName) =>
            type.GetProperty(propertyName)?.PropertyType == typeof(Ttarget);

        public static bool IsAbstractProperty(this Type type, string propertyName) =>
            type.GetProperty(propertyName)?.GetMethod?.IsAbstract == true;

        public static bool IsImmutable(this Type type)
        {
            var properties = type.GetProperties();
            if (!properties.Any()) { return false; }
            return type.GetProperties().All(p => p.CanWrite == false);
        }
    }

    /// <summary>
    /// Fügt JsonEquals für LINQ Übungen hinzu. Aufruf: x.JsonEquals(other). Erweitert jedes
    /// object.
    /// </summary>
    public static class JsonElementExtensions
    {
        private static int CalculateHash(this JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.Number => (int)(element.GetDecimal() * 1000),
            JsonValueKind.String => element.GetString()!.GetHashCode(),
            JsonValueKind.Array => element
                .EnumerateArray()
                .Aggregate(0, (prev, current) => prev ^ current.CalculateHash()),
            JsonValueKind.Object => element
                .EnumerateObject()
                .Aggregate(0,
                    (prev, current) => prev ^ HashCode.Combine(current.Name.GetHashCode(), current.Value.CalculateHash())),
            _ => (int)element.ValueKind
        };

        private static JsonDocument ToJsonDocument(this object obj) => obj switch
        {
            JsonDocument doc => doc,
            string str => JsonDocument.Parse(str),
            object o => JsonDocument.Parse(JsonSerializer.Serialize(o))
        };

        public static bool JsonEquals(this JsonElement element, JsonElement other) =>
            element.CalculateHash() == other.CalculateHash();

        public static bool JsonEquals(this object obj, JsonElement other)
        {
            using var doc1 = obj.ToJsonDocument();
            return doc1.RootElement.JsonEquals(other);
        }

        public static bool JsonEquals(this object obj, object other)
        {
            using var doc1 = obj.ToJsonDocument();
            using var doc2 = other.ToJsonDocument();
            return doc1.RootElement.JsonEquals(doc2.RootElement);
        }

        public static void WriteJson(this object data, string title = "", bool indent = false)
        {
            string result = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions() { WriteIndented = indent });
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            if (!string.IsNullOrEmpty(title)) { Console.Write(title); Console.Write(" "); }
            Console.ForegroundColor = oldColor;
            Console.WriteLine(result);
        }
    }

    /// <summary>
    /// Implementiert die Punktezählung und Helper Methoden für die Main Methode.
    /// </summary>
    public static class ProgramChecker
    {
        private static int _testCount = 0;
        private static int _testsSucceeded = 0;
        private static int _points = 0;
        private static int _pointsMax = 0;

        private static int Grade => _pointsMax > 0
            ? Math.Min(5, 9 - (int)Math.Ceiling(8M * _points / _pointsMax))
            : 0;

        private static readonly string[] Grades = new string[] { string.Empty, "Sehr gut", "Gut", "Befriedigend", "Genügend", "Nicht genügend" };

        public static void CheckAndWrite(Func<bool> predicate, string message, int weight = 1)
        {
            _testCount++;
            _pointsMax += weight;
            if (predicate())
            {
                Console.WriteLine($"   ({_testCount}) OK: {message}");
                _testsSucceeded++;
                _points += weight;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   ({_testCount}) Nicht erfüllt: {message}");
            Console.ResetColor();
        }

        public static void CheckJsonAndWrite(object? obj1, JsonElement element, string message, int weight = 1)
        {
            if (obj1 is null)
            {
                CheckAndWrite(() => false, message, weight);
                return;
            }
            var equals = obj1.JsonEquals(element);
            CheckAndWrite(() => equals, message, weight);
            if (!equals)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Geliefertes Ergebnis: ");
                obj1.WriteJson(string.Empty, true);
                Console.Write("   Korrektes Ergebnis: ");
                Console.WriteLine(element);
                Console.ResetColor();
            }
        }

        public static void WriteSummary()
        {
            Console.WriteLine();
            Console.WriteLine($"Ergebnis des Programmes in {System.IO.Directory.GetCurrentDirectory()}:");
            Console.WriteLine($"{_testsSucceeded} von {_testCount} Tests erfüllt.");
            if (Grade == 0)
            {
                Console.WriteLine($"{_points} von {_pointsMax} Punkte erreicht.");
                return;
            }
            Console.ForegroundColor = Grade == 5 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"{_points} von {_pointsMax} Punkte erreicht. Das ist die Note {Grades[Grade]}.");
            Console.ResetColor();
        }
    }
}