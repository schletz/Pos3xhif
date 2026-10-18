using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReader
{
    // *********************************************************************************************
    // KLASSE SCHUELER
    // *********************************************************************************************
    /// <summary>
    /// Datenhaltende Klasse für den Schüler.
    /// </summary>
    class Schueler
    {
        public int Id { get; set; }
        public string Zuname { get; set; } = "";
        public string Vorname { get; set; } = "";
        public string Email { get; set; } = "";
        [TypedReader(IgnoreProperty = true)]            // Ignoriert das Property Klasse beim Lesen.
        public string Klasse { get; set; } = "";
        public DateTime Gebdat { get; set; }
        public int Besuchsjahr { get; set; }
        public double Notenschnitt { get; set; }
    }
}
