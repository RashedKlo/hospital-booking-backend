-- Migration script to add new fields to clinics table
-- Run this script on your hospital_booking database

USE hospital_booking;
GO

-- Check if the table exists before altering
IF OBJECT_ID('dbo.clinics', 'U') IS NOT NULL
BEGIN
    PRINT 'Starting migration of clinics table...';

    -- Rename 'title' column to 'name' if it exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'title')
    BEGIN
        EXEC sp_rename 'dbo.clinics.title', 'name', 'COLUMN';
        PRINT 'Renamed column: title -> name';
    END

    -- Add email column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'email')
    BEGIN
        ALTER TABLE dbo.clinics ADD email NVARCHAR(250) NULL;
        PRINT 'Added column: email';
    END

    -- Add website column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'website')
    BEGIN
        ALTER TABLE dbo.clinics ADD website NVARCHAR(500) NULL;
        PRINT 'Added column: website';
    END

    -- Add image_url column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'image_url')
    BEGIN
        ALTER TABLE dbo.clinics ADD image_url NVARCHAR(1000) NULL;
        PRINT 'Added column: image_url';
    END

    -- Add rating column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'rating')
    BEGIN
        ALTER TABLE dbo.clinics ADD rating FLOAT DEFAULT(0) NULL;
        PRINT 'Added column: rating';
    END

    -- Add review_count column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'review_count')
    BEGIN
        ALTER TABLE dbo.clinics ADD review_count INT DEFAULT(0) NULL;
        PRINT 'Added column: review_count';
    END

    -- Add opening_hours column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'opening_hours')
    BEGIN
        ALTER TABLE dbo.clinics ADD opening_hours NVARCHAR(500) NULL;
        PRINT 'Added column: opening_hours';
    END

    -- Add latitude column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'latitude')
    BEGIN
        ALTER TABLE dbo.clinics ADD latitude FLOAT NULL;
        PRINT 'Added column: latitude';
    END

    -- Add longitude column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'longitude')
    BEGIN
        ALTER TABLE dbo.clinics ADD longitude FLOAT NULL;
        PRINT 'Added column: longitude';
    END

    -- Add created_at column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'created_at')
    BEGIN
        ALTER TABLE dbo.clinics ADD created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL;
        PRINT 'Added column: created_at';
    END

    -- Add updated_at column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.clinics') AND name = 'updated_at')
    BEGIN
        ALTER TABLE dbo.clinics ADD updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL;
        PRINT 'Added column: updated_at';
    END

    PRINT 'Migration completed successfully!';
    PRINT '';
    PRINT 'Summary of clinics table structure:';
    
    -- Display the current table structure
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'clinics'
    ORDER BY ORDINAL_POSITION;
END
ELSE
BEGIN
    PRINT 'ERROR: clinics table does not exist!';
    PRINT 'Please run create-database.sql first to create the table.';
END
GO
