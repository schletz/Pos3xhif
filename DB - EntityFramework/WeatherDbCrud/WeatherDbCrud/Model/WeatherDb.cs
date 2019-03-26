namespace WeatherDbCrud.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WeatherDb : DbContext
    {
        public WeatherDb()
            : base("name=WeatherDb")
        {
        }

        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<Station> Stations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Measurement>()
                .Property(e => e.M_Temperature)
                .HasPrecision(4, 1);

            modelBuilder.Entity<Station>()
                .Property(e => e.S_Location)
                .IsUnicode(false);

            modelBuilder.Entity<Station>()
                .HasMany(e => e.Measurements)
                .WithRequired(e => e.Station)
                .HasForeignKey(e => e.M_Station)
                .WillCascadeOnDelete(false);
        }
    }
}
