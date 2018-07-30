--05. Showroom
SELECT Manufacturer, Model FROM Models
ORDER BY Manufacturer, Id DESC

--06. Y Generation
SELECT FirstName, LastName FROM Clients
 WHERE YEAR(BirthDate) >= 1977 AND YEAR(BirthDate) <= 1994
ORDER BY FirstName, LastName, Id

--07. Spacious Office
SELECT t.[Name], o.[Name], o.ParkingPlaces
  FROM Offices AS o
JOIN Towns AS t ON t.Id = o.TownId
WHERE o.ParkingPlaces > 25
ORDER BY t.[Name], o.Id

--08. Available Vehicles
SELECT m.Model, m.Seats, v.Mileage
  FROM Vehicles AS v
JOIN Models AS m ON m.Id = v.ModelId
WHERE v.Id NOT IN(SELECT VehicleId FROM Orders WHERE ReturnDate IS NULL)
ORDER BY v.Mileage, m.Seats DESC, m.Id

--09. Offices per Town
SELECT t.[Name], COUNT(*) AS OfficesNumber
  FROM Towns AS t
JOIN Offices AS o ON t.Id = o.TownId
GROUP BY t.Id, t.[Name]
ORDER BY OfficesNumber DESC, t.[Name]

--10. Buyers Best Choice
SELECT m.Manufacturer,
	   m.Model,
	   COUNT(o.Id) AS TimesOrdered
  FROM Vehicles AS v
JOIN Models AS m ON m.Id = v.ModelId
LEFT JOIN Orders AS o ON o.VehicleId = v.Id
GROUP BY m.Manufacturer, m.Model
ORDER BY TimesOrdered DESC, m.Manufacturer DESC, m.Model

--11. Kinda Person
SELECT Names, Class
  FROM
(		
	SELECT CONCAT(c.FirstName, ' ', c.LastName) AS Names,
		   c.Id,
		   DENSE_RANK() OVER (PARTITION BY c.Id ORDER BY COUNT(m.Class) DESC) AS OrdereddClassRank,
		   m.Class
	  FROM Clients AS c
	JOIN Orders AS o ON o.ClientId = c.Id
	JOIN Vehicles AS v ON v.Id = o.VehicleId
	JOIN Models AS m ON m.Id = v.ModelId
	GROUP BY CONCAT(c.FirstName, ' ', c.LastName), c.Id, m.Class
) AS r
WHERE r.OrdereddClassRank = 1
ORDER BY Names, r.Id

--12. Age Groups Revenue
SELECT 
		(
		CASE 
		WHEN YEAR(BirthDate) >= 1970 AND YEAR(BirthDate) <= 1979 THEN '70''s'
		WHEN YEAR(BirthDate) >= 1980 AND YEAR(BirthDate) <= 1989 THEN '80''s'
		WHEN YEAR(BirthDate) >= 1990 AND YEAR(BirthDate) <= 1999 THEN '90''s'
		ELSE 'Others'
		END
		) AS AgeGroup,
		SUM(o.Bill) AS Revenue,
		AVG(o.TotalMileage) AS AverageMileage
  FROM Clients AS c
	JOIN Orders AS o ON o.ClientId = c.Id
GROUP BY 
	CASE 
	WHEN YEAR(BirthDate) >= 1970 AND YEAR(BirthDate) <= 1979 THEN '70''s'
	WHEN YEAR(BirthDate) >= 1980 AND YEAR(BirthDate) <= 1989 THEN '80''s'
	WHEN YEAR(BirthDate) >= 1990 AND YEAR(BirthDate) <= 1999 THEN '90''s'
	ELSE 'Others'
	END

--13. Consumption in Mind
SELECT m.Manufacturer, t.AverageConsumption 
  FROM
(
	SELECT TOP(7) m.Model,
				  COUNT(o.VehicleId) AS OrdersCount,
				  AVG(m.Consumption) AS AverageConsumption
	  FROM Vehicles AS v
	JOIN Models AS m ON m.Id = v.ModelId
	JOIN Orders AS o ON o.VehicleId = v.Id
	GROUP BY m.Model
	ORDER BY OrdersCount DESC
) AS t
JOIN Models AS m ON m.Model = t.Model
WHERE t.AverageConsumption BETWEEN 5 AND 15

