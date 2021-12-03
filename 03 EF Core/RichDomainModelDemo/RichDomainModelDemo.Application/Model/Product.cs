using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Product")]
    public class Product
    {
        public Product(int ean, string name, ProductCategory productCategory)
        {
            Ean = Ean;
            Name = name;
            ProductCategoryId = productCategory.Id;
            ProductCategory = productCategory;
        }
#pragma warning disable CS8618
        protected Product() { }
#pragma warning restore CS8618
        // Ean is the PK and not an auto increment column. Annotations are used
        // for the next property (ean)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Ean { get; private set; }
        public string Name { get; set; }
        public int ProductCategoryId { get; set; }            // Value of the FK
        public virtual ProductCategory ProductCategory { get; set; }  // Navigation property

        public virtual ICollection<Offer> Offers { get; } = new List<Offer>();
        public decimal AveragePrice => Offers.Average(o => o.Price);
    }
}
