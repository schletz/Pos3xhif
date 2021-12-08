using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstDemo.Application.Model
{
    [Table("Store")]
    public class Store
    {
        public Store(string name)
        {
            Name = name;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Store() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        [MaxLength(255)]      // Produces NVARCHAR(255) in SQL Server
        public string Name { get; set; }
    }
}
