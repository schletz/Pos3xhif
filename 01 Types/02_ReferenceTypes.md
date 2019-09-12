```c#
using System;

namespace ReferenceTypesApp
{
    class Person
    {
        public int age = 0;
    }
    // Entspricht Pupil extends Person
    class Pupil : Person
    {
        public string klasse = "";
    }

    class Program
    {
        static void Main(string[] args)
        {
            string myStr = null;

            if (myStr == null)
            {
                Console.WriteLine("myStr kann NULL sein, da es ein Referenztyp ist.");
            }
            // Ermittelt die Länge des Strings
            int len = myStr.Length;
            // In Java
            if (myStr == null)
            {
                len = 0;
            }
            else
            {
                len = myStr.Length;
            }
            // 1) myStr?.Length ist der "ternary conditional operator".
            //    Liefert NULL, wenn myStr NULL ist und keine Exception.
            // 2) ?? ist der NULL coalescing Operator. Er liefert 0, wenn
            //    der 1. Operand NULL ist.
            len = myStr?.Length ?? 0;
            // 1) IsNullOrEmpty ist eine statische Methode von string.
            // 2) Condition ? A : B ist die bedingte Zuweisung.
            len = string.IsNullOrEmpty(myStr) ? 0 : myStr.Length;

            // Eigene Typen
            // p zeigt noch auf keinen speicherbereich und ist daher
            // null.
            Person p;          // p ist null.
            p = new Person();  // Eine Person wird am Heap erstellt und
                               // die Referenzadresse in p geschrieben.
            Person p2 = p;     // Nun habe ich eine Instanz, auf die 2
                               // Referenzvariablen zeigen.
            p2.age = 18;       // p.age liefert natürlich auch 18.

            Pupil pu = new Pupil();
            Person p3;

            // "Hinaufcasten" ist möglich, da die Vererbung ja
            // eine "is-a" beziehung ist. 
            p3 = (Person)pu;    
            // p3.klasse = "3BHIF";
            // Alles ist von object abgeleitet.
            object obj1 = pu;
            // Natürlich nicht mehr möglich.
            // obj1.age = 12;
            // Das würde gehen, aber nur wenn obj1 ein
            // Pupil war.
            ((Pupil)obj1).age = 12;

            // Wenn pu nicht in Pupil umgewandelt werden
            // kann, wird NULL geliefert. In diesem Fall
            // wird eine neue Instanz von Pupil erstellt.
            p3 = pu as Pupil ?? new Pupil();
            // true, da is angibt, ob ein Typencast durchgeführt
            // werden kann.
            if (pu is Person)
            {
                Console.WriteLine("pu is Person.");
            }
            // In object gibt es die Methode GetType() und
            // ToString().
            // Liefert "ReferenceTypesApp.Pupil".
            string type = pu.GetType().ToString();

            Console.WriteLine("Hello World!");
        }
    }
}

```