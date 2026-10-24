using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// Add
///     using TestHelpers;
///     using static TestHelpers.ProgramChecker;
/// to Program.cs to use the extension and helper methods.
/// </summary>
namespace TestHelpers
{
    /// <summary>
    /// Extends <see cref="Type"/> with checks for the structure of a class.
    /// </summary>
    public static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type) =>
            type.GetConstructor(Type.EmptyTypes) is not null;

        public static bool PropertyHasType<TTarget>(this Type type, string propertyName) =>
            type.GetProperty(propertyName)?.PropertyType == typeof(TTarget);

        public static bool HasProtectedConstructor(this Type type, params Type[] parameterTypes) =>
            type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, parameterTypes)?.IsFamily == true;

        public static bool IsAbstractProperty(this Type type, string propertyName) =>
            type.GetProperty(propertyName)?.GetMethod?.IsAbstract == true;

        /// <summary>
        /// True if the property exists and cannot be set from outside the class.
        /// A private setter is allowed, because only the class itself can use it.
        /// </summary>
        public static bool IsReadonlyProperty(this Type type, string propertyName) =>
            type.GetProperty(propertyName) is { } property && property.SetMethod?.IsPublic != true;

        /// <summary>
        /// True if the type has at least one property, no property can be set from outside
        /// and there is no public field that can be changed.
        /// </summary>
        public static bool IsImmutable(this Type type)
        {
            var properties = type.GetProperties();
            if (properties.Length == 0) { return false; }
            return properties.All(p => p.SetMethod?.IsPublic != true)
                && type.GetFields().All(f => f.IsInitOnly || f.IsLiteral);
        }
    }

    /// <summary>
    /// Counts the points and prints the results of the checks in the Main method.
    /// </summary>
    public static class ProgramChecker
    {
        private static int _testCount = 0;
        private static int _testsSucceeded = 0;
        private static int _points = 0;
        private static int _pointsMax = 0;

        /// <summary>
        /// Austrian grading scale. The first matching limit wins: 87.5 % Sehr gut, 75 % Gut,
        /// 62.5 % Befriedigend, 50 % Genügend, below Nicht genügend.
        /// </summary>
        private static string Grade
        {
            get
            {
                // Integer comparison (8 * points >= 7 * max means points / max >= 87.5 %),
                // so a result exactly on a limit is never lost by rounding.
                if (8 * _points >= 7 * _pointsMax) { return "Sehr gut"; }
                if (4 * _points >= 3 * _pointsMax) { return "Gut"; }
                if (8 * _points >= 5 * _pointsMax) { return "Befriedigend"; }
                if (2 * _points >= _pointsMax) { return "Genügend"; }
                return "Nicht genügend";
            }
        }

        /// <summary>
        /// Runs the check and prints OK or FAILED. An exception in the check counts as failed,
        /// so the following checks still run.
        /// </summary>
        public static void CheckAndWrite(Func<bool> predicate, string message, int weight = 1)
        {
            _testCount++;
            _pointsMax += weight;
            bool succeeded;
            string error = string.Empty;
            try
            {
                succeeded = predicate();
            }
            catch (Exception e)
            {
                succeeded = false;
                error = $" ({e.GetType().Name}: {e.Message})";
            }
            if (succeeded)
            {
                Console.WriteLine($"   ({_testCount}) OK: {message}");
                _testsSucceeded++;
                _points += weight;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   ({_testCount}) FAILED: {message}{error}");
            Console.ResetColor();
        }

        public static void WriteSummary()
        {
            Console.WriteLine();
            Console.WriteLine($"Result of the program in {System.IO.Directory.GetCurrentDirectory()}:");
            Console.WriteLine($"{_testsSucceeded} of {_testCount} checks passed.");
            string grade = Grade;
            Console.ForegroundColor = grade == "Nicht genügend" ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"{_points} of {_pointsMax} points. Grade: {grade}.");
            Console.ResetColor();
        }
    }
}
