using Bogus;
using Bogus.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artikelverwaltung.Model
{
    public class ArtikelContext : DbContext
    {
        public DbSet<Kategorie> Kategorien { get; set; }
        public DbSet<Artikel> Artikel { get; set; }
        public DbSet<Kunde> Kunden { get; set; }
        public DbSet<Bestellung> Bestellungen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlite("Data Source=Artikel.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kategorie>().HasIndex(k => k.Name).IsUnique();
            modelBuilder.Entity<Artikel>().HasIndex(a => a.Name).IsUnique();
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(201227);
            Faker f = new Faker();

            var kategorien = (from k in f.Commerce.Categories(20).Distinct()
                              select new Kategorie
                              {
                                  Name = k
                              }).Take(10).ToList();
            Kategorien.AddRange(kategorien);
            SaveChanges();

            var kunden = new Faker<Kunde>()
                .Rules((f, k) =>
                {
                    k.Vorname = f.Name.FirstName();
                    k.Zuname = f.Name.LastName();
                    k.Adresse = f.Address.StreetAddress();
                    k.Plz = f.Random.Int(1000, 9999);
                    k.Ort = f.Address.City();
                })
                .Generate(20);
            Kunden.AddRange(kunden);
            SaveChanges();

            var artikel = new Faker<Artikel>()
                .Rules((f, a) =>
                {
                    a.Ean = f.Commerce.Ean13();
                    a.Name = f.Commerce.ProductName();
                    a.Preis = Math.Round(f.Random.Decimal(10, 1000), 4);
                    a.Hersteller = f.Company.CompanyName();
                    a.ProduziertAb = new DateTime(2018, 1, 1).AddDays(f.Random.Int(0, 2 * 365));
                    a.EingestelltAb = a.ProduziertAb.AddDays(f.Random.Int(100, 365)).OrNull(f, 0.5f);
                    a.Kategorie = f.Random.ListItem(kategorien);
                })
                .Generate(200).GroupBy(a => a.Name).Select(a => a.First())
                .ToList();
            Artikel.AddRange(artikel);
            SaveChanges();

            var bestellungen = new Faker<Bestellung>()
                .Rules((f, b) =>
                {
                    b.Artikel = f.Random.ListItem(artikel);
                    b.Kunde = f.Random.ListItem(kunden);
                    b.Datum = b.Artikel.ProduziertAb.AddSeconds(f.Random.Long(0, 1L * 365 * 86400));
                    b.BezahltAm = b.Datum.AddSeconds(f.Random.Int(1 * 86400, 10 * 86400)).OrNull(f, 0.1f);
                    b.Menge = f.Random.Int(1, 5);
                }).Generate(200);
            Bestellungen.AddRange(bestellungen);
            SaveChanges();
        }
    }
}
