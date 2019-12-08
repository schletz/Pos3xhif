using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelDemoApp.Model
{
    /// <summary>
    /// Simuliert eine Datenbank mit einer Tabelle Personen.
    /// </summary>
    public class PersonDb
    {
        /// <summary>
        /// Tabelle mit den Personendaten.
        /// </summary>
        public ICollection<Person> Persons { get; private set; }
        /// <summary>
        /// Verhindert eine direkte Instanzierung mit einer null Liste.
        /// </summary>
        private PersonDb() { }
        /// <summary>
        /// Füllt die Instanz mit Musterdaten und gibt sie zurück.
        /// </summary>
        /// <returns></returns>
        public static PersonDb FromMockup()
        {
            PersonDb personDb = new PersonDb();
            personDb.Persons = new List<Person>();

            personDb.Persons.Add(new Person { Firstname = "Vorname1", Lastname = "Nachname1", DateOfBirth = new DateTime(2000, 1, 21), Sex = Sex.Male });
            personDb.Persons.Add(new Person { Firstname = "Vorname2", Lastname = "Nachname2", DateOfBirth = new DateTime(2001, 1, 22), Sex = Sex.Male });
            personDb.Persons.Add(new Person { Firstname = "Vorname3", Lastname = "Nachname3", DateOfBirth = new DateTime(2002, 1, 23), Sex = Sex.Female });
            personDb.Persons.Add(new Person { Firstname = "Vorname4", Lastname = "Nachname4", DateOfBirth = new DateTime(2003, 1, 24), Sex = Sex.Male });
            personDb.Persons.Add(new Person { Firstname = "Vorname5", Lastname = "Nachname5", DateOfBirth = new DateTime(2004, 1, 25), Sex = Sex.Female });
            return personDb;
        }
    }
}
