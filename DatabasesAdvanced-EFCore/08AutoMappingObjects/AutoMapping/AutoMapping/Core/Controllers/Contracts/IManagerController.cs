namespace AutoMapping.Core.Controllers.Contracts
{
    using AutoMapping.Employees.DTOModels;

    interface IManagerController
    {
        string[] SetManager(int employeeId, int managerId);

        ManagerDTO GetManagerInfo(int employeeId);
    }
}
