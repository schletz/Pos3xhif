using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Füge mit
///     using TestHelpers;
///     using static TestHelpers.ProgramChecker;
/// die Extension- und Helper Methoden zur Datei Program.cs hinzu.
/// </summary>
namespace TestHelpers
{
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
                Console.WriteLine($"   {_testCount} OK: {message}");
                _testsSucceeded++;
                _points += weight;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   {_testCount} Nicht erfüllt: {message}");
            Console.ResetColor();
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