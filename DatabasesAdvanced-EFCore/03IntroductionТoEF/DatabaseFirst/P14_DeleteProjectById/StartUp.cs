namespace P14_DeleteProjectById
{
    using P02_DatabaseFirst.Data;
    using System;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var projects = context.EmployeesProjects.Where(p => p.ProjectId == 2);
                context.EmployeesProjects.RemoveRange(projects);
                var project = context.Projects.Find(2);
                context.Projects.Remove(project);
                context.SaveChanges();

                var tenProj = context.Projects
                    .Select(p => p.Name)
                    .Take(10)
                    .ToArray();

                Console.WriteLine(string.Join(Environment.NewLine, tenProj));
            }
        }
    }
}
