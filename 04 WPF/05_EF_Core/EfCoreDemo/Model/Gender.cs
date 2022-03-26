using System.ComponentModel.DataAnnotations;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für das Geschlecht
    /// </summary>
    public class Gender
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Gender() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Gender(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        [MaxLength(16)]
        public string Name { get; set; }
    }
}
