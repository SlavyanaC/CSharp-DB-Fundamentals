namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Employees.DTOModels;

    class EmployeeInfoCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public EmployeeInfoCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            EmployeeDTO employee = this.employeeController.GetEmployeeInfo(employeeId);
            return employee.ToString();
        }
    }
}
