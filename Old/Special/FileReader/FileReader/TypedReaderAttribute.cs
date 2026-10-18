using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReader
{
    // *********************************************************************************************
    // KLASSE TYPEDREADERATTRIBUTE
    // *********************************************************************************************
    /// <summary>
    /// Ein Benutzerdefiniertes Attribut, welches wir über jedes Property einer Klasse
    /// schreiben können. Es muss pro Attribut eine Klasse definiert werden, die sich von
    /// Attribute ableitet. Das Wort "Attribute" wird bei der Verwendung angehängt, also
    /// [TypedReader(IgnoreProperty = true)] wird zu [TypedReaderAttribute(IgnoreProperty = true)]
    /// 
    /// Hier kann noch einiges erweitert werden (Angabe der dazugehörigen Spalte in der Textdatei,
    /// Formate, ...).
    /// </summary>
    class TypedReaderAttribute : Attribute
    {
        /// <summary>
        /// Übergeht das Property bei der Befüllung aus einer Textdatei.
        /// </summary>
        public bool IgnoreProperty { get; set; } = false;
    }
}
