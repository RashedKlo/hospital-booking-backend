using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Repositories.Patient;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;

namespace hospital_booking.Tests.Repositories
{
    public class PatientRepositoryTests
    {
        private readonly Mock<ILogger<PatientRepository>> _mockLogger;
        private readonly TokenHandler _tokenHandler;
        private readonly PatientRepository _repository;

        public PatientRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<PatientRepository>>();
            
            // Create test JWT settings
            var jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsATestSecretKeyWithMinimum32Characters!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationInMinutes = 60
            };
            
            _tokenHandler = new TokenHandler(jwtSettings);
            _repository = new PatientRepository(_mockLogger.Object, _tokenHandler);
        }

        [Fact]
        public async Task RegisterPatientAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new PatientRegistrationDto
            {
                FullName = "John Doe",
                Email = $"john.doe.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "SecurePassword123!",
                ConfirmPassword = "SecurePassword123!",
                IsGoogleLogin = false
            };

            // Act
            var result = await _repository.RegisterPatientAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Patient);
            Assert.NotNull(result.Data.AccessToken);
            Assert.NotNull(result.Data.RefreshToken);
            Assert.Equal(dto.Email, result.Data.Patient.Email);
        }

        [Fact]
        public async Task RegisterPatientAsync_WithDuplicateEmail_ShouldReturnFailure()
        {
            // Arrange
            var email = $"duplicate.{Guid.NewGuid()}@test.com";
            
            var firstDto = new PatientRegistrationDto
            {
                FullName = "John Doe",
                Email = email,
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "SecurePassword123!",
                ConfirmPassword = "SecurePassword123!",
                IsGoogleLogin = false
            };

            var secondDto = new PatientRegistrationDto
            {
                FullName = "Jane Doe",
                Email = email,
                Phone = "+0987654321",
                DateOfBirth = new DateTime(1992, 2, 2),
                Password = "AnotherPassword123!",
                ConfirmPassword = "AnotherPassword123!",
                IsGoogleLogin = false
            };

            // Act
            var firstResult = await _repository.RegisterPatientAsync(firstDto);
            var secondResult = await _repository.RegisterPatientAsync(secondDto);

            // Assert
            Assert.True(firstResult.IsSuccess);
            Assert.False(secondResult.IsSuccess);
            Assert.Contains("already exists", secondResult.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LoginPatientAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Login Test User",
                Email = $"logintest.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "LoginPassword123!",
                ConfirmPassword = "LoginPassword123!",
                IsGoogleLogin = false
            };

            var registrationResult = await _repository.RegisterPatientAsync(registrationDto);
            Assert.True(registrationResult.IsSuccess);

            var loginDto = new PatientLoginDto
            {
                Email = registrationDto.Email,
                Password = registrationDto.Password
            };

            // Act
            var loginResult = await _repository.LoginPatientAsync(loginDto);

            // Assert
            Assert.True(loginResult.IsSuccess);
            Assert.NotNull(loginResult.Data);
            Assert.NotNull(loginResult.Data.AccessToken);
            Assert.Equal(registrationDto.Email, loginResult.Data.Patient.Email);
        }

        [Fact]
        public async Task LoginPatientAsync_WithInvalidPassword_ShouldReturnFailure()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Invalid Password Test",
                Email = $"invalidpwd.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "CorrectPassword123!",
                ConfirmPassword = "CorrectPassword123!",
                IsGoogleLogin = false
            };

            await _repository.RegisterPatientAsync(registrationDto);

            var loginDto = new PatientLoginDto
            {
                Email = registrationDto.Email,
                Password = "WrongPassword123!"
            };

            // Act
            var result = await _repository.LoginPatientAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LoginPatientAsync_WithNonExistentEmail_ShouldReturnFailure()
        {
            // Arrange
            var loginDto = new PatientLoginDto
            {
                Email = "nonexistent@test.com",
                Password = "SomePassword123!"
            };

            // Act
            var result = await _repository.LoginPatientAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetPatientByIdAsync_WithValidId_ShouldReturnPatient()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Get By ID Test",
                Email = $"getbyid.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "TestPassword123!",
                ConfirmPassword = "TestPassword123!",
                IsGoogleLogin = false
            };

            var registrationResult = await _repository.RegisterPatientAsync(registrationDto);
            var patientId = registrationResult.Data!.Patient.Id;

            // Act
            var result = await _repository.GetPatientByIdAsync(patientId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(patientId, result.Data.Id);
            Assert.Equal(registrationDto.Email, result.Data.Email);
        }

        [Fact]
        public async Task GetPatientByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _repository.GetPatientByIdAsync(999999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetPatientByEmailAsync_WithValidEmail_ShouldReturnPatient()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Get By Email Test",
                Email = $"getbyemail.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "TestPassword123!",
                ConfirmPassword = "TestPassword123!",
                IsGoogleLogin = false
            };

            await _repository.RegisterPatientAsync(registrationDto);

            // Act
            var result = await _repository.GetPatientByEmailAsync(registrationDto.Email);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(registrationDto.Email, result.Data.Email);
        }

        [Fact]
        public async Task UpdatePatientAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Update Test User",
                Email = $"updatetest.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "TestPassword123!",
                ConfirmPassword = "TestPassword123!",
                IsGoogleLogin = false
            };

            var registrationResult = await _repository.RegisterPatientAsync(registrationDto);
            var patientId = registrationResult.Data!.Patient.Id;

            var updateDto = new PatientUpdateDto
            {
                FullName = "Updated Name",
                Phone = "+9876543210",
                DateOfBirth = new DateTime(1991, 2, 2)
            };

            // Act
            var result = await _repository.UpdatePatientAsync(patientId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.FullName, result.Data.FullName);
            Assert.Equal(updateDto.Phone, result.Data.Phone);
        }

        [Fact]
        public async Task DeletePatientAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange - First register a patient
            var registrationDto = new PatientRegistrationDto
            {
                FullName = "Delete Test User",
                Email = $"deletetest.{Guid.NewGuid()}@test.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "TestPassword123!",
                ConfirmPassword = "TestPassword123!",
                IsGoogleLogin = false
            };

            var registrationResult = await _repository.RegisterPatientAsync(registrationDto);
            var patientId = registrationResult.Data!.Patient.Id;

            // Act
            var result = await _repository.DeletePatientAsync(patientId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Verify patient is deactivated
            var getResult = await _repository.GetPatientByIdAsync(patientId);
            Assert.False(getResult.IsSuccess);
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnPatientList()
        {
            // Arrange - Register multiple patients
            for (int i = 0; i < 3; i++)
            {
                var dto = new PatientRegistrationDto
                {
                    FullName = $"Test User {i}",
                    Email = $"testuser{i}.{Guid.NewGuid()}@test.com",
                    Phone = $"+123456789{i}",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Password = "TestPassword123!",
                    ConfirmPassword = "TestPassword123!",
                    IsGoogleLogin = false
                };
                await _repository.RegisterPatientAsync(dto);
            }

            // Act
            var result = await _repository.GetAllPatientsAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count >= 3);
        }
    }
}