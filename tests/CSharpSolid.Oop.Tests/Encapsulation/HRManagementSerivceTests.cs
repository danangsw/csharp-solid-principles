using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using CSharpSolid.Oop.Encapsulation;

namespace CSharpSolid.Oop.Tests.Encapsulation;
public class HRManagementServiceTests
{
    private readonly Mock<ILogger<HRManagementService>> _loggerMock;
    private readonly Mock<ITimeProvider> _timeProviderMock;
    private readonly HRManagementService _hrService;

    // Constructor to setup common test data
    public HRManagementServiceTests()
    {
        _loggerMock = new Mock<ILogger<HRManagementService>>();
        _timeProviderMock = new Mock<ITimeProvider>();

        // setup default behavior for time provider
        _timeProviderMock.Setup(tp => tp.UtcNow).Returns(DateTime.UtcNow);
        _timeProviderMock.Setup(tp => tp.Now).Returns(DateTime.Now);

        _hrService = new HRManagementService(_loggerMock.Object, _timeProviderMock.Object);
    }

    // Helper method to create a valid EmployeeDataModel
    private EmployeeDataModel CreateValidEmployeeModel()
    {
        return new EmployeeDataModel
        {
            FirstName = "John",
            LastName = "Doe",
            SocialSecurityNumber = "123-45-6789",
            Department = "Engineering",
            Salary = 60000m,
            HireDate = DateTime.UtcNow.AddDays(-30) // Hired 30 days ago
        };
    }

    private static string MaskingSSN(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 4)
            return "***-**-****";

        return $"***-**-{value.Substring(value.Length - 4)}";
    }

    private void VerifyLoggerCalled(LogLevel level, Times times)
    {
        _loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    private void VerifyUtcNowTimeProviderCalled(Times times)
    {
        _timeProviderMock.Verify(tp => tp.UtcNow, times);
    }

    [Fact]
    public void HireEmployee_ValidModel_ReturnsEmployeeId()
    {
        // Arrange
        var model = CreateValidEmployeeModel();

        // Act
        var result = _hrService.HireEmployee(model);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith("EMP-");
        result.Should().Contain(_timeProviderMock.Object.UtcNow.ToString("yyyyMMdd"));
    }

    [Theory]
    [InlineData(null, "Doe", "123-45-6789", "Engineering", 60000, "2023-01-01", "FirstName")]
    [InlineData("John", null, "123-45-6789", "Engineering", 60000, "2023-01-01", "LastName")]
    [InlineData("John", "Doe", null, "Engineering", 60000, "2023-01-01", "SocialSecurityNumber")]
    [InlineData("John", "Doe", "123-45-6789", null, 60000, "2023-01-01", "Department")] // Added Department null case
    [InlineData("John", "Doe", "123-45-6789", "Engineering", 60000, "2029-01-01", "HireDate")] // Hire date in the future
    [InlineData("John", "Doe", "123-45-6789", "Engineering", -5000, "2023-01-01", "Salary")] // Negative salar
    [InlineData("John", "Doe", "123-45-6789", "Engineering", 0, "2023-01-01", "Salary")] // Zero salary
    [InlineData("John", "Doe", "123-45-6789", "Engineering", 60000, "1899-12-01", "HireDate")] // Invalid hire date
    [InlineData("John", "Doe", "123-45-6789", "Engineering", 1000001, "2023-01-01", "Salary")] // Null hire date
    public void HireEmployee_InvalidModel_ThrowsArgumentException(string firstName, string lastName, string ssn, string department, decimal salary, string hireDateStr, string expectedParamName)
    {
        // Arrange
        var model = new EmployeeDataModel
        {
            FirstName = firstName,
            LastName = lastName,
            SocialSecurityNumber = ssn,
            Department = department,
            Salary = salary,
            HireDate = hireDateStr != null ? DateTime.Parse(hireDateStr) : default
        };

        // Act
        Action act = () => _hrService.HireEmployee(model);

        // Assert
        act.Should().Throw<ArgumentException>()
            .Where(e => e.ParamName == expectedParamName);

        // Additional check for ILogger calls and time provider can be added here if needed
        // Verify logger was called for error logging
        VerifyLoggerCalled(LogLevel.Error, Times.Once());
        
        // Verify time provider was called (if used in validation)
        VerifyUtcNowTimeProviderCalled(Times.AtLeastOnce());
    }

    [Fact]
    public void HireEmployee_NullModel_ThrowsArgumentException()
    {
        // Arrange
        EmployeeDataModel model = null;

        // Act
        Action act = () => _hrService.HireEmployee(model);

        // Assert
        act.Should().Throw<ArgumentException>()
            .Where(e => e.ParamName == "model");

              // Additional check for ILogger calls and time provider can be added here if needed
        // Verify logger was called for error logging
        VerifyLoggerCalled(LogLevel.Error, Times.Once());
    }

    [Fact]
    public void GetEmployeeDetails_ValidId_ReturnsEmployeeDataModel()
    {
        // Arrange
        var model = CreateValidEmployeeModel();
        var employeeId = _hrService.HireEmployee(model);

        // Act
        var result = _hrService.GetEmployeeDetails(employeeId);

        // Assert
        result.Should().NotBeNull();
        result.EmployeeId.Should().Be(employeeId);
        result.FirstName.Should().Be(model.FirstName);
        result.LastName.Should().Be(model.LastName);
        result.SocialSecurityNumber.Should().Be(MaskingSSN(model.SocialSecurityNumber));
        result.Salary.Should().Be(model.Salary);
        result.Department.Should().Be(model.Department);
        result.HireDate.Should().Be(model.HireDate);
    }

    public void UpdateEmployee_ValidUpdate_ChangesSalaryAndDepartment()
    {
        // Arrange
        var model = CreateValidEmployeeModel();
        var employeeId = _hrService.HireEmployee(model);

        var updateModel = new EmployeeDataModel
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            SocialSecurityNumber = model.SocialSecurityNumber,
            Department = "Marketing", // Changed department
            Salary = 70000m, // Increased salary
            HireDate = model.HireDate
        };

        // Act
        _hrService.UpdateEmployeeDetails(employeeId, updateModel);
        var updatedEmployee = _hrService.GetEmployeeDetails(employeeId);

        // Assert
        updatedEmployee.Should().NotBeNull();
        updatedEmployee.EmployeeId.Should().Be(employeeId);
        updatedEmployee.FirstName.Should().Be(updateModel.FirstName);
        updatedEmployee.LastName.Should().Be(updateModel.LastName);
        updatedEmployee.SocialSecurityNumber.Should().Be(MaskingSSN(updateModel.SocialSecurityNumber));
        updatedEmployee.Salary.Should().Be(updateModel.Salary);
        updatedEmployee.Department.Should().Be(updateModel.Department);
        updatedEmployee.HireDate.Should().Be(updateModel.HireDate);
        updatedEmployee.IsActive.Should().BeTrue();}
}
