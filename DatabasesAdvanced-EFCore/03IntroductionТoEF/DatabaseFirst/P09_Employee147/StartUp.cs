namespace P09_Employee147
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
                var employee = context.Employees
                    .Where(e => e.EmployeeId == 147)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle,
                        Projects = e.EmployeesProjects.Select(ep => ep.Project.Name).ToArray(),
                    })
                    .ToArray();

                foreach (var emp in employee)
                {
                    Console.WriteLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                    foreach (var project in emp.Projects.OrderBy(p => p))
                    {
                        Console.WriteLine($"{project}");
                    }
                }
            }
        }
    }
}
