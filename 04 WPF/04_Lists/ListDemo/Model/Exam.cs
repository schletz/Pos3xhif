using System;
using System.Collections.Generic;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für eine Prüfung.
    /// </summary>
    public class Exam
    {
        /// <summary>
        /// Gegenstand
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Datum der Prüfung
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Prüfer
        /// </summary>
        public Teacher Examiner { get; set; }
        /// <summary>
        /// Note. Kann auch NULL sein, wenn noch nicht geprüft wurde.
        /// </summary>
        public int? Grade { get; set; }
        /// <summary>
        /// Geprüfter Schüler.
        /// </summary>
        public Pupil Pupil { get; set; }
    }
}
