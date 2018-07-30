namespace AutoMapping.Employees.DTOModels
{
    using System;

    public class EmployeeWithManagerDTO
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public DateTime Birthday { get; set; }

        public string ManagerLastName { get; set; }

        public override string ToString()
        {
            string managerLastName = this.ManagerLastName == null ? "[no manager]" : this.ManagerLastName;
            var result = $"{this.FirstName} {this.LastName} - ${this.Salary} - Manager: {managerLastName}";
            return result;
        }
    }
}
