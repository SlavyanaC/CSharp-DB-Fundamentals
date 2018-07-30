--17.	Find My Ride
CREATE FUNCTION udf_CheckForVehicle(@townName NVARCHAR(50), @seatsNumber INT)
RETURNS NVARCHAR(100)
AS
BEGIN
	DECLARE @result NVARCHAR(100) = (
										SELECT TOP(1) o.[Name] + ' - ' + m.Model
										  FROM Offices AS o
										JOIN Towns AS t ON t.Id = o.TownId
										JOIN Vehicles AS v ON v.OfficeId = o.Id
										JOIN Models AS m ON m.Id = v.ModelId
										WHERE t.[Name] = @townName AND m.Seats = @seatsNumber
										ORDER BY o.[Name]
									);
	
	IF(@result IS NULL)
	BEGIN
		RETURN 'NO SUCH VEHICLE FOUND';
	END

	RETURN @result;
END
GO

--18. Move a Vehicle
CREATE PROC usp_MoveVehicle(@vehicleId INT, @officeId INT) 
AS
BEGIN
	BEGIN TRANSACTION
	DECLARE @takenSpots INT = (SELECT COUNT(*) FROM Vehicles WHERE OfficeId = @officeId);
	DECLARE @parkingPlaces INT = (SELECT ParkingPlaces FROM Offices WHERE Id = @officeId)
		UPDATE Vehicles
		SET OfficeId = @officeId
		WHERE Id = @vehicleId


		IF(@takenSpots >= @parkingPlaces)
		BEGIN
			ROLLBACK;
			RAISERROR('Not enough room in this office!', 16, 1);
			RETURN;
		END

	COMMIT
END

EXEC usp_MoveVehicle 7, 32;
SELECT OfficeId FROM Vehicles WHERE Id = 7
GO

--19. Move the Tally
CREATE TRIGGER tr_MoveTheTally ON Orders AFTER UPDATE
AS
BEGIN
	DECLARE @newValue INT = (SELECT TotalMileage FROM inserted);
	DECLARE @oldValue INT = (SELECT TotalMileage FROM deleted);
	DECLARE @vehicleId INT = (SELECT VehicleId FROM inserted);

	IF(@oldValue IS NULL AND @newValue IS NOT NULL)
	BEGIN
		UPDATE Vehicles
		SET Mileage += @newValue
		WHERE Id = @vehicleId
	END
END