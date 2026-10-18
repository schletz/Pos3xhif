using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelDemoApp.Model
{
    public enum Sex { Male, Female}
    /// <summary>
    /// Data Transfer Object (DTO) für einen Personendatensatz.
    /// </summary>
    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Sex Sex { get; set; }
    }
}
