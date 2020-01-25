using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Bogus;                // PM Console: Install-Package Bogus
using Bogus.Extensions;     // Für .OrNull()
using BogusTest.Model;

namespace BogusTest
{
    static class IEnumerableExtensions
    {
        // Da Distinct basierend auf einzelnen Properties nicht geht, schreiben wir es uns selbst.
        // Wir gruppieren wir nach dem gewünschtem Property und wählen das erste Element pro Gruppe.
        // Über Type inference findet der Compiler selbst heraus, was Tsource und Tkey sein muss.
        public static IEnumerable<Tsource> Distinct<Tsource, Tkey>(this IEnumerable<Tsource> list,
                                                                   Func<Tsource, Tkey> keySelector) =>
            list.GroupBy(keySelector).Select(l => l.First());
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Hinweis: Drücke F12 über dem Wort Randomizer. Dann werden alle Features angezeigt,
            //          die der Generator beherrscht.
            //          Um die anderen Profile (Name, ...) anzusehen, besuche
            //          https://github.com/bchavez/Bogus#bogus-api-support

            // Damit wir deterministisch arbeiten (es werden immer die gleichen Daten generiert),
            // setzen wir ein fixes Seed.
            Randomizer.Seed = new Random(42);

            using (SempruefContext db = new SempruefContext())
            {
                // Erstellt eine neue SQLite Datenbank aus den Modelklassen heraus.
                db.Database.EnsureDeleted();                 // Naturlich nur für dieses Beispiel!!!
                db.Database.EnsureCreated();

                // Generator für Klassen
                Faker<Klasse> classGenerator = new Faker<Klasse>()
                    .RuleFor(k => k.K_Nr, f =>
                        f.Random.String2(1, "12345") +
                        f.Random.String2(1, "ABCDE") +
                        f.Random.CollectionItem(new string[] { "HIF", "HBGM" }));

                // Die Klassen könnten auch mehrmals vorkommen, da ein "Ziehen mit zurücklegen"
                // durchgeführt wird. Daher verwenden wir unsere Distinct Methode.
                List<Klasse> classes = classGenerator
                    .Generate(3)
                    .Distinct(c=>c.K_Nr)
                    .ToList();

                // Generator für Schüler. Die Klassenzuordnung wird über das Navigation Property
                // S_KlasseNavigation gesetzt. Verwende nicht den Fremdschlüssel direkt!
                Faker<Schueler> pupilGenerator = new Faker<Schueler>()
                    .RuleFor(s => s.S_Nr, f => 1000 + f.UniqueIndex)
                    .RuleFor(s => s.S_Geschl, f => f.Random.String2(1, "mw"))
                    // Mit (f, s) können wir nicht nur auf den Faker, sondern auch auf das bisher
                    // erstellte Objekt zugreifen.
                    .RuleFor(s => s.S_Vorname, (f, s) => f.Name.FirstName(s.S_Geschl == "m" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))
                    .RuleFor(s => s.S_Zuname, f => f.Name.LastName())
                    // Mit den Bogus.Extensions gibt es für jeden Typ eine OrNull Methode, die
                    // in n % der Fälle NULL liefert. Hier wird in 20% NULL als Gebdat geliefert.
                    .RuleFor(s => s.S_Gebdat, f => f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2002, 1, 1)).OrNull(f, 0.2f))
                    // Es wird eine Klasse zufällig ausgewählt.
                    .RuleFor(s => s.S_KlasseNavigation, f => f.Random.ListItem(classes));

                List<Schueler> pupils = pupilGenerator
                    .Generate(90)
                    .ToList();



                /* *********************************************************************************
                 * Auch möglich: Befüllen der Collection (also von der 1er auf die n Seite gehend)
                 * Wenn pro Klasse eine fixe Anzahl an Schüler zugeordnet werden soll, dann muss
                 * ausgehend von der Klasse die Liste der Schüler generiert werden. Dafür muss 
                 * allerdings etwas gezaubert werden:
                 * Zuerst muss
                 *     RuleFor(s => s.S_KlasseNavigation, f => f.Random.ListItem(classes));
                 * natürlich aus pupilGenerator entfernt werden.
                 * 
                 * Die Regel
                 *     RuleFor(k => k.Schueler, pupilsGenerator.Generate(10))
                 * kann allerdings nicht in classGenerator eingefügt werden, denn es werden immer
                 * die gleichen Schüler generiert, da beim Instanzieren schon die Liste erzeugt wird.
                 * Daher müssen wir die Regel für jede Klasse neu setzen.
                 * 
                 * Statt Generate(3) bei den Klassen muss Enumerable.Range(1, 3) und Generate()
                 * verwendet werden, da sonst auch die selbe Schülerliste 3x verwendet wird.
                 * 
                var classes =
                    Enumerable.Range(1, 3)              // Funktionale for Schleife
                    .Select(i => classGenerator         // Für jedes i eine Klasse
                        .RuleFor(k => k.Schueler, pupilGenerator.Generate(10))
                        .Generate())
                    .GroupBy(c => c.K_Nr).Select(g => g.First());
                 * *********************************************************************************
                 */

                // Die Schüler schreiben. Die Klassen werden durch die Navigation auch implizit
                // angelegt. Ein db.Klasse.AddRange(classes) stört aber nicht.
                db.Schueler.AddRange(pupils);
                db.SaveChanges();

                Console.WriteLine(
                    JsonSerializer.Serialize(
                        db.Klasse.Select(k => new { k.K_Nr, Count = k.Schueler.Count() })));
            }
        }
    }
}
