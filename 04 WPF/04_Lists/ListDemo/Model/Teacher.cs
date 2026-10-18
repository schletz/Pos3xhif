using System;
using System.Collections.Generic;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für einen Lehrer.
    /// </summary>
    public class Teacher
    {
        public Teacher(string teacherNr, string firstname, string lastname, string email)
        {
            TeacherNr = teacherNr;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
        }

        /// <summary>
        /// Lehrer ID (z. B. ABC9999)
        /// </summary>
        public string TeacherNr { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}
