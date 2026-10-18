using DeliveryManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace DeliveryManager.Test;

[Collection("Sequential")]
public class DeliveryContextTests
{
    [Fact]
    public void T01_SeedCreatesMinimumNumberOfEntitiesTest()
    {
        using var db = GetSeededDbContext();

        Assert.True(db.Customers.Count() >= 3, "At least 3 customers expected");
        Assert.True(db.Restaurants.Count() >= 3, "At least 3 restaurants expected");
        Assert.True(db.Drivers.Count() >= 5, "At least 5 drivers expected");
        Assert.True(db.Products.Count() >= 3, "At least 3 products expected");
        Assert.True(db.Orders.Count() >= 10, "At least 10 orders expected");
        Assert.True(db.OrderItems.Count() >= 20, "At least 20 order items expected");
    }

    [Fact]
    public void T02_SeedCreatesValidRelationsBetweenEntitiesTest()
    {
        using var db = GetSeededDbContext();

        // Mindestens eine Order hat einen Driver
        Assert.True(
            db.Orders.Any(o => o.Driver != null));

        // Mindestens eine Order hat ein DeliveredAt date.
        Assert.True(
            db.Orders.Any(o => o.DeliveredAt != null));

        // Es gibt mindestens einen Fahrer ohne Orders
        Assert.True(
            db.Drivers.Include(d => d.Orders).Any(d => !d.Orders.Any()),
            "At least one driver without orders expected");

        // Alle OrderItems haben eine Menge 1–5
        Assert.True(
            db.OrderItems.All(o => o.Quantity >= 1 && o.Quantity <= 5),
            "All order items should have quantity between 1 and 5");

        // Jede Order hat mindestens ein OrderItem
        Assert.True(
            db.Orders.Include(o => o.OrderItems).All(o => o.OrderItems.Any()),
            "Every order should have at least one order item");
    }

    [Fact]
    public void T03_SeedCreatesOrdersWithValidBusinessRules()
    {
        using var db = GetSeededDbContext();

        // Es gibt Orders ohne Fahrer
        Assert.True(
            db.Orders.Any(o => o.Driver == null),
            "At least one order without driver expected");

        // Es gibt Orders ohne DeliveredAt
        Assert.True(
            db.Orders.Any(o => o.DeliveredAt == null),
            "At least one undelivered order expected");

        // Keine Order ist geliefert, aber ohne Fahrer
        Assert.False(
            db.Orders.Any(o => o.DeliveredAt != null && o.Driver == null),
            "No delivered order should be without driver");

        // Alle Orders sind 2024
        Assert.True(
            db.Orders.All(o => o.OrderDate.Year == 2024),
            "All orders should be from year 2024");

        // Zeitspanne zwischen OrderDate und DeliveredAt
        var deliveredOrders = db.Orders
            .Where(o => o.DeliveredAt != null)
            .ToList();

        Assert.True(
            deliveredOrders.All(o =>
                o.OrderDate >= o.DeliveredAt!.Value.AddHours(-2) &&
                o.OrderDate <= o.DeliveredAt!.Value.AddMinutes(-10)),
            "Delivered orders must be within 2h and 10min before DeliveredAt");
    }


    private DeliveryContext GetSeededDbContext()
    {
        var slnDirectory = GetSolutionDirectoryInfo();
        var database = Path.Combine(slnDirectory.FullName, "delivery.db");
        var options = new DbContextOptionsBuilder()
            .UseSqlite($"DataSource={database}")
            .Options;

        var db = new DeliveryContext(options);
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Seed();
        return db;
    }

    public DirectoryInfo GetSolutionDirectoryInfo()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any())
            directory = directory.Parent;

        return directory ?? new DirectoryInfo(Directory.GetCurrentDirectory());
    }
}
