--16. Customers With Countries 
CREATE VIEW v_UserWithCountries AS
SELECT CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
	   c.Age,
	   c.Gender,
	   co.Name AS CountryName 
  FROM Customers AS c
JOIN Countries AS co ON co.Id = c.CountryId
GO

--17. Feedback by Product Name 
CREATE FUNCTION udf_GetRating(@Name VARCHAR(25))
RETURNS VARCHAR(9)
AS
BEGIN
	DECLARE @avgRate DECIMAL(4, 2)= (SELECT AVG(Rate) FROM Feedbacks AS f
									 RIGHT JOIN Products AS p ON p.Id = f.ProductId
									 WHERE p.Name = @Name
									);

	DECLARE @result VARCHAR(9);
	
	IF(@avgRate IS NULL)
		SET @result = 'No rating';

	IF(@avgRate < 5)
		SET @result = 'Bad';

	IF(@avgRate BETWEEN 5 AND 8)
		SET @result ='Average';

	IF(@avgRate > 8)
		SET @result = 'Good';

	RETURN @Result;
END
GO

--18. Send Feedback 
CREATE PROC usp_SendFeedback @customerId INT, @productId INT, @rate DECIMAL(4, 2), @description NVARCHAR(255)
AS
BEGIN
	BEGIN TRANSACTION
		INSERT INTO Feedbacks (Description, Rate, ProductId, CustomerId) VALUES
		(@description, @rate, @productId, @customerId)

		DECLARE @feedbacksCount INT = (SELECT COUNT(*) 
									   FROM Feedbacks 
									   WHERE CustomerId = @customerId AND ProductId = @productId);

		IF(@feedbacksCount > 3)
		BEGIN 
			ROLLBACK;
			RAISERROR('You are limited to only 3 feedbacks per product!', 16, 1);
			RETURN;
		END
	COMMIT
END
GO

--19. Delete Products 
CREATE TRIGGER tr_DeleteProducts ON Products INSTEAD OF DELETE
AS
BEGIN
	DECLARE @productId INT = (SELECT Id FROM deleted);

    DELETE FROM ProductsIngredients
    WHERE ProductId = @productId

    DELETE FROM Feedbacks
    WHERE ProductId = @productId

    DELETE FROM Products
    WHERE Id = @productId
END
GO

--20. Products by One Distributor 
WITH CTE_ProductWithSingleDistrib(ProductId) AS
(
	SELECT p.Id FROM Products p
	JOIN ProductsIngredients pro ON p.Id = pro.ProductId
	JOIN Ingredients i ON pro.IngredientId = i.Id
	JOIN Distributors AS d ON i.DistributorId = d.Id
	GROUP BY p.Id
	HAVING COUNT(DISTINCT d.Id) = 1
)

SELECT r.ProductName,
	   r.AverageRate,
	   r.DistributorName,
	   r.DistributorCountry
FROM 
(
       SELECT p.Name AS ProductName,
			  AVG(f.Rate) AS AverageRate,
			  d.Name AS DistributorName,
			  c.Name AS DistributorCountry
       FROM CTE_ProductWithSingleDistrib s 
	   JOIN Products p ON s.ProductId = p.Id
       JOIN ProductsIngredients pro ON p.Id = pro.ProductId
       JOIN Ingredients i ON pro.IngredientId = i.Id
       JOIN Distributors d ON i.DistributorId = d.Id
       JOIN Countries c ON d.CountryId = c.Id
       JOIN Feedbacks f ON p.Id = f.ProductId
       GROUP BY p.Name, d.Name, c.Name
) AS r
JOIN Products p ON r.ProductName = p.Name
ORDER BY p.Id