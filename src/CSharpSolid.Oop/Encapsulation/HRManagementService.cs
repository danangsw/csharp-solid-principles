using Microsoft.Extensions.Logging;

namespace CSharpSolid.Oop.Encapsulation;

public class HRManagementService
{
    private readonly IList<GoodEmployee> _employees;
    private readonly ILogger<HRManagementService> _logger;
    private readonly ITimeProvider _timeProvider;

    public HRManagementService(ILogger<HRManagementService> logger, ITimeProvider? timeProvider = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _employees = new List<GoodEmployee>();
        _timeProvider = timeProvider ?? new SystemTimeProvider();
    }

    public string HireEmployee(EmployeeDataModel model)
    {
        try
        {
            var employeeId = GenerateEmployeeId();
            var newEmployee = new EmployeeDetailBuilder()
            .WithEmployeeId(employeeId)
            .WithFirstName(model.FirstName ?? throw new ArgumentNullException(nameof(model.FirstName)))
            .WithLastName(model.LastName ?? throw new ArgumentNullException(nameof(model.LastName)))
            .WithSocialSecurityNumber(model.SocialSecurityNumber ?? throw new ArgumentNullException(nameof(model.SocialSecurityNumber)))
            .WithDepartment(model.Department ?? throw new ArgumentNullException(nameof(model.Department)))
            .WithInitialSalary(model.Salary)
            .WithHireDate(model.HireDate)
            .WithTimeProvider(_timeProvider)
            .Build();

            _employees.Add(newEmployee);
            return employeeId;
        }
        catch (Exception ex)
        {       
            _logger.LogError(ex, "Error hiring employee {EmployeeId}", model.EmployeeId);
            throw;
        }
    }

    public void PromoteEmployee(EmployeeDataModel model, string newTitle, decimal salaryIncrease)
    {
        try
        {
            ValidateModel(model); // Ensure model is valid, and EmployeeId is present
            
            var employee = FindEmployeeById(model.EmployeeId);
            employee.PromoteWithSalaryIncrease(salaryIncrease, newTitle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error promoting employee {EmployeeId}", model.EmployeeId);
            throw;
        }
    }

    public void TerminateEmployee(EmployeeDataModel model, string reason)
    {
        try
        {
            ValidateModel(model); // Ensure model is valid, and EmployeeId is present
            
            var employee = FindEmployeeById(model.EmployeeId);
            employee.Terminate(reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating employee {EmployeeId}", model.EmployeeId);
            throw;
        }
    }

    public EmployeeDataModel GetEmployeeDetails(string employeeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new ArgumentException("EmployeeId cannot be null or empty", nameof(employeeId));

            var employee = FindEmployeeById(employeeId);
            return new EmployeeDataModel
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                SocialSecurityNumber = employee.SocialSecurityNumber,
                Salary = employee.Salary,
                Department = employee.Department,
                HireDate = employee.HireDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detail for employee {EmployeeId}", employeeId);
            throw;
        }
    }

    private static void ValidateModel(EmployeeDataModel model)
    {
        if (model == null) throw new ArgumentException("Model cannot be null", nameof(model));
        if (string.IsNullOrWhiteSpace(model.EmployeeId)) throw new ArgumentException("EmployeeId cannot be null or empty", nameof(model.EmployeeId));
    }

    private GoodEmployee FindEmployeeById(string? employeeId)
    {
        var employee = _employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        if (employee == null)
        {
            throw new ArgumentException($"Employee with ID {employeeId} not found.");
        }
        return employee;
    }

    private string GenerateEmployeeId()
    {
        return $"EMP-{_timeProvider.UtcNow:yyyyMMdd}-{_employees.Count + 1:D4}";
    }
}

public class EmployeeDataModel
{
    public string? EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public decimal Salary { get; set; }
    public string? Department { get; set; }
    public DateTime HireDate { get; set; }
}
