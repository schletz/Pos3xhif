namespace WienerLinienApp.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class HaltestellenDb : DbContext
    {
        public HaltestellenDb()
            : base("name=HaltestellenDb")
        {
        }

        public virtual DbSet<Haltestelle> Haltestelles { get; set; }
        public virtual DbSet<Linie> Linies { get; set; }
        public virtual DbSet<Steig> Steigs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Haltestelle>()
                .Property(e => e.H_Name)
                .IsUnicode(false);

            modelBuilder.Entity<Haltestelle>()
                .HasMany(e => e.Steigs)
                .WithRequired(e => e.Haltestelle)
                .HasForeignKey(e => e.S_Haltestelle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Linie>()
                .Property(e => e.L_Bezeichnung)
                .IsUnicode(false);

            modelBuilder.Entity<Linie>()
                .Property(e => e.L_Verkehrsmittel)
                .IsUnicode(false);

            modelBuilder.Entity<Linie>()
                .HasMany(e => e.Steigs)
                .WithRequired(e => e.Linie)
                .HasForeignKey(e => e.S_Linie)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Steig>()
                .Property(e => e.S_Steig)
                .IsUnicode(false);

            modelBuilder.Entity<Steig>()
                .Property(e => e.S_Richtung)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
