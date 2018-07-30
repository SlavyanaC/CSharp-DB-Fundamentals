namespace P02.VillainNames
{
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connecton = new SqlConnection(Configuration.connectionString);
            connecton.Open();
            using (connecton)
            {
                var villianInfoCommand = new SqlCommand("SELECT v.Name, COUNT(*)  FROM Villains AS v JOIN MinionsVillains AS mv on mv.VillainId = v.Id GROUP BY v.Name HAVING COUNT(MinionId) > 3 ORDER BY COUNT(*) DESC", connecton);

                var reader = villianInfoCommand.ExecuteReader();
                while (reader.Read())
                {
                    var villain = (string)reader["Name"];
                    var minionsCount = (int)reader[1];
                    Console.WriteLine($"{villain} - {minionsCount}");
                }
            }
        }
    }
}
