using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace QuerySyntax.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class TestsContext : DbContext
    {
        public TestsContext()
        {
        }

        public TestsContext(DbContextOptions<TestsContext> options)
            : base(options)
        {
        }
        [JsonProperty]
        public virtual DbSet<Lesson> Lesson { get; set; }
        [JsonProperty]
        public virtual DbSet<Period> Period { get; set; }
        [JsonProperty]
        public virtual DbSet<Pupil> Pupil { get; set; }
        [JsonProperty]
        public virtual DbSet<Schoolclass> Schoolclass { get; set; }
        [JsonProperty]
        public virtual DbSet<Teacher> Teacher { get; set; }
        [JsonProperty]
        public virtual DbSet<Test> Test { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.LogTo(Console.WriteLine);
                optionsBuilder.UseSqlite("DataSource=db/Tests.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.L_ID);

                entity.HasIndex(e => e.L_Class)
                    .HasDatabaseName("SchoolclassLesson");

                entity.HasIndex(e => e.L_Hour)
                    .HasDatabaseName("PeriodLesson");

                entity.HasIndex(e => e.L_Teacher)
                    .HasDatabaseName("TeacherLesson");

                entity.HasIndex(e => e.L_Untis_ID)
                    .HasDatabaseName("idx_L_Untis_ID");

                entity.Property(e => e.L_ID).ValueGeneratedOnAdd();

                entity.Property(e => e.L_Class)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.L_Day).HasDefaultValueSql("0");

                entity.Property(e => e.L_Hour).HasDefaultValueSql("0");

                entity.Property(e => e.L_Room).HasColumnType("VARCHAR(8)");

                entity.Property(e => e.L_Subject)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.L_Teacher)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.L_Untis_ID).HasDefaultValueSql("0");

                entity.HasOne(d => d.L_ClassNavigation)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.L_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.L_HourNavigation)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.L_Hour);

                entity.HasOne(d => d.L_TeacherNavigation)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.L_Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.HasKey(e => e.P_Nr);

                entity.Property(e => e.P_Nr).ValueGeneratedNever();

                entity.Property(e => e.P_From).HasColumnType("TIMESTAMP");

                entity.Property(e => e.P_To).HasColumnType("TIMESTAMP");
            });

            modelBuilder.Entity<Pupil>(entity =>
            {
                entity.HasKey(e => e.P_ID);

                entity.HasIndex(e => e.P_Account)
                    .HasDatabaseName("idx_P_Account")
                    .IsUnique();

                entity.HasIndex(e => e.P_Class)
                    .HasDatabaseName("SchoolclassPupil");

                entity.Property(e => e.P_ID).ValueGeneratedOnAdd();

                entity.Property(e => e.P_Account)
                    .IsRequired()
                    .HasColumnType("VARCHAR(16)");

                entity.Property(e => e.P_Class)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.P_Firstname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.Property(e => e.P_Lastname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.HasOne(d => d.P_ClassNavigation)
                    .WithMany(p => p.Pupils)
                    .HasForeignKey(d => d.P_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Schoolclass>(entity =>
            {
                entity.HasKey(e => e.C_ID);

                entity.HasIndex(e => e.C_ClassTeacher)
                    .HasDatabaseName("TeacherSchoolclass");

                entity.Property(e => e.C_ID).HasColumnType("VARCHAR(8)");

                entity.Property(e => e.C_ClassTeacher).HasColumnType("VARCHAR(8)");

                entity.Property(e => e.C_Department)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.HasOne(d => d.C_ClassTeacherNavigation)
                    .WithMany(p => p.Schoolclasses)
                    .HasForeignKey(d => d.C_ClassTeacher);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.T_ID);

                entity.HasIndex(e => e.T_Account)
                    .HasDatabaseName("idx_T_Account")
                    .IsUnique();

                entity.Property(e => e.T_ID).HasColumnType("VARCHAR(8)");

                entity.Property(e => e.T_Account)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.Property(e => e.T_Email).HasColumnType("VARCHAR(255)");

                entity.Property(e => e.T_Firstname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.Property(e => e.T_Lastname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.TE_ID);

                entity.HasIndex(e => e.TE_Class)
                    .HasDatabaseName("SchoolclassTest");

                entity.HasIndex(e => e.TE_Lesson)
                    .HasDatabaseName("PeriodTest");

                entity.HasIndex(e => e.TE_Teacher)
                    .HasDatabaseName("TeacherTest");

                entity.Property(e => e.TE_ID).ValueGeneratedOnAdd();

                entity.Property(e => e.TE_Class)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.TE_Date)
                    .IsRequired()
                    .HasColumnType("TIMESTAMP");


                entity.Property(e => e.TE_Subject)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.Property(e => e.TE_Teacher)
                    .IsRequired()
                    .HasColumnType("VARCHAR(8)");

                entity.HasOne(d => d.TE_ClassNavigation)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.TE_Class)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TE_LessonNavigation)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.TE_Lesson)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TE_TeacherNavigation)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.TE_Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
