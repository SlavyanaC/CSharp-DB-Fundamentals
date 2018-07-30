namespace AutoMapping.Core.Controllers
{
    using System;
    using AutoMapping.Data;
    using AutoMapping.Data.Models;
    using AutoMapping.Core.Controllers.Contracts;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using AutoMapping.Employees.DTOModels;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;

    class ManagerController : IManagerController
    {
        private const string EmployeeNotFoundErr = "Employee not found!";
        private const string ManagerNotFoundErr = "Manager not found!";

        private readonly EmployeesContext context;
        //private readonly IMapper mapper;


        public ManagerController(EmployeesContext employeesContext /*, IMapper mapper*/)
        {
            this.context = employeesContext;
            //this.mapper = mapper;
        }

        public string[] SetManager(int employeeId, int managerId)
        {
            Employee employee = this.context.Employees.Find(employeeId);
            Employee manager = this.context.Employees.Find(managerId);

            if (employee == null)
            {
                throw new ArgumentException(EmployeeNotFoundErr);
            }

            employee.Manager = manager ?? throw new ArgumentException(ManagerNotFoundErr);
            this.context.SaveChanges();

            string employeeNames = employee.FirstName + " " + employee.LastName;
            string managerNames = manager.FirstName + " " + manager.LastName;

            var result = new string[2] { employeeNames, managerNames };
            return result;
        }

        public ManagerDTO GetManagerInfo(int employeeId)
        {
            ManagerDTO manager = context.Employees
                .Include(e => e.ManagerEmployees)
                .Where(e => e.EmployeeId == employeeId)
                .ProjectTo<ManagerDTO>()
                .SingleOrDefault();

            //Employee employee = context.Employees
            //    .Find(employeeId);
            //ManagerDTO manager = mapper.Map<ManagerDTO>(employee);

            if (manager == null)
            {
                throw new ArgumentException(ManagerNotFoundErr);
            }

            return manager;
        }
    }
}
