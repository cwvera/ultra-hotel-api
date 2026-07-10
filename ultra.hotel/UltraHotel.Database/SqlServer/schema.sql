-- ============================================================
-- UltraHotel Database Schema
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UltraHotelDb')
BEGIN
    CREATE DATABASE UltraHotelDb;
END
GO

USE UltraHotelDb;
GO

-- ─── Hotels ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Hotels')
BEGIN
    CREATE TABLE Hotels (
        Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()   CONSTRAINT PK_Hotels PRIMARY KEY,
        Name        NVARCHAR(200)    NOT NULL,
        City        NVARCHAR(100)    NOT NULL,
        Address     NVARCHAR(300)    NOT NULL,
        Description NVARCHAR(1000)   NULL,
        AgentEmail  NVARCHAR(200)    NOT NULL,
        IsEnabled   BIT              NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- ─── Rooms ───────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Rooms')
BEGIN
    CREATE TABLE Rooms (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()   CONSTRAINT PK_Rooms PRIMARY KEY,
        HotelId         UNIQUEIDENTIFIER NOT NULL,
        RoomType        NVARCHAR(50)     NOT NULL,   -- SINGLE | DOUBLE | SUITE | FAMILY
        BasePrice       DECIMAL(18,2)    NOT NULL,
        TaxRate         DECIMAL(5,4)     NOT NULL,   -- e.g. 0.1900 = 19%
        Capacity        INT              NOT NULL,
        LocationInHotel NVARCHAR(200)    NULL,
        IsEnabled       BIT              NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_Rooms_Hotels FOREIGN KEY (HotelId) REFERENCES Hotels(Id)
    );
END
GO

-- ─── Bookings ────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
BEGIN
    CREATE TABLE Bookings (
        Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()   CONSTRAINT PK_Bookings PRIMARY KEY,
        RoomId                UNIQUEIDENTIFIER NOT NULL,
        HotelId               UNIQUEIDENTIFIER NOT NULL,
        CheckInDate           DATE             NOT NULL,
        CheckOutDate          DATE             NOT NULL,
        Status                NVARCHAR(50)     NOT NULL DEFAULT 'CONFIRMED',  -- CONFIRMED | CANCELLED
        TotalPrice            DECIMAL(18,2)    NOT NULL,
        EmergencyContactName  NVARCHAR(200)    NOT NULL,
        EmergencyContactPhone NVARCHAR(50)     NOT NULL,
        CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_Bookings_Rooms  FOREIGN KEY (RoomId)  REFERENCES Rooms(Id),
        CONSTRAINT FK_Bookings_Hotels FOREIGN KEY (HotelId) REFERENCES Hotels(Id),
        CONSTRAINT CK_Bookings_Dates  CHECK (CheckOutDate > CheckInDate)
    );
END
GO

-- ─── Guests ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
BEGIN
    CREATE TABLE Guests (
        Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()   CONSTRAINT PK_Guests PRIMARY KEY,
        BookingId      UNIQUEIDENTIFIER NOT NULL,
        FirstName      NVARCHAR(100)    NOT NULL,
        LastName       NVARCHAR(100)    NOT NULL,
        BirthDate      DATE             NOT NULL,
        Gender         NVARCHAR(20)     NOT NULL,   -- MALE | FEMALE | OTHER
        DocumentType   NVARCHAR(50)     NOT NULL,   -- CC | PASSPORT | CE
        DocumentNumber NVARCHAR(50)     NOT NULL,
        Email          NVARCHAR(200)    NOT NULL,
        Phone          NVARCHAR(50)     NOT NULL,

        CONSTRAINT FK_Guests_Bookings FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
    );
END
GO

-- ─── Users ───────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()   CONSTRAINT PK_Users PRIMARY KEY,
        Email        NVARCHAR(200)    NOT NULL,
        PasswordHash NVARCHAR(200)    NOT NULL,
        Role         NVARCHAR(20)     NOT NULL,   -- ADMIN | AGENT | TRAVELER
        CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END
GO

-- ─── Indexes ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rooms_HotelId')
    CREATE INDEX IX_Rooms_HotelId ON Rooms(HotelId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_HotelId')
    CREATE INDEX IX_Bookings_HotelId ON Bookings(HotelId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_RoomId')
    CREATE INDEX IX_Bookings_RoomId ON Bookings(RoomId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_Dates')
    CREATE INDEX IX_Bookings_Dates ON Bookings(CheckInDate, CheckOutDate);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Guests_BookingId')
    CREATE INDEX IX_Guests_BookingId ON Guests(BookingId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE INDEX IX_Users_Email ON Users(Email);
GO

PRINT 'UltraHotelDb schema initialized successfully.';
GO
