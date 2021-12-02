using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstDemo.Application.Model
{
    public class Product
    {
        public Product(int ean, string name, ProductCategory productCategory)
        {
            Ean = Ean;
            Name = name;
            ProductCategoryId = productCategory.Id;
            ProductCategory = productCategory;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Product() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Ean is the PK and not an auto increment column. Annotations are used
        // for the next property (ean)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Ean { get; private set; }
        public string Name { get; set; }
        public int ProductCategoryId { get; set; }            // Value of the FK
        public ProductCategory ProductCategory { get; set; }  // Navigation property
    }
}
