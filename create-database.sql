-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HospitalBookingDb')
BEGIN
    CREATE DATABASE HospitalBookingDb;
END;
GO

-- Use the database
USE HospitalBookingDb;
GO

-- Create users table if it doesn't exist
IF OBJECT_ID('dbo.users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.users
    (
        user_id INT IDENTITY(1,1) PRIMARY KEY,
        fullname NVARCHAR(250) NOT NULL,
        email NVARCHAR(250) NOT NULL UNIQUE,
        password NVARCHAR(250) NULL,
        isGoogleLogin BIT DEFAULT(0) NULL
    );
END;
GO

-- Create clinics table if it doesn't exist
IF OBJECT_ID('dbo.clinics', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.clinics
    (
        clinic_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(250) NOT NULL,
        description NVARCHAR(MAX) NULL,
        address NVARCHAR(500) NOT NULL,
        phone NVARCHAR(20) NOT NULL,
        email NVARCHAR(250) NULL,
        website NVARCHAR(500) NULL,
        image_url NVARCHAR(1000) NULL,
        rating FLOAT DEFAULT(0) NULL,
        review_count INT DEFAULT(0) NULL,
        opening_hours NVARCHAR(500) NULL,
        latitude FLOAT NULL,
        longitude FLOAT NULL,
        created_at DATETIME2 DEFAULT(GETDATE()) NOT NULL,
        updated_at DATETIME2 DEFAULT(GETDATE()) NOT NULL
    );
END;
GO

-- Create doctors table if it doesn't exist
IF OBJECT_ID('dbo.doctors', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.doctors
    (
        doctor_id INT IDENTITY(1,1) PRIMARY KEY,
        clinic_id INT NOT NULL,
        full_name NVARCHAR(250) NOT NULL,
        bio NVARCHAR(MAX) NULL,
        phone NVARCHAR(50) NULL,
        is_active BIT DEFAULT(1) NOT NULL,
        experience_years INT DEFAULT(0) NOT NULL,
        CONSTRAINT FK_Doctors_Clinics FOREIGN KEY (clinic_id) REFERENCES dbo.clinics(clinic_id)
    );
END;
GO

-- Create admins table if it doesn't exist
IF OBJECT_ID('dbo.admins', 'U') IS NULL
BEGIN
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

    CREATE INDEX IX_Admins_IsActive ON dbo.admins(is_active);
END;
GO

-- Create patients table if it doesn't exist
IF OBJECT_ID('dbo.patients', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.patients
    (
        patient_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL,
        full_name NVARCHAR(250) NOT NULL,
        birthDate DATETIME2 NULL,
        gender NVARCHAR(50) NULL,
        notes NVARCHAR(MAX) NULL,
        CONSTRAINT FK_Patients_Users FOREIGN KEY (user_id) REFERENCES dbo.users(user_id)
    );
END;
GO

-- Create appointments table if it doesn't exist
IF OBJECT_ID('dbo.appointments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.appointments
    (
        appointment_id INT IDENTITY(1,1) PRIMARY KEY,
        patient_id INT NOT NULL,
        doctor_id INT NOT NULL,
        appointment_time DATETIME2 NOT NULL,
        reason NVARCHAR(MAX) NULL,
        status NVARCHAR(100) NOT NULL DEFAULT('pending'),
        CONSTRAINT FK_Appointments_Patients FOREIGN KEY (patient_id) REFERENCES dbo.patients(patient_id),
        CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (doctor_id) REFERENCES dbo.doctors(doctor_id)
    );
END;
GO

-- Create prescriptions table if it doesn't exist
IF OBJECT_ID('dbo.prescriptions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.prescriptions
    (
        prescription_id INT IDENTITY(1,1) PRIMARY KEY,
        appointment_id INT NOT NULL,
        instructions NVARCHAR(MAX) NULL,
        CONSTRAINT FK_Prescriptions_Appointments FOREIGN KEY (appointment_id) REFERENCES dbo.appointments(appointment_id)
    );
END;
GO

-- Create prescription_items table if it doesn't exist
IF OBJECT_ID('dbo.prescription_items', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.prescription_items
    (
        item_id INT IDENTITY(1,1) PRIMARY KEY,
        prescription_id INT NOT NULL,
        name NVARCHAR(250) NOT NULL,
        dosage NVARCHAR(100) NULL,
        duration NVARCHAR(100) NULL,
        frequency NVARCHAR(100) NULL,
        CONSTRAINT FK_PrescriptionItems_Prescriptions FOREIGN KEY (prescription_id) REFERENCES dbo.prescriptions(prescription_id)
    );
END;
GO

-- Create medical_reports table if it doesn't exist
IF OBJECT_ID('dbo.medical_reports', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.medical_reports
    (
        report_id INT IDENTITY(1,1) PRIMARY KEY,
        appointment_id INT NOT NULL,
        diagnosis NVARCHAR(MAX) NULL,
        notes NVARCHAR(MAX) NULL,
        required_tests NVARCHAR(MAX) NULL,
        CONSTRAINT FK_MedicalReports_Appointments FOREIGN KEY (appointment_id) REFERENCES dbo.appointments(appointment_id)
    );
END;
GO
