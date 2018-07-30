namespace P12_IncreaseSalaries
{
    using P02_DatabaseFirst.Data;
    using System;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            var wantedDepartments = new string[] { "Engineering", "Marketing", "Tool Design", "Information Services" };

            using (var context = new SoftUniContext())
            {
                context.Employees
                   .Where(e => wantedDepartments.Contains(e.Department.Name))
                   .ToList()
                   .ForEach(e => e.Salary *= 1.12M);

                context.SaveChanges();

                var employees = context.Employees
                    .Where(e => wantedDepartments.Contains(e.Department.Name))
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.Salary
                    })
                    .ToArray();


                foreach (var emp in employees)
                {
                    Console.WriteLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:F2})");
                }
            }
        }
    }
}
