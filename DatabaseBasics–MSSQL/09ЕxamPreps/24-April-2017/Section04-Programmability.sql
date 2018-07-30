--17. Cost of Order
CREATE FUNCTION udf_GetCost(@jobId INT)
RETURNS DECIMAL(6, 2)
AS
BEGIN
	DECLARE @totalCost DECIMAL(6, 2) = 
	(
		SELECT ISNULL(SUM(p.Price * op.Quantity), 0) FROM Jobs AS j
		JOIN Orders AS o ON o.JobId = j.JobId
		JOIN OrderParts AS op ON op.OrderId = o.OrderId
		JOIN Parts AS p ON p.PartId = op.PartId
		WHERE j.JobId = @jobId
	)
	RETURN  @totalCost
END
GO

--18. Place Order
CREATE PROCEDURE usp_PlaceOrder(@jobId INT, @serialNumber VARCHAR(50), @quantity INT)
AS
BEGIN
	IF(@quantity <= 0)
	BEGIN
		RAISERROR('Part quantity must be more than zero!', 16, 1);
		RETURN;
	END

	DECLARE @selectedJobId INT = (SELECT JobId FROM Jobs WHERE JobId = @jobId);
	IF(@selectedJobId IS NULL)
	BEGIN
		RAISERROR('Job not found!', 16, 1);
		RETURN;
	END

	DECLARE @jobStatus VARCHAR(11) = (SELECT [Status] FROM Jobs WHERE JobId = @selectedJobId);
	IF(@jobStatus = 'Finished')
	BEGIN
		RAISERROR('This job is not active!', 16, 1);
		RETURN;
	END
	
	DECLARE @selectedPartId INT = (SELECT PartId FROM Parts WHERE SerialNumber = @serialNumber);
	IF(@selectedPartId IS NULL)
	BEGIN
		RAISERROR('Part not found!', 16, 1);
		RETURN;
	END

	DECLARE @orderId INT = (
								SELECT o.OrderId FROM Orders AS o
								JOIN OrderParts AS op ON op.OrderId = o.OrderId
								JOIN Parts AS p ON p.PartId = op.PartId
								WHERE JobId = @jobId AND p.PartId = @selectedPartId AND o.IssueDate IS NULL
							);

	IF(@orderId IS NULL)
	BEGIN
		INSERT INTO Orders (JobId, IssueDate) VALUES
		(@jobId, NULL)		

		INSERT INTO OrderParts (OrderId, PartId, Quantity) VALUES
		(IDENT_CURRENT('Orders'), @selectedPartId, @quantity)
	END

	ELSE
	BEGIN
		DECLARE @matchesCount INT = (SELECT @@ROWCOUNT FROM OrderParts WHERE OrderId = @orderId AND PartId = @selectedPartId);

		IF(@matchesCount IS NULL)
		BEGIN
			INSERT INTO OrderParts (OrderId, PartId, Quantity) VALUES
			(@orderId, @selectedPartId, @quantity)
		END

		ELSE 
		BEGIN
			UPDATE OrderParts
			SET Quantity += @quantity
			WHERE OrderId = @orderId AND PartId = @selectedPartId
		END
	END

END
GO


--19. Detect Delivery
CREATE TRIGGER tr_DetectDelivery ON Orders AFTER UPDATE
AS
BEGIN
	DECLARE @initialStatus INT = (SELECT Delivered FROM deleted);
	DECLARE @newStatus INT = (SELECT Delivered FROM inserted);

	IF(@initialStatus = 0 AND @newStatus = 1)
	BEGIN
		UPDATE Parts
		SET StockQty += op.Quantity
		FROM Parts AS p
		JOIN OrderParts AS op ON op.PartId = p.PartId
		JOIN Orders AS o ON o.OrderId = op.OrderId
		JOIN inserted AS i ON i.OrderId = o.OrderId
		JOIN deleted AS d ON d.OrderId = i.OrderId
	END

END
GO

--20
WITH CTE_Parts(MechanicId, VendorId, TotalParts) AS
(
	SELECT m.MechanicId, 
		   v.VendorId,
		   SUM(op.Quantity) AS TotalParst
	  FROM Mechanics AS m
	  JOIN Jobs AS j ON j.MechanicId = m.MechanicId
	  JOIN Orders AS o ON o.JobId = j.JobId
	  JOIN OrderParts AS op ON op.OrderId = o.OrderId
	  JOIN Parts AS p ON p.PartId = op.PartId
	  JOIN Vendors AS v ON v.VendorId = p.VendorId
	  GROUP BY m.MechanicId, v.VendorId
)
SELECT CONCAT(m.FirstName, ' ', m.LastName) AS Mechanic,
	   v.[Name] AS Vendor,
	   cp.TotalParts AS Parts,
	   CAST((cp.TotalParts * 100) / (SELECT SUM(TotalParts) FROM CTE_Parts WHERE MechanicId = m.MechanicId) AS varchar) + '%' AS Preference
  FROM CTE_Parts AS cp
  JOIN Mechanics AS m ON m.MechanicId = cp.MechanicId
  JOIN Vendors AS v ON v.VendorId = cp.VendorId
ORDER BY Mechanic, TotalParts DESC, v.[Name]