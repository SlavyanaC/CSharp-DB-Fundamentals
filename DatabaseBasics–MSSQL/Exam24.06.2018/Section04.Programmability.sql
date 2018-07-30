--18
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATE, @People INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @roomId INT = (SELECT TOP(1) r.Id FROM Hotels AS h
						   JOIN Rooms AS r ON r.HotelId = h.Id
						   JOIN Trips AS t ON t.RoomId = r.Id
						   WHERE HotelId = @HotelId AND
						   r.Beds >= @People AND
						   r.Id NOT IN (SELECT RoomId FROM Trips WHERE ArrivalDate < @Date AND ReturnDate > @Date)
						   ORDER BY (h.BaseRate + r.Price) * @People DESC);

	IF (@roomId IS NULL)
	BEGIN
		RETURN 'No rooms available';
	END


	DECLARE @roomType NVARCHAR(20) = (SELECT TOP(1) r.Type FROM Hotels AS h
									   JOIN Rooms AS r ON r.HotelId = h.Id
									   JOIN Trips AS t ON t.RoomId = r.Id
									   WHERE HotelId = @HotelId AND
									   r.Beds >= @People AND
									   r.Id NOT IN (SELECT RoomId FROM Trips WHERE ArrivalDate < @Date AND ReturnDate > @Date)
									   ORDER BY (h.BaseRate + r.Price) * @People DESC);

	DECLARE @beds INT = (SELECT TOP(1) r.Beds FROM Hotels AS h
						JOIN Rooms AS r ON r.HotelId = h.Id
						JOIN Trips AS t ON t.RoomId = r.Id
						WHERE HotelId = @HotelId AND
						r.Beds >= @People AND
						r.Id NOT IN (SELECT RoomId FROM Trips WHERE ArrivalDate < @Date AND ReturnDate > @Date)
						ORDER BY (h.BaseRate + r.Price) * @People DESC);

	DECLARE @price DECIMAL(14, 2) = (SELECT TOP(1) (h.BaseRate + r.Price) * @People FROM Hotels AS h
									JOIN Rooms AS r ON r.HotelId = h.Id
									JOIN Trips AS t ON t.RoomId = r.Id
									WHERE HotelId = @HotelId AND
									r.Beds >= @People AND
									r.Id NOT IN (SELECT RoomId FROM Trips WHERE ArrivalDate < @Date AND ReturnDate > @Date)
									ORDER BY (h.BaseRate + r.Price) * @People DESC);

	DECLARE @Result NVARCHAR(MAX) = 'Room ' + CAST(@roomId AS nvarchar) + ': ' + @roomType + ' ' + '(' + CAST(@beds AS nvarchar) + ' beds) - $' + CAST(@price AS nvarchar);

	RETURN @Result;
END
GO

--19
CREATE PROCEDURE usp_SwitchRoom(@TripId INT, @TargetRoomId INT)
AS
BEGIN

	BEGIN TRANSACTION
		UPDATE Trips
		SET RoomId = @TargetRoomId
		WHERE Id = @TripId

		DECLARE @currentHotelId INT = (SELECT r.HotelId FROM Trips AS t
										 JOIN Rooms AS r ON r.Id = t.RoomId
										 WHERE t.Id = @TripId);

		DECLARE @wantedHotelId INT = (SELECT TOP(1) r.HotelId FROM Trips AS t
										JOIN Rooms AS r ON r.Id = t.RoomId
										WHERE r.Id = 11);

		IF(@currentHotelId <> @wantedHotelId)
		BEGIN
			RAISERROR('Target room is in another hotel!', 16, 1);
			ROLLBACK;
			RETURN;
		END

		DECLARE @currentRoomBeds INT = (SELECT COUNT(AccountId) FROM AccountsTrips WHERE TripId = @TripId
										GROUP BY TripId);

		DECLARE @targetRoomBeds INT = (SELECT Beds FROM Rooms WHERE Id = @TargetRoomId);

		IF(@targetRoomBeds < @currentRoomBeds)
		BEGIN
			RAISERROR('Not enough beds in target room!', 16, 1);
			ROLLBACK;
			RETURN;
		END
	COMMIT
END
GO

--20
CREATE OR ALTER TRIGGER tr_CancelTrip ON Trips INSTEAD OF DELETE
AS
BEGIN
	DECLARE @tripCancelDate DATE = (SELECT CancelDate FROM inserted);
	IF(@tripCancelDate IS NULL)
	BEGIN
		UPDATE Trips
		SET CancelDate = GETDATE()
		FROM Trips AS t
		JOIN deleted AS d ON d.Id = t.Id
		WHERE t.CancelDate IS NULL
	END
END