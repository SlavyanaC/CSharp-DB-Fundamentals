namespace P05.ChangeTownNamesCasing
{
    using P02.VillainNames;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            string countryName = Console.ReadLine();

            using (connection)
            {
                connection.Open();

                int countryId = GetCountryId(countryName, connection);
                if (countryId == 0)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    UpdateTownNames(countryId, connection);
                    GetOutputTownsNames(countryId, connection);
                }

                connection.Close();
            }
        }

        private static void GetOutputTownsNames(int countryId, SqlConnection connection)
        {
            var outputSql = "SELECT Name FROM Towns WHERE CountryCode = @countryId";
            var outputCmd = new SqlCommand(outputSql, connection);
            outputCmd.Parameters.AddWithValue("@countryId", countryId);
            var reader = outputCmd.ExecuteReader();
            List<string> townsNames = new List<string>();
            while (reader.Read())
            {
                townsNames.Add((string)reader[0]);
            }

            Console.WriteLine($"[{string.Join(", ", townsNames)}]");
        }

        private static void UpdateTownNames(int countryId, SqlConnection connection)
        {
            var updateTownsSql = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = @countryId";
            var updateTownsCmd = new SqlCommand(updateTownsSql, connection);
            updateTownsCmd.Parameters.AddWithValue("@countryId", countryId);
            int rowsAffected = updateTownsCmd.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} town names were affected.");
        }

        private static int GetCountryId(string countryName, SqlConnection connection)
        {
            var countryIdSql = "SELECT TOP(1) c.Id FROM Towns AS t JOIN Countries AS c ON c.Id = t.CountryCode WHERE c.Name = @countryName";
            var countryIdCmd = new SqlCommand(countryIdSql, connection);
            countryIdCmd.Parameters.AddWithValue("@countryName", countryName);

            if (countryIdCmd.ExecuteScalar() == null)
            {
                return 0;
            }

            var countryId = (int)countryIdCmd.ExecuteScalar();
            return countryId;
        }
    }
}
