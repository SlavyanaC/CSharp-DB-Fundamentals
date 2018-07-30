namespace P06_AddingNewAddressAndUpdatingEmployee
{
    using P02_DatabaseFirst.Data;
    using P02_DatabaseFirst.Data.Models;
    using System;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            using (context)
            {
                var address = new Address()
                {
                    AddressText = "Vitoshka 15",
                    TownId = 4,
                };

                Employee employee = context.Employees
                      .FirstOrDefault(e => e.LastName == "Nakov");

                employee.Address = address;

                context.SaveChanges();

                var addressTexts = context.Employees
                    .OrderByDescending(e => e.Address.AddressId)
                    .Take(10)
                    .Select(e => e.Address.AddressText)
                    .ToArray();

                foreach (var addressText in addressTexts)
                {
                    Console.WriteLine(addressText);
                }
            }
        }
    }
}