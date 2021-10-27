using System;
using System.Collections.Generic;
using System.Text;

namespace LinqUebung1.Application.Model
{
    /// <summary>
    /// Speichert eine Prüfung eines Schülers.
    /// </summary>
    internal class Exam
    {
        public Exam(string subject, string examinator, int grade, DateTime date, Student student)
        {
            Subject = subject;
            Examinator = examinator;
            Grade = grade;
            Date = date;
            Student = student;
        }

        private Exam()   // For EF Core
        {
        }

        public int Id { get; private set; }
        public string Subject { get; private set; } = default!;    // For EF Core
        public string Examinator { get; private set; } = default!; // For EF Core
        public int Grade { get; set; }
        public DateTime Date { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Student Student { get; private set; } = default!; // For EF Core
    }
}