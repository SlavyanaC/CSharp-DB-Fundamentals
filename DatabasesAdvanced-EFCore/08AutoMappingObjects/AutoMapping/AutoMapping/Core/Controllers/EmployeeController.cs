namespace AutoMapping.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Data;
    using AutoMapping.Data.Models;
    using AutoMapping.Employees.DTOModels;

    class EmployeeController : IEmployeeController
    {
        private const string EmployeeNotFoundErr = "Employee not found!";

        private readonly EmployeesContext context;
        private readonly IMapper mapper;

        public EmployeeController(EmployeesContext employeesContext, IMapper mapper)
        {
            this.context = employeesContext;
            this.mapper = mapper;
        }

        public void AddEmployee(EmployeeDTO employeeDTO)
        {
            Employee employee = mapper.Map<Employee>(employeeDTO);
            this.context.Employees.Add(employee);
            this.context.SaveChanges();
        }

        public string SetAddress(int employeeId, string address)
        {
            Employee employee = this.context.Employees.Find(employeeId);
            if (employee == null)
            {
                throw new ArgumentException(EmployeeNotFoundErr);
            }

            employee.Address = address;
            this.context.SaveChanges();

            var employeeNames = $"{employee.FirstName} {employee.LastName}";
            return employeeNames;
        }

        public string SetBirthday(int employeeId, DateTime date)
        {
            Employee employee = context.Employees.Find(employeeId);
            if (employee == null)
            {
                throw new ArgumentException(EmployeeNotFoundErr);
            }

            employee.Birthday = date;
            this.context.SaveChanges();

            var employeeNames = $"{employee.FirstName} {employee.LastName}";
            return employeeNames;
        }

        public EmployeeDTO GetEmployeeInfo(int employeeId)
        {
            //EmployeeDTO employee = context.Employees
            //    .Where(e => e.EmployeeId == employeeId)
            //    .ProjectTo<EmployeeDTO>()
            //    .SingleOrDefault();

            Employee employee = this.context.Employees
                .Find(employeeId);
            EmployeeDTO employeeDTO = mapper.Map<EmployeeDTO>(employee);

            if (employee == null)
            {
                throw new ArgumentException(EmployeeNotFoundErr);
            }

            return employeeDTO;
        }

        public EmployeePersonalDTO GetEmployeePersonalInfo(int employeeId)
        {
            //EmployeePersonalDTO employee = context.Employees
            //    .Where(e => e.EmployeeId == employeeId)
            //    .ProjectTo<EmployeePersonalDTO>()
            //    .SingleOrDefault();

            Employee employee = context.Employees.Find(employeeId);
            EmployeePersonalDTO employeeDTO = mapper.Map<EmployeePersonalDTO>(employee);

            if (employee == null)
            {
                throw new ArgumentException(EmployeeNotFoundErr);
            }

            return employeeDTO;
        }

        public EmployeeWithManagerDTO[] ListEmployeesOlderThan(int age)
        {
            EmployeeWithManagerDTO[] employees = context.Employees
                .Where(e => (DateTime.Now.Year - e.Birthday.Value.Year) > age)
                .ProjectTo<EmployeeWithManagerDTO>()
                .OrderByDescending(e => e.Salary)
                .ToArray();

            return employees;
        }
    }
}
