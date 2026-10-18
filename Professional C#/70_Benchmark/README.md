# Benchmarks mit Benchmark.NET

Das Paket *BenchmarkDotNet* ist ein bekanntes Paket im .NET Bereich um Laufzeitmessungen
durchzuführen. Es steht auf https://www.nuget.org/packages/BenchmarkDotNet/ über NuGet zur Verfügung.

## Anlegen eines Benchmark Projektes

Benchmarks werden in Konsolenapplikationen durchgeführt. Erstelle daher ein neues Projekt mit den
folgenden Anweisungen:

```text
dotnet new console
dotnet add package BenchmarkDotNet --version 0.*
```

## Schreiben von Benchmarks

Kern des Paketes ist die Klasse *BenchmarkRunner* mit der Methode *Run()*. Als Typparameter wird
die zu testende Klasse übergeben. Die Run Methode sucht nach Methoden, die die Annotation
*[Benchmark]* besitzen und testst diese. Dabei wird die Methode mehrere male Aufgerufen, um ein
statistisches Profil zu bekommen. Das Ergebnis wird dann auf der Konsole ausgegeben:

```text
 |                  Method |     Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
 |------------------------ |---------:|----------:|----------:|------:|------:|------:|----------:|
 |     IsPrimeBadBenchmark | 3.989 us | 0.0076 us | 0.0067 us |     - |     - |     - |         - |
 | IsPrimeBetterBenchmark2 | 3.051 us | 0.0177 us | 0.0166 us |     - |     - |     - |         - |
```

### Aufuf des Programmes

Benchmarks können nur im Release Build durchgeführt werden. Daher muss das Projekt im entsprechenden
Ordner mit

```text
dotnet run -c Release
```

in der Konsole gestartet werden.

### Codebeispiel

Das folgende Beispiel soll eine vorhandene Implementierung in der Klasse *PrimeAnalyzer* testen.
Diese Klasse ist auch in dieser Datei, in der Regel wird aber über Projektreferenzen auf Klassen
anderer Projekte verwiesen.

Dann wird eine Klasse *PrimeAnalyzerBenchmarks* erstellt. Diese Klasse soll die Daten für den
Benchmark vorbereiten und implementiert die tatsächlich zu messende Methode.

```c#
// BENCHMARK DEMO
// Jede der Methoden in PrimeAnalyzerBenchmarks wird gemessen. Dabei wird die Methode vom
// Framework mehrere Male aufgerufen um Abweichungen rauszumitteln.
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// MAINMETHODE. Ruft die Benchmarkklasse auf.
BenchmarkRunner.Run<PrimeAnalyzerBenchmarks>();

/// <summary>
/// Benchmark Klasse. Implementiert die Methoden, die gemessen werden sollen.
/// Meist werden Sie Methoden einer Implementierung 1:1 aufrufen.
/// </summary>
[MemoryDiagnoser]
public class PrimeAnalyzerBenchmarks
{
    private const int COUNT = 100;
    private readonly int[] _numbers = new int[COUNT];
    /// <summary>
    /// Konstruktor. Wird vor dem Testlauf jeder Methode aufgerufen,
    /// aber nicht vor jeder Iteration. In diesem Fall also 2x
    /// (vor IsPrimeBadBenchmark und IsPrimeBetterBenchmark2)
    /// </summary>
    public PrimeAnalyzerBenchmarks()
    {
        var rnd = new Random(2028);         // Fixes Seed, damit immer die gleiche Zahlenfolge generiert wird (Tests müssen deterministisch sein)
        for(int i = 0; i < COUNT; i++)
        {
            _numbers[i] = rnd.Next();
        }
    }

    /// <summary>
    /// Misst die Laufzeit für die Verifizierung von COUNT Zahlen. Das Ergebnis ist die Laufzeit
    /// der Methode IsPrimeBadBenchmark im Gesamten.
    /// </summary>
    [Benchmark]
    public void IsPrimeBadBenchmark()
    {
        for(int i = 0; i<COUNT; i++)
        {
            PrimeAnalyzer.IsPrimeBad(i);
        }
    }
    [Benchmark]
    public void IsPrimeBetterBenchmark2()
    {
        for (int i = 0; i < COUNT; i++)
        {
            PrimeAnalyzer.IsPrimeBetter(i);
        }
    }
}


/// <summary>
/// Implementierung, dies kann natürlich jede Klasse sein die gemessen werden
/// soll.
/// </summary>
static class PrimeAnalyzer
{
    public static bool IsPrimeBad(int number)
    {
        for (int i = 2; i < number; i++)
        {
            if (number % i == 0) { return true; }
        }
        return false;
    }

    public static bool IsPrimeBetter(int number)
    {
        for(int i = (int) Math.Sqrt(number); i >= 2; i--)
        {
            if (number % i == 0) { return true; }
        }
        return false;
    }
}
```