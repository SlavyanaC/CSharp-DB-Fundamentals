--01. Employees with Salary Above 35000
CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000
AS
SELECT FirstName, LastName FROM Employees
WHERE Salary > 35000

EXEC usp_GetEmployeesSalaryAbove35000
GO

--02. Employees with Salary Above Number
CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber(@minNumber DECIMAL(18, 4))
AS
SELECT FirstName, LastName FROM Employees
WHERE Salary >= @minNumber

EXEC usp_GetEmployeesSalaryAboveNumber 48100
GO

--03. Town Names Starting With
CREATE PROCEDURE usp_GetTownsStartingWith(@startString VARCHAR(50))
AS
SELECT [Name] FROM Towns
WHERE [Name] LIKE @startString + '%'

EXEC  usp_GetTownsStartingWith 'b'
GO

--04. Employees from Town
CREATE PROCEDURE usp_GetEmployeesFromTown(@townName VARCHAR(50))
AS
SELECT e.FirstName, e.LastName FROM Employees AS e
JOIN Addresses AS a ON a.AddressID = e.AddressID
JOIN Towns AS t ON t.TownID = a.TownID
WHERE t.[Name] = @townName 

EXEC usp_GetEmployeesFromTown 'Sofia'
GO

--05. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4)) 
RETURNS VARCHAR(7)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(7)

	IF (@salary < 30000)
		SET @salaryLevel = 'Low'
	ELSE IF(@salary >= 30000 AND @salary <= 50000)
		SET @salaryLevel = 'Average'
	ELSE
		SET @salaryLevel = 'High'

	RETURN @salaryLevel
END

GO
SELECT FirstName, LastName, Salary, dbo.ufn_GetSalaryLevel(Salary) FROM Employees
GO

--06. Employees by Salary Level
CREATE PROCEDURE usp_EmployeesBySalaryLevel(@salaryLevel VARCHAR(7))
AS
SELECT FirstName, LastName FROM Employees
WHERE dbo.ufn_GetSalaryLevel(Salary) = @salaryLevel

EXEC usp_EmployeesBySalaryLevel 'High'
GO

--07. Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(50), @word VARCHAR(50))
RETURNS BIT
AS
BEGIN
	DECLARE @result BIT = 0;
	DECLARE @currentIndex INT = 1;
	DECLARE @currentChar CHAR;

	WHILE(@currentIndex <= LEN(@word))
	BEGIN
		SET @currentChar = SUBSTRING(@word, @currentIndex, 1);
		IF(CHARINDEX(@currentChar, @setOfLetters) = 0)
			RETURN @result;
					
		SET @currentIndex += 1;
	END

	RETURN @result + 1;
END
GO

SELECT dbo.ufn_IsWordComprised('oistmiahf', 'Sofia')
GO

--08. Delete Employees and Departments 
CREATE PROC usp_DeleteEmployeesFromDepartment(@departmentId INT)
AS
  ALTER TABLE Departments
    ALTER COLUMN ManagerID INT NULL

  DELETE FROM EmployeesProjects
  WHERE EmployeeID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  UPDATE Employees
  SET ManagerID = NULL
  WHERE ManagerID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  UPDATE Departments
  SET ManagerID = NULL
  WHERE ManagerID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  DELETE FROM Employees
  WHERE EmployeeID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  DELETE FROM Departments
  WHERE DepartmentID = @departmentId

  SELECT COUNT(*) AS [Employees Count]
  FROM Employees AS E
    JOIN Departments AS D
      ON D.DepartmentID = E.DepartmentID
  WHERE E.DepartmentID = @departmentId

--09. Find Full Name
CREATE PROCEDURE usp_GetHoldersFullName 
AS
SELECT FirstName + ' ' + LastName AS [Full Name] FROM AccountHolders

EXEC dbo.usp_GetHoldersFullName
GO

--10. People with Balance Higher Than
CREATE PROCEDURE usp_GetHoldersWithBalanceHigherThan(@minimum DECIMAL(15, 2))
AS
BEGIN
	SELECT FirstName, LastName FROM  
	(
		SELECT FirstName, LastName, SUM(a.Balance) AS [Total Balance] FROM AccountHolders AS ah
		JOIN Accounts AS a ON a.AccountHolderId = ah.Id
		GROUP BY ah.FirstName, ah.LastName
	) AS tb
	WHERE tb.[Total Balance] > @minimum
END

EXEC dbo.usp_GetHoldersWithBalanceHigherThan 3500
GO

--11. Future Value Function
CREATE FUNCTION ufn_CalculateFutureValue(@sum DECIMAL(15, 2), @yearlyInterestRate FLOAT, @numberOfYears INT)
RETURNS DECIMAL(15, 4)
AS 
BEGIN
	DECLARE @result DECIMAL(15, 4);
	SET @result = @SUM * POWER(1 + @yearlyInterestRate, @numberOfYears);

	RETURN @result;
END
GO

SELECT dbo.ufn_CalculateFutureValue(1000, 0.10, 5)
GO

--12. Calculating Interest
CREATE PROCEDURE usp_CalculateFutureValueForAccount(@AccountId INT, @interestRate FLOAT)
AS
SELECT ah.Id, 
		FirstName, 
		LastName, 
		a.Balance, 
		dbo.ufn_CalculateFutureValue(a.Balance, @interestRate, 5) AS [Balance in 5 years]
  FROM AccountHolders AS ah
JOIN Accounts AS a ON a.AccountHolderId = ah.Id
WHERE a.Id = @AccountId

