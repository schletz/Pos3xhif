using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitAsyncDemo
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static void Main(string[] args)
        {
            Console.WriteLine($"Starte Programm in Thread ID {Thread.CurrentThread.ManagedThreadId}");

            MainAsync().Wait();    // Alternativ: Task.WaitAll(MainAsync());
            Console.WriteLine($"MAIN Thread ID {Thread.CurrentThread.ManagedThreadId}");
            Console.ReadLine();
        }

        /// <summary>
        /// Da die Main Methode nicht async sein kann, lagern wir unsere Beispiele in eine MainAsync
        /// aus. Ist auch logisch, das Programm beginnt in einem Thread zu laufen und kann dann nicht
        /// "irgendwo" beendet werden.
        /// </summary>
        /// <returns></returns>
        private static async Task MainAsync()
        {
            Console.WriteLine($"Starte MainAsync in Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // Häufiger Anwendungsfall: Laden von einer Webquelle. Es werden die Async Methoden von HttpClient
            // verwendet.
            string result = await client.GetStringAsync("https://www.google.com/search?q=c%23+better+than+java");
            Console.WriteLine($"    MainAsync01 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // 2. Anwendungsfall: Asynchrones Schreiben in eine Datei über die StreamWriter Klasse.
            // Natürlich kann auch asynchron gelesen werden.
            using (StreamWriter stream = new StreamWriter("googleResult.txt"))
            {
                await stream.WriteAsync(result);
            }
            Console.WriteLine($"    MainAsync02 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // Einmalige CPU intensive Arbeit wird in einen eigenen Task ausgelagert. Das Ergebnis wird
            // dann ausgewertet, wenn es zur Verfügung steht.
            double result2 = await HeavyWorkAsync(1000000);
            Console.WriteLine($"    MainAsync03 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // Arbeiten mit Task.Delay(), um periodische Vorgänge durchführen zu können.
            await PeriodicAsync(2, 100);
            Console.WriteLine($"    MainAsync04 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // DO:
            // Wenn gewartet werden soll, bis mehrere Methoden beendet sind, dann kann (und soll) in 
            // einer async Methode Task.WhenAll() verwendet werden.            
            await Task.WhenAll(PeriodicAsync(2, 100), HeavyWorkAsync(10000000));
            Console.WriteLine($"    MainAsync05 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // DON'T:
            // Hier wird dim selben Thread fortgesetzt. Die Funktion blockiert also bis zum Ende
            // bei der Methoden. Das macht nur Sinn, wenn in einer synchronen Methode synchronisiert
            // werden soll, und man sich bewusst ist, dass hier blockiert wird.
            Task.WaitAll(PeriodicAsync(2, 100), HeavyWorkAsync(10000000));
            Console.WriteLine($"    MainAsync06 Thread ID {Thread.CurrentThread.ManagedThreadId}");

            // DO:
            // Bei einer async Methode, die einen Task zurückliefert, können Exceptions gefangen werden.
            try { await PeriodicAsync(-1, 100); }
            catch (ArgumentException e) { Console.Error.WriteLine(e.Message); }

            // DON'T:
            // Auf eine async Methode, die void liefert, kann nicht gewartet werden. Daher wird der Fehler
            // nicht abgefangen. Schreibe daher nie void async Methoden außer bei Eventhandlern, was in
            // einer ViewModel Architektur selten ist! Probiere es aus, indem du die folgenden Anweisungen
            // aktivierst.
            // try { BadAdync(-1, 100); }
            // catch (ArgumentException e) { Console.Error.WriteLine(e.Message); }
        }

        /// <summary>
        /// Demonstriert das Auslagern CPU intensiver Arbeit mit Task.Run(), indem kryptografisch sichere
        /// Zufallszahlen generiert werden. Diese Methode braucht kein async keyword, da wir den Task
        /// von Task.Run direkt zurückliefern.
        /// DON'T:
        ///     - Die Methode async machen und vor Task.Run() await schreiben.
        ///     - Blockierende Anweisungen in Task.Run() schreiben.
        /// </summary>
        /// <param name="iterations">Anzahl der Zahlen, um die Dauer zu steuern.</param>
        /// <returns>Die Summe der Zahlen im Bereich von 0 - 255*iterations</returns>
        private static Task<double> HeavyWorkAsync(long iterations)
        {
            Console.WriteLine($"        Starte HeavyWork in Thread ID {Thread.CurrentThread.ManagedThreadId}");
            return Task.Run(() =>
            {
                Console.WriteLine($"        Starte HeavyWork Thread in Thread ID {Thread.CurrentThread.ManagedThreadId}");
                RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
                byte[] values = new byte[1];
                double result = 0;

                while (iterations-- != 0)
                {
                    rnd.GetBytes(values);
                    result += values[0];
                }
                return result;
            });
        }

        /// <summary>
        /// Gibt periodisch BEEP aus und demonstriert Task.Delay. Verwende NICHT Thread.Sleep(), diese
        /// Anweisung blockiert den Thread.
        /// </summary>
        /// <returns></returns>
        private static async Task PeriodicAsync(int count, int sleep)
        {
            if (count < 0) { throw new ArgumentException("Invalid count value."); }
            while (count-- != 0)
            {
                await Task.Delay(sleep);
                Console.WriteLine($"        Periodic Thread ID {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("            BEEP!");
            }
        }
        /// <summary>
        /// DON'T: Async Methoden, die void Liefern. Auf sie kann nicht gewartet werden und Exceptions
        /// können nicht abgefangen werden.
        /// </summary>
        private static async void BadAdync(int count, int sleep)
        {
            if (count < 0) { throw new ArgumentException("Invalid count value."); }
            if (sleep < 0) { throw new ArgumentException("Invalid sleep value."); }
            while (count-- != 0)
            {
                await Task.Delay(sleep);
                Console.WriteLine($"        Periodic Thread ID {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("            BEEP!");
            }
        }
    }
}
