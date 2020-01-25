using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BogusTest.Model
{
    public partial class SempruefContext : DbContext
    {
        public SempruefContext()
        {
        }

        public SempruefContext(DbContextOptions<SempruefContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Fach> Fach { get; set; }
        public virtual DbSet<Klasse> Klasse { get; set; }
        public virtual DbSet<Lehrer> Lehrer { get; set; }
        public virtual DbSet<Schueler> Schueler { get; set; }
        public virtual DbSet<Sempruef> Sempruef { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"DataSource=Sempruef.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Klasse>(entity =>
            {
                entity.HasIndex(e => e.K_KV)
                    .HasName("LehrerKlasse");
            });

            modelBuilder.Entity<Schueler>(entity =>
            {
                entity.HasIndex(e => e.S_Klasse)
                    .HasName("KlasseSchueler");

                entity.Property(e => e.S_Nr).ValueGeneratedNever();

                entity.HasOne(d => d.S_KlasseNavigation)
                    .WithMany(p => p.Schueler)
                    .HasForeignKey(d => d.S_Klasse)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Sempruef>(entity =>
            {
                entity.HasIndex(e => e.SP_Fach)
                    .HasName("FachSempruef");

                entity.HasIndex(e => e.SP_Lehrer)
                    .HasName("LehrerSempruef");

                entity.HasIndex(e => e.SP_Schueler)
                    .HasName("SchuelerSempruef");

                entity.Property(e => e.SP_Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SP_Note).HasDefaultValueSql("0");

                entity.Property(e => e.SP_Schueler).HasDefaultValueSql("0");

                entity.HasOne(d => d.SP_FachNavigation)
                    .WithMany(p => p.Sempruef)
                    .HasForeignKey(d => d.SP_Fach)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SP_LehrerNavigation)
                    .WithMany(p => p.Sempruef)
                    .HasForeignKey(d => d.SP_Lehrer)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
