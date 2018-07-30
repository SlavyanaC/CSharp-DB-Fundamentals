SELECT * FROM Employees

SELECT FirstName + ' ' + LastName AS [Full Name],
	   JobTitle AS [Job Title],
	   Salary
   FROM Employees

SELECT DISTINCT DepartmentId
  FROM Employees

SELECT FirstName + ' ' + LastName AS [Full Name],
	   DepartmentId
  FROM Employees
 WHERE DepartmentID = 1

SELECT FirstName + ' ' + LastName AS [Full Name],
	   Salary
  FROM Employees
 WHERE Salary <= 20000
 ORDER BY Salary DESC

SELECT LastName
  FROM Employees
 WHERE NOT (ManagerId = 3 OR ManagerId = 4)

SELECT * FROM v_EmployeesBySalary
 WHERE Salary >= 20000 AND Salary < 22000

SELECT FirstName + ' ' + LastName AS [Full Name],
	   ManagerId
  FROM Employees
 WHERE ManagerID IN (109, 3, 16)
GO

CREATE VIEW v_EmployeesBySalary AS
SELECT FirstName + ' ' + LastName AS [Full Name],
	   Salary
  FROM Employees
GO

USE Geography

SELECT * FROM Peaks
GO

CREATE VIEW v_HighestPeak AS
SELECT TOP(1) * 
	  FROM Peaks
  ORDER BY Elevation DESC
GO

SELECT * FROM v_HighestPeak

USE SoftUni

INSERT INTO Projects ([Name], StartDate)
	 SELECT [Name] + ' Restructuring', GETDATE()
	  FROM Departments

SELECT * FROM Projects

UPDATE Projects
   SET EndDate = GETDATE()
 WHERE EndDate IS NULL