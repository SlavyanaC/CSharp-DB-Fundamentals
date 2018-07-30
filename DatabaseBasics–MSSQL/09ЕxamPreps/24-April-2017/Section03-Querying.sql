--05. Clients by Name
SELECT FirstName, LastName, Phone FROM Clients
ORDER BY LastName, ClientId

--06. Job Status
SELECT Status, IssueDate FROM Jobs
WHERE Status = 'In Progress' OR Status = 'Pending'
ORDER BY IssueDate, JobId

--07. Mechanic Assignments
SELECT	m.FirstName + ' ' + m.LastName AS Mechanic,
	    j.[Status],
		j.IssueDate
  FROM Mechanics AS m
JOIN Jobs AS j ON j.MechanicId = m.MechanicId
ORDER BY m.MechanicId, j.IssueDate, j.JobId

--08. Current Clients
SELECT c.FirstName + ' ' + c.LastName AS Client,
	   DATEDIFF(DAY, j.IssueDate, '2017-04-24') AS [Days going],
	   [Status]
  FROM Clients AS c
JOIN Jobs AS j ON j.ClientId = c.ClientId
WHERE j.[Status] != 'Finished'
ORDER BY [Days going] DESC

--09. Mechanic Performance
SELECT CONCAT(m.FirstName, ' ', m.LastName) AS Mechanic,
	   AVG(DATEDIFF(DAY, j.IssueDate, J.Finishdate)) AS [Average Days]
  FROM Mechanics AS m
JOIN Jobs AS j ON j.MechanicId = m.MechanicId
GROUP BY CONCAT(m.FirstName, ' ', m.LastName), m.MechanicId
ORDER BY m.MechanicId

--10. Hard Earners
SELECT TOP(3) CONCAT(m.FirstName, ' ', m.LastName) AS Mechanic,
	   COUNT(j.JobId) AS Jobs
  FROM Mechanics AS m
JOIN Jobs AS j ON j.MechanicId = m.MechanicId
WHERE j.[Status] <> 'Finished' 
GROUP BY CONCAT(m.FirstName, ' ', m.LastName)
HAVING COUNT(j.JobId) > 1
ORDER BY Jobs DESC

--11. Available Mechanics
SELECT FirstName + ' ' + LastName AS Available
  FROM Mechanics AS m
JOIN
(
	SELECT MechanicId FROM Mechanics
	WHERE MechanicId NOT IN
	(
		SELECT MechanicId FROM Jobs WHERE [Status] <> 'Finished' AND MechanicId IS NOT NULL
	) 
) AS a ON a.MechanicId = m.MechanicId
ORDER BY m.MechanicId

--12. Parts Cost
SELECT ISNULL(SUM(p.Price * op.Quantity), 0) AS [Parts Total]
  FROM Parts AS p
  JOIN OrderParts AS op ON op.PartId = p.PartId
  JOIN Orders AS o  ON o.OrderId = op.OrderId
 WHERE DATEDIFF(WEEK, o.IssueDate, '2017-04-24') <= 3

--13. Past Expenses
SELECT j.JobId,
	   ISNULL(SUM(p.Price * op.Quantity), 0) AS	Total 
  FROM Jobs AS j
LEFT JOIN Orders AS o ON o.JobId = j.JobId
LEFT JOIN OrderParts AS op ON op.OrderId = o.OrderId
LEFT JOIN Parts AS p ON p.PartId = op.PartId
WHERE j.[Status] = 'Finished'
GROUP BY j.JobId
ORDER BY Total DESC, j.JobId

--14. Model Repair Time
SELECT m.ModelId,
		   m.[Name],
		  CONCAT(AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate)), ' days')  AS [Average Service Time]
  FROM Models AS m
JOIN Jobs AS j ON j.ModelId = m.ModelId
GROUP BY m.ModelId, m.[Name]
ORDER BY AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate))

--15. Faultiest Model
SELECT TOP(1) m.[Name], 
			  COUNT(*) AS [Times Serviced],
			  (
			  	  SELECT ISNULL(SUM(p.Price * op.Quantity), 0) 
			  	  FROM Jobs AS j
			  	  JOIN Orders AS o ON O.JobId = j.JobId
			  	  JOIN OrderParts AS op ON op.OrderId = o.OrderId
			  	  JOIN Parts AS p ON p.PartId = op.PartId
			  	  WHERE j.ModelId = m.ModelId
			  ) AS [Parts Total]
 FROM Models AS m
JOIN Jobs AS j ON j.ModelId = m.ModelId
GROUP BY m.ModelId, m.Name
ORDER BY [Times Serviced] DESC

--16. Missing Parts
SELECT p.PartId,
	   p.[Description],
	   SUM(pn.Quantity) AS [Required],
	   AVG(p.StockQty) AS [In Stock],
	   ISNULL(SUM(op.Quantity), 0) AS [Ordered]
 FROM Parts AS p
JOIN PartsNeeded AS pn ON pn.PartId = p.PartId
JOIN Jobs AS j ON j.JobId = pn.JobId
LEFT JOIN Orders AS o ON o.JobId = j.JobId
LEFT JOIN OrderParts AS op ON op.OrderId = o.OrderId
WHERE j.Status <> 'Finished'
GROUP BY p.PartId, p.[Description]
HAVING SUM(pn.Quantity) > AVG(p.StockQty) + ISNULL(SUM(op.Quantity), 0)
ORDER BY p.PartId