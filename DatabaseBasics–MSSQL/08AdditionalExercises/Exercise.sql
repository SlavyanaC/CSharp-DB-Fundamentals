--1. Number of Users for Email Provider
SELECT SUBSTRING(Email, CHARINDEX('@', Email) + 1, LEN(Email)) AS [Email Provider],
	   COUNT(*) AS [Number Of Users]
  FROM Users
GROUP BY SUBSTRING(Email, CHARINDEX('@', Email) + 1, LEN(Email))
ORDER BY [Number Of Users] DESC, [Email Provider]

--02. All Users in Games
SELECT g.[Name] AS Game, 
       gt.[Name] AS [Game Type], 
	   u.Username, 
	   ug.[Level], 
	   ug.Cash, c.[Name]
  FROM Games AS g
JOIN GameTypes AS gt ON gt.Id = g.GameTypeId
JOIN UsersGames AS ug ON ug.GameId = g.Id
JOIN Users AS u ON u.Id = ug.UserId
JOIN Characters AS c ON ug.CharacterId = c.Id
ORDER BY ug.[Level] DESC, u.Username, g.[Name]

--03. Users in Games with Their Items
SELECT u.Username,
	   g.[Name] AS [Game],
	   COUNT(ugi.ItemId) AS [Items Count],
	   SUM(i.Price) AS [Items Price]
  FROM Users AS u
JOIN UsersGames AS ug ON ug.UserId = u.Id
JOIN Games AS g ON g.Id = ug.GameId
JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugi.ItemId
GROUP BY u.Username, g.[Name]
HAVING COUNT(ugi.ItemId) >= 10
ORDER BY [Items Count] DESC, [Items Price] DESC, u.Username

--05. All Items with Greater than Average Statistics
WITH CTE_AboveAvgStats (Id) AS	
(
	SELECT Id 
	  FROM [Statistics]
	 WHERE Mind > (SELECT AVG(Mind) FROM [Statistics]) AND
		   Luck > (SELECT AVG(Luck) FROM [Statistics]) AND 
		  Speed > (SELECT AVG(Speed) FROM [Statistics])
)
SELECT i.[Name],
	   i.Price,
	   i.MinLevel,
	   s.Strength,
	   s.Defence,
	   s.Speed,
	   s.Luck,
	   s.Mind
  FROM CTE_AboveAvgStats AS aas
JOIN Items AS i ON i.StatisticId = aas.Id
JOIN [Statistics] AS s ON s.Id = aas.Id
ORDER BY i.[Name]

--06. Display All Items about Forbidden Game Type
SELECT i.[Name],
	   i.Price,
	   i.MinLevel,
	   gt.[Name] AS [Forbidden Game Type]
  FROM GameTypeForbiddenItems AS gtfi
RIGHT JOIN Items AS i ON i.Id = gtfi.ItemId
LEFT JOIN GameTypes AS gt ON gt.Id = gtfi.GameTypeId
ORDER BY [Forbidden Game Type] DESC, i.[Name] 

SELECT * FROM Users
SELECT * FROM UsersGames
SELECT * FROM Games
SELECT * FROM Items
SELECT * FROM UserGameItems
SELECT * FROM [Statistics]
SELECT * FROM Items
SELECT * FROM GameTypeForbiddenItems
SELECT * FROM GameTypes

--08. Peaks and Mountains
SELECT p.PeakName, m.MountainRange, p.Elevation 
  FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
ORDER BY p.Elevation DESC

--09. Peaks with Mountain, Country and Continent
SELECT p.PeakName,
	   m.MountainRange,
	   c.CountryName,
	   con.ContinentName
  FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
JOIN Countries AS c ON c.CountryCode = mc.CountryCode
JOIN Continents AS con ON con.ContinentCode = c.ContinentCode
ORDER BY p.PeakName, c.CountryName

--10. Rivers by Country
SELECT c.CountryName,
	   con.ContinentName,
	   ISNULL(COUNT(r.Id), 0) AS RiversCount,
	   ISNULL(SUM(r.[Length]), 0) AS TotalLength
  FROM Countries AS c
LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
JOIN Continents AS con ON con.ContinentCode = c.ContinentCode
GROUP BY c.CountryName, con.ContinentName
ORDER BY RiversCount DESC, TotalLength DESC, c.CountryName

--11. Count of Countries by Currency
SELECT cur.CurrencyCode,
	   cur.[Description],
	   COUNT(cou.CountryCode) AS NumberOfCountries
  FROM Currencies AS cur
LEFT JOIN Countries AS cou ON cou.CurrencyCode = cur.CurrencyCode
GROUP BY cur.CurrencyCode, cur.[Description]
ORDER BY NumberOfCountries DESC, cur.[Description]

--12. Population and Area by Continent
SELECT con.ContinentName,
	   SUM(c.AreaInSqKm) AS CountriesArea,
	  SUM(CAST(c.[Population] AS bigint)) AS CountriesPopulation
  FROM Continents AS con
JOIN Countries AS c ON c.ContinentCode = con.ContinentCode
GROUP BY con.ContinentName
ORDER BY CountriesPopulation DESC
