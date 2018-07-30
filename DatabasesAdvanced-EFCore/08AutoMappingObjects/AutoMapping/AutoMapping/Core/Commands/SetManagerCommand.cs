namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;

    class SetManagerCommand : ICommand
    {
        private readonly IManagerController managerController;

        public SetManagerCommand(IManagerController managerController)
        {
            this.managerController = managerController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            int managerId = int.Parse(args[1]);

            string[] commandResult = this.managerController.SetManager(employeeId, managerId);

            var employeeName = commandResult[0];
            var managerName = commandResult[1];

            return $"{managerName} was set as manager of {employeeName}";
        }
    }
}
