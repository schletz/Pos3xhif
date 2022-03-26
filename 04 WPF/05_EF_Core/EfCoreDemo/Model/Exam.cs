using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für eine Prüfung.
    /// </summary>
    public class Exam
    {
        private static int lastId = 0;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Exam() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        [MaxLength(8)]
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
