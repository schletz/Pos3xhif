using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für eine Schulklasse.
    /// </summary>
    public class Schoolclass
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Schoolclass() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Schoolclass(string name, string room, Teacher kV)
        {
            Name = name;
            Room = room;
            KV = kV;
        }

        /// <summary>
        /// Bezeichnung (1AHIF, ...)
        /// </summary>
        [Key]
        [MaxLength(8)]
        public string Name { get; set; }
        /// <summary>
        /// Stammraum.
        /// </summary>
        [MaxLength(16)]
        public string Room { get; set; }
        /// <summary>
        /// Klassenvorstand.
        /// </summary>
        public Teacher KV { get; set; }
    }
}
