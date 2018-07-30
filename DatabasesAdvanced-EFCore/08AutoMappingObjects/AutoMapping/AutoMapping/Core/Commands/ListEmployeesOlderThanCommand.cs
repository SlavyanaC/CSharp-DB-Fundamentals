namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Employees.DTOModels;
    using System.Text;

    class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public ListEmployeesOlderThanCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            var age = int.Parse(args[0]);
            EmployeeWithManagerDTO[] employeeWithManagerDTOs = this.employeeController.ListEmployeesOlderThan(age);

            if (employeeWithManagerDTOs.Length < 1)
            {
                return $"No employees older than {age} found.";
            }

            StringBuilder builder = new StringBuilder();
            foreach (var employee in employeeWithManagerDTOs)
            {
                builder.AppendLine(employee.ToString());
            }

            return builder.ToString().TrimEnd();
        }
    }
}
