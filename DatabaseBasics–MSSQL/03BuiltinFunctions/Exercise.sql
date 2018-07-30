--01. Find Names of All Employees by First Name
SELECT FirstName, LastName FROM Employees
 WHERE FirstName LIKE 'SA%'

--02. Find Names of All employees by Last Name 
SELECT FirstName, LastName FROM Employees
 WHERE LastName LIKE '%EI%'

--03. Find First Names of All Employees
SELECT FirstName FROM Employees
 WHERE DepartmentID IN(3, 10) AND (HireDate >= 1995) OR (HireDate <=2005)

--04. Find All Employees Except Engineers
SELECT FirstName, LastName FROM Employees
 WHERE JobTitle NOT LIKE '%engineer%'

--05. Find Towns with Name Length
SELECT Name FROM Towns
 WHERE LEN(Name) = 5 OR LEN(Name) = 6
ORDER BY Name

--06. Find Towns Starting With
SELECT TownId, Name FROM TOWNS
 WHERE Name LIKE 'M%' OR Name LIKE 'K%' OR Name LIKE 'B%' OR Name LIKE 'E%'
ORDER BY Name

--07. Find Towns Not Starting With
SELECT TownId, Name FROM TOWNS
 WHERE Name NOT LIKE 'R%' AND Name NOT LIKE 'B%' AND Name NOT LIKE 'D%'
ORDER BY Name
GO

--08. Create View Employees Hired After 2000 Year
CREATE VIEW V_EmployeesHiredAfter2000 AS 
SELECT FirstName, LastName FROM Employees
 WHERE HireDate >'2001'
GO

--9. Length of Last Name
SELECT FirstName, LastName FROM Employees
 WHERE LEN(LastName) = 5

 --10. Countries Holding 'A'
SELECT CountryName, IsoCode FROM Countries
WHERE CountryName LIKE '%A%A%A%'
ORDER BY IsoCode

--11. Mix of Peak and River Names
SELECT PeakName, RiverName, LOWER(PeakName + RIGHT(RiverName, LEN(RiverName) -1)) 
AS 'Mix'
  FROM Rivers, Peaks
 WHERE RIGHT(PeakName, 1) = LEFT(RiverName, 1)
ORDER BY Mix

--12. Games From 2011 and 2012 Year
SELECT TOP(50) Name, 
			FORMAT(Start, 'yyyy-MM-dd') AS Start
	  FROM Games
 WHERE YEAR(Start) BETWEEN 2011 AND 2012
ORDER BY Start, Name

--13. User Email Providers
SELECT Username,
	   SUBSTRING(Email, CHARINDEX('@', email) + 1, LEN(Email) - CHARINDEX('@', Email)) AS [Email Provider]
  FROM Users
ORDER BY [Email Provider], Username

--14. Get Users with IPAddress Like Pattern
SELECT Username, IpAddress FROM Users
 WHERE IpAddress LIKE '___.1%.%.___'
ORDER BY Username

--15. Show All Games with Duration
SELECT Name AS Game,
	   CASE 
	   WHEN DATEPART(HOUR, Start) BETWEEN 0 AND 11 THEN 'Morning'
	   WHEN DATEPART(HOUR, Start) BETWEEN 12 AND 17 THEN 'Afternoon'
	   WHEN DATEPART(HOUR, Start) BETWEEN 18 AND 23 THEN 'Evening'
	   END
	   AS [Part of the Day],
	   CASE
	   WHEN Duration <= 3 THEN 'Extra Short'
	   WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
	   WHEN Duration > 6 THEN 'Long'
	   WHEN Duration IS NULL THEN 'Extra Long'
	   END
	   AS Duration
 FROM Games
 ORDER BY Game, Duration

 --16. Orders Table
SELECT ProductName, OrderDate,
	   DATEADD(DAY, 3, OrderDate) AS [Pay Due],
	   DATEADD(MONTH, 1, OrderDate) AS [Deliver Due]
 FROM Orders