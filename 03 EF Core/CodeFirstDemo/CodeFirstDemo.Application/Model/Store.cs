using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstDemo.Application.Model
{
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
        public string Name { get; set; }
    }
}
