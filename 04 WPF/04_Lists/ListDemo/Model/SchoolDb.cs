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
        private SchoolDb(List<Student> pupils, List<Schoolclass> classes, List<Teacher> teachers, List<Gender> genders, List<Exam> exams)
        {
            Pupils = pupils;
            Classes = classes;
            Teachers = teachers;
            Genders = genders;
            Exams = exams;
        }

        /// <summary>
        /// Liste aller Schüler.
        /// </summary>
        public List<Student> Pupils { get; }
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
        /// Liste aller Exams
        /// </summary>
        public List<Exam> Exams { get; }



        /// <summary>
        /// Erstellt eine Datenbank mit Musterdaten und gibt sie zurück.
        /// </summary>
        /// <returns></returns>
        public static SchoolDb FromMockup()
        {

            Randomizer.Seed = new Random(16030829);
            var faker = new Faker();

            var genders = new List<Gender>(2)
            {
                new Gender (genderId: 1, name: "Male"),
                new Gender (genderId: 2, name: "Female")
            };

            // 20 Lehrer erzeugen.
            int teacherNr = 1000;
            var teachers = new Faker<Teacher>().CustomInstantiator(f =>
            {
                var fistname = f.Name.FirstName();
                var lastname = f.Name.LastName();
                var teacherShortname = $"{lastname.Substring(0, 3).ToUpper()}{teacherNr++}";
                return new Teacher(
                    teacherNr: teacherShortname,
                    firstname: f.Name.FirstName(),
                    lastname: f.Name.LastName(),
                    email: teacherShortname.ToLower() + "@spengergasse.at"
                );
            })
            .Generate(20)
            .ToList();

            // Die Klassen 1AHIF - 5AHIF erzeugen.
            var classes = Enumerable.Range(1, 5)
                .Select(i => new Schoolclass(
                    name: $"{i}AHIF",
                    room: $"{faker.Random.String2(1, "ABC")}{faker.Random.String2(1, "1234")}.{faker.Random.String2(1, "01")}{faker.Random.String2(1, "123456789")}",
                    kV: faker.Random.ListItem(teachers)
                ))
                .ToList();

            var subjects = new string[] { "AM", "D", "E", "POS", "DBI" };

            // Schüler erzeugen und in die Klasse setzen.
            var pupils = new Faker<Student>().CustomInstantiator(f =>
            {
                var gender = f.Random.ListItem(genders);
                var schoolclass = f.Random.ListItem(classes);
                return new Student(
                    firstname: f.Name.FirstName((Bogus.DataSets.Name.Gender)(gender.GenderId - 1)),
                    lastname: f.Name.LastName(),
                    gender: gender,
                    schoolclass: f.Random.ListItem(classes),
                    dateOfBirth: f.Date.Between(
                                  new DateTime(2006 - int.Parse(schoolclass.Name.Substring(0, 1)), 9, 1),
                                  new DateTime(2007 - int.Parse(schoolclass.Name.Substring(0, 1)), 9, 1)));
                
            })
            .Generate(100);


            // Jedem Student 1-3 Exams zuweisen. Da wir eine Liste von Listen erhalten,
            // verwenden wir SelectMany. Dies erzeugt uns eine flache Liste von Exams.
            var exams = pupils
                .SelectMany(p => new Faker<Exam>().CustomInstantiator(f =>
                        new Exam(
                            subject: f.Random.ListItem(subjects),
                            date: f.Date.Between(new DateTime(2019, 9, 1), new DateTime(2020, 7, 1)).Date,
                            examiner: f.Random.ListItem(teachers),
                            student: p,
                            grade: f.Random.Int(1, 5).OrNull(f, 0.2f))
                    )
                .Generate(faker.Random.Int(1, 3)))
                .ToList();
            return new SchoolDb(pupils, classes, teachers, genders, exams);
        }
    }
}