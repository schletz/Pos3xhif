using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WahlfachAnmeldung.Models
{
    public class RegistrationContext : DbContext
    {
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherToken> TeacherTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Registration.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Registration>().HasIndex("TokenId", "SubjectId").IsUnique();
            modelBuilder.Entity<Subject>().HasIndex(s => s.Name).IsUnique();
        }

        /// <summary>
        /// Legt die Fächer und Musteranmeldungen an, falls noch keine Datenbank existiert.
        /// Die Token 1-4 (3AHIF1...3AHIF4) haben eine Anmeldung, 5 - 10 nicht.
        /// </summary>
        public void Seed()
        {
            Randomizer.Seed = new Random(1015);
            Faker f = new Faker();

            TeacherTokens.Add(new TeacherToken { TeacherTokenId = Guid.NewGuid().ToString("n") });
            SaveChanges();

            var subjects = new List<Subject>
            {
                new Subject{SubjectId = "BAP", Name = "Business Applications", Order = 1},
                new Subject{SubjectId = "GAD", Name = "Game Development", Order = 2},
                new Subject{SubjectId = "IOT", Name = "Internet of Things", Order = 3},
                new Subject{SubjectId = "OPS", Name = "Operations and Services", Order = 4}
            };
            Subjects.AddRange(subjects);
            SaveChanges();

            foreach (var sc in new string[] { "3AHIF", "3BHIF", "3CHIF", "3EHIF" })
            {
                foreach (var nr in Enumerable.Range(1, 10))
                {
                    Token token = new Token 
                    { 
                        SchoolClass = sc, 
                        TokenId = sc + nr.ToString(),
                    };

                    token.Registrations = new List<Registration>();
                    if (nr < 5)
                    {
                        int i = 1;
                        foreach (var s in f.Random.Shuffle(subjects))
                        {
                            token.Registrations.Add(new Registration { Subject = s, Rating = i++ });
                        }
                        token.LastValidRegistration = new DateTime(2020, 5, 1).AddSeconds(f.Random.Int(0, 20 * 86400));

                    }
                    Tokens.Add(token);
                }
            }
            SaveChanges();
        }
    }
}
