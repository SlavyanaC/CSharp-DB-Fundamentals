--15. Radians 
CREATE FUNCTION udf_GetRadians(@degrees FLOAT)
RETURNS FLOAT
AS
BEGIN
	DECLARE @result FLOAT = (SELECT (@degrees * PI()) / 180);
	RETURN @result;
END
GO

--16. Change Password 
CREATE PROCEDURE udp_ChangePassword(@email VARCHAR(30), @newPassword VARCHAR(20))
AS
BEGIN
	DECLARE @userId INT = (SELECT u.Id FROM Users AS u
						   JOIN Credentials AS c ON c.Id = u.Id
						   WHERE c.Email = @email);

	IF(@userId IS NULL)
	BEGIN 
		RAISERROR('The email does''t exist!', 16, 1);
		RETURN;
	END

	UPDATE Credentials 
	SET Password = @newPassword
	WHERE Email = @email
END
GO

--17.Send Message 
CREATE PROCEDURE udp_SendMessage(@userId INT, @chatId INT, @content VARCHAR(200))
AS
BEGIN
	IF(EXISTS(SELECT * FROM UsersChats WHERE UserId = @userId AND ChatId = @chatId))
	BEGIN
		INSERT INTO Messages(Content, SentOn, ChatId, UserId) VALUES
		(@content, GETDATE(), @chatId, @userId)
	END
	ELSE
	BEGIN
		RAISERROR('There is no chat with that user!', 16, 1);
	END
END
GO

--18.Log Messages 
CREATE TRIGGER tr_LogMessages ON Messages AFTER DELETE
AS
BEGIN
    INSERT INTO MessageLogs(Id, Content, SentOn, ChatId, UserId)
      SELECT  Id, Content, SentOn, ChatId, UserId FROM deleted;
END

--19. Delete users 
CREATE TRIGGER tr_DeleteUser ON Users INSTEAD OF DELETE
AS
  BEGIN
    DECLARE @UserId INT = (SELECT Id FROM deleted);

    DELETE FROM UsersChats
    WHERE UserId = @UserId

    DELETE FROM Messages
    WHERE UserId = @UserId

    DELETE FROM Users
    WHERE Id = @UserId
END