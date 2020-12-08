using Projects.Model;
using System;
using System.Linq;

namespace Projects
{
#nullable enable
    class Program
    {
        static void Main(string[] args)
        {
            using (ProjectsContext context = new ProjectsContext())
            {
                Project firstProject = new Project
                {
                    Name = "First Project",
                    EstimatedCost = 100000,
                };
                firstProject.Task.Add(new Task { Name = "First Task", DateStarted = new DateTime(2019, 1, 1), DateFinished = new DateTime(2020, 1, 1) });
                firstProject.Task.Add(new Task { Name = "Second Task", DateStarted = new DateTime(2019, 2, 1), DateFinished = null });
                firstProject.Task.Add(new Task { Name = "Third Task" });



                Project secondProject = new Project
                {
                    Name = "Second Project"
                };
                secondProject.Task.Add(new Task { Name = "First Task", DateStarted = new DateTime(2019, 3, 1), DateFinished = new DateTime(2020, 2, 1) });
                secondProject.Task.Add(new Task { Name = "Second Task", DateStarted = new DateTime(2019, 4, 1), DateFinished = null });

                context.Project.Add(firstProject);
                context.Project.Add(secondProject);

                context.SaveChanges();
            }

            using (ProjectsContext context = new ProjectsContext())
            {
                var stats = from p in context.Project
                            select new
                            {
                                ProjectId = p.ID,
                                ProjectName = p.Name,
                                TaskCount = p.Task.Count(),
                                FirstTask = p.Task.Min(t=>t.DateStarted)
                            };

                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(stats));
            }

        }
    }
}
