using Microsoft.EntityFrameworkCore;
using System.Linq;
using TeamsManager.Application.Model;

namespace TeamsManager.Application.Infrastructure;

public class TeamsContext : DbContext
{
    // TODO: Füge hier deine DbSets ein.

    public TeamsContext(DbContextOptions opt) : base(opt)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tabellen-Namensschema anwenden (kleingeschriebene Klassennamen)
        // Sonst laufen die Grading Tests nicht durch.
        // Muss die letzte Anweisung sein.
        ApplyNamingConventions(modelBuilder);
    }
    private void ApplyNamingConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => !t.IsOwned()))
        {
            var clrName = entityType.ClrType.Name;
            entityType.SetTableName(clrName);
        }
    }
}
