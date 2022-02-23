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
        public ICollection<Person> Persons { get; private set; } = new List<Person>();
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

            personDb.Persons.Add(new Person (firstname: "Vorname1", lastname: "Nachname1", dateOfBirth: new DateTime(2000, 1, 21), sex: Sex.Male));
            personDb.Persons.Add(new Person (firstname: "Vorname2", lastname: "Nachname2", dateOfBirth: new DateTime(2001, 1, 22), sex: Sex.Male));
            personDb.Persons.Add(new Person (firstname: "Vorname3", lastname: "Nachname3", dateOfBirth: new DateTime(2002, 1, 23), sex: Sex.Female));
            personDb.Persons.Add(new Person (firstname: "Vorname4", lastname: "PersonNachname4", dateOfBirth: new DateTime(2003, 1, 24), sex: Sex.Male));
            personDb.Persons.Add(new Person (firstname: "Vorname5", lastname: "PersonNachname5", dateOfBirth: new DateTime(2004, 1, 25), sex: Sex.Female));
            return personDb;
        }
    }
}
