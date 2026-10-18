using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für Schüler.
    /// </summary>
    public class Student
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Student() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Student(string firstname, string lastname, Gender gender, Schoolclass schoolclass, DateTime? dateOfBirth = null)
        {
            Firstname = firstname;
            Lastname = lastname;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Schoolclass = schoolclass;
        }

        public int Id { get; set; }
        [MaxLength(255)]
        public string Firstname { get; set; }
        [MaxLength(255)]
        public string Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Schoolclass Schoolclass { get; set; }
    }
}
