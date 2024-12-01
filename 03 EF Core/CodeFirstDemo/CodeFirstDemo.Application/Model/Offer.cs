using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstDemo.Application.Model
{
    [Table("Offer")]       // EF Core Annotation for a specific table name.
    public class Offer
    {
        public Offer(Product product, Store store, decimal price, DateTime lastUpdate)
        {
            Product = product;
            Store = store;
            Price = price;
            LastUpdate = lastUpdate;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Offer() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }          // private set, EF Core does not support changing the PK.
        [ForeignKey("ProductEan")]                   // EF Core Annotation for a specific foreign key name.
        public Product Product { get; set; }        // Navigation property
        [ForeignKey("StoreId")]                     // EF Core Annotation for a specific foreign key name.
        public Store Store { get; set; }            // Navigation property
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
