USE HospitalBookingDb;
GO

IF OBJECT_ID('dbo.clinic_reviews', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.clinic_reviews
    (
        review_id INT IDENTITY(1,1) PRIMARY KEY,
        clinic_id INT NOT NULL,
        patient_id INT NOT NULL,
        rating TINYINT NOT NULL CHECK (rating >= 1 AND rating <= 5),
        review_comment NVARCHAR(MAX) NULL,
        review_date DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        CONSTRAINT FK_ClinicReviews_Clinics FOREIGN KEY (clinic_id) REFERENCES dbo.clinics(clinic_id) ON DELETE CASCADE,
        CONSTRAINT FK_ClinicReviews_Patients FOREIGN KEY (patient_id) REFERENCES dbo.patients(patient_id) ON DELETE CASCADE
    );

    CREATE INDEX IX_ClinicReviews_ClinicId ON dbo.clinic_reviews(clinic_id);
    CREATE INDEX IX_ClinicReviews_PatientId ON dbo.clinic_reviews(patient_id);
END;
GO

-- Seed data for clinic_reviews
-- Assuming clinic_id 1 and patient_id 1 exist
IF EXISTS (SELECT 1 FROM dbo.clinics WHERE clinic_id = 1) AND EXISTS (SELECT 1 FROM dbo.patients WHERE patient_id = 1)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.clinic_reviews WHERE clinic_id = 1 AND patient_id = 1)
    BEGIN
        INSERT INTO dbo.clinic_reviews (clinic_id, patient_id, rating, review_comment)
        VALUES 
        (1, 1, 5, 'Excellent service and very professional doctors.'),
        (1, 1, 4, 'Friendly staff, but the waiting time was a bit long.');
    END
END
GO
