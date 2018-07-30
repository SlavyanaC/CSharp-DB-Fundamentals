namespace P08.IncreaseMinionAge
{
    using P02.VillainNames;
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            int[] minionIds = Console.ReadLine().Split().Select(int.Parse).ToArray();

            using (connection)
            {
                connection.Open();

                foreach (var id in minionIds)
                {
                    UpdateMinionAge(id, connection);
                    UpdateMinionName(id, connection);
                }

                var outputSql = "SELECT [Name], Age FROM Minions";
                var outputCmd = new SqlCommand(outputSql, connection);
                var reader = outputCmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
                }

                connection.Close();
            }
        }

        private static void UpdateMinionName(int id, SqlConnection connection)
        {
            var nameSql = "SELECT [Name] FROM Minions WHERE Id = @id";
            var nameCmd = new SqlCommand(nameSql, connection);
            nameCmd.Parameters.AddWithValue("@id", id);
            string minionName = (string)nameCmd.ExecuteScalar();

            int secondNameIndex = 0;
            for (int i = 0; i < minionName.Length; i++)
            {
                if ((char)minionName[i] == ' ')
                {
                    secondNameIndex = i + 1;
                    break;
                }
            }

            if (secondNameIndex == 0)
            {
                var titleCaseSql = "UPDATE Minions SET Name = UPPER(LEFT(Name, 1)) + LOWER(SUBSTRING(Name, 2, LEN(Name) - 1)) WHERE Id = @id";
                var titleCaseCmd = new SqlCommand(titleCaseSql, connection);
                titleCaseCmd.Parameters.AddWithValue("id", id);
                titleCaseCmd.ExecuteNonQuery();
            }

            else
            {
                var secondNameTitleCaseSql = "UPDATE Minions SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, @secondNameIndex - 1) + UPPER(SUBSTRING(Name, @secondNameIndex + 1, 1)) + SUBSTRING(Name, @secondNameIndex + 2, LEN(Name)) WHERE Id = @id";
                var secondNameTitleCaseCmd = new SqlCommand(secondNameTitleCaseSql, connection);
                secondNameTitleCaseCmd.Parameters.AddWithValue("@secondNameIndex", secondNameIndex);
                secondNameTitleCaseCmd.Parameters.AddWithValue("id", id);
                secondNameTitleCaseCmd.ExecuteNonQuery();
            }
        }

        private static void UpdateMinionAge(int id, SqlConnection connection)
        {
            var updateAgeSql = "UPDATE Minions SET Age += 1 WHERE Id = @id";
            var updateAgeCmd = new SqlCommand(updateAgeSql, connection);
            updateAgeCmd.Parameters.AddWithValue("@id", id);
            updateAgeCmd.ExecuteNonQuery();
        }
    }
}
