--01. One-To-One Relationship
CREATE TABLE Passports(
	PassportID INT,
	PassportNumber NVARCHAR(50)

	CONSTRAINT PK_PassportID PRIMARY KEY (PassportID)
)

CREATE TABLE Persons (
	PersonID INT IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	Salary DECIMAL(15, 2) NOT NULL, 
	PassportID INT NOT NULL UNIQUE,

	CONSTRAINT PK_PersonId PRIMARY KEY (PersonID),
	CONSTRAINT FK_PersonPassport FOREIGN KEY (PassportID) REFERENCES  Passports(PassportID)
)


INSERT INTO Passports (PassportID, PassportNumber) VALUES
(101, 'N34FG21B'),
(102, 'K65LO4R7'),
(103, 'ZE657QP2')

INSERT INTO Persons (FirstName, Salary, PassportID) VALUES
('Roberto', 43300.00, 102),
('Tom', 56100.00, 103),
('Yana', 60200.00, 101)

--02. One-To-Many Relationship
CREATE TABLE Manufacturers(
	ManufacturerID INT IDENTITY,
	[Name] NVARCHAR(50),
	EstablishedOn DATE

	CONSTRAINT PK_ManufacturerID PRIMARY KEY (ManufacturerID),
)

CREATE TABLE Models(
	ModelID INT,
	[Name] NVARCHAR(50),
	ManufacturerID INT NOT NULL,

	CONSTRAINT PK_ModelID PRIMARY KEY (ModelID),
	CONSTRAINT FK_ModelManufacturer FOREIGN KEY (ManufacturerID) REFERENCES Manufacturers(ManufacturerID)
)


INSERT INTO Manufacturers ([Name], EstablishedOn) VALUES
('BMW', '07/03/1916'),
('Tesla', '01/01/2003'),
('Lada', '01/05/1966')

INSERT INTO Models (ModelID, [Name], ManufacturerID) VALUES
(101, 'X1', 1),
(102, 'i6', 1),
(103, 'Model S', 2),
(104, 'Model X', 2),
(105, 'Model 3', 2),
(106, 'Nova', 3)

--03. Many-To-Many Relationship
CREATE TABLE Students (
	StudentID INT IDENTITY,
	[Name] NVARCHAR(50)

	CONSTRAINT PK_StudentID PRIMARY KEY (StudentID)
)

CREATE TABLE Exams (
	ExamID INT IDENTITY (101, 1),
	[Name] NVARCHAR(50)

	CONSTRAINT PK_ExamsID PRIMARY KEY (ExamID)
)

CREATE TABLE StudentsExams (
	StudentID INT NOT NULL,
	ExamID INT NOT NULL,

	CONSTRAINT PK_StudentsExams PRIMARY KEY (StudentID, ExamID),
	CONSTRAINT FK_StudentsExams_Student FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
	CONSTRAINT FK_StudentsExams_Exam FOREIGN KEY (ExamID) REFERENCES Exams(ExamID)
)

INSERT INTO Students ([Name]) VALUES
('Mila'),
('Toni'),
('Ron')

INSERT INTO Exams ([Name]) VALUES
('SpringMVC'),
('Neo4j'),
('Oracle 11g')

INSERT INTO StudentsExams (StudentID, ExamID) VALUES
(1, 101),
(1, 102),
(2, 101),
(3, 103),
(2, 102),
(2, 103)

--04. Self-Referencing
CREATE TABLE Teachers (
	TeacherID INT IDENTITY(101, 1),
	[Name] NVARCHAR(50),
	ManagerID INT

	CONSTRAINT PK_TeacherID PRIMARY KEY (TeacherID),
	CONSTRAINT FK_TeachersManager FOREIGN KEY (ManagerID) REFERENCES Teachers(TeacherID)
)

INSERT INTO Teachers ([Name], ManagerID) VALUES
('John', NULL),
('Maya', 106),
('Silvia', 106),
('Ted', 105),
('Mark', 101),
('Greta', 101)

--05. Online Store Database
CREATE TABLE Cities (
	CityID INT IDENTITY,
	[Name] VARCHAR(50),
	
	CONSTRAINT PK_CityID PRIMARY KEY (CityID)
)

CREATE TABLE Customers (
	CustomerID INT IDENTITY,
	[Name] VARCHAR(50),
	Birthday DATE,
	CityID INT NOT NULL

	CONSTRAINT PK_CustomerID PRIMARY KEY (CustomerID),
	CONSTRAINT FK_CustomerCity FOREIGN KEY (CityID) REFERENCES Cities(CityID)
)

CREATE TABLE Orders (
	OrderID INT IDENTITY,
	CustomerID INT NOT NULL,

	CONSTRAINT PK_OrderID PRIMARY KEY (OrderID),
	CONSTRAINT FK_OrderCustomer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
)

CREATE TABLE ItemTypes (
	ItemTypeID INT IDENTITY,
	[Name] VARCHAR(50),

	CONSTRAINT PK_ItemType PRIMARY KEY (ItemTypeID)
)

CREATE TABLE Items (
	ItemID INT IDENTITY,
	[Name] VARCHAR(50),
	ItemTypeID INT NOT NULL,

	CONSTRAINT PK_Item PRIMARY KEY (ItemID),
	CONSTRAINT FK_ItemItemType FOREIGN KEY (ItemTypeID) REFERENCES ItemTypes(ItemTypeID)
)

CREATE TABLE OrderItems (
	OrderID INT NOT NULL,
	ItemID INT NOT NULL,

	CONSTRAINT PK_OrderItem PRIMARY KEY (OrderID, ItemID),
	CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
	CONSTRAINT FK_OrderItems_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
)

--06. University Database
CREATE TABLE Subjects (
	SubjectID INT IDENTITY,
	SubjectName VARCHAR(50),

	CONSTRAINT PK_Subject PRIMARY KEY (SubjectID)
)

CREATE TABLE Majors (
	MajorID INT IDENTITY,
	[Name] VARCHAR(50),

	CONSTRAINT PK_Major PRIMARY KEY (MajorID)
)

CREATE TABLE Students (
	StudentID INT IDENTITY,
	StudentNumber INT NOT NULL,
	StudentName VARCHAR(50),
	MajorID INT NOT NULL,

	CONSTRAINT PK_Student PRIMARY KEY (StudentID),
	CONSTRAINT FK_StudentMajor FOREIGN KEY (MajorID) REFERENCES Majors(MajorID)
)

CREATE TABLE Payments (
	PaymentID INT IDENTITY,
	PaymentDate DATE,
	PaymentAmount DECIMAL(15, 2),
	StudentID INT NOT NULL,

	CONSTRAINT PK_Payment PRIMARY KEY (PaymentID),
	CONSTRAINT FK_PaymentStudent FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
)

CREATE TABLE Agenda (
	StudentID INT NOT NULL,
	SubjectID INT NOT NULL,

	CONSTRAINT PK_Agenda PRIMARY KEY (StudentID, SubjectID),
	CONSTRAINT FK_Agenda_Student FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
	CONSTRAINT FK_Agenda_Subject FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID)
)

--09. *Peaks in Rila
SELECT m.MountainRange, p.PeakName, p.Elevation FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
WHERE m.MountainRange = 'Rila'
ORDER BY Elevation DESC
