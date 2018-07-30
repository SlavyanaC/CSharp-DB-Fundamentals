namespace _01.InitialSetup
{
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            var builder = new SqlConnectionStringBuilder()
            {
                ["Server"] = ".",
                ["Integrated Security"] = "Тrue"
            };
            var connection = new SqlConnection(builder.ToString());
            connection.Open();
            using (connection)
            {
                string createDbQuery = "CREATE DATABASE MinionsDB";
                ExecuteNonQueryCommand(createDbQuery, connection);
            }

            builder.Add("Database", "MinionsDb");
            connection = new SqlConnection(builder.ToString());

            using (connection)
            {
                connection.Open();
                string createCountriesSql = "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))";
                string createTownsSql = "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))";
                string createMinionsSql = "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))";
                string createEvilnesFactorSql = "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))";
                string createVillainsSql = "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))";
                string createMinionsVillainsSql = "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))";

                ExecuteNonQueryCommand(createCountriesSql, connection);
                ExecuteNonQueryCommand(createTownsSql, connection);
                ExecuteNonQueryCommand(createMinionsSql, connection);
                ExecuteNonQueryCommand(createEvilnesFactorSql, connection);
                ExecuteNonQueryCommand(createVillainsSql, connection);
                ExecuteNonQueryCommand(createMinionsVillainsSql, connection);

                string insertCountries = "INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')";
                string insertTowns = "INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)";
                string insertMinions = "INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)";
                string insertEvilnessFactor = "INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')";
                string insertVillains = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2)";
                string insertMinionsVillains = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,6),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1),(2,7)";

                ExecuteNonQueryCommand(insertCountries, connection);
                ExecuteNonQueryCommand(insertTowns, connection);
                ExecuteNonQueryCommand(insertMinions, connection);
                ExecuteNonQueryCommand(insertEvilnessFactor, connection);
                ExecuteNonQueryCommand(insertVillains, connection);
                ExecuteNonQueryCommand(insertMinionsVillains, connection);
                connection.Close();
            }
        }

        static void ExecuteNonQueryCommand(string command, SqlConnection connection)
        {
            var sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
        }
    }
}
