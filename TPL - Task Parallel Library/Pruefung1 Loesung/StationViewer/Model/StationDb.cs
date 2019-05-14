namespace StationViewer.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class StationDb : DbContext
    {
        public StationDb()
            : base("name=StationDb")
        {
        }

        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Value> Values { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Station>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Station>()
                .Property(e => e.BUNDESLAND)
                .IsUnicode(false);

            modelBuilder.Entity<Station>()
                .HasMany(e => e.Values)
                .WithRequired(e => e.Station1)
                .HasForeignKey(e => e.STATION)
                .WillCascadeOnDelete(false);
        }
    }
}
