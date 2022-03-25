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
        /// <summary>
        /// Bezeichnung (1AHIF, ...)
        /// </summary>
        public string ClassNr { get; set; }
        /// <summary>
        /// Stammraum.
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// Klassenvorstand.
        /// </summary>
        public Teacher KV { get; set; }
        public List<Pupil> Pupils { get; set; }
    }
}
