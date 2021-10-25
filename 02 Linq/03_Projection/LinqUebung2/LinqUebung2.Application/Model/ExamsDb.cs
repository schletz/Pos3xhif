using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinqUebung2.Application.Model
{
    internal class ExamsDb : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Exam> Exams => Set<Exam>();

        public ExamsDb(DbContextOptions opt) : base(opt)
        {
        }

        public static ExamsDb FromSeed()
        {
            var opt = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "Exams")
                .Options;
            var db = new ExamsDb(opt);
            db.Seed();
            return db;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "Exams");
        }

        public void Seed()
        {
            var students = new Student[]
            {
                new Student(id: 1000, lastname: "Elt", firstname: "Célia", gender: Gender.Female, schoolclass: "3AHIF"),
                new Student(id: 1001, lastname: "Mattack", firstname: "Loïca", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1002, lastname: "Nayshe", firstname: "Eliès", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1003, lastname: "Domanek", firstname: "Noémie", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1004, lastname: "Avramovitz", firstname: "Chloé", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1005, lastname: "Curtin", firstname: "Maëline", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1006, lastname: "Riseborough", firstname: "Lauréna", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1007, lastname: "Kynge", firstname: "Valérie", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1008, lastname: "Dibden", firstname: "Maéna", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1009, lastname: "Pinder", firstname: "Jú", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1010, lastname: "Cuseick", firstname: "Cléa", gender: Gender.Female, schoolclass: "3CHIF"),
                new Student(id: 1011, lastname: "Calladine", firstname: "Clémence", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1012, lastname: "Stiff", firstname: "Maéna", gender: Gender.Female, schoolclass: "3AHIF"),
                new Student(id: 1013, lastname: "Elbourn", firstname: "Josée", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1014, lastname: "Fosdike", firstname: "Kallisté", gender: Gender.Female, schoolclass: "3AHIF"),
                new Student(id: 1015, lastname: "Wilton", firstname: "Lèi", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1016, lastname: "Billson", firstname: "Eléa", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1017, lastname: "Dunstall", firstname: "Lyséa", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1018, lastname: "Santori", firstname: "Céline", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1019, lastname: "Sharpe", firstname: "Béatrice", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1020, lastname: "Minerdo", firstname: "Laurélie", gender: Gender.Female, schoolclass: "3CHIF"),
                new Student(id: 1021, lastname: "Gianulli", firstname: "Léonie", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1022, lastname: "Works", firstname: "Styrbjörn", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1023, lastname: "Dixon", firstname: "Personnalisée", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1024, lastname: "Browne", firstname: "Esbjörn", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1025, lastname: "Clearley", firstname: "Åsa", gender: Gender.Male, schoolclass: "3CHIF"),
                new Student(id: 1026, lastname: "Jeandin", firstname: "Maïté", gender: Gender.Male, schoolclass: "3BHIF"),
                new Student(id: 1027, lastname: "McComiskey", firstname: "Léa", gender: Gender.Female, schoolclass: "3CHIF"),
                new Student(id: 1028, lastname: "Castellan", firstname: "Léa", gender: Gender.Male, schoolclass: "3AHIF"),
                new Student(id: 1029, lastname: "Spurnier", firstname: "Stéphanie", gender: Gender.Female, schoolclass: "3CHIF")
            };
            Students.AddRange(students);
            SaveChanges();

            var exams = new Exam[]
            {
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2018, 5, 12), student: students[0]),
                new Exam(subject:"AM", examinator: "KY", grade: 2, date: new DateTime(2018, 3, 10), student: students[1]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 1, date: new DateTime(2018, 4, 18), student: students[1]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 4, date: new DateTime(2018, 4, 21), student: students[1]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2018, 1, 28), student: students[1]),
                new Exam(subject:"POS", examinator: "SZ", grade: 5, date: new DateTime(2017, 11, 28), student: students[1]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 2, date: new DateTime(2018, 2, 2), student: students[10]),
                new Exam(subject:"E", examinator: "FAV", grade: 2, date: new DateTime(2018, 4, 2), student: students[10]),
                new Exam(subject:"POS", examinator: "SZ", grade: 4, date: new DateTime(2018, 2, 16), student: students[10]),
                new Exam(subject:"AM", examinator: "KY", grade: 5, date: new DateTime(2017, 9, 12), student: students[11]),
                new Exam(subject:"D", examinator: "KY", grade: 3, date: new DateTime(2018, 3, 24), student: students[11]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2018, 2, 13), student: students[11]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 5, 22), student: students[12]),
                new Exam(subject:"POS", examinator: "SZ", grade: 5, date: new DateTime(2018, 6, 18), student: students[12]),
                new Exam(subject:"AM", examinator: "KY", grade: 3, date: new DateTime(2018, 2, 4), student: students[13]),
                new Exam(subject:"D", examinator: "KY", grade: 1, date: new DateTime(2017, 9, 20), student: students[13]),
                new Exam(subject:"E", examinator: "FAV", grade: 1, date: new DateTime(2017, 11, 26), student: students[13]),
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2018, 6, 22), student: students[14]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 12, 9), student: students[15]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 4, 14), student: students[16]),
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2017, 9, 3), student: students[17]),
                new Exam(subject:"POS", examinator: "SZ", grade: 5, date: new DateTime(2017, 10, 24), student: students[17]),
                new Exam(subject:"E", examinator: "FAV", grade: 2, date: new DateTime(2017, 12, 10), student: students[19]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 10, 13), student: students[19]),
                new Exam(subject:"D", examinator: "KY", grade: 2, date: new DateTime(2017, 12, 8), student: students[2]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 5, date: new DateTime(2017, 12, 21), student: students[2]),
                new Exam(subject:"E", examinator: "FAV", grade: 1, date: new DateTime(2018, 3, 31), student: students[2]),
                new Exam(subject:"E", examinator: "FAV", grade: 3, date: new DateTime(2018, 3, 27), student: students[2]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 6, 19), student: students[2]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 11, 27), student: students[2]),
                new Exam(subject:"D", examinator: "KY", grade: 3, date: new DateTime(2018, 3, 29), student: students[20]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 5, date: new DateTime(2018, 5, 3), student: students[20]),
                new Exam(subject:"D", examinator: "NAI", grade: 3, date: new DateTime(2018, 1, 13), student: students[22]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 1, date: new DateTime(2018, 5, 22), student: students[22]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2017, 11, 11), student: students[23]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 9, 12), student: students[23]),
                new Exam(subject:"POS", examinator: "SZ", grade: 3, date: new DateTime(2017, 12, 4), student: students[23]),
                new Exam(subject:"AM", examinator: "KY", grade: 1, date: new DateTime(2018, 1, 12), student: students[24]),
                new Exam(subject:"AM", examinator: "KY", grade: 5, date: new DateTime(2018, 6, 9), student: students[24]),
                new Exam(subject:"E", examinator: "FAV", grade: 1, date: new DateTime(2017, 9, 20), student: students[24]),
                new Exam(subject:"E", examinator: "FAV", grade: 3, date: new DateTime(2018, 3, 26), student: students[24]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 9, 19), student: students[24]),
                new Exam(subject:"AM", examinator: "KY", grade: 5, date: new DateTime(2017, 10, 29), student: students[25]),
                new Exam(subject:"D", examinator: "KY", grade: 1, date: new DateTime(2018, 4, 15), student: students[25]),
                new Exam(subject:"D", examinator: "KY", grade: 2, date: new DateTime(2018, 4, 1), student: students[25]),
                new Exam(subject:"E", examinator: "FAV", grade: 2, date: new DateTime(2017, 10, 27), student: students[25]),
                new Exam(subject:"AM", examinator: "KY", grade: 4, date: new DateTime(2018, 6, 2), student: students[26]),
                new Exam(subject:"D", examinator: "KY", grade: 2, date: new DateTime(2018, 3, 6), student: students[26]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 2, date: new DateTime(2018, 6, 18), student: students[26]),
                new Exam(subject:"POS", examinator: "SZ", grade: 1, date: new DateTime(2018, 3, 23), student: students[26]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2018, 2, 18), student: students[26]),
                new Exam(subject:"D", examinator: "NAI", grade: 1, date: new DateTime(2017, 10, 22), student: students[27]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 4, date: new DateTime(2017, 10, 29), student: students[27]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2018, 1, 14), student: students[27]),
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2018, 4, 3), student: students[28]),
                new Exam(subject:"E", examinator: "FAV", grade: 1, date: new DateTime(2018, 6, 8), student: students[28]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 5, 28), student: students[28]),
                new Exam(subject:"POS", examinator: "SZ", grade: 5, date: new DateTime(2017, 9, 28), student: students[28]),
                new Exam(subject:"D", examinator: "KY", grade: 1, date: new DateTime(2017, 12, 30), student: students[29]),
                new Exam(subject:"D", examinator: "NAI", grade: 5, date: new DateTime(2018, 5, 18), student: students[29]),
                new Exam(subject:"AM", examinator: "KY", grade: 1, date: new DateTime(2018, 1, 22), student: students[3]),
                new Exam(subject:"AM", examinator: "KY", grade: 4, date: new DateTime(2018, 2, 11), student: students[3]),
                new Exam(subject:"AM", examinator: "KY", grade: 4, date: new DateTime(2018, 6, 18), student: students[3]),
                new Exam(subject:"AM", examinator: "KY", grade: 5, date: new DateTime(2017, 12, 12), student: students[3]),
                new Exam(subject:"AM", examinator: "KY", grade: 5, date: new DateTime(2018, 4, 6), student: students[3]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 1, date: new DateTime(2018, 6, 13), student: students[3]),
                new Exam(subject:"AM", examinator: "KY", grade: 3, date: new DateTime(2018, 4, 5), student: students[4]),
                new Exam(subject:"D", examinator: "NAI", grade: 2, date: new DateTime(2017, 12, 23), student: students[4]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 5, date: new DateTime(2017, 12, 11), student: students[4]),
                new Exam(subject:"AM", examinator: "KY", grade: 3, date: new DateTime(2017, 11, 30), student: students[5]),
                new Exam(subject:"E", examinator: "FAV", grade: 2, date: new DateTime(2017, 12, 25), student: students[5]),
                new Exam(subject:"E", examinator: "FAV", grade: 3, date: new DateTime(2018, 6, 6), student: students[5]),
                new Exam(subject:"POS", examinator: "SZ", grade: 2, date: new DateTime(2017, 10, 4), student: students[5]),
                new Exam(subject:"AM", examinator: "KY", grade: 1, date: new DateTime(2017, 10, 19), student: students[6]),
                new Exam(subject:"AM", examinator: "KY", grade: 1, date: new DateTime(2018, 2, 4), student: students[6]),
                new Exam(subject:"D", examinator: "KY", grade: 2, date: new DateTime(2017, 12, 30), student: students[6]),
                new Exam(subject:"D", examinator: "KY", grade: 3, date: new DateTime(2017, 12, 27), student: students[7]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 4, date: new DateTime(2017, 11, 3), student: students[7]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 5, 15), student: students[7]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 5, 17), student: students[7]),
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2017, 12, 5), student: students[8]),
                new Exam(subject:"D", examinator: "KY", grade: 4, date: new DateTime(2018, 3, 20), student: students[8]),
                new Exam(subject:"DBI", examinator: "FZ", grade: 5, date: new DateTime(2017, 12, 3), student: students[8]),
                new Exam(subject:"E", examinator: "FAV", grade: 1, date: new DateTime(2018, 2, 14), student: students[8]),
                new Exam(subject:"E", examinator: "FAV", grade: 4, date: new DateTime(2018, 4, 2), student: students[8]),
                new Exam(subject:"POS", examinator: "SZ", grade: 4, date: new DateTime(2018, 4, 16), student: students[8]),
                new Exam(subject:"D", examinator: "KY", grade: 2, date: new DateTime(2018, 4, 8), student: students[9]),
                new Exam(subject:"D", examinator: "KY", grade: 5, date: new DateTime(2018, 1, 4), student: students[9])
            };
            Exams.AddRange(exams);
            SaveChanges();
        }
    }
}