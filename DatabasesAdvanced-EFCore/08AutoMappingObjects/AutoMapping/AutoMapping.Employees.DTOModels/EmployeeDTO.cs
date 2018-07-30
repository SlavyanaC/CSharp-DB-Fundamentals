namespace AutoMapping.Employees.DTOModels
{
    public class EmployeeDTO
    {
        public EmployeeDTO() { }

        public EmployeeDTO(string firstName, string lastName, decimal salary)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Salary = salary;
        }

        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public override string ToString()
        {
            var employeeInfo = $"ID: {this.EmployeeId} - {this.FirstName} {this.LastName} - ${this.Salary:F2}";
            return employeeInfo;
        }
    }
}
