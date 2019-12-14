using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Queries.Model
{
    public partial class TestsContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public TestsContext()
        {
        }

        public TestsContext(DbContextOptions<TestsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<Period> Period { get; set; }
        public virtual DbSet<Pupil> Pupil { get; set; }
        public virtual DbSet<Schoolclass> Schoolclass { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<Test> Test { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLoggerFactory(MyLoggerFactory) // Warning: Do not create a new ILoggerFactory instance each time                    
                    .UseSqlite("DataSource=../Tests.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasIndex(e => e.L_Class)
                    .HasName("SchoolclassLesson");

                entity.HasIndex(e => e.L_Hour)
                    .HasName("PeriodLesson");

                entity.HasIndex(e => e.L_Teacher)
                    .HasName("TeacherLesson");

                entity.HasIndex(e => e.L_Untis_ID)
                    .HasName("idx_L_Untis_ID");

                entity.Property(e => e.L_ID).ValueGeneratedOnAdd();

                entity.Property(e => e.L_Day).HasDefaultValueSql("0");

                entity.Property(e => e.L_Hour).HasDefaultValueSql("0");

                entity.Property(e => e.L_Untis_ID).HasDefaultValueSql("0");

                entity.HasOne(d => d.L_ClassNavigation)
                    .WithMany(p => p.Lesson)
                    .HasForeignKey(d => d.L_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.L_TeacherNavigation)
                    .WithMany(p => p.Lesson)
                    .HasForeignKey(d => d.L_Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.Property(e => e.P_Nr).ValueGeneratedNever();
            });

            modelBuilder.Entity<Pupil>(entity =>
            {
                entity.HasIndex(e => e.P_Account)
                    .HasName("idx_P_Account")
                    .IsUnique();

                entity.HasIndex(e => e.P_Class)
                    .HasName("SchoolclassPupil");

                entity.Property(e => e.P_ID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.P_ClassNavigation)
                    .WithMany(p => p.Pupil)
                    .HasForeignKey(d => d.P_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Schoolclass>(entity =>
            {
                entity.HasIndex(e => e.C_ClassTeacher)
                    .HasName("TeacherSchoolclass");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasIndex(e => e.T_Account)
                    .HasName("idx_T_Account")
                    .IsUnique();
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasIndex(e => e.TE_Class)
                    .HasName("SchoolclassTest");

                entity.HasIndex(e => e.TE_Lesson)
                    .HasName("PeriodTest");

                entity.HasIndex(e => e.TE_Teacher)
                    .HasName("TeacherTest");

                entity.Property(e => e.TE_ID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.TE_ClassNavigation)
                    .WithMany(p => p.Test)
                    .HasForeignKey(d => d.TE_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TE_LessonNavigation)
                    .WithMany(p => p.Test)
                    .HasForeignKey(d => d.TE_Lesson)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TE_TeacherNavigation)
                    .WithMany(p => p.Test)
                    .HasForeignKey(d => d.TE_Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
