USE HospitalBookingDb;
GO

-- Create a temporary table to back up data if needed, or just ALTER directly. 
-- Since we are making structural changes like adding a NOT NULL Foreign Key (user_id), 
-- existing rows would fail unless we have a default or truncate. 
-- I'll assume we can truncate for this refactor as it's a dev environment refactor request.

IF OBJECT_ID('dbo.admins', 'U') IS NOT NULL
BEGIN
    -- We will drop and recreate to match the exact definition easily, 
    -- as mapping existing admins to users might be complex without manual intervention.
    DROP TABLE dbo.admins;
END
GO

CREATE TABLE dbo.admins
(
    admin_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(250) NOT NULL,
    email NVARCHAR(250) NOT NULL,
    role NVARCHAR(100) NOT NULL,
    phone NVARCHAR(50) NULL,
    is_active BIT DEFAULT(1) NOT NULL,
    created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
    updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL
);
GO

-- Create Index on user_id (already unique constraint creates one, but explicit if needed)
-- Unique constraint creates an index usually.
-- Index on is_active
CREATE INDEX IX_Admins_IsActive ON dbo.admins(is_active);
GO
