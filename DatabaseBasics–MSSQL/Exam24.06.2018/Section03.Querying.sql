--05
SELECT Id, Name FROM Cities
WHERE CountryCode = 'BG'
ORDER BY Name

--06
SELECT 
	CASE 
	WHEN MiddleName IS NOT NULL THEN CONCAT(FirstName, ' ', MiddleName, ' ', LastName)
	ELSE FirstName + ' ' + LastName
	END AS [Full Name],
	YEAR(BirthDate) AS BirthYear
	FROM Accounts
	WHERE YEAR(BirthDate) > 1991
ORDER BY BirthYear DESC, FirstName

--07
SELECT a.FirstName, 
	   a.LastName,
	   FORMAT(a.BirthDate, 'MM-dd-yyyy') AS BirthDate,
	   c.Name AS Hometown,
	   a.Email
  FROM Accounts AS a
JOIN Cities AS c ON c.Id = a.CityId
WHERE Email LIKE ('E%')
ORDER BY c.Name DESC

--08
SELECT c.Name AS City,
	   ISNULL(COUNT(h.Id), 0) AS Hotels
    FROM Cities AS c
	LEFT JOIN Hotels AS h ON h.CityId = c.Id
GROUP BY c.Id, c.Name
ORDER BY Hotels DESC, City

--09
SELECT r.Id, r.Price, h.Name AS Hotel, c.Name AS City
  FROM Rooms AS r
JOIN Hotels AS h ON h.Id = r.HotelId
JOIN Cities AS c ON c.Id = h.CityId
WHERE r.Type = 'First Class'
ORDER BY r.Price DESC, r.Id

--10
SELECT a.Id,
	   CONCAT(a.FirstName, ' ', a.LastName) AS FullName,
	   MAX(DATEDIFF(DAY, ArrivalDate, ReturnDate)) AS LongestTrip,
	   MIN(DATEDIFF(DAY, ArrivalDate, ReturnDate)) AS ShortestTrip
  FROM Trips AS t
JOIN AccountsTrips AS act ON act.TripId = t.Id
JOIN Accounts AS a ON a.Id = act.AccountId
WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
GROUP BY a.Id, CONCAT(a.FirstName, ' ', a.LastName)
ORDER BY LongestTrip DESC, a.Id

--11
SELECT TOP(5) c.Id,
			  Name AS City,
			  CountryCode AS Country,
			  COUNT(a.Id) AS Accounts
  FROM Cities AS c
JOIN Accounts AS a ON a.CityId = c.Id
GROUP BY c.Id, c.Name, c.CountryCode
ORDER BY Accounts DESC

--12
SELECT a.Id,
	   a.Email,
	   c.Name AS City,
	   COUNT(t.ID) AS Trips
  FROM Accounts AS a
  JOIN AccountsTrips AS act ON act.AccountId = a.Id
  JOIN Trips AS t ON t.Id = act.TripId
  JOIN Rooms AS r ON r.Id = t.RoomId
  JOIN Hotels AS h ON h.Id = r.HotelId
  JOIN Cities AS c ON c.Id = h.CityId
 WHERE a.CityId = c.Id
GROUP BY a.Id, a.Email, c.Name
ORDER BY Trips DESC, a.Id

--13
SELECT TOP(10) c.Id,
	   c.Name,
	   SUM(h.BaseRate + r.Price) AS [Total Revenue],
	   COUNT(t.Id) AS Trips
  FROM Cities AS c
  JOIN Hotels AS h ON h.CityId = c.Id
  JOIN Rooms AS r ON r.HotelId = h.Id
  JOIN Trips AS t ON t.RoomId = r.Id
  WHERE YEAR(t.BookDate) = '2016'
GROUP BY c.Id, c.Name
ORDER BY [Total Revenue] DESC, Trips DESC