--14. Debt Hunter
SELECT sub.ClientNames, sub.Email, sub.Bill, sub.TownName
  FROM
(
	SELECT CONCAT(c.FirstName, ' ', c.LastName) AS ClientNames,
		   c.Id AS ClientId,
		   c.Email,
		   o.Bill,
		   t.[Name] AS TownName,
		   ROW_NUMBER() OVER (PARTITION BY t.Id ORDER BY o.Bill DESC) AS [Rank]
	  FROM Clients AS c
	 JOIN Orders AS o ON o.ClientId = c.Id
	 JOIN Towns AS t ON t.Id = o.TownId
	WHERE c.CardValidity < o.CollectionDate
) AS sub
WHERE sub.[Rank] <= 2 AND sub.Bill IS NOT NULL
ORDER BY sub.TownName, sub.Bill, sub.ClientId

--15. Town Statistics
WITH CTE_TotalClients(TownId, TotalCount) AS
(
	SELECT t.Id, COUNT(o.ClientId) 
	  FROM Towns AS t
	LEFT JOIN Orders AS o ON o.TownId = t.Id
	LEFT JOIN Clients AS c ON c.Id = o.ClientId
	GROUP BY t.Id, t.[Name]
),
CTE_FemaleClients(TownId, FemaleCount) AS
(
	SELECT t.Id, COUNT(o.ClientId) 
	  FROM Towns AS t
	LEFT JOIN Orders AS o ON o.TownId = t.Id
	LEFT JOIN Clients AS c ON c.Id = o.ClientId
	WHERE c.Gender = 'F'
	GROUP BY t.Id, t.[Name]
),
CTE_MaleClients(TownId, MaleCount) AS
(
	SELECT t.Id, COUNT(o.ClientId) 
	  FROM Towns AS t
	LEFT JOIN Orders AS o ON o.TownId = t.Id
	LEFT JOIN Clients AS c ON c.Id = o.ClientId
	WHERE c.Gender = 'M'
	GROUP BY t.Id, t.[Name]
)
SELECT t.[Name] AS TownName,
	   (mc.MaleCount * 100) / tc.TotalCount AS MalePercent,
	   (fc.FemaleCount * 100) / tc.TotalCount AS FemalePercent
  FROM Towns AS t
LEFT JOIN Orders AS o ON o.TownId = t.Id
LEFT JOIN Clients AS c ON c.Id = o.ClientId
LEFT JOIN CTE_TotalClients AS tc ON tc.TownId = t.Id
LEFT JOIN CTE_FemaleClients AS fc ON fc.TownId = t.Id
LEFT JOIN CTE_MaleClients AS mc ON mc.TownId = t.Id
GROUP BY t.[Name], t.Id, mc.MaleCount, fc.FemaleCount, tc.TotalCount
ORDER BY t.[Name], t.Id

--16. Home Sweet Home
WITH CTE_Information([Rank], ReturnOfficeId, HomeOfficeId, VehicleId, Manufacturer, Model) AS
(
	SELECT DENSE_RANK() OVER (PARTITION BY v.Id ORDER BY o.CollectionDate DESC) AS [Rank],
		   o.ReturnOfficeId,
		   v.OfficeId,
		   v.Id,
		   m.Manufacturer,
		   m.Model
	  FROM Vehicles AS v
	JOIN Models AS m ON m.Id = v.ModelId
	LEFT JOIN Orders AS o ON o.VehicleId = v.Id
)
SELECT CONCAT(Manufacturer, ' - ',  Model) AS Vehicle,
	   CASE
			WHEN (SELECT COUNT(*) FROM Orders WHERE VehicleId = c.VehicleId) = 0 OR
				 c.HomeOfficeId = c.ReturnOfficeId
			THEN 'home'

			WHEN c.ReturnOfficeId IS NULL THEN 'on a rent'

			WHEN c.HomeOfficeId <> c.ReturnOfficeId THEN
			(
				SELECT CONCAT(t.[Name], ' - ', o.[Name])
				  FROM Offices AS o
				JOIN Towns AS t ON t.Id = o.TownId
				WHERE c.ReturnOfficeId = o.Id 
			)
	   END AS [Location]
  FROM CTE_Information AS c
WHERE c.[Rank] = 1
ORDER BY Vehicle, c.VehicleId