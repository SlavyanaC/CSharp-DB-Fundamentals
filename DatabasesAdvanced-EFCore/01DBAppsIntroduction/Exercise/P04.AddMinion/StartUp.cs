namespace P4.AddMinion
{
    using P02.VillainNames;
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(Configuration.connectionString);

            string[] minionsInfo = Console.ReadLine().Split();
            string[] villainInfo = Console.ReadLine().Split();

            string minionName = minionsInfo[1];
            int age = int.Parse(minionsInfo[2]);
            string townName = minionsInfo[3];
            string villainName = villainInfo[1];

            using (connection)
            {
                connection.Open();

                int townId = GetTownId(townName, connection);
                int villainId = GetVillainId(villainName, connection);
                int minionId = InsertMinionAndGetId(minionName, age, townId, connection);
                AssignMinionToVillain(villainId, minionId, connection);
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}");

                connection.Close();
            }
        }

        private static void AssignMinionToVillain(int villainId, int minionId, SqlConnection connection)
        {
            string insertMinionVillainSql = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";
            var insertMinionVillainCmd = new SqlCommand(insertMinionVillainSql, connection);
            insertMinionVillainCmd.Parameters.AddWithValue("@minionId", minionId);
            insertMinionVillainCmd.Parameters.AddWithValue("@villainId", villainId);
            insertMinionVillainCmd.ExecuteNonQuery();
        }

        private static int InsertMinionAndGetId(string minionName, int age, int townId, SqlConnection connection)
        {
            string insetMinionSql = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            var insertMinionCmd = new SqlCommand(insetMinionSql, connection);
            insertMinionCmd.Parameters.AddWithValue("@name", minionName);
            insertMinionCmd.Parameters.AddWithValue("@age", age);
            insertMinionCmd.Parameters.AddWithValue("@townId", townId);
            insertMinionCmd.ExecuteNonQuery();

            string minionIdSql = "SELECT Id FROM Minions WHERE Name = @name";
            var minionIdCmd = new SqlCommand(minionIdSql, connection);
            minionIdCmd.Parameters.AddWithValue("@name", minionName);

            int minionId = (int)minionIdCmd.ExecuteScalar();
            return minionId;
        }

        private static int GetVillainId(string villainName, SqlConnection connection)
        {
            string villainSql = "SELECT Id FROM Villains WHERE [Name] = @villainName";
            var villainCmd = new SqlCommand(villainSql, connection);
            villainCmd.Parameters.AddWithValue("@villainName", villainName);

            if (villainCmd.ExecuteScalar() == null)
            {
                InsertIntoVillains(villainName, connection);
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            int villainId = (int)villainCmd.ExecuteScalar();
            return villainId;
        }

        private static void InsertIntoVillains(string villainName, SqlConnection connection)
        {
            string insertVillainSql = "INSERT INTO Villains (Name) VALUES (@villainName)";
            var insertVillainCmd = new SqlCommand(insertVillainSql, connection);
            insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);
            insertVillainCmd.ExecuteNonQuery();
        }

        private static int GetTownId(string townName, SqlConnection connection)
        {
            string townSql = "SELECT Id FROM Towns WHERE [Name] = @townName";
            var townCmd = new SqlCommand(townSql, connection);
            townCmd.Parameters.AddWithValue("@townName", townName);

            if (townCmd.ExecuteScalar() == null)
            {
                InsertIntoTowns(townName, connection);
                Console.WriteLine($"Town {townName} was added to the database.");
            }

            int townId = (int)townCmd.ExecuteScalar();
            return townId;
        }

        private static void InsertIntoTowns(string townName, SqlConnection connection)
        {
            string insertTownSql = "INSERT INTO Towns (Name) VALUES (@townName)";
            var insertTownCmd = new SqlCommand(insertTownSql, connection);
            insertTownCmd.Parameters.AddWithValue("@townName", townName);
            insertTownCmd.ExecuteNonQuery();
        }
    }
}
