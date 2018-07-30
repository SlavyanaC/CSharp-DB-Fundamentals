--01. Employee Address
SELECT TOP(5) e.EmployeeID, e.JobTitle, e.AddressID, a.AddressText 
  FROM Employees AS e
JOIN Addresses AS a
ON a.AddressID = e.AddressID
ORDER BY a.AddressID

--02. Addresses with Towns
SELECT TOP(50) e.FirstName, e.LastName, t.Name, a.AddressText 
  FROM Employees AS e
JOIN Addresses AS a
ON a.AddressID = e.AddressID
JOIN Towns AS t
ON t.TownID = a.TownID
ORDER BY e.FirstName, e.LastName

--03. Sales Employees
SELECT e.EmployeeID, e.FirstName, e.LastName, d.[Name] AS [DepartmentName] 
  FROM Employees AS e
JOIN Departments AS d
ON e.DepartmentID = d.DepartmentID
WHERE d.DepartmentID = 3
ORDER BY EmployeeID

--04. Employee Departments
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name] 
  FROM Employees AS e
JOIN Departments AS d
ON d.DepartmentID = e.DepartmentID
WHERE e.Salary > 15000
ORDER BY d.DepartmentID

--05. Employees Without Projects
SELECT TOP(3) e.EmployeeID, e.FirstName 
  FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep
ON ep.EmployeeID = e.EmployeeID
WHERE ep.EmployeeID IS NULL
ORDER BY e.EmployeeID

--06. Employees Hired After
SELECT e.FirstName, e.LastName, e.HireDate, d.[Name] AS [DeptName]
  FROM Employees AS e
JOIN Departments AS d
ON d.DepartmentID = e.DepartmentID
WHERE d.[Name] IN ('Finance', 'Sales') AND e.HireDate > '1999-01-019'
ORDER BY e.HireDate

--07. Employees With Project
SELECT TOP(5) e.EmployeeID, e.FirstName, p.Name AS [ProjectName] 
  FROM Employees AS e
JOIN EmployeesProjects AS ep
ON ep.EmployeeID = e.EmployeeID
JOIN Projects AS p
ON p.ProjectID = ep.ProjectID
WHERE p.StartDate > '2002-08-13' AND p.EndDate IS NULL
ORDER BY e.EmployeeID

--08. Employee 24
SELECT e.EmployeeID, 
	   e.FirstName, 
	 IIF(p.StartDate > '2005-01-01', NULL,  p.[Name]) AS [ProjectName] 
  FROM Employees AS e
RIGHT JOIN EmployeesProjects AS ep
ON ep.EmployeeID = e.EmployeeID
RIGHT JOIN Projects AS p
ON p.ProjectID = ep.ProjectID
WHERE e.EmployeeID = 24

--09. Employee Manager
SELECT e.EmployeeID,
	   e.FirstName,
	   e.ManagerID AS [ManagerID],
	   e1.FirstName AS [ManagerName]
  FROM Employees AS e
JOIN Employees AS e1
ON e1.EmployeeID = e.ManagerID
WHERE e1.EmployeeID IN (3, 7)
ORDER BY e.EmployeeID

--10. Employees Summary
SELECT TOP(50) e.EmployeeID, 
	   e.FirstName + ' ' + e.LastName AS [EmployeeName],
	   e1.FirstName + ' ' + e1.LastName AS [ManagerName],
	   d.[Name]
  FROM Employees AS e
JOIN Employees AS e1
ON e1.EmployeeID = e.ManagerID
JOIN Departments AS D
ON d.DepartmentID = e.DepartmentID
ORDER BY e.EmployeeID

--11. Min Average Salary
SELECT TOP(1) AVG(Salary) AS MinAverageSalary
  FROM Employees
GROUP BY DepartmentID
ORDER BY MinAverageSalary

--12. Highest Peaks in Bulgaria
SELECT mc.CountryCode,
	   m.MountainRange,
	   p.PeakName,
	   p.Elevation
  FROM MountainsCountries AS mc
JOIN Peaks AS p
ON p.MountainId = mc.MountainId
JOIN Mountains AS m
ON m.Id = mc.MountainId
WHERE mc.CountryCode = 'BG' AND p.Elevation > 2835
ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT mc.CountryCode, 
	   COUNT(mc.MountainId) AS [MountainRanges]
  FROM MountainsCountries AS mc
JOIN Mountains AS m
ON m.Id = mc.MountainId
WHERE mc.CountryCode IN ('BG', 'RU', 'US')
GROUP BY mc.CountryCode

--14. Countries With or Without Rivers
SELECT TOP(5) c.CountryName, 
			  r.RiverName
  FROM Rivers AS r
RIGHT JOIN CountriesRivers AS cr
ON cr.RiverId = r.Id
RIGHT JOIN Countries AS c
ON c.CountryCode = cr.CountryCode
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName

--16. Countries Without any Mountains
SELECT COUNT(*)
  FROM Countries AS c
LEFT JOIN MountainsCountries AS mc
ON mc.CountryCode = c.CountryCode 
WHERE mc.MountainId IS NULL

--17. Highest Peak and Longest River by Country
SELECT TOP(5) c.CountryName,
	   MAX(p.Elevation) AS [HighestPeakElevation],
	   MAX(r.Length) AS [LongestRiverLength]
  FROM Countries AS c
JOIN MountainsCountries AS mc
ON mc.CountryCode = c.CountryCode
JOIN Peaks AS p
ON p.MountainId = mc.MountainId
JOIN CountriesRivers AS cr
ON cr.CountryCode = c.CountryCode
JOIN Rivers AS r
ON r.Id = cr.RiverId
GROUP BY c.CountryName
ORDER BY MAX(p.Elevation) DESC, MAX(r.Length) DESC, c.CountryName
