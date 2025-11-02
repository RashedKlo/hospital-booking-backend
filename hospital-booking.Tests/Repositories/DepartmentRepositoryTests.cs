using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Repositories.Department;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Settings;

namespace hospital_booking.Tests.Repositories
{
    public class DepartmentRepositoryTests
    {
        private readonly Mock<ILogger<DepartmentRepository>> _mockLogger;
        private readonly DepartmentRepository _repository;

        public DepartmentRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<DepartmentRepository>>();
            
            _repository = new DepartmentRepository(_mockLogger.Object);
        }

        [Fact]
        public async Task CreateDepartmentAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateDepartmentDto
            {
                Name = $"Test Department {Guid.NewGuid()}",
                Description = "Test department description"
            };

            // Act
            var result = await _repository.CreateDepartmentAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(dto.Name, result.Data.Name);
            Assert.Equal(dto.Description, result.Data.Description);
            Assert.True(result.Data.IsActive);
        }

        [Fact]
        public async Task CreateDepartmentAsync_WithDuplicateName_ShouldReturnFailure()
        {
            // Arrange
            var name = $"Duplicate Dept {Guid.NewGuid()}";
            var firstDto = new CreateDepartmentDto
            {
                Name = name,
                Description = "First description"
            };

            var secondDto = new CreateDepartmentDto
            {
                Name = name,
                Description = "Second description"
            };

            // Act
            var firstResult = await _repository.CreateDepartmentAsync(firstDto);
            var secondResult = await _repository.CreateDepartmentAsync(secondDto);

            // Assert
            Assert.True(firstResult.IsSuccess);
            Assert.False(secondResult.IsSuccess);
            Assert.Contains("already exists", secondResult.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_WithValidId_ShouldReturnDepartment()
        {
            // Arrange - Create a department first
            var createDto = new CreateDepartmentDto
            {
                Name = $"Get By ID Test {Guid.NewGuid()}",
                Description = "Test description"
            };

            var createResult = await _repository.CreateDepartmentAsync(createDto);
            var departmentId = createResult.Data.Id;

            // Act
            var result = await _repository.GetDepartmentByIdAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(departmentId, result.Data.Id);
            Assert.Equal(createDto.Name, result.Data.Name);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _repository.GetDepartmentByIdAsync(999999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange - Create a department first
            var createDto = new CreateDepartmentDto
            {
                Name = $"Update Test {Guid.NewGuid()}",
                Description = "Original description"
            };

            var createResult = await _repository.CreateDepartmentAsync(createDto);
            var departmentId = createResult.Data.Id;

            var updateDto = new UpdateDepartmentDto
            {
                Name = $"Updated Name {Guid.NewGuid()}",
                Description = "Updated description"
            };

            // Act
            var result = await _repository.UpdateDepartmentAsync(departmentId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.Name, result.Data.Name);
            Assert.Equal(updateDto.Description, result.Data.Description);
            Assert.NotNull(result.Data.UpdatedAt);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange - Create a department first
            var createDto = new CreateDepartmentDto
            {
                Name = $"Delete Test {Guid.NewGuid()}",
                Description = "Test description"
            };

            var createResult = await _repository.CreateDepartmentAsync(createDto);
            var departmentId = createResult.Data.Id;

            // Act
            var result = await _repository.DeleteDepartmentAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Verify department is deactivated
            var getResult = await _repository.GetDepartmentByIdAsync(departmentId);
            Assert.False(getResult.IsSuccess);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ShouldReturnDepartmentList()
        {
            // Arrange - Create multiple departments
            for (int i = 0; i < 3; i++)
            {
                var dto = new CreateDepartmentDto
                {
                    Name = $"List Test Dept {i} {Guid.NewGuid()}",
                    Description = $"Description {i}"
                };
                await _repository.CreateDepartmentAsync(dto);
            }

            // Act
            var result = await _repository.GetAllDepartmentsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count >= 3);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_WithDuplicateName_ShouldReturnFailure()
        {
            // Arrange - Create two departments
            var name1 = $"Dept One {Guid.NewGuid()}";
            var name2 = $"Dept Two {Guid.NewGuid()}";

            var createDto1 = new CreateDepartmentDto { Name = name1, Description = "Desc 1" };
            var createDto2 = new CreateDepartmentDto { Name = name2, Description = "Desc 2" };

            var result1 = await _repository.CreateDepartmentAsync(createDto1);
            var result2 = await _repository.CreateDepartmentAsync(createDto2);

            // Try to update second department with first department's name
            var updateDto = new UpdateDepartmentDto { Name = name1, Description = "Updated desc" };

            // Act
            var updateResult = await _repository.UpdateDepartmentAsync(result2.Data.Id, updateDto);

            // Assert
            Assert.False(updateResult.IsSuccess);
            Assert.Contains("already exists", updateResult.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}