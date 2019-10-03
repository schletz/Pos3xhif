using System;
using System.Collections.Generic;
using System.IO;         // Für den StreamReader
using System.Linq;
using System.Reflection; // Für PropertyInfo
using System.Text;

namespace FileReader
{
    // *********************************************************************************************
    // KLASSE TYPEDREADER
    // *********************************************************************************************

    /// <summary>
    /// Stellt einen generischen Reader zur Verfügung, der Zeilen einer Textdatei als Instanzen
    /// vom Typ T zurückgibt. Dabei wird die 1. Zeile der Textdatei als Überschrift betrachtet.
    /// Die dort angegebenen Spaltennamen werden den einzelnen Properties zugewiesen.
    /// </summary>
    /// <typeparam name="T">Ein Referenztyp, der zurückgegeben wird. Durch where T: new()
    /// schränken wir den Typ auf Referenztypen ein. Sonst können wir im Code keine Instanz
    /// von T erstellen.</typeparam>
    class TypedReader<T> : StreamReader where T : new()
    {
        public char Delimiter { get; set; } = ',';
        public bool Headings { get; set; } = false;
        public int CurrentRowNr { get; private set; } = 0;
        /// <summary>
        /// Speichert die Namen der Properties, die wir befüllen sollen.
        /// </summary>
        private PropertyInfo[] propertyInfos = null;
        /// <summary>
        /// Speichert die dazugehörigen Annotations, also ob 
        /// [TypedReaderAttribute(IgnoreProperty = true)]
        /// gesetzt ist.
        /// </summary>
        private TypedReaderAttribute[] attributes = null;
        private int propertyCount = 0;

        /// <summary>
        /// Liest eine Zeile von der aktuellen Position und schreibt die Werte in die Properties.
        /// </summary>
        /// <returns>Eine Instanz des angegebenen Typs, bei dem die Properties mit den Inhalten
        /// der Textdatei wenn möglich (Format!) befüllt werden.</returns>
        public T ReadTypedLine()
        {
            try
            {
                // Müssen wir noch lesen, welche Properties der Typ hat, den wir zurück geben?
                if (propertyInfos == null) readProperties();
                string[] cols = new string[0];
                T result = new T();

                // Sollen wir die erste Zeile überlesen?
                if (CurrentRowNr == 0 && Headings) { base.ReadLine(); }
                CurrentRowNr++;
                // Wenn die Zeile nicht gelesen werden kann (es kann immer was schiefgehen), geben wir
                // den default Wert des Typs (also immer null, da wir nur Referenztypen als T erlauben)
                // zurück.
                cols = base.ReadLine().Split(Delimiter);

                int i = 0;
                // Die erste Spalte wird dem ersten Property, was wir nicht ignorieren sollen,
                // zugewiesen, usw.
                foreach (string col in cols.Take(propertyCount))
                {
                    try
                    {
                        // Hier kann immer was passieren (Formatfehler, leere Werte, ...)
                        propertyInfos[i].SetValue(result,
                            Convert.ChangeType(col, propertyInfos[i].PropertyType));
                    }
                    catch { }
                    i++;                         // Bitte nicht im try Block als letzte Anweisung!!!
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Fehler beim Lesen der Zeile.", e);
            }
        }

        /// <summary>
        /// Liest alle Zeilen der Eingabedatei.
        /// </summary>
        /// <returns>Ein Array mit einem erzeugten Objekt pro Zeile.</returns>
        public T[] ReadAllLines()
        {
            List<T> result = new List<T>();
            while (!EndOfStream)
            {
                T row = ReadTypedLine();
                // Wir wollen keine leeren Zeilen.
                if (row != null) { result.Add(row); }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Liest alle nicht statischen public Properties des Typs aus, der zurückgegeben werden 
        /// soll. Dabei werden auch die Annotations erfasst. Ist das Attribut IgnoreProperty 
        /// gesetzt, wird dieses übergangen.
        /// </summary>
        private void readProperties()
        {
            try
            {
                int i = 0;
                // Alle Properties auslesen, die als Annotation nicht mit Ignore Properties versehen
                // sind.
                var properties = (from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      // GetCustomAttribute liefert NULL, wenn keine Annotation vorhanden ist.
                                  let attr = p.GetCustomAttribute<TypedReaderAttribute>() ?? new TypedReaderAttribute()
                                  where attr.IgnoreProperty == false
                                  select new
                                  {
                                      PropertyInfo = p,
                                      Attribute = attr
                                  }).ToList();

                PropertyInfo[] propertyInfos = new PropertyInfo[properties.Count];
                TypedReaderAttribute[] attributes = new TypedReaderAttribute[properties.Count];
                properties.ForEach(p =>
                {
                    propertyInfos[i] = p.PropertyInfo; attributes[i] = p.Attribute; i++;
                });

                this.propertyCount = properties.Count;
                this.propertyInfos = propertyInfos;
                this.attributes = attributes;
            }
            catch (Exception e)
            {
                throw new Exception($"Keine Properties im Typ {typeof(T)} gefunden.", e);
            }
        }

        public TypedReader(Stream stream) : base(stream) { }
        public TypedReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks) { }
        public TypedReader(Stream stream, Encoding encoding) : base(stream, encoding) { }
        public TypedReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks) { }
        public TypedReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }
        public TypedReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen) { }
        public TypedReader(string path) : base(path) { }
        public TypedReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks) { }
        public TypedReader(string path, Encoding encoding) : base(path, encoding) { }
        public TypedReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks) { }
        public TypedReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }
    }
}
