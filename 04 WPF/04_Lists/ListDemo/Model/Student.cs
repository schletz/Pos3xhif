using System;
using System.Collections.Generic;
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
        private static int lastId = 0;
        public Student(string firstname, string lastname,  Gender gender, Schoolclass schoolclass, DateTime? dateOfBirth = null)
        {
            Id = ++lastId;
            Firstname = firstname;
            Lastname = lastname;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Schoolclass = schoolclass;
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Schoolclass Schoolclass { get; set; }
    }
}
