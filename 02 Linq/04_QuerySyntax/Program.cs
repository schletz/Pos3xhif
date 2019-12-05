using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuerySyntax.Model;
using System.Text.Json;

namespace QuerySyntax
{
    class Program
    {
        static void Main(string[] args)
        {
            TestsData data = TestsData.FromFile("../db/tests.json");

            // *************************************************************************************
            // DEMO 1: Zeige alle Schüler der 3BHIF
            // *************************************************************************************
            IEnumerable<Pupil> demo1 = from p in data.Pupil
                                       where p.P_Class == "3BHIF"
                                       select p;
            // Alternative mit Method Syntax:
            demo1 = data.Pupil.Where(p => p.P_Class == "3BHIF");


            // *************************************************************************************
            // ÜBUNG 1: Gib alle Tests des Lehrers ZUM in der 4EHIF aus.
            // *************************************************************************************
            // [{"TE_ID":450,"TE_Class":"4EHIF","TE_Teacher":"ZUM","TE_Subject":"BWM3","TE_Date":"2020-05-20T00:00:00","TE_Lesson":8},
            //  {"TE_ID":451,"TE_Class":"4EHIF","TE_Teacher":"ZUM","TE_Subject":"PRE","TE_Date":"2020-02-17T00:00:00","TE_Lesson":1}]
            IEnumerable<Test> result1 = null;
            Console.WriteLine("RESULT1");
            Console.WriteLine(JsonSerializer.Serialize(result1));

            // *************************************************************************************
            // ÜBUNG 2: Gib alle Stunden der 3BHIF mit dem Lehrer FZ geordnet nach Tag (L_Day) und
            //          Unterrichtsstunde (L_Hour) aus.
            // *************************************************************************************
            // [{"L_ID":27,"L_Untis_ID":16,"L_Class":"3BHIF","L_Teacher":"FZ","L_Subject":"DBI1","L_Room":"C2.10","L_Day":2,"L_Hour":1},
            //  {"L_ID":695,"L_Untis_ID":536,"L_Class":"3BHIF","L_Teacher":"FZ","L_Subject":"DBI1x","L_Room":"B3.07MM","L_Day":5,"L_Hour":5},
            //  {"L_ID":696,"L_Untis_ID":536,"L_Class":"3BHIF","L_Teacher":"FZ","L_Subject":"DBI1x","L_Room":"B3.07MM","L_Day":5,"L_Hour":6}]
            IEnumerable<Lesson> result2 = null;
            Console.WriteLine(Environment.NewLine + "RESULT2");
            Console.WriteLine(JsonSerializer.Serialize(result2));

            // *************************************************************************************
            // ÜBUNG 3: Wie Übung 2, allerdings soll ein neues Objekt mit den Properties Subject,
            //          Room, Day und Hour erstellt werden.
            // *************************************************************************************
            // [{"Subject":"DBI1","Room":"C2.10","Day":2,"Hour":1},
            //  {"Subject":"DBI1x","Room":"B3.07MM","Day":5,"Hour":5},
            //  {"Subject":"DBI1x","Room":"B3.07MM","Day":5,"Hour":6}]
            var result3 = (object)null;
            Console.WriteLine(Environment.NewLine + "RESULT3");
            Console.WriteLine(JsonSerializer.Serialize(result3));

            // *************************************************************************************
            // ÜBUNG 4: Gib alle Lehrer, die NW2 Tests (TE_Subject ist
            //          NW2) hatten, aus. Erstelle ein neues Objekt mit TeacherID und TeacherName.
            //          TeacherName ist eine Verknüpfung aus T_Lastname und T_Firstname.
            //          Hinweis: Verwende Any in der where Klausel.
            // *************************************************************************************
            // [{"TeacherId":"BOM","TeacherName":"Boltz Michael"},
            //  {"TeacherId":"BRA","TeacherName":"Bramkamp Heiko"},
            //  {"TeacherId":"CSR","TeacherName":"Csaszar Robert"},
            //  {"TeacherId":"HD","TeacherName":"Haidegger Ingrid"},
            //  {"TeacherId":"HOM","TeacherName":"H\u00F6rzinger Michael"},
            //  {"TeacherId":"MOS","TeacherName":"Moser Gabriele"},
            //  {"TeacherId":"PUH","TeacherName":"Puhm Ursula"},
            //  {"TeacherId":"WAA","TeacherName":"Wagner Anna"},
            //  {"TeacherId":"WK","TeacherName":"Wodnar Karl"}]
            var result4 = (object)null;
            Console.WriteLine(Environment.NewLine + "RESULT4");
            Console.WriteLine(JsonSerializer.Serialize(result4));

            // *************************************************************************************
            // ÜBUNG 5: In welchen Fächern (TE_Subject) gab nach der 7. Stunde Tests? Betrachte 
            //          nur Klassen, wo SZ Klassenvorstand ist. Beachte, dass ein KV auch
            //          von mehreren Klassen KV sein kann.
            //          Die Fächer sind nur 1x auszugeben, verwende dafür Distinct(). Sortiere
            //          die Ausgabe nach dem Fach.
            // *************************************************************************************
            // ["BWM1","BWM2","DBI1","E1","GAD","GGPB","NVS1y","POS1"]
            IEnumerable<string> result5 = null;
            Console.WriteLine(Environment.NewLine + "RESULT5");
            Console.WriteLine(JsonSerializer.Serialize(result5));

            // *************************************************************************************
            // ÜBUNG 6: Welche Tests fanden in Klassen mit SZ als Klassenvorstand an einem Montag
            //          statt? Verwende das DayOfWeek Property des Feldes TE_Date.
            // *************************************************************************************
            // [{"Class":"4AHIF","Teacher":"GAL","Subject":"E1"},{"Class":"4AHIF","Teacher":"NAI","Subject":"GGPB"}]
            var result6 = (object)null;
            Console.WriteLine(Environment.NewLine + "RESULT6");
            Console.WriteLine(JsonSerializer.Serialize(result6));

            // *************************************************************************************
            // ÜBUNG 7: Zeige alle Tests an, die der Klassenvorstand der 3AHIF in der 5CHIF hatte. 
            // Hinweis:
            //          Finde vorher den KV der 3AHIF heraus und speichere ihn in eine string
            //          Variable.
            // *************************************************************************************
            // [{"TE_ID":510,"TE_Class":"5CHIF","TE_Teacher":"GC","TE_Subject":"BWM1","TE_Date":"2020-05-09T00:00:00","TE_Lesson":5},
            //  {"TE_ID":511,"TE_Class":"5CHIF","TE_Teacher":"GC","TE_Subject":"BWM2","TE_Date":"2020-06-12T00:00:00","TE_Lesson":9},
            //  {"TE_ID":512,"TE_Class":"5CHIF","TE_Teacher":"GC","TE_Subject":"BWM3","TE_Date":"2019-11-01T00:00:00","TE_Lesson":5}]
            var kv = data.Schoolclass.Where(s => s.C_ID == "3AHIF").FirstOrDefault().C_ClassTeacher;
            var result7 = from t in data.Test
                          where t.TE_Teacher == kv && t.TE_Class == "5CHIF"
                          select t;
            Console.WriteLine(Environment.NewLine + "RESULT7");
            Console.WriteLine(JsonSerializer.Serialize(result7));

            // *************************************************************************************
            // ÜBUNG 8: Welche Unterrichtsstunden der HIF Abteilung (C_Department = HIF) beginnen
            //          an einem Dienstag (L_Day = 2) nach 17:00? Verwende dafür das TimeOfDay 
            //          Property von P_From und vergleiche mit einem erstellen 
            //          TimeSpan(hour, minute, second) Objekt. Für die Ausgabe erstelle mit
            //          P_From.ToString("H:mm") den Zeitstring.
            // *************************************************************************************
            // [{"Class":"1EHIF","Subject":"POS1x","Teacher":"Puljic","Time":"17:10"},
            //  {"Class":"1EHIF","Subject":"POS1y","Teacher":"Schenk","Time":"17:10"},
            //  {"Class":"1EHIF","Subject":"POS1z","Teacher":"Lackinger","Time":"17:10"},
            //  {"Class":"2CHIF","Subject":"BSPKx","Teacher":"Csaszar","Time":"17:10"},
            //  {"Class":"2CHIF","Subject":"BSPKy","Teacher":"Hofbauer","Time":"17:10"},
            //  {"Class":"5AHIF","Subject":"POS1","Teacher":"Schletz","Time":"17:10"}]
            var result8 = (object)null;
            Console.WriteLine(Environment.NewLine + "RESULT8");
            Console.WriteLine(JsonSerializer.Serialize(result8));

            // *************************************************************************************
            // ÜBUNG 9: Wann ist der späteste Test (späteste Unterrichtsstunde in TE_Lesson) von PUH? 
            //          Erstelle dafür mit der Querysyntax eine Liste mit allen Stunden von PUH und
            //          ermittle dann mit Max() die späteste Stunde.
            // *************************************************************************************
            // 9
            long result9 = 0;
            Console.WriteLine(Environment.NewLine + "RESULT9");
            Console.WriteLine(JsonSerializer.Serialize(result9));

            // *************************************************************************************
            // ÜBUNG 10: Ermittle die Daten des Lehrers SZ. Achtung: Gib keine Collection, sondern
            //           ein einzelnes Objekt zurück. Achte bei der Ausgabe darauf, dass das Ergebnis
            //           in ein JSON Objekt (beginnt mit {) und nicht in ein Array mit einem Element
            //           serialisiert wird.
            // *************************************************************************************
            // {"ID":"SZ","Lastname":"Schletz","Firstname":"Michael"}
            var result10 = (object)null;
            Console.WriteLine(Environment.NewLine + "RESULT10");
            Console.WriteLine(JsonSerializer.Serialize(result10));

        }
    }
}
