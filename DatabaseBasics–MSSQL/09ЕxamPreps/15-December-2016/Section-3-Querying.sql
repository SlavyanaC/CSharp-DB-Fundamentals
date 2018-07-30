--05. Age Range 
SELECT Nickname, Gender, Age FROM Users
WHERE Age BETWEEN 22 AND 37

--06. Messages
SELECT Content, SentOn FROM Messages
WHERE SentOn > '2014-05-12' AND Content LIKE('%JUST%')
ORDER BY Id DESC

--07. Chats 
SELECT Title, IsActive FROM Chats
WHERE IsActive = 0 AND LEN(Title) < 5 OR SUBSTRING(Title, 3, 2) = 'TL'
ORDER BY Title DESC

--08. Chat Messages 
SELECT c.Id, c.Title, m.Id
  FROM Chats AS c
JOIN Messages AS m ON m.ChatId = c.Id
WHERE m.SentOn < '2012-03-26' AND RIGHT(c.Title, 1) = 'x'
ORDER BY c.Id, m.Id

--09.	Message Count
SELECT TOP(5) c.Id,
			  COUNT(m.Id) AS TotalMessages
  FROM Chats AS c
RIGHT JOIN Messages AS m ON m.ChatId = c.Id
WHERE m.Id < 90 
GROUP BY c.Id
ORDER BY TotalMessages DESC, c.Id

--10. Credentials
SELECT u.Nickname, c.Email, c.Password
  FROM Users AS u
JOIN Credentials AS c ON c.Id = u.CredentialId
WHERE c.Email LIKE '%co.uk'
ORDER BY c.Email

--11. Locations
SELECT Id, Nickname, Age
  FROM Users
WHERE LocationId IS NULL

--12. Left Users 
SELECT Id, ChatId, UserId
  FROM Messages
WHERE ChatId = 17 AND UserId NOT IN
(
	SELECT UserId FROM UsersChats WHERE ChatId = 17
) OR UserId IS NULL
ORDER BY Id DESC

--13. Users in Bulgaria 
SELECT u.Nickname, c.Title, l.Latitude, l.Longitude 
  FROM Users AS u
JOIN Locations AS l ON l.Id = u.LocationId
JOIN UsersChats AS uc ON uc.UserId = u.Id
JOIN Chats AS c ON c.Id = uc.ChatId
WHERE l.Latitude BETWEEN 41.139999 AND 44.12999 AND
	  l.Longitude BETWEEN 22.20999 AND 28.35999
ORDER BY c.Title

-- 14. Last Chat 
WITH CTE_LastChat(ChatId, Title) AS
(
	SELECT TOP(1) Id, Title
	  FROM Chats
	GROUP BY Id, Title
	ORDER BY MAX(StartDate) DESC
)
SELECT lc.Title, m.Content
  FROM Messages AS m
RIGHT JOIN CTE_LastChat AS lc ON lc.ChatId = m.ChatId