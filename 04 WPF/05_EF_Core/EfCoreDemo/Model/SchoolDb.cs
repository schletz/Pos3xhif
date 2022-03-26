using Bogus;
using Bogus.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDemo.Model
{
    /// <summary>
    /// Simuliert eine Schülerdatenbank.
    /// </summary>
    public class SchoolDb : DbContext
    {
        public DbSet<Student> Pupils => Set<Student>();
        public DbSet<Schoolclass> Classes => Set<Schoolclass>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Gender> Genders => Set<Gender>();
        public DbSet<Exam> Exams => Set<Exam>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Aus der Datei app.config den Connection String laden.
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["ExamDatabase"].ConnectionString);
        }

        /// <summary>
        /// Erstellt eine Datenbank mit Musterdaten und gibt sie zurück.
        /// </summary>
        /// <returns></returns>
        public void Seed()
        {

            Randomizer.Seed = new Random(16030829);
            var faker = new Faker();

            var genders = new List<Gender>(2)
            {
                new Gender (name: "Male"),
                new Gender (name: "Female")
            };
            Genders.AddRange(genders);
            SaveChanges();
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
            Teachers.AddRange(teachers);
            SaveChanges();

            // Die Klassen 1AHIF - 5AHIF erzeugen.
            var classes = Enumerable.Range(1, 5)
                .Select(i => new Schoolclass(
                    name: $"{i}AHIF",
                    room: $"{faker.Random.String2(1, "ABC")}{faker.Random.String2(1, "1234")}.{faker.Random.String2(1, "01")}{faker.Random.String2(1, "123456789")}",
                    kV: faker.Random.ListItem(teachers)
                ))
                .ToList();
            Classes.AddRange(classes);
            SaveChanges();


            // Schüler erzeugen und in die Klasse setzen.
            var pupils = new Faker<Student>().CustomInstantiator(f =>
            {
                var gender = f.Random.ListItem(genders);
                var schoolclass = f.Random.ListItem(classes);
                return new Student(
                    firstname: f.Name.FirstName(gender.Name == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female),
                    lastname: f.Name.LastName(),
                    gender: gender,
                    schoolclass: f.Random.ListItem(classes),
                    dateOfBirth: f.Date.Between(
                                  new DateTime(2006 - int.Parse(schoolclass.Name.Substring(0, 1)), 9, 1),
                                  new DateTime(2007 - int.Parse(schoolclass.Name.Substring(0, 1)), 9, 1)).Date);
                
            })
            .Generate(100);
            Pupils.AddRange(pupils);
            SaveChanges();

            // Jedem Student 1-3 Exams zuweisen. Da wir eine Liste von Listen erhalten,
            // verwenden wir SelectMany. Dies erzeugt uns eine flache Liste von Exams.
            var subjects = new string[] { "AM", "D", "E", "POS", "DBI" };
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
            Exams.AddRange(exams);
            SaveChanges();
        }
    }
}