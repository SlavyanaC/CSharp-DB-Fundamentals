namespace P06.RemoveVillain
{
    using P02.VillainNames;
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            int villainId = int.Parse(Console.ReadLine());

            using (connection)
            {
                connection.Open();
                try
                {
                    string villainNameSql = "SELECT [Name] FROM Villains WHERE Id = @villainId";
                    var villainNameCmd = new SqlCommand(villainNameSql, connection);
                    villainNameCmd.Parameters.AddWithValue("@villainId", villainId);
                    var villainName = (string)villainNameCmd.ExecuteScalar();
                    if (string.IsNullOrWhiteSpace(villainName))
                    {
                        throw new ArgumentException("No such villain was found.");
                    }

                    var villainMinionsSql = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
                    var minionsVillainsCmd = new SqlCommand(villainMinionsSql, connection);
                    minionsVillainsCmd.Parameters.AddWithValue("@villainId", villainId);
                    int minionsReleased = minionsVillainsCmd.ExecuteNonQuery();

                    string villainDeleteSql = "DELETE FROM Villains WHERE Id = @VillainId";
                    var villainCmd = new SqlCommand(villainDeleteSql, connection);
                    villainCmd.Parameters.AddWithValue("@VillainId", villainId);
                    villainCmd.ExecuteNonQuery();

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{minionsReleased} minions were released.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                connection.Close();
            }
        }
    }
}
