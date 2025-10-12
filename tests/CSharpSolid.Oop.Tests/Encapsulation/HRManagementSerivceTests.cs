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
}
