// Measures two ways of finding duplicates in an array.
// Start in Release mode:
//   dotnet run -c Release              measures n = 1 000 ... 1 000 000 (takes about two minutes)
//   dotnet run -c Release -- doubling  runs the doubling test
using System.Diagnostics;

int[] warmUp = CreateData(1_000);
for (int i = 0; i < 100; i++)
{
    HasDuplicateList(warmUp);
    HasDuplicateHashSet(warmUp);
}

if (args.Length > 0 && args[0] == "doubling")
{
    DoublingTest("List", HasDuplicateList, 10_000, 6);
    DoublingTest("HashSet", HasDuplicateHashSet, 1_000_000, 5);
}
else
{
    int[] sizes = [1_000, 10_000, 100_000, 1_000_000];
    Console.WriteLine($"{"n",10} {"List (ms)",12} {"HashSet (ms)",14}");
    foreach (int n in sizes)
    {
        int[] data = CreateData(n);
        // A single run of the List variant with one million numbers takes so long that noise does not matter.
        double listMs = Measure(() => HasDuplicateList(data), n < 1_000_000 ? 5 : 1);
        double setMs = Measure(() => HasDuplicateHashSet(data), 5);
        Console.WriteLine($"{n,10} {listMs,12:F3} {setMs,14:F3}");
    }
}

/// <summary>
/// Doubles n starting with <paramref name="startN"/> and prints the runtime and the ratio T(2n) / T(n).
/// </summary>
/// <param name="name">Name of the variant for the output.</param>
/// <param name="method">The method to measure.</param>
/// <param name="startN">The first input size.</param>
/// <param name="steps">The number of measurements.</param>
static void DoublingTest(string name, Func<int[], bool> method, int startN, int steps)
{
    Console.WriteLine($"{name}:");
    double previous = 0;
    for (int i = 0, n = startN; i < steps; i++, n *= 2)
    {
        int[] data = CreateData(n);
        double ms = Measure(() => method(data), 5);
        string ratio = previous > 0 ? $"{ms / previous:F2}" : "";
        Console.WriteLine($"{n,10} {ms,10:F2} ms {ratio,6}");
        previous = ms;
    }
}

/// <summary>
/// Runs <paramref name="action"/> several times and returns the fastest runtime in milliseconds.
/// </summary>
/// <param name="action">The code to measure. It must return false (test data without duplicates).</param>
/// <param name="repetitions">How many times the code is measured.</param>
/// <exception cref="InvalidOperationException">The action returned true.</exception>
static double Measure(Func<bool> action, int repetitions)
{
    double best = double.MaxValue;
    for (int i = 0; i < repetitions; i++)
    {
        // Collect garbage from previous runs so that the GC does not interrupt the measurement.
        GC.Collect();
        long start = Stopwatch.GetTimestamp();
        bool result = action();
        TimeSpan elapsed = Stopwatch.GetElapsedTime(start);
        // Using the result prevents the JIT from removing the call and checks the test data.
        if (result) { throw new InvalidOperationException("Test data must not contain duplicates."); }
        // Noise only makes a run slower, so the fastest run is the best estimate.
        best = Math.Min(best, elapsed.TotalMilliseconds);
    }
    return best;
}

/// <summary>
/// Creates the numbers 0 ... n - 1 in a random but reproducible order (no duplicates).
/// </summary>
/// <param name="n">The number of elements.</param>
static int[] CreateData(int n)
{
    int[] data = Enumerable.Range(0, n).ToArray();
    new Random(42).Shuffle(data);
    return data;
}

/// <summary>
/// Checks for duplicates by remembering all numbers seen so far in a list.
/// </summary>
/// <param name="data">The numbers to check.</param>
/// <returns>true if a number occurs at least twice.</returns>
static bool HasDuplicateList(int[] data)
{
    List<int> seen = [];
    foreach (int x in data)
    {
        if (seen.Contains(x)) { return true; }
        seen.Add(x);
    }
    return false;
}

/// <summary>
/// Checks for duplicates by remembering all numbers seen so far in a hash set.
/// </summary>
/// <param name="data">The numbers to check.</param>
/// <returns>true if a number occurs at least twice.</returns>
static bool HasDuplicateHashSet(int[] data)
{
    HashSet<int> seen = [];
    foreach (int x in data)
    {
        if (!seen.Add(x)) { return true; }
    }
    return false;
}
