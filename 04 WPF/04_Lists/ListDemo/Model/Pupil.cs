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
    public class Pupil
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Schoolclass Schoolclass { get; set; }
        public List<Exam> Exams { get; set; }
    }
}
