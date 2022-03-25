using ListDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDemo.Dto
{
    /// <summary>
    /// DTO Klasse für den Schüler
    /// Wenn wir einen neuen Schüler eingeben, müssen wir eine Instanz als Ziel für das Binding
    /// erstellen. Die Schülerklasse hat aber einen Konstruktor. Daher verwenden wir eine DTO
    /// Klasse mit offenen Settern.
    /// Da wir nicht wissen, was initialisiert wird, ist jedes Property nullable.
    /// </summary>
    public class StudentDto
    {
        public int Id { get; set; }  // Default Wert 0
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public Schoolclass? Schoolclass { get; set; }
    }
}
