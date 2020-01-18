using System;
using System.Collections.Generic;

// https://blogs.msdn.microsoft.com/joelpob/2004/11/17/clr-generics-and-code-sharing/
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/differences-between-cpp-templates-and-csharp-generics

namespace _09_Generics
{
    class Pupil
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public override string ToString() => $"{Id}: {Firstname} {Lastname}";
    }

    class History
    {
        private object _value;
        public object OldValue { get; private set; }
        public object Value
        {
            get => _value;
            set
            {
                OldValue = _value;
                _value = value;
            }
        }
        public void Undo()
        {
            _value = OldValue;
            //OldValue = null;
        }
    }

    class History<T>
    {
        private T _value;
        public T OldValue { get; private set; }
        public T Value
        {
            get => _value;
            set
            {
                OldValue = _value;
                _value = value;
            }
        }
        public void Undo()
        {
            _value = OldValue;
            OldValue = default(T);
        }
    }

    class Cached<T>
    {
        // Der aktuelle Stand (der "Cache")
        private T _value;
        // Eine Function, die das Objekt liefert. Kann z. B. ein Webrequest sein, der die Collection
        // liefert.
        private readonly Func<T> _objectBuilder;
        // Wann wurde das Objekt zum letzten Mal geladen?
        private DateTime _generated = DateTime.MinValue;
        public T Value
        {
            get
            {
                if (_generated + Expiration < DateTime.Now)
                {
                    Console.WriteLine("Generiere neues Objekt.");
                    _generated = DateTime.Now;
                    // Aufruf der übergebenen Function, die das Objekt laden soll.
                    _value = _objectBuilder();
                }
                return _value;
            }
        }
        // Nach welcher Zeitspanne soll das Objekt "ablaufen"?
        public TimeSpan Expiration { get; }
        public Cached(TimeSpan expires, Func<T> objectBuilder)
        {
            Expiration = expires;
            _objectBuilder = objectBuilder;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            History objectHistory = new History();
            objectHistory.Value = "Erster!";
            objectHistory.Value = "Zweiter!";
            Console.WriteLine(objectHistory.OldValue);   // Erster!

            History<string> stringHistory = new History<string>();
            stringHistory.Value = "Erster!";
            stringHistory.Value = "Zweiter!";
            Console.WriteLine(stringHistory.OldValue);   // Erster!

            History<int> intHistory = new History<int>();
            intHistory.Value = 1;
            intHistory.Value = 2;
            Console.WriteLine(intHistory.OldValue);   // 1

            Console.WriteLine(Min<int>(3, 4));
            Console.WriteLine(Min<DateTime>(new DateTime(2000, 1, 1), new DateTime(2001, 1, 1)));


            Cached<Pupil> myPupil = new Cached<Pupil>(TimeSpan.FromSeconds(2), () => new Pupil());
            Pupil p;

            Console.WriteLine("Lese Pupil Objekt:");
            p = myPupil.Value;               // Ausgabe: Generiere neues Objekt. 
            Console.WriteLine("Lese ein weiteres Mal das Pupil Objekt:");
            p = myPupil.Value;               // Keine Ausgabe, da im Cache
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Lese ein weiteres Mal das Pupil Objekt:");
            p = myPupil.Value;               // Ausgabe: Generiere neues Objekt. 

            Console.WriteLine(stringHistory.GetType());  // _09_Generics.History`1[System.String]
            Console.WriteLine(intHistory.GetType());     // _09_Generics.History`1[System.Int32]


        }

        static T Min<T>(T val1, T val2) where T : IComparable
        {
            if (val1.CompareTo(val2) < 0)
            { return val1; }
            return val2;
        }
    }
}
