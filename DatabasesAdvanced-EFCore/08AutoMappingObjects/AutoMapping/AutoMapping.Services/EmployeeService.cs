namespace AutoMapping.Services
{
    using Microsoft.EntityFrameworkCore;
    using AutoMapping.Data;
    using AutoMapping.Services.Contracts;

    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeesContext context;

        public EmployeeService(EmployeesContext context)
        {
            this.context = context;
        }

        public void InitializeDatabase()
        {
            this.context.Database.Migrate();
        }
    }
}
