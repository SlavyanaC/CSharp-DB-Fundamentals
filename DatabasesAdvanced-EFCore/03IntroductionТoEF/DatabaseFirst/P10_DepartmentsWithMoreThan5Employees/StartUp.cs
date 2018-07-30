namespace P10_DepartmentsWithMoreThan5Employees
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
                var departments = context.Departments
                    .Where(d => d.Employees.Count > 5)
                    .OrderBy(d => d.Employees.Count)
                    .ThenBy(d => d.Name)
                    .Select(d => new
                    {
                        d.Name,
                        ManagerFirstName = d.Manager.FirstName,
                        ManagerLastName = d.Manager.LastName,
                        d.Employees,
                    })
                    .ToArray();

                foreach (var department in departments)
                {
                    Console.WriteLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");
                    foreach (var emp in department.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
                    {
                        Console.WriteLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                    }
                    Console.WriteLine(new string('-', 10));
                }
            }
        }
    }
}
