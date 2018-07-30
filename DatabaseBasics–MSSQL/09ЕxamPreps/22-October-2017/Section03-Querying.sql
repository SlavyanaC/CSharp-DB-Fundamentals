--05. Users by Age
SELECT Username, Age FROM Users
ORDER BY Age, Username DESC

--06. Unassigned Reports
SELECT [Description], OpenDate FROM Reports
WHERE EmployeeId IS NULL
ORDER BY OpenDate, [Description]

--07. Employees & Reports
SELECT e.FirstName, e.LastName, r.[Description], FORMAT(r.OpenDate, 'yyyy-MM-dd') AS OpenDate FROM Employees AS e
JOIN Reports AS r ON r.EmployeeId = e.Id
ORDER BY e.Id, r.OpenDate, r.Id

--08. Most Reported Category
SELECT c.[Name], COUNT(*) AS ReportsNumber
  FROM Categories AS c
JOIN Reports AS r ON r.CategoryId = c.Id
GROUP BY c.Id, c.Name
ORDER BY ReportsNumber DESC, c.[Name]

--09. Employees in Category
SELECT c.[Name],
	   COUNT(e.Id) AS [Employees Number]
  FROM Categories AS c
JOIN Departments AS d ON d.Id = c.DepartmentId
JOIN Employees AS e ON e.DepartmentId = d.Id
GROUP BY c.[Name]
ORDER BY c.[Name]

--10. Users per Employee
SELECT e.FirstName + ' ' + e.LastName AS [Name],
	   COUNT(DISTINCT r.UserId) AS [Users Number]
  FROM Employees AS e
LEFT JOIN Reports AS r ON r.EmployeeId = e.Id
GROUP BY e.FirstName + ' ' + e.LastName
ORDER BY [Users Number] DESC, [Name]

--11. Emergency Patrol
SELECT r.OpenDate, r.[Description], u.Email
  FROM Reports AS r
JOIN Users AS u ON u.Id = r.UserId
JOIN Categories AS c ON c.Id = r.CategoryId
JOIN Departments AS d ON d.Id = c.DepartmentId
WHERE r.CloseDate IS NULL AND LEN(r.[Description]) > 20 AND r.[Description] LIKE '%str%' AND d.Id IN (1, 4, 5)
ORDER BY r.OpenDate, u.Email, r.Id

--12. Birthday Report
SELECT DISTINCT c.[Name] FROM Categories AS c
JOIN Reports AS r ON r.CategoryId = c.Id
JOIN Users AS u ON u.Id = r.UserId
 WHERE DATEPART(MONTH, u.BirthDate) = DATEPART(MONTH, r.OpenDate) AND
	   DATEPART(DAY, u.BirthDate) = DATEPART(DAY, r.OpenDate)
ORDER BY c.[Name]

--13. Numbers Coincidence
SELECT DISTINCT u.Username FROM Users AS u
JOIN Reports AS r ON r.UserId = u.Id
JOIN Categories AS c ON c.Id = r.CategoryId
 WHERE (u.Username LIKE '[0-9]_%' AND CAST(c.Id AS varchar) = LEFT(u.Username, 1)) OR
	  (u.Username LIKE '%_[0-9]' AND CAST(c.Id AS varchar) = RIGHT(u.Username, 1))
ORDER BY u.Username

--14. Open/Closed Statistics
SELECT e.FirstName + ' ' + e.LastName AS [Name],
	ISNULL(CONVERT(varchar, c.[CLOSED]), 0)	 + '/' + ISNULL(CONVERT(varchar, o.[OPEN]), '0') AS [Closed Open Reports]
  FROM Employees AS e
JOIN 
(
	SELECT EmployeeId, COUNT(*) AS [OPEN]
	  FROM Reports
	 WHERE YEAR(OpenDate) = 2016
	GROUP BY EmployeeId
) AS o ON o.EmployeeId = e.Id
LEFT JOIN
(
	SELECT EmployeeId, COUNT(*) AS [CLOSED]
	  FROM Reports
	 WHERE YEAR(CloseDate) = 2016
	GROUP BY EmployeeId
) AS c ON c.EmployeeId = o.EmployeeId
ORDER BY [Name]

--15. Average Closing Time
SELECT d.[Name],
	   ISNULL
	   (
			CONVERT
			(
				VARCHAR, AVG(DATEDIFF(DAY, r.OpenDate, r.CloseDate))
			),
			'no info'
	   )
	    AS [Average Duration]
  FROM Departments AS d
JOIN Categories AS c ON c.DepartmentId = d.Id
JOIN Reports AS r ON r.CategoryId = c.Id
GROUP BY d.[Name]
GO

--16. Favorite Categories
WITH CTE_TotalReportsByDepartment(DepartmentId, [Count]) AS
(
	SELECT d.Id, 
		   COUNT(r.Id)
	  FROM Departments AS d
	JOIN Categories AS c ON c.DepartmentId = d.Id
	JOIN Reports AS r ON r.CategoryId = c.Id
	GROUP BY d.Id
)
SELECT d.[Name] AS [Department Name],
	   c.[Name] AS [Category Name],
	   CAST
	   (
			ROUND
			(
				CEILING(CAST(COUNT(r.Id) AS DECIMAL(7,2)) * 100
			) / tr.[Count], 0
		) AS INT) AS [Percentage]
  FROM Departments AS d
JOIN CTE_TotalReportsByDepartment AS tr ON tr.DepartmentId = d.Id
JOIN Categories AS c ON c.DepartmentId = d.Id
JOIN Reports AS r ON r.CategoryId = c.Id
GROUP BY d.[Name], c.[Name], tr.[Count]