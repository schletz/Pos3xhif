using System;
using System.Collections.Generic;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für eine Schulklasse.
    /// </summary>
    public class Schoolclass
    {
        public Schoolclass(string name, string room, Teacher kV)
        {
            Name = name;
            Room = room;
            KV = kV;
        }

        /// <summary>
        /// Bezeichnung (1AHIF, ...)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Stammraum.
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// Klassenvorstand.
        /// </summary>
        public Teacher KV { get; set; }
    }
}
