using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Lab
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var connecton = new SqlConnection(@"Server=DESKTOP-C5PEG4G\SQLEXPRESS;Database=SoftUni;Integrated Security=True");

            connecton.Open();
            using (connecton)
            {
                //Get EmployeesCount
                var employeesCountCommand = new SqlCommand("SELECT COUNT(*) FROM Employees", connecton);
                var employeeCoun = (int)employeesCountCommand.ExecuteScalar();
                Console.WriteLine($"Employee Count: {employeeCoun}");

                //Insert Town
                var insertTownCommand = new SqlCommand("INSERT INTO Towns (Name) VALUES ('Vratsa')", connecton);
                var rowsAffected = insertTownCommand.ExecuteNonQuery();
                Console.WriteLine($"Rows Affected: {rowsAffected}");

                //Transaction For Deleting Town
                var transaction = connecton.BeginTransaction("");
                var deleteTownCommand = new SqlCommand("DELETE FROM Towns WHERE Name = 'Vratsa'",connecton, transaction);
                rowsAffected = deleteTownCommand.ExecuteNonQuery();
                transaction.Rollback();
                Console.WriteLine($"Rows Affected: {rowsAffected}");

                //Employees Data
                var dataReaderCommand = new SqlCommand("SELECT FirstName, LastName, JobTitle FROM Employees", connecton);

                var people = new List<Person>();
                var reader = dataReaderCommand.ExecuteReader();
                while (reader.Read())
                {
                    var firstName = (string)reader["FirstName"];
                    var lastName = (string)reader["LastName"];
                    var jobTitle = (string)reader["JobTitle"];

                    Person person = new Person()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        JobTitle = jobTitle
                    };

                    people.Add(person);
                }

                var groupPeople = people
                    .GroupBy(p => p.JobTitle)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                foreach (var group in groupPeople)
                {
                    Console.WriteLine($"{group.Key}: ({group.Count()} people)");
                    foreach (var person in group)
                    {
                        Console.WriteLine("\t" + person);
                    }
                }
            }
        }
    }
}
