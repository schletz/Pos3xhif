using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeDemoApp
{
    /// <summary>
    /// Demoprogramm, welches alle Primzahlen zwischen X und X+n ausgibt.
    /// </summary>
    class Program
    {
        // Startwert, ab dem wir prüfen sollen, ob eine Zahl prim ist. Diese Größe kann an die
        // CPU Geschwindigkeit angepasst werden.
        const int START_NUMBER = 500;      // Testwerte: 0 und 500000000
        // Ab dem Startwert werden so viele Zahlen auf ihre Primzahleigenschaft geprüft.
        const int NUMBER_COUNT = 100;
        static void Main(string[] args)
        {
            int runningTasks = 0;
            List<int> primes;

            Console.WriteLine($"SUCHE DIE PRIMZAHLEN ZWISCHEN {START_NUMBER} und {START_NUMBER + NUMBER_COUNT}...");
            primes = new List<int>();
            DateTime start = DateTime.UtcNow;
            Console.WriteLine("SYNCHRONES ERMITTELN DER PRIMZAHLEN");
            // Wie gewohnt eine Zahl nach der Anderen prüfen.
            for (int number = START_NUMBER; number < START_NUMBER + NUMBER_COUNT; number++)
            {
                if (IsPrime(number))
                    primes.Add(number);
            }
            Console.WriteLine($"{primes.Count} Zahlen gefunden: {String.Join(",", primes)}");
            DateTime syncEnd = DateTime.UtcNow;

            Console.WriteLine("PARALLELES ERMITTELN DER PRIMZAHLEN");
            primes = new List<int>();
            Console.WriteLine($"{Environment.ProcessorCount} Prozessoren");
            Console.Write($"Laufende Tasks:");
            // Die statische Klasse Parallel stellt For und ForEach bereit. Diese Methoden machen folgendes:
            // Es wird für jeden Schleifendurchlauf ein Task erstellt.
            // Der Tash Scheduler führt nur so viele Tasks gleichzeitis aus, wie es sinnvoll ist (CPU Kerne)
            // Es wird gewartet, bis alle Tasks beendet wurden, sodass nachher die Ergebnisse verfügbar
            // sind.
            Parallel.For(START_NUMBER, START_NUMBER + NUMBER_COUNT,
                (number) =>
                {
                    // Da dieser Code parallel ausgeführt wird, können wir nicht so einfach schreibend
                    // auf gemeinsame Variablen zugreifen. Das Gefährlich daran ist, dass es keinen Fehler
                    // liefern würde, aber der Wert kann falsch sein!
                    // Interlocked stellt mit der Add Methode einen atomaren Zugriff bereit. Hier wird
                    // die Variable mit einer CPU Instruktion verändert, dadurch kann nicht zwischen 
                    // Auslesen, erhöhen und reinschreiben ein anderer Thread die Variable ändern.
                    Interlocked.Add(ref runningTasks, 1);
                    Console.Write($" {runningTasks}");
                    if (IsPrime(number))
                        // Beim Zugriff auf eine Liste muss ebenfalls Sorge getragen werden, dass nicht 
                        // 2 Threads gleichzeitig hineinschreiben. Auch hier gilt wieder: es kann ohne lock
                        // gut gehen, aber je nach Eingabedaten gibt es dann Probleme.
                        lock (primes) { primes.Add(number); }
                    Interlocked.Add(ref runningTasks, -1);

                });
            Console.WriteLine($"{Environment.NewLine}{primes.Count} Zahlen gefunden: {String.Join(",", primes)}");
            DateTime parallelEnd = DateTime.UtcNow;

            double syncDuration = (syncEnd - start).TotalSeconds;
            double parallelDuration = (parallelEnd - syncEnd).TotalSeconds;
            Console.WriteLine($"Synchron: {syncDuration:0.00}s, Parallel: {parallelDuration:0.00}s (Faktor {parallelDuration / syncDuration:0.00})");

            Console.ReadLine();
        }

        /// <summary>
        /// Prüft, ob eine übergebene Zahl eine Primzahl ist. Der Algorithmus soll die CPU auslasten,
        /// daher wurde er auch nicht optimiert (prüfen nur bis Math.Sqrt(x), speichern gefundener
        /// Primzahlen.
        /// </summary>
        /// <param name="number">Zu prüfende Zahl.</param>
        /// <returns></returns>
        static bool IsPrime(int number)
        {
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
