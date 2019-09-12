## Pupil.cs
```c#
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassDemo
{
    class Pupil
    {
        // Oftmals wird 1:1 zugewiesen, was so aussehen würde:
        private string vorname2;
        public string Vorname2
        {
            get { return vorname2; }
            set { vorname2 = value; }
        }
        // Default Properties.
        // Erstellt eine Variable + einen "Default Setter und Getter"
        public string Vorname { get; set; }
        // Hinweis: Eine Initialisierung ist bei Referenztypen von Vorteil.
        // Mit = wird das Property initialisiert.
        public string Zuname { get; set; } = "";

        // Dieses Property wird berechnet. Es macht also wenig Sinn,
        // hier einen Wert zuzuweisen. Deswegen machen wir nur ein
        // get{}, d. h. ich kann dem Property nichts zuweisen.
        public string Longname
        {
            get { return $"{Vorname} {Zuname}"; }
        }

        // Alternative, werden wir noch kennen lernen.
        public string Longname2 => $"{Vorname} {Zuname}";

        private int alter;
        // In get {} steht der Inhalt des Getters und wird 1:1 ausgeführt.
        // Natürlich muss er etwas zurückliefern.
        // In set {} steht der Inhalt des Setters mit der Besonderheit, dass
        // der übergebene Wert in value steht.
        public int Alter
        {
            get { return alter; }
            // VORSICHT: Liefert eine endlose Rekursion:
            // get { return Alter; }
            set { alter = value >= 0 ? value : throw new ArgumentException("Ungültiges Alter!"); }
        }
    }

    class PupilJava
    {
        string vorname;
        string zuname;
        int alter;

        public PupilJava(string vorname, string zuname)
        {
            this.vorname = vorname;
            this.zuname = zuname;
            setAlter(0);
        }

        public PupilJava(string vorname, string zuname, int alter)
        {
            this.vorname = vorname;
            this.zuname = zuname;
            setAlter(alter);
        }

        public void setAlter(int alter)
        {
            if (alter >= 0)
            {
                this.alter = alter;
            }
            else
            {
                throw new ArgumentException("Ungültiges Alter");
            }
        }

        public int getAlter()
        {
            return alter;
        }
    }
}
```

## Program.cs
```c#
using System;

namespace ClassDemo
{

    class Program
    {
        static void Main(string[] args)
        {
            // Geht nicht, da kein Standardkonstruktor
            // mehr generiert wird.
            // PupilJava pj = new PupilJava();
            PupilJava pj = new PupilJava("VN1", "ZN1");
            int age = pj.getAlter();   // Liefert 0
            pj.setAlter(18);

            // Der Initializer kann Properties beim Instanzieren mit einem
            // Wert belegen.
            Pupil p = new Pupil() { Vorname = "VN1", Zuname = "ZN1" };

            // Hier wird wie erwartet eine ArgumentException geworfen, da die
            // set Methode des Properties Alter verwendet wird.
            try
            {
                Pupil p2 = new Pupil() { Vorname = "VN1", Zuname = "ZN1", Alter = -1 };
            }
            catch { }
            // Auf p2 kann nicht zugegriffen werden, da der Scope auf den try
            // Block beschränkt ist.
            // p2.Alter = 1;

            Pupil p3 = new Pupil() { Vorname = "VN1", Zuname = "ZN1" };
            p3.Alter = 18;
            Console.WriteLine($"Age ist {p3.Alter}!");

            // Folgendes liefert einen Syntaxfehler.
            // p3.Longname = "Testname";

            // p4.Zuname ist "", da wir keinen Wert zuweisen und
            // bei der Definition mit "" initialisiert wird.
            // Sonst wäre dieser Wert null.
            Pupil p4 = new Pupil() { Vorname = "VN1" };
        }
    }
}

```