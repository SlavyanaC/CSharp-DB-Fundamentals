--17. Employee's Load
CREATE FUNCTION udf_GetReportsCount(@employeeId INT, @statusId INT)
RETURNS INT
AS
BEGIN
	DECLARE @CountOfReports INT = (
		SELECT COUNT(*)
		  FROM Reports AS r
		WHERE EmployeeId = @employeeId AND StatusId = @statusId
	  );

	  RETURN @CountOfReports;
END
GO

SELECT Id, FirstName, Lastname, dbo.udf_GetReportsCount(Id, 2) AS ReportsCount
FROM Employees
ORDER BY Id
GO

--18. Assign Employee
CREATE PROC usp_AssignEmployeeToReport(@employeeId INT, @reportId INT)
AS
BEGIN
	DECLARE @EmployeeDepartmentId INT = (SELECT DepartmentId FROM Employees WHERE Id = @employeeId);
	DECLARE @ReportDepartmentId INT = (
										SELECT c.DepartmentId FROM Reports AS r
										JOIN Categories AS c ON c.Id = r.CategoryId
										WHERE r.Id = @reportId
									  );

	BEGIN TRANSACTION
		UPDATE Reports
		SET EmployeeId = @employeeId
		WHERE Id = @reportId

		IF(@EmployeeDepartmentId <> @ReportDepartmentId)
		BEGIN
			ROLLBACK;
			RAISERROR('Employee doesn''t belong to the appropriate department!', 16, 1);
			RETURN;
		END

	COMMIT
END
GO

EXEC usp_AssignEmployeeToReport 17, 2;
SELECT EmployeeId FROM Reports WHERE id = 2
GO

--19. Close Reports
CREATE TRIGGER tr_CloseReport ON Reports AFTER UPDATE
AS 
BEGIN
	UPDATE Reports
	SET StatusId = 3
	WHERE Id IN (SELECT Id FROM inserted
			      WHERE Id IN (SELECT Id FROM deleted
						        WHERE CloseDate IS NULL)
			           AND CloseDate IS NOT NULL)  
END

--20. Categories Revision 
SELECT c.[Name],
	   COUNT(r.Id) AS [Reports Number],
	   CASE
			WHEN SUM(CASE WHEN r.StatusId = 2 THEN 1 ELSE 0 END) >
				 SUM(CASE WHEN r.StatusId = 1 THEN 1 ELSE 0 END) THEN 'in progress'
			WHEN SUM(CASE WHEN r.StatusId = 2 THEN 1 ELSE 0 END) <
				 SUM(CASE WHEN r.StatusId = 1 THEN 1 ELSE 0 END) THEN 'waiting'
			ELSE 'equal'
	   END AS MainStatus
  FROM Reports AS r
JOIN Categories AS c ON c.Id = r.CategoryId
WHERE r.StatusId IN (1, 2)
GROUP BY c.[Name]
ORDER BY c.[Name], [Reports Number], MainStatus
