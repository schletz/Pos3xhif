using Microsoft.EntityFrameworkCore;
using Crud.Model;
using System;
using System.Linq;
using System.Text.Json;

namespace Crud
{
    class Program
    {
        static void Main(string[] args)
        {
            // Am Ende des Blockes wird Dispose() aufgerufen und die Verbindung wird geschlossen.
            using (TestsContext context = new TestsContext())
            {

                int pupils = 0;
                try
                {
                    pupils = context.Schoolclass.Where(c => c.C_ID == "3BHIF").Select(c => c.Pupil.Count()).SingleOrDefault();
                    Console.WriteLine($"{pupils} in der 3BHIF");

                    // Beachte: Keine Zuweisung der Klasse.
                    var newPupil = new Pupil { P_Account = "ZZZ9999", P_Firstname = "XXX", P_Lastname = "YYY" };
                    // Da pupil sonst leer ist, müssen wir nachladen.
                    var myClass = context.Schoolclass.Single(c => c.C_ID == "3BHIF");
                    myClass.Pupil.Add(newPupil);
                    // INSERT INTO "Pupil" ("P_Account", "P_Class", "P_Firstname", "P_Lastname")
                    // VALUES (@p0, @p1, @p2, @p3);
                    // SELECT "P_ID"
                    // FROM "Pupil"
                    // WHERE changes() = 1 AND "rowid" = last_insert_rowid();
                    context.SaveChanges();
                    pupils = context.Schoolclass.Where(c => c.C_ID == "3BHIF").Select(c => c.Pupil.Count()).SingleOrDefault();
                    Console.WriteLine($"{pupils} in der 3BHIF");

                    Pupil deletePupil = context.Pupil.SingleOrDefault(p => p.P_Account == "ZZZ9999");
                    context.Pupil.Remove(deletePupil);
                    context.SaveChanges();

                    var classToDelete = context.Schoolclass.Include(c => c.Pupil).SingleOrDefault(c => c.C_ID == "3BHIF");
                    context.Pupil.RemoveRange(classToDelete.Pupil);
                    context.SaveChanges();

                    pupils = context.Schoolclass.Where(c => c.C_ID == "3BHIF").Select(c => c.Pupil.Count()).SingleOrDefault();
                    Console.WriteLine($"{pupils} in der 3BHIF");

                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }

        }
    }
}
