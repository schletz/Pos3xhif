using Microsoft.EntityFrameworkCore;

namespace EfCoreInheritance.Application.Infrastructure;

public class UniversityContext : DbContext
{
    // TODO: Add your DbSets

    public UniversityContext(DbContextOptions opt) : base(opt)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Add your configuration
    }
}