namespace P03_EmployeesFullInformation
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
                var employees = context.Employees
                    .Select(e => new
                    {
                        e.EmployeeId,
                        e.FirstName,
                        e.LastName,
                        e.MiddleName,
                        e.JobTitle,
                        e.Salary,
                    })
                    .OrderBy(e => e.EmployeeId)
                    .ToArray();

                foreach (var employee in employees)
                {
                    Console.WriteLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
                }
            }
        }
    }
}
