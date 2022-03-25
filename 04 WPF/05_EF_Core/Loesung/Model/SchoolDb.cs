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
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Schoolclass> Classes { get; set; }
        public DbSet<Pupil> Pupils { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Soll die LocalDb (ein Mini SQL Server) verwendet werden, dann muss folgendes
            // geschrieben werden:
            // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SchoolDatabase;Trusted_Connection=True;");
            optionsBuilder.UseSqlite(@"Data Source=School.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/modeling/

            modelBuilder.Entity<Gender>().ToTable("Gender");
            modelBuilder.Entity<Teacher>().ToTable("Teacher");
            modelBuilder.Entity<Schoolclass>().ToTable("Schoolclass");
            modelBuilder.Entity<Pupil>().ToTable("Pupil");
            modelBuilder.Entity<Exam>().ToTable("Exam");

            // Primärschlüssel definieren.
            modelBuilder.Entity<Gender>().HasKey(g => g.GenderNr);
            modelBuilder.Entity<Teacher>().HasKey(t => t.TeacherNr);
            modelBuilder.Entity<Schoolclass>().HasKey(s => s.ClassNr);
            modelBuilder.Entity<Pupil>().HasKey(p => p.PupilId);
            modelBuilder.Entity<Exam>().HasKey(e => e.ExamId);
            // Auto ID Werte aktivieren oder deaktivieren (bei int Schlüssel standardmäßig aktiv)
            modelBuilder.Entity<Gender>().Property(g => g.GenderNr).ValueGeneratedNever();

            // Unique Felder definieren.
            modelBuilder.Entity<Gender>().HasIndex(g => g.Name).IsUnique();

            // NOT NULL Constraints für Referenztypen (die ja null sein können). Wertetypen sind immer NOT
            // NULL, außer es wird ein nullable type wie int? verwendet.
            modelBuilder.Entity<Exam>().Property(e => e.Subject).IsRequired();
            modelBuilder.Entity<Gender>().Property(g => g.Name).IsRequired();
            modelBuilder.Entity<Pupil>().Property(p => p.Firstname).IsRequired();
            modelBuilder.Entity<Pupil>().Property(p => p.Lastname).IsRequired();
            modelBuilder.Entity<Teacher>().Property(t => t.Firstname).IsRequired();
            modelBuilder.Entity<Teacher>().Property(t => t.Lastname).IsRequired();

            // Stringlängen definieren.
            modelBuilder.Entity<Teacher>().Property(t => t.TeacherNr).HasMaxLength(16);
            modelBuilder.Entity<Schoolclass>().Property(s => s.ClassNr).HasMaxLength(16);

            // Die Navigations konfigurieren. Wir setzen, wenn sie NOT NULL sein müssen und dass
            // das beim Löschen ein Fehler geworfen wird, wenn der Wert als Fremdschlüssel in der
            // n Tabelle verwendet wird.
            modelBuilder.Entity<Schoolclass>().HasOne(s => s.KV).WithMany().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Pupil>().HasOne(p => p.Gender).WithMany().OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Pupil>().HasOne(p => p.Schoolclass).WithMany(s => s.Pupils).OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Exam>().HasOne(e => e.Examiner).WithMany(t => t.Exams).OnDelete(DeleteBehavior.Restrict).IsRequired();
            modelBuilder.Entity<Exam>().HasOne(e => e.Pupil).WithMany(p => p.Exams).OnDelete(DeleteBehavior.Restrict).IsRequired();
        }

        /// <summary>
        /// Erstellt eine Datenbank mit Musterdaten. Ist die Datenbank schon vorhanden, werden keine
        /// Änderungen daran durchgeführt.
        /// </summary>
        /// <param name="deleteDb">Gibt ab, ob eine vorhandene Datenbank gelöscht werden soll.</param>
        public void CreateDatabase(bool deleteDb = false)
        {
            if (deleteDb) Database.EnsureDeleted();
            if (!Database.EnsureCreated()) { return; }

            // Damit bei jedem Programmstart die gleichen Daten generiert werden, wird der
            // Zufallsgenerator mit einem fixen Wert initialisiert. Das ist wichtig, um Fehler
            // reproduzieren zu können.
            Randomizer.Seed = new Random(16030829);
            Randomizer rnd = new Randomizer();

            // Die Geschlechter in die DB schreiben.
            var genders = new List<Gender>
            {
                new Gender {GenderNr = 1, Name = "Male"},
                new Gender {GenderNr = 2, Name = "Female"},
            };
            Genders.AddRange(genders);
            SaveChanges();

            // 20 Lehrer erzeugen.
            int teacherNr = 1000;
            var teachers = new Faker<Teacher>()
                .RuleFor(t => t.Firstname, f => f.Name.FirstName())
                .RuleFor(t => t.Lastname, f => f.Name.LastName())
                .RuleFor(t => t.TeacherNr, (f, t) => $"{t.Lastname.Substring(0, 3).ToUpper()}{teacherNr++}")
                .RuleFor(t => t.Email, (f, t) => t.TeacherNr.ToLower() + "@spengergasse.at")
                .Generate(20);

            Teachers.AddRange(teachers);
            SaveChanges();

            // Die Klassen 1AHIF - 5AHIF erzeugen.
            var classes = Enumerable.Range(1, 5)
                .Select(i => new Schoolclass
                {
                    ClassNr = i.ToString() + "AHIF",
                    Room = $"{rnd.String2(1, "ABC")}{rnd.String2(1, "1234")}.{rnd.String2(1, "01")}{rnd.String2(1, "123456789")}",
                    KV = rnd.ListItem(teachers)

                }).ToList();
            Classes.AddRange(classes);
            SaveChanges();

            // 50 Schüler erzeugen und in eine zufällige Klasse setzen.
            var pupils = new Faker<Pupil>()
                .RuleFor(p => p.Gender, f => f.Random.ListItem(genders))
                .RuleFor(p => p.Lastname, f => f.Name.LastName())
                .RuleFor(p => p.Firstname, (f, p) => f.Name.FirstName((Bogus.DataSets.Name.Gender)(p.Gender.GenderNr - 1)))
                .RuleFor(p => p.Schoolclass, f => f.Random.ListItem(classes))
                // Bei nullable Werten erzeugen wir 20 % NULL werten. In diesem Fall wird das Geburtsdatum
                // nicht bei jedem Schüler eingetragen.
                .RuleFor(p => p.DateOfBirth, (f, p) =>
                      f.Date.Between(
                          new DateTime(2006 - int.Parse(p.Schoolclass.ClassNr.Substring(0, 1)), 9, 1),
                          new DateTime(2007 - int.Parse(p.Schoolclass.ClassNr.Substring(0, 1)), 9, 1)).Date.OrNull(f, 0.2f))
                .Generate(50);
            Pupils.AddRange(pupils);
            SaveChanges();

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
                // Der Schüler wird erst jetzt zugewiesen, da wir für jeden Schüler Prüfungen
                // generieren wollen.
                Exams.AddRange(examFaker.RuleFor(e => e.Pupil, f => p).Generate(examCount));
            }
            SaveChanges();
        }
    }
}