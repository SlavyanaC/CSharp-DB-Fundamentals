namespace AutoMapping.Core.Commands
{
    using System.Linq;
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;

    class SetAddressCommand : ICommand
    {
        private readonly IEmployeeController employeeController;

        public SetAddressCommand(IEmployeeController employeeController)
        {
            this.employeeController = employeeController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            string address = string.Join(" ", args.Skip(1).ToArray());

            var employeeName = this.employeeController.SetAddress(employeeId, address);

            var commandOutput = $"{employeeName}'s address was successfully set to {address}";
            return commandOutput;
        }
    }
}
