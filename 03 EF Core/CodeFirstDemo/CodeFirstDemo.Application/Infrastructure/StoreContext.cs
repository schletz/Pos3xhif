using CodeFirstDemo.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstDemo.Application.Infrastructure
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions opt) : base(opt) { }
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    }
}
