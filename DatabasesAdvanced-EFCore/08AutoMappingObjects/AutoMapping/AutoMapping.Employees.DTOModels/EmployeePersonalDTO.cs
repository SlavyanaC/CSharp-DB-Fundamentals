namespace AutoMapping.Employees.DTOModels
{
    using System;
    using System.Text;

    public class EmployeePersonalDTO
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public DateTime? Birthday { get; set; }

        public string Address { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            var birthday = this.Birthday?.ToString("dd-MM-yyyy") ?? "[no birthday specified]";
            var address = this.Address ?? "[no address specified]";

            builder.AppendLine($"ID: {this.EmployeeId} - {this.FirstName} {this.LastName} - ${this.Salary:F2}");
            builder.AppendLine($"Birthday: {birthday}");
            builder.AppendLine($"Address: {address}");

            return builder.ToString().Trim();
        }
    }
}
