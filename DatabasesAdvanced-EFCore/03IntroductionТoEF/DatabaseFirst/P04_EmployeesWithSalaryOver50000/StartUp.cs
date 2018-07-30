namespace P04_EmployeesWithSalaryOver50000
{
    using P02_DatabaseFirst.Data;
    using System;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            using (context)
            {
                var employeeNames = context.Employees
                    .Where(e => e.Salary > 50_000)
                    .OrderBy(e => e.FirstName)
                    .Select(e => e.FirstName)
                    .ToArray();

                foreach (var name in employeeNames)
                {
                    Console.WriteLine(name);
                }
            }
        }
    }
}
