namespace P08_AddressesByTown
{
    using P02_DatabaseFirst.Data;
    using System;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var addresses = context.Addresses
                    .OrderByDescending(a => a.Employees.Count())
                    .ThenBy(a => a.Town.Name)
                    .ThenBy(a => a.AddressText)
                    .Take(10)
                    .Select(a => new
                    {
                        a.AddressText,
                        a.Town.Name,
                        a.Employees.Count,
                    })
                    .ToArray();

                foreach (var address in addresses)
                {
                    Console.WriteLine($"{address.AddressText}, {address.Name} - {address.Count} employees");
                }
            }
        }
    }
}
