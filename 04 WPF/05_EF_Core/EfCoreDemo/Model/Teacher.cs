using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ListDemo.Model
{
    /// <summary>
    /// Entity Klasse für einen Lehrer.
    /// </summary>
    public class Teacher
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Teacher() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        [Key]
        [MaxLength(8)]
        public string TeacherNr { get; set; }
        [MaxLength(255)]
        public string Firstname { get; set; }
        [MaxLength(255)]
        public string Lastname { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
    }
}
