namespace AutoMapping.Core.Commands
{
    using System;
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;

    class SetBirthdayCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public SetBirthdayCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            DateTime date = DateTime.ParseExact(args[1], "dd-MM-yyyy", null);

           var employeeName = this.employeeController.SetBirthday(employeeId, date);

            var commandOutput = $"{employeeName}'s birthday was successfully set to {args[1]}";
            return commandOutput;
        }
    }
}
