namespace TicketGo.Data
{
    // tables for Events and Tickets (the Identity tables are created by Entity Framework)
    public static class Schema
    {
        public const string SqlServer = @"
IF OBJECT_ID(N'dbo.Events', N'U') IS NULL
CREATE TABLE Events (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Venue NVARCHAR(200) NOT NULL,
    [Date] DATETIME2 NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    TotalSeats INT NOT NULL,
    OrganizerId NVARCHAR(450) NULL
);

IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL REFERENCES Events(Id) ON DELETE CASCADE,
    UserId NVARCHAR(450) NOT NULL,
    SeatNumber NVARCHAR(10) NOT NULL,
    TicketCode NVARCHAR(20) NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Tickets_Event_Seat' AND object_id = OBJECT_ID(N'dbo.Tickets'))
CREATE UNIQUE INDEX UX_Tickets_Event_Seat ON Tickets(EventId, SeatNumber);
";

        public const string Sqlite = @"
CREATE TABLE IF NOT EXISTS Events (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Venue TEXT NOT NULL,
    [Date] TEXT NOT NULL,
    Price NUMERIC NOT NULL,
    TotalSeats INTEGER NOT NULL,
    OrganizerId TEXT NULL
);

CREATE TABLE IF NOT EXISTS Tickets (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL REFERENCES Events(Id) ON DELETE CASCADE,
    UserId TEXT NOT NULL,
    SeatNumber TEXT NOT NULL,
    TicketCode TEXT NOT NULL,
    IsUsed INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS UX_Tickets_Event_Seat ON Tickets(EventId, SeatNumber);
";
    }
}
