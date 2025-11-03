using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using hospital_booking.Data.Repositories.Admin;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;

namespace hospital_booking.Tests.Repositories
{
    public class AdminRepositoryTests
    {
        private readonly Mock<ILogger<AdminRepository>> _mockLogger;
        private readonly AdminRepository _repository;
        private readonly TokenHandler _tokenHandler;

        public AdminRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<AdminRepository>>();

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
            _repository = new AdminRepository(_mockLogger.Object, _tokenHandler, databaseSettings);
        }

        [Fact]
        public async Task RegisterAdminAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateAdminDto
            {
                FullName = "Admin Test",
                Email = $"admin.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "AdminPassword123!",
                Role = "admin"
            };

            // Act
            var result = await _repository.RegisterAdminAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dto.Email, result.Data.Admin.Email);
            Assert.Equal(dto.Role, result.Data.Admin.Role);
        }

        [Fact]
        public async Task LoginAdminAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var registrationDto = new CreateAdminDto
            {
                FullName = "Admin Login Test",
                Email = $"admin.login.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "AdminPassword123!",
                Role = "receptionist"
            };

            await _repository.RegisterAdminAsync(registrationDto);

            var loginDto = new AdminLoginDto
            {
                Email = registrationDto.Email,
                Password = registrationDto.Password
            };

            // Act
            var result = await _repository.LoginAdminAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data.AccessToken);
        }

        [Fact]
        public async Task UpdateAdminAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var registrationDto = new CreateAdminDto
            {
                FullName = "Admin Update Test",
                Email = $"admin.update.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "Password123!",
                Role = "admin"
            };

            var registrationResult = await _repository.RegisterAdminAsync(registrationDto);
            var adminId = registrationResult.Data.Admin.Id;

            var updateDto = new UpdateAdminDto
            {
                FullName = "Updated Admin Name",
                Phone = "+9876543210",
                Role = "super_admin"
            };

            // Act
            var result = await _repository.UpdateAdminAsync(adminId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(updateDto.FullName, result.Data.FullName);
            Assert.Equal(updateDto.Role, result.Data.Role);
        }

        [Fact]
        public async Task DeleteAdminAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var registrationDto = new CreateAdminDto
            {
                FullName = "Admin Delete Test",
                Email = $"admin.delete.{Guid.NewGuid()}@hospital.com",
                Phone = "+1234567890",
                Password = "Password123!",
                Role = "admin"
            };

            var registrationResult = await _repository.RegisterAdminAsync(registrationDto);
            var adminId = registrationResult.Data.Admin.Id;

            // Act
            var result = await _repository.DeleteAdminAsync(adminId);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify admin is deactivated
            var getResult = await _repository.GetAdminByIdAsync(adminId);
            Assert.False(getResult.IsSuccess);
        }

        [Fact]
        public async Task GetAllAdminsAsync_ShouldReturnAdminList()
        {
            // Arrange - Create multiple admins
            for (int i = 0; i < 2; i++)
            {
                var dto = new CreateAdminDto
                {
                    FullName = $"Admin List Test {i}",
                    Email = $"admin.list.{i}.{Guid.NewGuid()}@hospital.com",
                    Phone = $"+123456789{i}",
                    Password = "Password123!",
                    Role = "admin"
                };
                await _repository.RegisterAdminAsync(dto);
            }

            // Act
            var result = await _repository.GetAllAdminsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data.Count >= 2);
        }
    }
}