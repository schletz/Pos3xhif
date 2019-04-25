using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDemoApp
{
    class Program
    {
        static readonly List<long> results = new List<long>();
        static readonly object lockObj = new object();  // "ID" des LOCK Blockes = Referenzadresse
        // Das geht NICHT, denn Random ist nicht thread safe. 
        // static readonly Random rnd = new Random();
        static void Main(string[] args)
        {
            List<int> numbers = Enumerable.Range(1, 10).ToList();

            // VARIANTE 1: SYNCHRON
            DateTime start = DateTime.UtcNow;
            foreach(int i  in numbers) { DoWork("file.txt"); }
            DateTime end = DateTime.UtcNow;
            Console.WriteLine($"FERTIG in {(end - start).TotalMilliseconds} ms");

            // VARIANTE 2: ASYNCHRON
            start = DateTime.UtcNow;
            Parallel.ForEach(numbers,
                (i) =>
                {
                    DoWork("file.txt");
                });
            end = DateTime.UtcNow;
            // ACHTUNG: Die Werte sind manchmal gleich, da das Seed durch das gleichzeitige Starten
            //          auch ident ist.
            Console.WriteLine($"FERTIG in {(end - start).TotalMilliseconds} ms");
            results.OrderBy(i => i).ToList().ForEach((i) => Console.WriteLine(i));
            Console.ReadKey();
        }
        static void DoWork(string filename)
        {
            // Lokale Variable rnd, da Random nicht thread safe ist und wir daher keine gemeinsame
            // Instanz machen können.
            Random rnd = new Random();
            long sum = 0;
            for (int i = 0; i < 100e6; i++)
            {
                sum += rnd.Next();
            }

            // Wir schreiben etwas in eine Liste.
            // VORSICHT: DAS DARF NICHT SEIN, weil mehrere Tasks dürfen nicht
            // unkontrolliert gleichzeitig in Speicherbereiche schreiben.            
            // results.Add(sum);
            lock (lockObj)
            {
                results.Add(sum);
            }
        }
    }
}
