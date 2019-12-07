using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileReader
{
    // *********************************************************************************************
    // PROGRAMMKLASSE
    // *********************************************************************************************
    class Program
    {
        static void Main(string[] args)
        {
            Schueler[] schuelerliste = new Schueler[0];
            try
            {
                foreach (string filename in new string[] { "Schueler1.csv", "Schueler2.csv", "Schueler3.csv" })
                {
                    int i = 1;
                    Console.WriteLine($"BEISPIELDATEI {filename}");
                    using (TypedReader<Schueler> tr =
                        new TypedReader<Schueler>(filename, Encoding.UTF8) { Headings = true })
                    {
                        schuelerliste = tr.ReadAllLines();
                        i = 1;
                        schuelerliste
                            .ToList()
                            .ForEach(s => Console.WriteLine($"{i++,2} {s.Id,5} {s.Vorname,-20} {s.Zuname,-20} {s.Email,-30} {s.Gebdat:dd.MM.yyyy} {s.Notenschnitt:0.00}"));
                    }
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                if (e.InnerException != null) { Console.Error.WriteLine(e.InnerException.Message); }
            }
            Console.ReadLine();
        }
    }
}