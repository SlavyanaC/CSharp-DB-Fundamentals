namespace P09.IncreaseAgeStoredProcedure
{
    using P02.VillainNames;
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            int id = int.Parse(Console.ReadLine());

            using (connection)
            {
                connection.Open();

                var getOlderSql = "EXEC usp_GetOlder @Id";
                var getOlderCmd = new SqlCommand(getOlderSql, connection);
                getOlderCmd.Parameters.AddWithValue("@Id", id);
                getOlderCmd.ExecuteNonQuery();

                getOlderCmd = new SqlCommand("SELECT [Name], Age FROM Minions WHERE Id = @Id", connection);
                getOlderCmd.Parameters.AddWithValue("@Id", id);

                var reader = getOlderCmd.ExecuteReader();
                using (reader)
                {
                    reader.Read();
                    Console.WriteLine($"{reader["Name"]} - {reader["Age"]} years old");
                    reader.Close();
                }

                connection.Close();
            }
        }
    }
}