EXEC dbo.usp_CalculateFutureValueForAccount 1, 0.1
GO

--14. Create Table Logs
CREATE TRIGGER tr_AccountBalanceUpdate ON Accounts AFTER UPDATE
AS
BEGIN
	DECLARE @accountId INT = (SELECT Id FROM inserted);
	DECLARE @oldBalance MONEY = (SELECT Balance FROM deleted);
	DECLARE @newBalance MONEY = (SELECT Balance FROM inserted);

	IF(@oldBalance <> @newBalance)
		INSERT INTO Logs VALUES 
		(@accountId, @oldBalance, @newBalance);
END
GO

--15. Create Table Emails
CREATE TRIGGER tr_NotificationEmails ON Logs AFTER INSERT
AS
BEGIN 
	DECLARE @recipient INT = (SELECT AccountId FROM inserted);
	DECLARE @oldBalance MONEY = (SELECT OldSum FROM inserted);
	DECLARE @newBalance MONEY = (SELECT NewSum FROM inserted);
	DECLARE @subject VARCHAR(MAX) = CONCAT('Balance change for account: ', @recipient);
	DECLARE @body VARCHAR(MAX) = CONCAT('On ', GETDATE(), 'your balance was changed from ', @oldBalance, 'to ', @newBalance, '.');

	INSERT INTO NotificationEmails VALUES
	(@recipient, @subject, @body);
END 
GO

--16. Deposit Money
CREATE PROCEDURE usp_DepositMoney(@accountId INT, @depositAmount MONEY)
AS
BEGIN
	BEGIN TRANSACTION
		UPDATE Accounts
		SET Balance += @depositAmount
		WHERE Id = @accountId

		IF @@ROWCOUNT <> 1
			BEGIN
				ROLLBACK
				RAISERROR('Invalid Account!', 16, 1);
				RETURN
			END
	COMMIT
END

EXEC dbo.usp_DepositMoney 1, 10
GO

--17. Withdraw Money Procedure
CREATE PROCEDURE usp_WithdrawMoney(@accountId INT, @depositAmount MONEY)
AS
BEGIN
	BEGIN TRANSACTION
		UPDATE Accounts
		SET Balance -= @depositAmount
		WHERE Id = @accountId

		IF @@ROWCOUNT <> 1
			BEGIN
				ROLLBACK
				RAISERROR('Invalid Account!', 16, 2);
				RETURN
			END
	COMMIT
END 
GO

--18. Money Transfer
CREATE PROCEDURE usp_TransferMoney(@senderId INT, @receiverId INT, @amount MONEY)
AS
BEGIN
	DECLARE @senderInitialBalance MONEY = (SELECT Balance FROM Accounts WHERE Id = @senderId)
	DECLARE @receiverInitialBalance MONEY = (SELECT Balance FROM Accounts WHERE Id = @receiverId);

	IF(@senderInitialBalance IS NULL)
	BEGIN
		RAISERROR('Insufficient Sender Balance!', 16, 1);
		RETURN;
	END

	IF(@receiverId IS NULL)
	BEGIN
		RAISERROR('Insufficient Receiver Balance!', 16, 2);
		RETURN;
	END

	IF(@amount <= 0)
	BEGIN
		RAISERROR('Transfer amount cannot be zero or negative!', 16, 3);
		RETURN;
	END

	BEGIN TRANSACTION
		EXEC dbo.usp_WithdrawMoney @senderId, @amount;
		EXEC dbo.usp_DepositMoney @receiverId, @amount;

		DECLARE @senderFinalBalance MONEY = (SELECT Balance FROM Accounts WHERE Id = @senderId);
		DECLARE @receiverFinalBalance MONEY = (SELECT Balance FROM Accounts WHERE Id = @receiverId);

		IF(@senderFinalBalance <> @senderInitialBalance - @amount OR
		   @receiverFinalBalance <> @receiverInitialBalance + @amount)
		BEGIN
			ROLLBACK;
			RAISERROR('Failed transaction!', 16 , 4);
			RETURN;	
		END
	COMMIT
END

EXEC dbo.usp_TransferMoney 5, 1, 5000 
GO

--21. Employees with Three Projects
CREATE PROCEDURE usp_AssignProject(@emloyeeId INT, @projectID INT)
AS
BEGIN
	DECLARE @maxProjectsCount INT = 3;
	DECLARE @currentProjectsCount INT;

	BEGIN TRANSACTION 
	INSERT INTO EmployeesProjects VALUES
	(@emloyeeId, @projectID)

	SET @currentProjectsCount = (SELECT COUNT(*) FROM EmployeesProjects WHERE EmployeeID = @emloyeeId)
	
	IF(@currentProjectsCount > @maxProjectsCount)
	BEGIN
		ROLLBACK
		RAISERROR('The employee has too many projects!', 16, 1);
		RETURN
	END
	
	COMMIT
END 

--22. Delete Employees
CREATE TABLE Deleted_Employees(
	EmployeeId INT NOT NULL IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL, 
	MiddleName VARCHAR(50),
	JobTitle VARCHAR(50) NOT NULL,
	DepartmentId INT NOT NULL,
	Salary MONEY NOT NULL,

	CONSTRAINT PK_Deleted_Employees PRIMARY KEY (EmployeeId)
) 
GO

CREATE TRIGGER tr_DeleteEmployees ON Employees AFTER DELETE
AS
BEGIN
	INSERT INTO Deleted_Employees
	SELECT FirstName, LastName, MiddleName, JobTitle, DepartmentID, Salary FROM deleted
END
