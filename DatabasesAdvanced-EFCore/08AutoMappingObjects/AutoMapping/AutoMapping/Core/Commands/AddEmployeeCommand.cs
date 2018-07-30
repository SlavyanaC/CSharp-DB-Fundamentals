namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Employees.DTOModels;

    class AddEmployeeCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public AddEmployeeCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            string firstName = args[0];
            string lastName = args[1];
            decimal salaray = decimal.Parse(args[2]);
            var employeeDto = new EmployeeDTO(firstName, lastName, salaray);

            this.employeeController.AddEmployee(employeeDto);

            return $"Employee {firstName} {lastName} successfully added.";
        }
    }
}
