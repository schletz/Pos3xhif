using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grouping.Model;
using System.Text.Json;

namespace Grouping
{
    class Pupil
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            TestsData data = TestsData.FromFile("../db/tests.json");

            // *************************************************************************************
            // ÜBUNG 1: Wie viele Klassen gibt es pro Abteilung?
            // *************************************************************************************
            var result1 = (object)null;
            Console.WriteLine("RESULT1");
            Console.WriteLine(JsonSerializer.Serialize(result1));

            // *************************************************************************************
            // ÜBUNG 2: Wie (1), allerdings sind nur Abteilungen mit mehr als 10 Klassen auszugeben.
            //          Hinweis: Filtere mit Where nach dem Erstellen der Objekte mit Department
            //                   und Count
            // *************************************************************************************
            var result2 = (object)null;
            Console.WriteLine("RESULT2");
            Console.WriteLine(JsonSerializer.Serialize(result2));

            // *************************************************************************************
            // ÜBUNG 3: Wann ist der letzte Test (Max von TE_Date) pro Lehrer und Fach der 5AHIF
            //          in der Tabelle Test?
            // *************************************************************************************
            var result3 = (object)null;
            Console.WriteLine("RESULT3");
            Console.WriteLine(JsonSerializer.Serialize(result3));

            // *************************************************************************************
            // ÜBUNG 4
            // Bei Verschmutzungen wird oft der Lehrer, der die letzte Stunde pro Tag in einem Raum
            // war, befragt. Dafür filtere die Tabelle data.Lesson so, dass die Stunde gleich der
            // letzten Stunde des entsprechenden Raumes und Tages ist. Dafür wird in where eine
            // Unterabfrage benötigt, die nochmals data.Lesson filtert und die letzte Stunde 
            // ermittelt.
            // Gib nur die Räume aus C2 (verwende StartsWith) am Montag (L_Day ist 1) aus. Beachte,
            // dass L_Room auch null sein kann. Verwende daher den ?. Operator. In diesem Fall soll
            // als Standardwert false geliefert werden. Achte außerdem auf die Rangfolge der Operatoren
            // ?? und &&.
            // *************************************************************************************
            var result4 = (object)null;
            Console.WriteLine("RESULT4");
            Console.WriteLine(JsonSerializer.Serialize(result4));
        }
    }
}
