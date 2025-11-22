# Hospital Booking Backend

This is the backend repository for the Hospital Booking System. It provides the necessary APIs and services to manage hospital operations, including appointments, doctors, patients, and medical records.

## Technologies

- **Framework:** .NET Core (configured for .NET 8/10)
- **Language:** C#
- **Database:** SQL Server (see `create-database.sql`)
- **Architecture:** Layered Architecture (API, Services, Data)

## Project Structure

- **hospital-booking.Api**: Contains the RESTful API controllers and entry point.
- **hospital-booking.Data**: Handles database context, entities, repositories, and DTOs.
- **hospital-booking.Services**: Contains business logic and service implementations.

## Features

- **User Management**: 
  - Admins
  - Doctors
  - Patients
  - Users
- **Booking System**:
  - Appointment scheduling
  - Clinic management
- **Medical Records**:
  - Prescriptions
  - Medical Reports
- **Authentication**:
  - JWT Authentication
  - Google Authentication support

## Getting Started

1. **Database Setup**: Run the `create-database.sql` script to initialize your database.
2. **Configuration**: Update `appsettings.json` with your connection strings and API keys.
3. **Run**: Build and run the `hospital-booking.Api` project.

```bash
dotnet run --project hospital-booking.Api
```
