namespace P03.MinionNames
{
    using P02.VillainNames;
    using System;
    using System.Data.SqlClient;

    class StatUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            int villainId = int.Parse(Console.ReadLine());

            using (connection)
            {
                connection.Open();
                string villainQuery = $"SELECT [Name] FROM Villains WHERE Id = @villainId";
                var villainCommand = new SqlCommand(villainQuery, connection);
                villainCommand.Parameters.AddWithValue("@villainId", villainId);

                var villainName = (string)villainCommand.ExecuteScalar();
                if (villainName == null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return;
                }
                Console.WriteLine($"Villain: {villainName}");

                var minionsQuery = "SELECT Name, Age FROM Minions AS m JOIN MinionsVillains AS mv ON mv.MinionId = m.Id WHERE mv.VillainId = @villainId";
                var minionsCommand = new SqlCommand(minionsQuery, connection);
                minionsCommand.Parameters.AddWithValue("@villainId", villainId);
                var reader = minionsCommand.ExecuteReader();
                var counter = 1;
                while (reader.Read())
                {
                    Console.WriteLine($"{counter++}. {reader[0]} {reader[1]}");
                }
            }
        }
    }
}
