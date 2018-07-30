namespace AutoMapping.Core.Commands
{
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Employees.DTOModels;

    class ManagerInfoCommand : ICommand
    {
        private readonly IManagerController managerController;

        public ManagerInfoCommand(IManagerController managerController)
        {
            this.managerController = managerController;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            ManagerDTO managerDTO = this.managerController.GetManagerInfo(employeeId);
            return managerDTO.ToString();
        }
    }
}