--14
WITH CTE_TripRevenue(TripId, Revenue)
AS
(
	SELECT	t.Id,
			SUM(h.BaseRate + r.Price)
	  FROM	Trips AS t
	  JOIN  AccountsTrips AS ac ON ac.TripId = t.Id
	  JOIN	Rooms AS r ON r.Id = t.RoomId
	  JOIN	Hotels AS h ON h.Id = r.HotelId
	GROUP BY t.Id
)
SELECT t.Id,
	   h.Name AS HotelName,
	   r.Type AS RoomType,
	   CASE 
		WHEN t.CancelDate IS NULL THEN Revenue
		ELSE 0
	END AS Revenue
  FROM CTE_TripRevenue AS ctr
JOIN Trips AS t ON t.Id = ctr.TripId
JOIN Rooms AS r ON r.Id = t.RoomId
JOIN Hotels AS h ON h.Id = r.HotelId
ORDER BY r.Type, t.Id

--15
WITH CTE_AccountCountriesTrips(AccountId, CountryCode, TripsCount, Rank)
AS
(
	SELECT a.Id,
		   c.CountryCode,
		   COUNT(t.Id),
		   ROW_NUMBER() OVER (PARTITION BY c.CountryCode ORDER BY COUNT(t.Id) DESC)
	  FROM Trips AS t
	  JOIN Rooms AS r ON r.Id = t.RoomId
	  JOIN Hotels AS h ON h.Id = r.HotelId
	  JOIN Cities AS c ON c.Id = h.CityId
	  JOIN AccountsTrips AS act ON act.TripId = t.Id
	  JOIN Accounts AS a ON a.Id = act.AccountId
	GROUP BY a.Id, c.CountryCode
)
SELECT  a.Id,
		a.Email,
		act.CountryCode,
		act.TripsCount AS Trips
	FROM CTE_AccountCountriesTrips AS act
	JOIN Accounts AS a ON a.Id = act.AccountId
WHERE act.Rank = 1
ORDER BY Trips DESC

--16
SELECT 
		tl.TripId,
		tl.Luggages,
		CASE
			WHEN tl.Luggages > 5 THEN '$' + CAST(tl.Luggages * 5 AS nvarchar)
			ELSE '$0'
		END AS Fee
  FROM Trips AS t
JOIN 
(
	SELECT TripId, SUM(Luggage) AS Luggages
	  FROM AccountsTrips
	WHERE Luggage > 0
	GROUP BY TripId
) AS tl ON tl.TripId = t.Id
ORDER BY tl.Luggages DESC 

--17
WITH CTE_AccountInfo(TripId, FullName, [From]) AS
(
	SELECT t.Id,
		   CASE 
				WHEN MiddleName IS NOT NULL THEN CONCAT(FirstName, ' ', MiddleName, ' ', LastName)
				ELSE FirstName + ' ' + LastName
		   END AS FullName,
		   c.Name
	FROM Trips AS t
	JOIN AccountsTrips AS atr ON atr.TripId = t.Id
	JOIN Accounts AS a ON a.Id = atr.AccountId
	JOIN Cities AS c ON c.Id = a.CityId
),
CTE_HotelInfo (TownId, [To], Duration) AS
(
	SELECT t.Id,
		   c.Name,
		   CASE
		   WHEN
			 t.CancelDate IS NULL THEN CONCAT(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate), ' days')
			 ELSE 'Canceled'
		   END
	  FROM Trips AS t
	JOIN Rooms AS r ON r.Id = t.RoomId
	JOIN Hotels AS h ON h.Id = r.HotelId
	JOIN Cities AS c ON c.Id = h.CityId
)
SELECT t.Id,
	   aci.FullName,
	   aci.[From],
	   hi.[To],
	   hi.Duration
  FROM Trips AS t
JOIN CTE_AccountInfo AS aci ON aci.TripId = t.Id
JOIN CTE_HotelInfo AS hi ON hi.TownId = t.Id
ORDER BY aci.FullName, t.Id