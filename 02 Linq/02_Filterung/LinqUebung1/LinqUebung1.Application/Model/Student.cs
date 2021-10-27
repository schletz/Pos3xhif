using System;
using System.Collections.Generic;
using System.Text;

namespace LinqUebung1.Application.Model
{
    /// <summary>
    /// Speichert die Schülerdaten und eine Liste aller Prüfungen des Schülers.
    /// </summary>
    internal class Student
    {
        public Student(int id, string lastname, string firstname, Gender gender, string schoolclass)
        {
            Id = id;
            Lastname = lastname;
            Firstname = firstname;
            Gender = gender;
            Schoolclass = schoolclass;
        }

        private Student()
        {
        }

        public int Id { get; private set; }
        public string Lastname { get; private set; } = default!; // for EF Core
        public string Firstname { get; private set; } = default!; // for EF Core
        public Gender Gender { get; private set; } = default!; // for EF Core
        public string Schoolclass { get; private set; } = default!; // for EF Core

        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Exam> Exams { get; } = new List<Exam>();
    }
}