-- ============================================================
-- UltraHotel Seed Data  (idempotente — usa IDs fijos)
-- Contraseña de ambos usuarios: Ultra1234!
-- ============================================================
USE UltraHotelDb;
GO

-- ─── HOTELS ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Id = 'A1A1A1A1-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Hotels (Id, Name, City, Address, Description, AgentEmail, IsEnabled, CreatedAt, UpdatedAt)
    VALUES
    ('A1A1A1A1-0000-0000-0000-000000000001',
     'Hotel Casa Bogotá',
     'Bogotá',
     'Cra 7 # 32-16, Teusaquillo',
     'Hotel boutique en el corazón de Bogotá, con vista a los cerros.',
     'agent@ultrahotel.com',
     1,
     GETUTCDATE(), GETUTCDATE()),

    ('A2A2A2A2-0000-0000-0000-000000000002',
     'Hotel El Poblado Medellín',
     'Medellín',
     'Calle 10 # 43D-20, El Poblado',
     'Moderno hotel en el mejor sector de Medellín.',
     'agent@ultrahotel.com',
     1,
     GETUTCDATE(), GETUTCDATE());
END
GO

-- ─── ROOMS ───────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Id = 'B1B1B1B1-0000-0000-0000-000000000001')
BEGIN
    -- Bogotá
    INSERT INTO Rooms (Id, HotelId, RoomType, BasePrice, TaxRate, Capacity, LocationInHotel, IsEnabled, CreatedAt, UpdatedAt)
    VALUES
    ('B1B1B1B1-0000-0000-0000-000000000001',
     'A1A1A1A1-0000-0000-0000-000000000001',
     'SINGLE', 120000.00, 0.1900, 1, 'Piso 2, habitación 201', 1, GETUTCDATE(), GETUTCDATE()),

    ('B1B1B1B1-0000-0000-0000-000000000002',
     'A1A1A1A1-0000-0000-0000-000000000001',
     'DOUBLE', 200000.00, 0.1900, 2, 'Piso 3, habitación 301', 1, GETUTCDATE(), GETUTCDATE()),

    ('B1B1B1B1-0000-0000-0000-000000000003',
     'A1A1A1A1-0000-0000-0000-000000000001',
     'SUITE',  450000.00, 0.1900, 3, 'Piso 10, suite presidencial', 1, GETUTCDATE(), GETUTCDATE()),

    -- Medellín
    ('B2B2B2B2-0000-0000-0000-000000000001',
     'A2A2A2A2-0000-0000-0000-000000000002',
     'FAMILY', 380000.00, 0.1900, 4, 'Piso 4, habitación 401', 1, GETUTCDATE(), GETUTCDATE()),

    ('B2B2B2B2-0000-0000-0000-000000000002',
     'A2A2A2A2-0000-0000-0000-000000000002',
     'DOUBLE', 210000.00, 0.1900, 2, 'Piso 2, habitación 204', 1, GETUTCDATE(), GETUTCDATE());
END
GO

-- ─── USERS ───────────────────────────────────────────────────
-- Contraseña de todos: Ultra1234!  (BCrypt cost=11)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 'C0C0C0C0-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Users (Id, Email, PasswordHash, Role, CreatedAt)
    VALUES
    -- ADMIN: único capaz de crear agentes vía POST /api/v1/admin/agents
    ('C0C0C0C0-0000-0000-0000-000000000001',
     'admin@ultrahotel.com',
     '$2a$11$r36MkYDxPrusvMZycej8ZeeXPvyP4Frlt3xkY.v.duBGZt5rm6JFa',
     'ADMIN',
     GETUTCDATE()),

    ('C1C1C1C1-0000-0000-0000-000000000001',
     'agent@ultrahotel.com',
     '$2a$11$RRihThsGZktNRhQ2kH3Ft.sfTMT1mQHnb0eFY9t2o4BAhMVxEtYFu',
     'AGENT',
     GETUTCDATE()),

    ('C2C2C2C2-0000-0000-0000-000000000001',
     'traveler@ultrahotel.com',
     '$2a$11$Dbka5dRU.oSfYQxtRbardemABYVwrakbY8W.d9B3b.6Jp/F3cz.tG',
     'TRAVELER',
     GETUTCDATE());
END
GO

-- ─── BOOKINGS ────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Bookings WHERE Id = 'D1D1D1D1-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Bookings (Id, RoomId, HotelId, CheckInDate, CheckOutDate, Status, TotalPrice,
                          EmergencyContactName, EmergencyContactPhone, CreatedAt)
    VALUES
    -- Reserva pasada (habitación single en Bogotá)
    ('D1D1D1D1-0000-0000-0000-000000000001',
     'B1B1B1B1-0000-0000-0000-000000000001',
     'A1A1A1A1-0000-0000-0000-000000000001',
     DATEADD(DAY, -10, CAST(GETUTCDATE() AS DATE)),
     DATEADD(DAY, -7,  CAST(GETUTCDATE() AS DATE)),
     'CONFIRMED',
     428400.00,   -- 120000 * 1.19 * 3 noches
     'María García', '314-555-0001',
     GETUTCDATE()),

    -- Reserva futura (habitación doble en Medellín)
    ('D2D2D2D2-0000-0000-0000-000000000002',
     'B2B2B2B2-0000-0000-0000-000000000002',
     'A2A2A2A2-0000-0000-0000-000000000002',
     DATEADD(DAY, 15, CAST(GETUTCDATE() AS DATE)),
     DATEADD(DAY, 18, CAST(GETUTCDATE() AS DATE)),
     'CONFIRMED',
     750090.00,   -- 210000 * 1.19 * 3 noches
     'Carlos Rodríguez', '300-555-0002',
     GETUTCDATE());
END
GO

-- ─── GUESTS ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Guests WHERE Id = 'E1E1E1E1-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Guests (Id, BookingId, FirstName, LastName, BirthDate, Gender,
                        DocumentType, DocumentNumber, Email, Phone)
    VALUES
    -- Huésped reserva 1 (adulto)
    ('E1E1E1E1-0000-0000-0000-000000000001',
     'D1D1D1D1-0000-0000-0000-000000000001',
     'Laura', 'Martínez', '1990-06-15', 'FEMALE',
     'CC', '10345678', 'laura.martinez@email.com', '310-555-1001'),

    -- Huésped reserva 2 (principal)
    ('E2E2E2E2-0000-0000-0000-000000000001',
     'D2D2D2D2-0000-0000-0000-000000000002',
     'Carlos', 'Rodríguez', '1985-11-20', 'MALE',
     'CC', '80765432', 'carlos.rodriguez@email.com', '300-555-0002'),

    -- Huésped reserva 2 (acompañante)
    ('E2E2E2E2-0000-0000-0000-000000000002',
     'D2D2D2D2-0000-0000-0000-000000000002',
     'Sofía', 'Rodríguez', '1988-03-08', 'FEMALE',
     'CC', '52456789', 'sofia.rodriguez@email.com', '300-555-0003');
END
GO

PRINT 'UltraHotelDb seed data loaded successfully.';
GO
