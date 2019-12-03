using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuerySyntax.Model;
using System.Text.Json;

namespace QuerySyntax
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
            IGrouping<string, string> Test;

            TestsData data = TestsData.FromFile("../db/tests.json");
            var result = from l in data.Lesson
                         group l by new { l.L_Class, l.L_Subject } into g
                         select new
                         {
                             Type = g.GetType(),
                             Class = g.Key.L_Class,
                             Subject = g.Key.L_Subject,
                             Count = g.Count(),
                             MaxHour = g.Max(x => x.L_Hour)
                         };
        }
    }
}
