using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace LottoTipp.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() =>
                    typeof(LottoTipp).GetFields(BindingFlags.Public | BindingFlags.Instance).Length == 0
                    && typeof(LottoTipp).GetProperties().Length == 1,
                    "LottoTipp has no public fields and TippCount is the only property.");
                CheckAndWrite(() =>
                {
                    var lottoTipp = new LottoTipp();
                    int countAtStart = lottoTipp.TippCount;
                    lottoTipp.AddQuicktipps(3);
                    int countAfterFirstAdd = lottoTipp.TippCount;
                    lottoTipp.AddQuicktipps(2);
                    return typeof(LottoTipp).IsReadonlyProperty(nameof(LottoTipp.TippCount))
                        && countAtStart == 0 && countAfterFirstAdd == 3 && lottoTipp.TippCount == 5;
                }, "TippCount is read-only and returns the number of stored tipps.");
            }

            {
                Console.WriteLine("Checking quicktipps");
                CheckAndWrite(() =>
                {
                    var lottoTipp = new LottoTipp();
                    lottoTipp.AddQuicktipps(1000);
                    for (int i = 0; i < 1000; i++)
                    {
                        var numbers = new HashSet<int>(lottoTipp.GetTipp(i));
                        if (numbers.Count != 6) { return false; }
                        foreach (int number in numbers)
                        {
                            if (number < 1 || number > 45) { return false; }
                        }
                    }
                    return true;
                }, "1000 tipps have 6 different numbers between 1 and 45.", weight: 2);
                CheckAndWrite(() =>
                {
                    var lottoTipp = new LottoTipp();
                    lottoTipp.AddQuicktipps(5);
                    return Sorted(lottoTipp.GetTipp(0)) == "2 3 12 14 30 43"
                        && Sorted(lottoTipp.GetTipp(1)) == "3 17 35 36 39 44"
                        && Sorted(lottoTipp.GetTipp(2)) == "8 21 32 33 37 39"
                        && Sorted(lottoTipp.GetTipp(3)) == "5 6 9 10 23 25"
                        && Sorted(lottoTipp.GetTipp(4)) == "3 12 30 34 36 40";
                }, "The first 5 tipps match the numbers of the fixed seed 906.", weight: 2);
                CheckAndWrite(() =>
                {
                    var lottoTipp = new LottoTipp();
                    lottoTipp.AddQuicktipps(1);
                    int[] tipp = lottoTipp.GetTipp(0);
                    string numbersBefore = Sorted(tipp);
                    tipp[0] = 99;
                    return Sorted(lottoTipp.GetTipp(0)) == numbersBefore;
                }, "Changing the array of GetTipp does not change the stored tipp.");
                CheckAndWrite(() =>
                {
                    var lottoTipp = new LottoTipp();
                    lottoTipp.AddQuicktipps(1);
                    // The first tipp of the seed 906 is 2 3 12 14 30 43.
                    return lottoTipp.CheckTipp(0, [2, 3, 12, 14, 30, 43]) == 6
                        && lottoTipp.CheckTipp(0, [43, 1, 2, 4, 5, 6]) == 2
                        && lottoTipp.CheckTipp(0, [1, 4, 5, 6, 7, 8]) == 0;
                }, "CheckTipp returns the number of right numbers.", weight: 2);
            }

            {
                Console.WriteLine("Checking 1 000 000 tipps");
                long usedMemory = GC.GetTotalMemory(forceFullCollection: true);
                var stopwatch = Stopwatch.StartNew();
                var lottoTipp = new LottoTipp();
                lottoTipp.AddQuicktipps(1_000_000);
                int[] drawnNumbers = [4, 2, 1, 8, 32, 16];
                int fiveRight = 0;
                var sixRight = new List<int>();
                for (int i = 0; i < lottoTipp.TippCount; i++)
                {
                    int rightNumbers = lottoTipp.CheckTipp(i, drawnNumbers);
                    if (rightNumbers == 5) { fiveRight++; }
                    if (rightNumbers == 6) { sixRight.Add(i); }
                }
                stopwatch.Stop();
                CheckAndWrite(() =>
                    lottoTipp.TippCount == 1_000_000 && fiveRight == 26
                    && sixRight.Count == 1 && sixRight[0] == 499_182,
                    "1 000 000 tipps have 26 tipps with 5 and tipp 499 182 with 6 right numbers.", weight: 2);
                Console.WriteLine($"   Info: {stopwatch.ElapsedMilliseconds} ms, "
                    + $"{(GC.GetTotalMemory(forceFullCollection: true) - usedMemory) / 1048576M:0.00} MBytes used.");
                GC.KeepAlive(lottoTipp);
            }
            WriteSummary();
        }

        /// <summary>
        /// Returns the numbers in ascending order, separated by a space. The order of the numbers
        /// in a tipp does not matter, so the checks compare the sorted numbers.
        /// </summary>
        private static string Sorted(int[] numbers)
        {
            var copy = (int[])numbers.Clone();
            Array.Sort(copy);
            return string.Join(" ", copy);
        }
    }
}
