using System;
using System.Collections.Generic;
using System.Text;

namespace SPG.LambdaTutorial
{
    /// <summary>
    /// Representiert ein Entity Schüler
    /// </summary>
    public class Schueler
    {
        /// <summary>
        /// Gibt die eindeutige Nummer eines Schülers zurück oder legt diese fest.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gibt den Nachnamen eines Schülers zurück oder legt diesen fest.
        /// </summary>
        public string Nachame { get; set; }

        /// <summary>
        /// Gibt den Vornamen eines Schülers zurück, oder legt diesen fest.
        /// </summary>
        public string Vorname { get; set; }

        /// <summary>
        /// Gibt die Klasse eines  Schüler zurück, oder legt diese fest.
        /// </summary>
        public string Klasse { get; set; }
    }
}
