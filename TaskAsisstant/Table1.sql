CREATE TABLE [dbo].[Table1]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Title] VARCHAR(50) NOT NULL, 
    [Description] VARCHAR(255) NOT NULL, 
    [Reminder] VARCHAR(50) NOT NULL, 
    [IsCompleted] BIT NULL DEFAULT 0,
)
