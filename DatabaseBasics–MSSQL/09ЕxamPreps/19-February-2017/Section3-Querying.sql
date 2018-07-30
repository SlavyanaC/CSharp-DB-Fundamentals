--05. Products By Price 
SELECT Name, Price, Description FROM Products
ORDER BY Price DESC, Name

--06. Ingredients 
SELECT Name, Description, OriginCountryId FROM Ingredients
WHERE OriginCountryId IN (1, 10, 20)
ORDER BY Id

--07. Ingredients from Bulgaria and Greece 
SELECT TOP(15) i.Name, i.Description, c.Name 
  FROM Ingredients AS i
JOIN Countries AS c ON c.Id = i.OriginCountryId
WHERE c.Name IN ('Bulgaria', 'Greece')
ORDER BY i.Name, c.Name

--08. Best Rated Products 
SELECT TOP(10) p.Name,
			   p.Description,
			   AVG(f.Rate) AS AverageRate,
			   COUNT(f.Id) AS FeedbacksAmount
  FROM Products AS p
JOIN Feedbacks AS f ON f.ProductId = p.Id
GROUP BY  p.Name, p.Description
ORDER BY AverageRate DESC, FeedbacksAmount DESC

--09. Negative Feedback 
SELECT	f.ProductId,
	    f.Rate,
		f.Description,
		c.Id,
		c.Age,
		c.Gender
  FROM Feedbacks AS f
JOIN Customers AS c ON c.Id = f.CustomerId
WHERE f.Rate < 5
ORDER BY f.ProductId DESC, f.Rate

--10.	Customers without Feedback 
SELECT CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
	   c.PhoneNumber,
	   c.Gender
  FROM Customers AS c
LEFT JOIN Feedbacks AS f ON f.CustomerId = c.Id
WHERE f.Id IS NULL
ORDER BY c.Id

--11. Honorable Mentions 
SELECT  f.ProductId AS ProductId,
		CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
		f.Description AS FeedbackDescription
	FROM Feedbacks AS f
JOIN ( SELECT CustomerId,
			  COUNT(*) AS FeedbackCout
		FROM Feedbacks
		GROUP BY CustomerId
		HAVING COUNT(*) >= 3
) AS fc ON fc.CustomerId = f.CustomerId
JOIN Customers AS c ON c.Id = f.CustomerId
ORDER BY f.ProductId, CustomerName, f.Id

--12. Customers by Criteria 
SELECT FirstName, Age, PhoneNumber  
  FROM Customers
WHERE (Age >= 21 AND FirstName LIKE '%AN%') OR
	  (PhoneNumber LIKE '%38' AND CountryId != 31)
ORDER BY FirstName, Age DESC

--13. Middle Range Distributors 
SELECT d.Name,
	   i.Name,
	   p.Name,
	   avgProdRate.AverageRate
  FROM Distributors AS d
JOIN Ingredients AS i ON i.DistributorId = d.Id
JOIN ProductsIngredients AS pin ON pin.IngredientId = i.Id
JOIN Products AS p ON p.Id = pin.ProductId
JOIN ( SELECT ProductId, AVG(Rate) AS AverageRate
		 FROM Feedbacks 
	   GROUP BY ProductId
	   HAVING AVG(Rate) BETWEEN 5 AND 8
) avgProdRate ON avgProdRate.ProductId = p.Id
ORDER BY d.Name, i.Name, p.Name

--14. The Most Positive Country 
SELECT TOP(1) WITH TIES c.Name,
	   AVG(f.Rate) AS  FeedbackRate
  FROM Countries AS c
JOIN Customers AS cu ON cu.CountryId = c.Id
JOIN Feedbacks AS f ON f.CustomerId = cu.Id
GROUP BY c.Name
ORDER BY FeedbackRate DESC

--15. Country Representative 
SELECT r.CountryName, r.DistributorName
  FROM
(
	SELECT c.Name AS CountryName,
		   d.Name AS DistributorName,
		   COUNT(i.Id) AS IngredientsCount,
		   DENSE_RANK() OVER (PARTITION BY c.Name ORDER BY COUNT(i.Id) DESC) AS [Rank]
	  FROM Countries AS c
	JOIN Distributors AS d ON d.CountryId = c.Id
	JOIN Ingredients AS i ON i.DistributorId = d.Id
	GROUP BY c.Name, d.Name
) AS r
WHERE r.[Rank] = 1
ORDER BY r.CountryName, r.DistributorName