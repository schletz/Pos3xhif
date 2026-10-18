using Microsoft.EntityFrameworkCore;

namespace TeamsManager.Application.Infrastructure;

public class StoreContext : DbContext
{

    // TODO: Add your DbSets

    public StoreContext(DbContextOptions opt) : base(opt)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Add your config
    }
}
