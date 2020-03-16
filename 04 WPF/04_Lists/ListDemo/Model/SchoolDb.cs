using Bogus;
using Bogus.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDemo.Model
{
    /// <summary>
    /// Simuliert eine Schülerdatenbank.
    /// </summary>
    public class SchoolDb
    {
        private SchoolDb(List<Pupil> pupils, List<Schoolclass> classes, List<Teacher> teachers, List<Gender> genders)
        {
            Pupils = pupils ?? throw new ArgumentNullException(nameof(pupils));
            Classes = classes ?? throw new ArgumentNullException(nameof(classes));
            Teachers = teachers ?? throw new ArgumentNullException(nameof(teachers));
            Genders = genders ?? throw new ArgumentNullException(nameof(genders));
        }

        /// <summary>
        /// Liste aller Schüler.
        /// </summary>
        public List<Pupil> Pupils { get; }
        /// <summary>
        /// Liste aller Klassen.
        /// </summary>
        public List<Schoolclass> Classes { get; }
        /// <summary>
        /// Liste aller Lehrer.
        /// </summary>
        public List<Teacher> Teachers { get; }
        /// <summary>
        /// Liste aller Geschlechter.
        /// </summary>
        public List<Gender> Genders { get; }
        /// <summary>
        /// Verhindert eine direkte Instanzierung mit einer null Liste.
        /// </summary>

        /// <summary>
        /// Erstellt eine Datenbank mit Musterdaten und gibt sie zurück.
        /// </summary>
        /// <returns></returns>
        public static SchoolDb FromMockup()
        {

            Randomizer.Seed = new Random(16030829);
            Randomizer rnd = new Randomizer();

            var genders = new List<Gender>
            {
                new Gender {GenderId = 1, Name = "Male"},
                new Gender {GenderId = 2, Name = "Female"},
            };

            // 20 Lehrer erzeugen.
            int teacherNr = 1000;
            var teachers = new Faker<Teacher>()
                .RuleFor(t => t.Firstname, f => f.Name.FirstName())
                .RuleFor(t => t.Lastname, f => f.Name.LastName())
                .RuleFor(t => t.TeacherNr, (f, t) => $"{t.Lastname.Substring(0, 3).ToUpper()}{teacherNr++}")
                .RuleFor(t => t.Email, (f, t) => t.TeacherNr.ToLower() + "@spengergasse.at")
                .Generate(20);

            // Die Klassen 1AHIF - 5AHIF erzeugen.
            var classes = Enumerable.Range(1, 5)
                .Select(i => new Schoolclass
                {
                    Name = i.ToString() + "AHIF",
                    Room = $"{rnd.String2(1, "ABC")}{rnd.String2(1, "1234")}.{rnd.String2(1, "01")}{rnd.String2(1, "123456789")}",
                    KV = rnd.ListItem(teachers)

                }).ToList();

            // Schüler erzeugen und in die Klasse setzen.
            var pupils = new Faker<Pupil>()
                .RuleFor(p => p.Gender, f => f.Random.ListItem(genders))
                .RuleFor(p => p.Lastname, f => f.Name.LastName())
                .RuleFor(p => p.Firstname, (f, p) => f.Name.FirstName((Bogus.DataSets.Name.Gender)(p.Gender.GenderId - 1)))
                .RuleFor(p => p.Schoolclass, f => f.Random.ListItem(classes))
                .RuleFor(p => p.DateOfBirth, (f, p) =>
                      f.Date.Between(
                          new DateTime(2006 - int.Parse(p.Schoolclass.Name.Substring(0, 1)), 9, 1),
                          new DateTime(2007 - int.Parse(p.Schoolclass.Name.Substring(0, 1)), 9, 1)))
                .Generate(50);

            // Generator für Prüfungen erzeugen.
            var examFaker = new Faker<Exam>()
                .RuleFor(e => e.Subject, f => f.Random.ListItem(new string[] { "AM", "D", "E", "POS", "DBI" }))
                .RuleFor(e => e.Date, f => f.Date.Between(new DateTime(2019, 9, 1), new DateTime(2020, 7, 1)).Date)
                .RuleFor(e => e.Examiner, f => f.Random.ListItem(teachers))
                .RuleFor(e => e.Grade, f => f.Random.Int(1, 5).OrNull(f, 0.2f));

            // 3 - 5 Prüfungen für jeden Schüler erzeugen und die Liste der Prüfungen des Schülers
            // setzen.
            foreach (var p in pupils)
            {
                int examCount = rnd.Int(3, 5);
                var exams = examFaker.RuleFor(e => e.Pupil, f => p).Generate(examCount);
                p.Exams = exams;
            }
            // Die Liste der Schüler pro Klasse erzeugen.
            foreach (var c in classes)
            {
                c.Pupils = pupils.Where(p => p.Schoolclass == c).ToList();
            }
            return new SchoolDb(pupils, classes, teachers, genders);
        }
    }
}