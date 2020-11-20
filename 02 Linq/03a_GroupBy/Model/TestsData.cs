using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Grouping.Model
{
    public class TestsData
    {
        public virtual IEnumerable<Lesson> Lesson { get; set; }
        public virtual IEnumerable<Period> Period { get; set; }
        public virtual IEnumerable<Pupil> Pupil { get; set; }
        public virtual IEnumerable<Schoolclass> Schoolclass { get; set; }
        public virtual IEnumerable<Teacher> Teacher { get; set; }
        public virtual IEnumerable<Test> Test { get; set; }
        public static async Task<TestsData> FromFile(string filename)
        {
            TestsData data;
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                data = await System.Text.Json.JsonSerializer.DeserializeAsync<TestsData>(file);
            }
            foreach (Lesson l in data.Lesson)
            {
                l.L_ClassNavigation = data.Schoolclass.SingleOrDefault(x => x.C_ID == l.L_Class);
                l.L_HourNavigation = data.Period.SingleOrDefault(x => x.P_Nr == l.L_Hour);
                l.L_TeacherNavigation = data.Teacher.SingleOrDefault(x => x.T_ID == l.L_Teacher);
            }
            foreach (Period p in data.Period)
            {
                p.Lessons = data.Lesson.Where(x => x.L_ID == p.P_Nr).ToList();
                p.Tests = data.Test.Where(x => x.TE_Lesson == p.P_Nr).ToList();
            }
            foreach(Pupil p in data.Pupil)
            {
                p.P_ClassNavigation = data.Schoolclass.SingleOrDefault(x => x.C_ID == p.P_Class);
            }
            foreach (Schoolclass c in data.Schoolclass)
            {
                c.C_ClassTeacherNavigation = data.Teacher.SingleOrDefault(x => x.T_ID == c.C_ClassTeacher);
                c.Lessons = data.Lesson.Where(x => x.L_Class == c.C_ID).ToList();
                c.Pupils = data.Pupil.Where(x => x.P_Class == c.C_ID).ToList();
                c.Tests = data.Test.Where(x => x.TE_Class == c.C_ID).ToList();
            }
            foreach (Teacher t in data.Teacher)
            {
                t.Lessons = data.Lesson.Where(x => x.L_Teacher == t.T_ID).ToList();
                t.Schoolclasses = data.Schoolclass.Where(x => x.C_ClassTeacher == t.T_ID).ToList();
                t.Tests = data.Test.Where(x => x.TE_Teacher == t.T_ID).ToList();
            }
            foreach (Test t in data.Test)
            {
                t.TE_ClassNavigation = data.Schoolclass.SingleOrDefault(x => x.C_ID == t.TE_Class);
                t.TE_LessonNavigation = data.Period.SingleOrDefault(x => x.P_Nr == t.TE_Lesson);
                t.TE_TeacherNavigation = data.Teacher.SingleOrDefault(x => x.T_ID == t.TE_Teacher);
            }
            return data;
        }
    }
}
