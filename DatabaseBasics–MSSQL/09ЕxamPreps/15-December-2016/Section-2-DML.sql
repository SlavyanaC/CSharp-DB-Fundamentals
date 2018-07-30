--02. Insert 
INSERT INTO Messages(Content, SentOn, ChatId, UserId)
SELECT CONCAT(u.Age, '-', u.Gender, '-', l.Latitude, '-', l.Longitude),
	   GETDATE(),
	   CASE
			WHEN u.Gender = 'F' THEN CEILING(SQRT(u.Age * 2))
			WHEN u.Gender = 'M' THEN CEILING(POWER((u.Age / 18), 3))
	   END,
	   u.Id
	FROM Users AS u
JOIN Locations AS l ON u.LocationId = l.Id
WHERE u.Id BETWEEN 10 AND 20

--3. Update 
UPDATE Chats
SET StartDate = d.Date
FROM (SELECT c.Id AS ChatId, 
			 MIN(m.SentOn) AS Date
	    FROM Chats AS c 
	  JOIN Messages AS m ON m.ChatId = c.Id
	  GROUP BY c.Id
	  ) AS d
JOIN Messages AS m ON m.ChatId = d.ChatId
WHERE Chats.Id = d.ChatId AND Chats.StartDate > d.Date

--4. Delete
DELETE Locations 
WHERE Id IN(SELECT l.Id FROM Users AS u
			RIGHT JOIN Locations AS l ON l.Id = u.LocationId
		    WHERE u.Id IS NULL)