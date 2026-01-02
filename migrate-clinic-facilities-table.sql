USE HospitalBookingDb;
GO

IF OBJECT_ID('dbo.clinic_facilities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.clinic_facilities
    (
        facility_id INT IDENTITY(1,1) PRIMARY KEY,
        clinic_id INT NOT NULL,
        title NVARCHAR(150) NOT NULL,
        created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        CONSTRAINT FK_ClinicFacilities_Clinics FOREIGN KEY (clinic_id) REFERENCES dbo.clinics(clinic_id) ON DELETE CASCADE
    );

    CREATE INDEX IX_ClinicFacilities_ClinicId ON dbo.clinic_facilities(clinic_id);
END;
GO

-- Seed data for clinic_facilities
IF EXISTS (SELECT 1 FROM dbo.clinics WHERE clinic_id = 1)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.clinic_facilities WHERE clinic_id = 1)
    BEGIN
        INSERT INTO dbo.clinic_facilities (clinic_id, title)
        VALUES 
        (1, 'Free Wi-Fi'),
        (1, 'Parking Available'),
        (1, 'Wheelchair Accessible'),
        (1, 'Pharmacy On-site'),
        (1, 'Emergency Care');
    END
END
GO
