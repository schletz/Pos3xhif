using Bogus;
using Microsoft.EntityFrameworkCore;
using Eventmanager.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventmanager.Infrastructure
{
    public class EventContext : DbContext
    {
        public DbSet<Contingent> Contingents => Set<Contingent>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Guest> Guests => Set<Guest>();
        public DbSet<Show> Shows => Set<Show>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        public EventContext()
        { }

        public EventContext(DbContextOptions options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=event.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contingent>().Property(c => c.ContingentType).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.TicketState).HasConversion<string>();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Generic config for all entities
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1028);
            var minDate = new DateTime(2023, 6, 1);
            var maxDate = new DateTime(2024, 6, 1);
            var faker = new Faker();

            var events = new Faker<Event>("de").CustomInstantiator(f => new Event(f.Music.Genre()))
                .Generate(5)
                .DistinctBy(e => e.Name)
                .ToList();
            Events.AddRange(events);
            SaveChanges();

            var shows = new Faker<Show>("de").CustomInstantiator(f =>
            {
                var date = f.Date.Between(minDate, maxDate).Date.AddHours(f.Random.Int(18, 20));
                return new Show(f.Random.ListItem(events), date);
            })
                .Generate(25)
                .ToList();
            Shows.AddRange(shows);
            SaveChanges();

            var guests = new Faker<Guest>("de").CustomInstantiator(f =>
            {
                return new Guest(
                    f.Name.FirstName(), f.Name.LastName(),
                    f.Date.BetweenDateOnly(new DateOnly(2000, 1, 1), new DateOnly(2005, 1, 1)));
            })
            .Generate(20)
            .ToList();
            Guests.AddRange(guests);
            SaveChanges();

            var contingents = shows.SelectMany(s => new Contingent[] {
                new Contingent(s, ContingentType.Floor, faker.Random.Int(3, 10)*10),
                new Contingent(s, ContingentType.Rang, faker.Random.Int(3, 10)*10),
                new Contingent(s, ContingentType.Stands, faker.Random.Int(3, 10)*10)})
                .ToList();
            Contingents.AddRange(contingents);

            // 30% of the contingents are sold out.
            var tickets = contingents.SelectMany(c =>
                GenerateTickets(
                    c, guests,
                    faker.Random.Bool(0.3f) ? c.AvailableTickets : faker.Random.Int(0, c.AvailableTickets)))
                .ToList();
            Tickets.AddRange(tickets);
            SaveChanges();
        }

        private IEnumerable<Ticket> GenerateTickets(Contingent contingent, List<Guest> guests, int personCount)
        {
            var guestNr = 0;
            var guestCount = guests.Count;
            var ticketsGenerator = new Faker<Ticket>("de").CustomInstantiator(f =>
            {
                var ticketState = f.Random.Enum<TicketState>();
                var reservationDate = contingent.Show.Date.AddHours(f.Random.Int(-21 * 24, -14 * 24));
                var guest = guests[guestNr];
                guestNr = (guestNr + 1) % guestCount;
                return new Ticket(
                    guest, contingent, ticketState,
                    reservationDate, f.Random.Int(0, 2));
            }).GenerateForever();
            int count = 0;
            while (count < personCount)
            {
                var ticket = ticketsGenerator.First();
                if (count + ticket.Pax + 1 > personCount - 1)
                    ticket.Pax = personCount - count - 1;
                count += ticket.Pax + 1;
                yield return ticket;
            }
        }
    }
}