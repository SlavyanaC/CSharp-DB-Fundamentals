namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;

    class EmployeePersonalInfoCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public EmployeePersonalInfoCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            var employee = this.employeeController.GetEmployeePersonalInfo(employeeId);
            return employee.ToString();
        }
    }
}
