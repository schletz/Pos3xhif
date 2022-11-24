using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Grouping.Model
{
    /// <summary>
    /// DB Contextklasse für die Exams Datenbank. Stellt die Tabellen als DbSet Collection zur
    /// Verfügung.
    /// </summary>
    class ExamsDb : DbContext
    {
        public DbSet<Period> Periods => Set<Period>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Schoolclass> Schoolclasss => Set<Schoolclass>();
        public DbSet<Pupil> Pupils => Set<Pupil>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Exam> Exams => Set<Exam>();

        private ExamsDb(DbContextOptions opt) : base(opt) { }

        /// <summary>
        /// Liest die Dateien im angegebenen Ordner in die Collection und fügt so die
        /// Musterdaten ein.
        /// </summary>
        public static ExamsDb FromFiles(string path)
        {
            var dbPathMatch = Regex.Match(AppContext.BaseDirectory, @"(?<path>.*Grouping\.Application)", RegexOptions.IgnoreCase);
            var appPath = dbPathMatch.Success ? dbPathMatch.Groups["path"].Value : ".";
            path = Path.Combine(appPath, path);

            var opt = new DbContextOptionsBuilder()
                .UseSqlite($"Data Source={Path.Combine(appPath, "Exams.db")}")
                //.UseSqlServer(@"Server=127.0.0.1,1433;Initial Catalog=ExamsDb;User Id=sa;Password=SqlServer2019")
                .Options;
            var db = new ExamsDb(opt);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.ReadFile(db.Periods, path, "periods.csv");
            db.ReadFile(db.Teachers, path, "teachers.csv");
            db.ReadFile(db.Schoolclasss, path, "schoolclasses.csv");
            db.ReadFile(db.Pupils, path, "pupils.csv");
            db.ReadFile(db.Lessons, path, "lessons.csv");
            db.ReadFile(db.Exams, path, "exams.csv");
            db.ChangeTracker.Clear();
            return db;
        }

        private void ReadFile<T>(DbSet<T> dbSet, string path, string filename) where T : class
        {
            using var reader = new StreamReader(path: Path.Combine(path, filename), encoding: new UTF8Encoding(false));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            dbSet.AddRange(csv.GetRecords<T>());
            try
            {
                SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.Error.WriteLine($"Error in {filename}: {e.InnerException?.Message}");
            }
        }
    }
}