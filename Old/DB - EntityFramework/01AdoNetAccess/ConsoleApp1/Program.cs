using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstDbApp.Model;

namespace FirstDbApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connectionstring zur lokalen SQL Server Default Instance mit integrierter Security.
            // Statt (local) kann auch . geschrieben werden.
            string connection = "Server=(local);Database=WeatherDb;Trusted_Connection=True;";

            try
            {   
                // Using schließt die Verbindung nach dem Block.
                using (WeatherDb db = new WeatherDb(connection))
                {
                    Station found = db.GetStations().FirstOrDefault(s => s.S_ID == 1001);
                    Console.WriteLine($"Station 1001 is at {(found?.S_Location)}");
                    foreach (Measurement m in db.GetMeasurements(found.S_ID).Take(10))
                    {
                        Console.WriteLine($"{m.M_Date} {m.M_Temperature}°C");
                    }
                }
            }
            catch (Exception e )
            {
                Console.Error.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
