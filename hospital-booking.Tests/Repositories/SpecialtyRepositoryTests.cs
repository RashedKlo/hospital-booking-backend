using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Repositories.Specialty;
using hospital_booking.Data.Repositories.Department;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Settings;

namespace hospital_booking.Tests.Repositories
{
    public class SpecialtyRepositoryTests
    {
        private readonly Mock<ILogger<SpecialtyRepository>> _mockLogger;
        private readonly Mock<ILogger<DepartmentRepository>> _mockDeptLogger;
        private readonly SpecialtyRepository _repository;
        private readonly DepartmentRepository _departmentRepository;

        public SpecialtyRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<SpecialtyRepository>>();
            _mockDeptLogger = new Mock<ILogger<DepartmentRepository>>();
            
       
            _repository = new SpecialtyRepository(_mockLogger.Object);
            _departmentRepository = new DepartmentRepository(_mockDeptLogger.Object);
        }

        private async Task<int> CreateTestDepartment()
        {
            var deptDto = new CreateDepartmentDto
            {
                Name = $"Test Department {Guid.NewGuid()}",
                Description = "Test department for specialty tests"
            };
            var result = await _departmentRepository.CreateDepartmentAsync(deptDto);
            return result.Data.Id;
        }

        [Fact]
        public async Task CreateSpecialtyAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            var dto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = $"Test Specialty {Guid.NewGuid()}",
                Description = "Test specialty description"
            };

            // Act
            var result = await _repository.CreateSpecialtyAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dto.Name, result.Data.Name);
            Assert.Equal(dto.DepartmentId, result.Data.DepartmentId);
            Assert.True(result.Data.IsActive);
        }

        [Fact]
        public async Task CreateSpecialtyAsync_WithInvalidDepartmentId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateSpecialtyDto
            {
                DepartmentId = 999999,
                Name = $"Invalid Dept Specialty {Guid.NewGuid()}",
                Description = "Test description"
            };

            // Act
            var result = await _repository.CreateSpecialtyAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Department not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateSpecialtyAsync_WithDuplicateName_ShouldReturnFailure()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            var name = $"Duplicate Specialty {Guid.NewGuid()}";
            
            var firstDto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = name,
                Description = "First description"
            };

            var secondDto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = name,
                Description = "Second description"
            };

            // Act
            var firstResult = await _repository.CreateSpecialtyAsync(firstDto);
            var secondResult = await _repository.CreateSpecialtyAsync(secondDto);

            // Assert
            Assert.True(firstResult.IsSuccess);
            Assert.False(secondResult.IsSuccess);
            Assert.Contains("already exists", secondResult.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetSpecialtyByIdAsync_WithValidId_ShouldReturnSpecialty()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            var createDto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = $"Get By ID Test {Guid.NewGuid()}",
                Description = "Test description"
            };

            var createResult = await _repository.CreateSpecialtyAsync(createDto);
            var specialtyId = createResult.Data.Id;

            // Act
            var result = await _repository.GetSpecialtyByIdAsync(specialtyId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(specialtyId, result.Data.Id);
            Assert.Equal(createDto.Name, result.Data.Name);
        }

        [Fact]
        public async Task UpdateSpecialtyAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            var createDto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = $"Update Test {Guid.NewGuid()}",
                Description = "Original description"
            };

            var createResult = await _repository.CreateSpecialtyAsync(createDto);
            var specialtyId = createResult.Data.Id;

            var updateDto = new UpdateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = $"Updated Name {Guid.NewGuid()}",
                Description = "Updated description"
            };

            // Act
            var result = await _repository.UpdateSpecialtyAsync(specialtyId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.Name, result.Data.Name);
            Assert.Equal(updateDto.Description, result.Data.Description);
        }

        [Fact]
        public async Task DeleteSpecialtyAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            var createDto = new CreateSpecialtyDto
            {
                DepartmentId = departmentId,
                Name = $"Delete Test {Guid.NewGuid()}",
                Description = "Test description"
            };

            var createResult = await _repository.CreateSpecialtyAsync(createDto);
            var specialtyId = createResult.Data.Id;

            // Act
            var result = await _repository.DeleteSpecialtyAsync(specialtyId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Verify specialty is deactivated
            var getResult = await _repository.GetSpecialtyByIdAsync(specialtyId);
            Assert.False(getResult.IsSuccess);
        }

        [Fact]
        public async Task GetAllSpecialtiesAsync_ShouldReturnSpecialtyList()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            for (int i = 0; i < 3; i++)
            {
                var dto = new CreateSpecialtyDto
                {
                    DepartmentId = departmentId,
                    Name = $"List Test Specialty {i} {Guid.NewGuid()}",
                    Description = $"Description {i}"
                };
                await _repository.CreateSpecialtyAsync(dto);
            }

            // Act
            var result = await _repository.GetAllSpecialtiesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count >= 3);
        }

        [Fact]
        public async Task GetSpecialtiesByDepartmentAsync_ShouldReturnFilteredList()
        {
            // Arrange
            var departmentId = await CreateTestDepartment();
            
            // Create specialties for this department
            for (int i = 0; i < 2; i++)
            {
                var dto = new CreateSpecialtyDto
                {
                    DepartmentId = departmentId,
                    Name = $"Dept Filter Test {i} {Guid.NewGuid()}",
                    Description = $"Description {i}"
                };
                await _repository.CreateSpecialtyAsync(dto);
            }

            // Act
            var result = await _repository.GetSpecialtiesByDepartmentAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count >= 2);
            Assert.All(result.Data, s => Assert.Equal(departmentId, s.DepartmentId));
        }
    }
}