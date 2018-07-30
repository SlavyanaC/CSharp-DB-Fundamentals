namespace AutoMapping.Employees.DTOModels
{
    using System.Collections.Generic;
    using System.Text;

    public class ManagerDTO
    {
        public ManagerDTO()
        {
            this.EmployeeDTOs = new HashSet<EmployeeDTO>();
        }

        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int EmployeesCount => EmployeeDTOs.Count;

        public ICollection<EmployeeDTO> EmployeeDTOs { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{this.FirstName} {this.LastName} | Employees: {this.EmployeesCount}");
            foreach (var employee in this.EmployeeDTOs)
            {
                builder.AppendLine($"    - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}");
            }

            return builder.ToString().TrimEnd();
        }
    }
}
