using System;

namespace SPG.LambdaTutorial
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Eine Liste von Schülern wird erstellt und miottels initializer befüllt.
            SchuelerList schuelers = new SchuelerList();
            schuelers.Add(new Schueler() { Id = 1, Nachame = "Muster1", Vorname = "Max1", Klasse = "3AHIF" });
            schuelers.Add(new Schueler() { Id = 2, Nachame = "Muster2", Vorname = "Max2", Klasse = "3AHIF" });
            schuelers.Add(new Schueler() { Id = 3, Nachame = "Muster3", Vorname = "Max3", Klasse = "3AHIF" });
            schuelers.Add(new Schueler() { Id = 4, Nachame = "Muster4", Vorname = "Max4", Klasse = "4AHIF" });
            schuelers.Add(new Schueler() { Id = 5, Nachame = "Muster5", Vorname = "Max5", Klasse = "5AHIF" });

            SchuelerList result = null;

            //TODO: Implementierung!

            // Ausgabe
            foreach (Schueler s in result)
            {
                Console.WriteLine($"{s.Id}: {s.Vorname} {s.Nachame} ({s.Klasse})");
            }
        }
    }
}
