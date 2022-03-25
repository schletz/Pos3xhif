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
        private static int lastId = 0;

        public Exam(string subject, DateTime date, Teacher examiner, Student student, int? grade = null)
        {
            Id = ++lastId;
            Subject = subject;
            Date = date;
            Examiner = examiner;
            Grade = grade;
            Student = student;
        }
        public int Id { get; set; }
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
        public Student Student { get; set; }
    }
}
