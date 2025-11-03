using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using hospital_booking.Data.Repositories.Doctor;
using hospital_booking.Data.Repositories.Specialty;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;

namespace hospital_booking.Tests.Repositories
{
    public class DoctorRepositoryTests : IDisposable
    {
        private readonly Mock<ILogger<DoctorRepository>> _mockLogger;
        private readonly Mock<ILogger<SpecialtyRepository>> _mockSpecialtyLogger;
        private readonly DoctorRepository _repository;
        private readonly SpecialtyRepository _specialtyRepository;
        private readonly TokenHandler _tokenHandler;

        public DoctorRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<DoctorRepository>>();
            _mockSpecialtyLogger = new Mock<ILogger<SpecialtyRepository>>();

            var databaseSettings = Options.Create(new DatabaseSettings
            {
                ConnectionString = "Server=localhost;Database=HospitalBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
            });

            var jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsATestSecretKeyWithMinimum32Characters!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationInMinutes = 60
            };

            _tokenHandler = new TokenHandler(jwtSettings);
            _repository = new DoctorRepository(_mockLogger.Object, _tokenHandler, databaseSettings);
            _specialtyRepository = new SpecialtyRepository(_mockSpecialtyLogger.Object, databaseSettings);
        }

        private async Task<int> CreateTestSpecialty()
        {
            var specialtyDto = new CreateSpecialtyDto
            {
                DepartmentId = 1, // Assuming department exists
                Name = $"Test Specialty {Guid.NewGuid()}",
                Description = "Test specialty for doctor tests"
            };
            var result = await _specialtyRepository.CreateSpecialtyAsync(specialtyDto);
            return result.Data.Id;
        }

        [Fact]
        public async Task RegisterDoctorAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var dto = new CreateDoctorDto
            {
                FullName = "Dr. John Smith",
                Email = $"dr.john.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "SecurePassword123!",
                SpecialtyId = specialtyId,
                ExperienceYears = 10,
                Bio = "Experienced doctor"
            };

            // Act
            var result = await _repository.RegisterDoctorAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Doctor);
            Assert.NotNull(result.Data.AccessToken);
            Assert.Equal(dto.Email, result.Data.Doctor.Email);
        }

        [Fact]
        public async Task RegisterDoctorAsync_WithDuplicateEmail_ShouldReturnFailure()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var email = $"duplicate.doctor.{Guid.NewGuid()}@hospital.com";

            var firstDto = new CreateDoctorDto
            {
                FullName = "Dr. First",
                Email = email,
                Phone = "+1234567890",
                Password = "Password123!",
                SpecialtyId = specialtyId
            };

            var secondDto = new CreateDoctorDto
            {
                FullName = "Dr. Second",
                Email = email,
                Phone = "+0987654321",
                Password = "Password456!",
                SpecialtyId = specialtyId
            };

            // Act
            var firstResult = await _repository.RegisterDoctorAsync(firstDto);
            var secondResult = await _repository.RegisterDoctorAsync(secondDto);

            // Assert
            Assert.True(firstResult.IsSuccess);
            Assert.False(secondResult.IsSuccess);
            Assert.Contains("already exists", secondResult.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LoginDoctorAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var registrationDto = new CreateDoctorDto
            {
                FullName = "Dr. Login Test",
                Email = $"login.test.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "LoginPassword123!",
                SpecialtyId = specialtyId
            };

            await _repository.RegisterDoctorAsync(registrationDto);

            var loginDto = new DoctorLoginDto
            {
                Email = registrationDto.Email,
                Password = registrationDto.Password
            };

            // Act
            var result = await _repository.LoginDoctorAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.AccessToken);
        }

        [Fact]
        public async Task GetDoctorByIdAsync_WithValidId_ShouldReturnDoctor()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var registrationDto = new CreateDoctorDto
            {
                FullName = "Dr. Get Test",
                Email = $"get.test.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "Password123!",
                SpecialtyId = specialtyId
            };

            var registrationResult = await _repository.RegisterDoctorAsync(registrationDto);
            var doctorId = registrationResult.Data.Doctor.Id;

            // Act
            var result = await _repository.GetDoctorByIdAsync(doctorId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(doctorId, result.Data.Id);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var registrationDto = new CreateDoctorDto
            {
                FullName = "Dr. Update Test",
                Email = $"update.test.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "Password123!",
                SpecialtyId = specialtyId
            };

            var registrationResult = await _repository.RegisterDoctorAsync(registrationDto);
            var doctorId = registrationResult.Data.Doctor.Id;

            var updateDto = new UpdateDoctorDto
            {
                FullName = "Dr. Updated Name",
                Phone = "+9876543210",
                SpecialtyId = specialtyId,
                ExperienceYears = 15,
                Bio = "Updated bio"
            };

            // Act
            var result = await _repository.UpdateDoctorAsync(doctorId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(updateDto.FullName, result.Data.FullName);
        }

        [Fact]
        public async Task DeleteDoctorAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();
            var registrationDto = new CreateDoctorDto
            {
                FullName = "Dr. Delete Test",
                Email = $"delete.test.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "Password123!",
                SpecialtyId = specialtyId
            };

            var registrationResult = await _repository.RegisterDoctorAsync(registrationDto);
            var doctorId = registrationResult.Data.Doctor.Id;

            // Act
            var result = await _repository.DeleteDoctorAsync(doctorId);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify doctor is deactivated
            var getResult = await _repository.GetDoctorByIdAsync(doctorId);
            Assert.False(getResult.IsSuccess);
        }

        [Fact]
        public async Task GetDoctorsBySpecialtyAsync_ShouldReturnFilteredList()
        {
            // Arrange
            var specialtyId = await CreateTestSpecialty();

            for (int i = 0; i < 2; i++)
            {
                var dto = new CreateDoctorDto
                {
                    FullName = $"Dr. Specialty Test {i}",
                    Email = $"specialty.test.{i}.{Guid.NewGuid()}@hospital.com",
                    Phone = $"+123456789{i}",
                    Password = "Password123!",
                    SpecialtyId = specialtyId
                };
                await _repository.RegisterDoctorAsync(dto);
            }

            // Act
            var result = await _repository.GetDoctorsBySpecialtyAsync(specialtyId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data.Count >= 2);
            Assert.All(result.Data, d => Assert.Equal(specialtyId, d.SpecialtyId));
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}