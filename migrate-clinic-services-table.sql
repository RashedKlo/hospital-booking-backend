USE HospitalBookingDb;
GO

IF OBJECT_ID('dbo.clinic_services', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.clinic_services
    (
        service_id INT IDENTITY(1,1) PRIMARY KEY,
        clinic_id INT NOT NULL,
        title NVARCHAR(250) NOT NULL,
        description NVARCHAR(MAX) NULL,
        price DECIMAL(18, 2) NOT NULL DEFAULT(0),
        created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        CONSTRAINT FK_ClinicServices_Clinics FOREIGN KEY (clinic_id) REFERENCES dbo.clinics(clinic_id) ON DELETE CASCADE
    );

    -- Optional: Add index for clinic_id for faster lookups
    CREATE INDEX IX_ClinicServices_ClinicId ON dbo.clinic_services(clinic_id);
END;
GO

-- Seed data for clinic_services
-- Assuming clinic_id 1 exists (from other seeds or create-database)
IF EXISTS (SELECT 1 FROM dbo.clinics WHERE clinic_id = 1)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.clinic_services WHERE clinic_id = 1)
    BEGIN
        INSERT INTO dbo.clinic_services (clinic_id, title, description, price)
        VALUES 
        (1, 'General Consultation', 'Standard check-up with a general practitioner.', 50.00),
        (1, 'Dental Cleaning', 'Professional dental cleaning and scaling.', 80.00),
        (1, 'Blood Test', 'Complete blood count and basic metabolic panel.', 30.00),
        (1, 'X-Ray', 'Digital X-ray imaging.', 100.00),
        (1, 'Vaccination', 'Annual flu vaccination.', 20.00);
    END
END
GO
