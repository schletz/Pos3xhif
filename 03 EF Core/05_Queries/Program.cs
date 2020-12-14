using Microsoft.EntityFrameworkCore;
using Queries.Model;
using System;
using System.Linq;
using System.Text.Json;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            // Am Ende des Blockes wird Dispose() aufgerufen und die Verbindung wird geschlossen.
            using (TestsContext context = new TestsContext())
            {
                // *********************************************************************************
                // LINQ Statements werden in SQL umgesetzt. Dabei ist es egal, ob die Query oder
                // Method Syntax verwendet wird.
                // SELECT "s"."C_ID" AS "Class", (
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p"
                //     WHERE "s"."C_ID" = "p"."P_Class") AS "Pupils"
                // FROM "Schoolclass" AS "s"
                // WHERE "s"."C_Department" = 'HIF'

                var result1 = context.Schoolclass
                    .Where(c => c.C_Department == "HIF")
                    .Select(c => new
                    {
                        Class = c.C_ID,
                        Pupils = c.Pupil.Count()
                    });
                Console.WriteLine(JsonSerializer.Serialize(result1));


                // *********************************************************************************
                // Es gibt keine Unterscheidung zwischen WHERE und HAVING, denn der Filter nach der
                // Anzahl wird als (korrespondierende) Unterabfrage übersetzt. Hier ist der 
                // Optimizer der Datenbank gefragt.
                // SELECT "s"."C_ID" AS "Class", (
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p"
                //     WHERE "s"."C_ID" = "p"."P_Class") AS "Pupils"
                // FROM "Schoolclass" AS "s"
                // WHERE("s"."C_Department" = 'HIF') AND((
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p0"
                //     WHERE "s"."C_ID" = "p0"."P_Class") > 30)
                var result2 = context.Schoolclass
                    .Where(c => c.C_Department == "HIF" && c.Pupil.Count() > 30)
                    .Select(c => new
                    {
                        Class = c.C_ID,
                        Pupils = c.Pupil.Count()
                    });
                Console.WriteLine(JsonSerializer.Serialize(result2));

                // *********************************************************************************
                // Wird die Abfrage in 2 Schritten definiert, liefert sie trotzdem das selbe SELECT.
                // result3a wird nämlich bei der Definition noch nicht ausgeführt.
                // SELECT "s"."C_ID" AS "Class", (
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p"
                //     WHERE "s"."C_ID" = "p"."P_Class") AS "Pupils"
                // FROM "Schoolclass" AS "s"
                // WHERE("s"."C_Department" = 'HIF') AND((
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p0"
                //     WHERE "s"."C_ID" = "p0"."P_Class") > 30)
                var result3a = context.Schoolclass.Where(c => c.C_Department == "HIF");
                var result3b = result3a
                                    .Where(c => c.Pupil.Count() > 30)
                                    .Select(c => new
                                    {
                                        Class = c.C_ID,
                                        PupilsCount = c.Pupil.Count()
                                    });
                Console.WriteLine(JsonSerializer.Serialize(result3b));

                // *********************************************************************************
                // Geben wir Navigations explizit zurück, so werden durch einen JOIN die 
                // entsprechenden Daten geladen.
                // SELECT "s"."C_ID", (
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p"
                //     WHERE "s"."C_ID" = "p"."P_Class"), "p0"."P_ID", "p0"."P_Account", "p0"."P_Class", "p0"."P_Firstname", "p0"."P_Lastname"
                // FROM "Schoolclass" AS "s"
                // LEFT JOIN "Pupil" AS "p0" ON "s"."C_ID" = "p0"."P_Class"
                // WHERE ("s"."C_Department" = 'HIF') AND ((
                //     SELECT COUNT(*)
                //     FROM "Pupil" AS "p1"
                //     WHERE "s"."C_ID" = "p1"."P_Class") > 30)
                // ORDER BY "s"."C_ID", "p0"."P_ID"
                var result4 = context.Schoolclass
                              .Where(c => c.C_Department == "HIF" && c.Pupil.Count() > 30)
                              .Select(c => new
                              {
                                  Class = c.C_ID,
                                  PupilsCount = c.Pupil.Count(),
                                  Pupils = c.Pupil
                              });
                Console.WriteLine("${result3.Count()} Results");

                // *********************************************************************************
                // Abfragen mit Any() erzeugen automatisch eine EXISTS Klausel in SQL.
                // SELECT "s"."C_ID", "s"."C_ClassTeacher", "s"."C_Department"
                // FROM "Schoolclass" AS "s"
                // WHERE EXISTS (
                //     SELECT 1
                //     FROM "Test" AS "t"
                //     WHERE ("s"."C_ID" = "t"."TE_Class") AND ("t"."TE_Subject" = 'BWM1'))
                var result5 = context.Schoolclass
                              .Where(c => c.Test.Any(t => t.TE_Subject == "BWM1"));
                Console.WriteLine(JsonSerializer.Serialize(result5));

                // *********************************************************************************
                // Gruppierungen werden auch automatisch erzeugt.
                // SELECT "t"."TE_Class" AS "Class", COUNT(*) AS "Tests"
                // FROM "Test" AS "t"
                // INNER JOIN "Schoolclass" AS "s" ON "t"."TE_Class" = "s"."C_ID"
                // WHERE "s"."C_ClassTeacher" = 'SZ'
                // GROUP BY "t"."TE_Class"
                var result6 = context.Test
                              .Where(te => te.TE_ClassNavigation.C_ClassTeacher == "SZ")
                              .GroupBy(te => te.TE_Class)
                              .Select(g => new
                              {
                                  Class = g.Key,
                                  Tests = g.Count()
                              });
                Console.WriteLine(JsonSerializer.Serialize(result6));

                // *********************************************************************************
                // LAZY LOADING UND NACHLADEN VON DATEN
                // *********************************************************************************

                // *********************************************************************************
                // Wird die Abfrage ausgeführt, sind die Navigations null bzw. leere Collections
                // ("Lazy Loading")
                // SELECT "s"."C_ID", "s"."C_ClassTeacher", "s"."C_Department"
                // FROM "Schoolclass" AS "s"
                // WHERE "s"."C_Department" = 'HIF'
                var result10 = context.Schoolclass
                                .Where(c => c.C_Department == "HIF")
                                .FirstOrDefault();
                // Im Speicher steht folgendes Ergebnis:
                // {"C_ID":"1AHIF","C_Department":"HIF","C_ClassTeacher":"NIJ",
                //  "C_ClassTeacherNavigation":null,"Lesson":[],"Pupil":[],"Test":[]}
                if (result10.C_ClassTeacherNavigation == null)
                {
                    Console.WriteLine("Oops, C_ClassTeacherNavigation is null.");
                }

                // *********************************************************************************
                // Mit Find() kann am schnellsten nach dem Primärschlüssel gesucht werden. Hier wird
                // die Abfrage ebenfalls ausgeführt, die Navigations sind null bzw. leer.
                var result11 = context.Schoolclass.Find("3BHIF");
                if (result11.C_ClassTeacherNavigation == null)
                {
                    Console.WriteLine("Oops, C_ClassTeacherNavigation is null.");
                }

                // *********************************************************************************
                // Mit .Include kann im LINQ Statement nachgeladen werden. Es muss vor der Filterung
                // mit SingleOrDefault passieren.
                // https://docs.microsoft.com/en-us/ef/core/querying/related-data
                // *********************************************************************************

                var result12 = context.Schoolclass
                    .Include(c => c.C_ClassTeacherNavigation)
                    .SingleOrDefault(c => c.C_ID == "3BHIF");
                Console.WriteLine($"KV der 3BHIF ist {result12.C_ClassTeacherNavigation.T_Lastname}");

                // Explizites Laden ist auch möglich, wenn man das Entity schon hat.
                var result12a = context.Schoolclass.Find("3BHIF");
                context.Entry(result12a)
                    .Reference(c => c.C_ClassTeacherNavigation)
                    .Load();
                Console.WriteLine($"KV der 3BHIF ist {result12a.C_ClassTeacherNavigation.T_Lastname}");

                // *********************************************************************************
                // Hier wird Klasse -> Lehrer -> Lesson
                //                  -> Schüler
                // geladen. Das sollte aber sparsam verwendet werden. Mit einer LINQ Abfrage, die 
                // die benötigten Daten über ein select new liefert ist man besser bedient. Bedenke,
                // dass hier alle Tabellen vollständig geladen werden!
                // https://docs.microsoft.com/en-us/ef/core/querying/related-data
                var result13 = context.Schoolclass
                    .Include(c => c.C_ClassTeacherNavigation)
                        .ThenInclude(t => t.Lesson)
                    .Include(c => c.Pupil)
                    .SingleOrDefault(c => c.C_ID == "3BHIF");

                Console.WriteLine($"Die 3BHIF hat {result13.Pupil.Count()} Schüler.");
                Console.WriteLine($"Der KV ist {result13.C_ClassTeacherNavigation.T_Lastname} und sie unterrichtet " +
                    $"{result13.C_ClassTeacherNavigation.Lesson.Count()} Stunden.");

                // So ist das viel besser.
                var result13a = from c in context.Schoolclass
                                where c.C_ID == "3BHIF"
                                select new
                                {
                                    PupilCount = c.Pupil.Count(),
                                    ClassTeacherName = c.C_ClassTeacherNavigation.T_Lastname,
                                    ClassTeacherHours = c.C_ClassTeacherNavigation.Lesson.Count()
                                };

            }
        }
    }
}
