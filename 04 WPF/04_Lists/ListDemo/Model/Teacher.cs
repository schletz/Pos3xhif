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
        /// <summary>
        /// Lehrer ID (z. B. ABC9999)
        /// </summary>
        public string TeacherNr { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}
