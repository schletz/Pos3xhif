using Microsoft.EntityFrameworkCore;

namespace WienerLinien.Model
{
    public class LinienContext : DbContext
    {
        public DbSet<Linie> Linien { get; set; }
        public DbSet<Haltestelle> Haltestellen { get; set; }
        public DbSet<Steig> Steige { get; set; }
        public DbSet<Config> Configs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Linien.db");
        }
    }
}