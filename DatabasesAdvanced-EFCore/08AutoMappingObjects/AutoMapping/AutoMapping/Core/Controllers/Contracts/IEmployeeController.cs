namespace AutoMapping.Core.Controllers.Contracts
{
    using AutoMapping.Employees.DTOModels;
    using System;

    interface IEmployeeController
    {
        void AddEmployee(EmployeeDTO employeeDTO);

        string SetBirthday(int employeeId, DateTime date);

        string SetAddress(int employeeId, string address);

        EmployeeDTO GetEmployeeInfo(int employeeId);

        EmployeePersonalDTO GetEmployeePersonalInfo(int employeeId);

        EmployeeWithManagerDTO[] ListEmployeesOlderThan(int age);
    }
}
