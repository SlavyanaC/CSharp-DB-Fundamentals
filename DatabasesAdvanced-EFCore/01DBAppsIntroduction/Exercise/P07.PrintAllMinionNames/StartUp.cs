namespace P07.PrintAllMinionNames
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

            using (connection)
            {
                connection.Open();
                try
                {
                    var command = new SqlCommand("SELECT [Name] FROM Minions", connection);
                    var reader = command.ExecuteReader();
                    var minionsNames = new List<string>();
                    using (reader)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                minionsNames.Add((string)reader[0]);
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        for (int i = 0; i < minionsNames.Count / 2; i++)
                        {
                            var firstName = minionsNames[i];
                            Console.WriteLine(firstName);
                            var lastName = minionsNames[minionsNames.Count - 1 - i];
                            Console.WriteLine(lastName);
                        }

                        if (minionsNames.Count % 2 != 0)
                        {
                            var middleName = minionsNames[minionsNames.Count / 2];
                            Console.WriteLine(middleName);
                        }
                    }
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
