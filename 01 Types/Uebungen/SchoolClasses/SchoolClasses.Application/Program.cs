using System;
using System.Collections.Generic;
using System.Text.Json;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace SchoolClasses.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() => !typeof(SchoolClass).HasDefaultConstructor(), "SchoolClass does not have a default constructor.");
                CheckAndWrite(() => !typeof(Student).HasDefaultConstructor(), "Student does not have a default constructor.");
                CheckAndWrite(() =>
                    typeof(SchoolClass).IsReadonlyProperty(nameof(SchoolClass.Name))
                    && typeof(SchoolClass).IsReadonlyProperty(nameof(SchoolClass.ClassTeacher)),
                    "SchoolClass.Name and SchoolClass.ClassTeacher are read-only.");
                CheckAndWrite(() =>
                    typeof(Student).IsReadonlyProperty(nameof(Student.Id))
                    && typeof(Student).IsReadonlyProperty(nameof(Student.Firstname))
                    && typeof(Student).IsReadonlyProperty(nameof(Student.Lastname))
                    && !typeof(Student).IsReadonlyProperty(nameof(Student.City)),
                    "Student.Id, Firstname and Lastname are read-only, City can be changed.");
                CheckAndWrite(() =>
                    typeof(SchoolClass).IsReadonlyProperty(nameof(SchoolClass.Students))
                    && typeof(SchoolClass).PropertyHasType<IReadOnlyList<Student>>(nameof(SchoolClass.Students))
                    && CreateClass().Students is not List<Student>,
                    "SchoolClass.Students is IReadOnlyList<Student> and uses AsReadOnly().");
                CheckAndWrite(() =>
                    typeof(SchoolClass).IsReadonlyProperty(nameof(SchoolClass.Cities))
                    && typeof(SchoolClass).PropertyHasType<HashSet<string>>(nameof(SchoolClass.Cities)),
                    "SchoolClass.Cities is a read-only property of type HashSet<string>.");
                CheckAndWrite(() =>
                    typeof(Student).PropertyHasType<SchoolClass>(nameof(Student.SchoolClass))
                    && typeof(Student).GetProperty(nameof(Student.SchoolClass))?.SetMethod?.IsAssembly == true,
                    "Student.SchoolClass has an internal set method.");
            }

            {
                Console.WriteLine("Checking constructors");
                CheckAndWrite(() =>
                {
                    var schoolClass = new SchoolClass(name: "3AHIF", classTeacher: "KV1");
                    return schoolClass.Name == "3AHIF" && schoolClass.ClassTeacher == "KV1"
                        && schoolClass.Students.Count == 0 && schoolClass.Cities.Count == 0;
                }, "Constructor of SchoolClass sets the values, a new class has no students.");
                CheckAndWrite(() =>
                {
                    var student = new Student(id: 1001, firstname: "FN1", lastname: "LN1", city: "CTY1");
                    return student.Id == 1001 && student.Firstname == "FN1" && student.Lastname == "LN1"
                        && student.City == "CTY1" && student.SchoolClass is null;
                }, "Constructor of Student sets the values, a new student has no class.");
            }

            {
                Console.WriteLine("Checking AddStudent and RemoveStudent");
                CheckAndWrite(() =>
                {
                    var schoolClass = CreateClass();
                    var students = schoolClass.Students;
                    return Ids(students) == "1001,1002,1003" && students[0].SchoolClass == schoolClass
                        && students[2].SchoolClass == schoolClass;
                }, "AddStudent adds the students in the correct order and sets SchoolClass.", weight: 2);
                CheckAndWrite(() =>
                {
                    var schoolClass = CreateClass();
                    schoolClass.AddStudent(schoolClass.Students[0]);
                    return Ids(schoolClass.Students) == "1001,1002,1003";
                }, "AddStudent ignores a student who is already in this class.");
                CheckAndWrite(() =>
                {
                    var schoolClass = CreateClass();
                    var cities = schoolClass.Cities;
                    schoolClass.Students[0].City = "CTY3";
                    var citiesAfterMove = schoolClass.Cities;
                    return cities.Count == 2 && cities.Contains("CTY1") && cities.Contains("CTY2")
                        && citiesAfterMove.Count == 3 && citiesAfterMove.Contains("CTY3");
                }, "Cities returns every city once and is up to date after a change of City.", weight: 2);
                CheckAndWrite(() =>
                {
                    var schoolClass = CreateClass();
                    var student = schoolClass.Students[1];
                    bool firstRemove = schoolClass.RemoveStudent(student);
                    bool secondRemove = schoolClass.RemoveStudent(student);
                    return firstRemove && !secondRemove && Ids(schoolClass.Students) == "1001,1003"
                        && student.SchoolClass is null;
                }, "RemoveStudent removes the student and sets SchoolClass to null.", weight: 2);
                CheckAndWrite(() =>
                {
                    var schoolClass = CreateClass();
                    var otherClass = new SchoolClass(name: "3BHIF", classTeacher: "KV2");
                    var student = schoolClass.Students[0];
                    return !otherClass.RemoveStudent(student) && student.SchoolClass == schoolClass;
                }, "RemoveStudent does not change a student of another class.");
            }

            {
                Console.WriteLine("Checking ChangeClass");
                CheckAndWrite(() =>
                {
                    var classes = CreateClasses();
                    var student = classes["3AHIF"].Students[0];
                    student.ChangeClass(classes["3BHIF"]);
                    return Ids(classes["3AHIF"].Students) == "1002,1003"
                        && Ids(classes["3BHIF"].Students) == "1011,1012,1013,1001"
                        && student.SchoolClass == classes["3BHIF"];
                }, "ChangeClass moves the student to the end of the new class.", weight: 2);
                CheckAndWrite(() =>
                {
                    var schoolClass = new SchoolClass(name: "3CHIF", classTeacher: "KV3");
                    var student = new Student(id: 1021, firstname: "FN7", lastname: "LN7", city: "CTY1");
                    student.ChangeClass(schoolClass);
                    return Ids(schoolClass.Students) == "1021" && student.SchoolClass == schoolClass;
                }, "ChangeClass works for a student without a class.");
                CheckAndWrite(() =>
                {
                    var classes = CreateClasses();
                    var student = classes["3AHIF"].Students[0];
                    classes["3BHIF"].AddStudent(student);
                    return Ids(classes["3AHIF"].Students) == "1002,1003"
                        && Ids(classes["3BHIF"].Students) == "1011,1012,1013,1001"
                        && student.SchoolClass == classes["3BHIF"];
                }, "AddStudent removes the student from the old class first.", weight: 2);
                CheckAndWrite(() =>
                {
                    var json = JsonSerializer.Serialize(CreateClass().Students);
                    return json.Contains("\"Id\":1001") && !json.Contains("ClassTeacher");
                }, "Students can be serialized to JSON without the class ([JsonIgnore]).");
            }
            WriteSummary();
        }

        /// <summary>
        /// Creates the classes 3AHIF and 3BHIF with 3 students each and an empty class 3CHIF.
        /// </summary>
        private static Dictionary<string, SchoolClass> CreateClasses()
        {
            Dictionary<string, SchoolClass> classes = new()
            {
                ["3AHIF"] = new SchoolClass(name: "3AHIF", classTeacher: "KV1"),
                ["3BHIF"] = new SchoolClass(name: "3BHIF", classTeacher: "KV2"),
                ["3CHIF"] = new SchoolClass(name: "3CHIF", classTeacher: "KV3")
            };
            classes["3AHIF"].AddStudent(new Student(id: 1001, firstname: "FN1", lastname: "LN1", city: "CTY1"));
            classes["3AHIF"].AddStudent(new Student(id: 1002, firstname: "FN2", lastname: "LN2", city: "CTY1"));
            classes["3AHIF"].AddStudent(new Student(id: 1003, firstname: "FN3", lastname: "LN3", city: "CTY2"));
            classes["3BHIF"].AddStudent(new Student(id: 1011, firstname: "FN4", lastname: "LN4", city: "CTY1"));
            classes["3BHIF"].AddStudent(new Student(id: 1012, firstname: "FN5", lastname: "LN5", city: "CTY1"));
            classes["3BHIF"].AddStudent(new Student(id: 1013, firstname: "FN6", lastname: "LN6", city: "CTY1"));
            return classes;
        }

        private static SchoolClass CreateClass() => CreateClasses()["3AHIF"];

        private static string Ids(IEnumerable<Student> students)
        {
            var ids = new List<int>();
            foreach (var student in students) { ids.Add(student.Id); }
            return string.Join(",", ids);
        }
    }
}
