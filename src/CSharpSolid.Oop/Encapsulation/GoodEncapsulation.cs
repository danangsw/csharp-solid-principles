
namespace CSharpSolid.Oop.Encapsulation;

// GOOD EXAMPLE: Proper encapsulation, private fields with public properties,
// validation in setters, controlled access to internal state, and methods
// to manipulate the object's data safely.

public class GoodEmployee
{
    //constants for magic numbers
    private const int MaxProjectsPerEmployee = 5;
    private const int MaxNameLength = 50;
    private const decimal MaxSalary = 1000000m;
    private const decimal MinSalary = 0m;
    private const decimal MinBonusPercentage = 0.0m; // 0%
    private const decimal MaxBonusPercentage = 1.0m; // 100%
    private const decimal MaxSalaryIncreasePercentage = 0.5m; // 50%
    private const decimal MinSalaryIncreasePercentage = 0.0m; // 0%

    // static field to set minimum hire date
    private static readonly DateTime MinHireDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Private fields
    private string? _firstName;
    private string? _lastName;
    private decimal _salary;
    private DateTime _hireDate;
    private string? _department;
    private readonly List<string> _projects;
    private bool _isActive;
    private string? _socialSecurityNumber;
    private decimal _bonusPercentage;
    private readonly List<PayrollRecord> _payrollHistory;
    private readonly string _employeeId;
    private readonly ITimeProvider _timeProvider;

    // Internal constructor for builder pattern
    internal GoodEmployee(EmployeeDetailBuilder builder)
    {
        _timeProvider = builder.TimeProvider ?? new SystemTimeProvider();
        _employeeId = builder.EmployeeId ?? throw new ArgumentException("Employee ID is required", nameof(builder.EmployeeId));

        FirstName = builder.FirstName;
        LastName = builder.LastName;
        SocialSecurityNumber = builder.SocialSecurityNumber;
        Salary = builder.Salary;
        Department = builder.Department;
        HireDate = builder.HireDate;

        _projects = new List<string>();
        _payrollHistory = new List<PayrollRecord>();
        _isActive = true;
        _bonusPercentage = MinBonusPercentage;
    }

    public string EmployeeId => _employeeId;

    public string? FirstName
    {
        get => _firstName ?? string.Empty;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.");

            if (value.Length > MaxNameLength)
                throw new ArgumentException($"First name should be less than {MaxNameLength} characters.");

            _firstName = value.Trim();
        }
    }

    public string? LastName
    {
        get => _lastName ?? string.Empty;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty.");

            if (value.Length > MaxNameLength)
                throw new ArgumentException($"Last name should be less than {MaxNameLength} characters.");

            _lastName = value.Trim();
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public decimal Salary
    {
        get => _salary;
        set
        {
            if (value <= MinSalary)
                throw new ArgumentException("Salary cannot be negative", nameof(Salary));

            if (value > MaxSalary)
                throw new ArgumentException($"Salary cannot exceed {MaxSalary:C}", nameof(Salary));

            _salary = value;
        }
    }

    public string? Department
    {
        get => _department ?? string.Empty;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Department cannot be empty", nameof(Department));

            var validDepartments = new[] { "IT", "Finance", "HR", "Marketing", "Operation", "Engineering" };
            if (!validDepartments.Contains(value))
                throw new InvalidOperationException($"Invalid department. Must be one of: {string.Join(", ", validDepartments)}");

            _department = value;
        }
    }

    public DateTime HireDate
    {
        get => _hireDate;
        set
        {   
            if (value > _timeProvider.UtcNow)
                throw new ArgumentException("Hire date cannot be in the future", nameof(HireDate));
            if (value < MinHireDate)
                throw new ArgumentException("Hire date cannot before 01-01-1900", nameof(HireDate));

            _hireDate = value;
        }
    }

    public bool IsActive
    {
        get => _isActive;
        private set => _isActive = value; // Private setter - controller termination
    }

    public decimal BonusPercentage
    {
        get => _bonusPercentage;
        set
        {
            if (value < MinBonusPercentage)
                throw new ArgumentException($"Bonus percentage cannot be negative value", nameof(BonusPercentage));
            if (value > MaxBonusPercentage)
                throw new ArgumentException($"Bonus percentage cannot be exceed {MaxBonusPercentage:P}", nameof(BonusPercentage));

            _bonusPercentage = value;
        }
    }

    public string? SocialSecurityNumber
    {
        get => _socialSecurityNumber == null ? string.Empty : MaskingSSN(_socialSecurityNumber);
        set
        {
            if (!IsValidSSN(value))
            {
                throw new ArgumentException("Invalid Social Security Number format", nameof(SocialSecurityNumber));
            }

            _socialSecurityNumber = value;
        }
    }

    // Read only properties
    public IReadOnlyList<string> Projects => _projects.AsReadOnly();

    public IReadOnlyList<PayrollRecord> PayrollHistory => _payrollHistory.AsReadOnly();

    public int YearOfService => CalculateYearsOfService(_timeProvider.UtcNow);

    // Operations methods
    public void AssignToProjects(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
            throw new ArgumentException("Project name cannot be empty", nameof(projectName));

        if (!_isActive)
            throw new InvalidOperationException("Cannot assign project to an inactive employee");

        if (_projects.Contains(projectName))
            throw new InvalidOperationException($"Employee already assigned to project: {projectName}");

        if (_projects.Count > MaxProjectsPerEmployee)
            throw new InvalidOperationException($"Employee cannot be assigned to more than {MaxProjectsPerEmployee} projects.");

        _projects.Add(projectName);
    }

    public void RemoveFromProject(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
            throw new ArgumentException("Project name cannot be empty", nameof(projectName));

        if (!_projects.Contains(projectName))
            throw new InvalidOperationException($"Employee is not assigned to project: {projectName}");

        _projects.Remove(projectName);
    }

    public void PromoteWithSalaryIncrease(decimal increasePercentage, string reason)
    {
        if (!_isActive)
            throw new InvalidOperationException("Cannot promote an inactive employee.");
        if (increasePercentage < MinSalaryIncreasePercentage || increasePercentage > MaxSalaryIncreasePercentage)
            throw new ArgumentException($"Salary increase must be between {MinSalaryIncreasePercentage:P} and {MaxSalaryIncreasePercentage:P}", nameof(increasePercentage));

        var oldSalary = _salary;
        _salary = _salary * (1 + increasePercentage);

        LogPayrollChange(PayrollChangeType.Promotion, oldSalary, _salary, $"Promoted with {increasePercentage:P} increase");
    }

    public void SetBonusPercentage(decimal bonusPercentage, string reason)
    {
        if (!_isActive)
            throw new InvalidOperationException("Cannot set bonus for inactive employee");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason for bonus change is required", nameof(reason));

        var oldBonus = _bonusPercentage;
        BonusPercentage = bonusPercentage; // Uses property validation

        LogPayrollChange(PayrollChangeType.BonusAdjustment, oldBonus, bonusPercentage, reason);
    }

    public void Terminate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Termination reason is required", nameof(reason));
        if (!_isActive)
            throw new InvalidOperationException("Employee already terminated");

        _isActive = false;
        _projects.Clear(); // Remove all projects

        LogPayrollChange(PayrollChangeType.Termination, 0, 0, reason);
    }

    public void Reactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reactivation reason is required", nameof(reason));

        if (_isActive)
            throw new InvalidOperationException("Employee is already active");

        _isActive = true;

        LogPayrollChange(PayrollChangeType.Reactivation, 0, 0, reason);
    }

    public decimal CalculateAnnualCompensation()
    {
        if (!_isActive)
            throw new InvalidOperationException("Cannot calculate an inactive employee.");

        var basedAnnualSalary = _salary * 12;
        var bonusAmount = basedAnnualSalary * _bonusPercentage;

        return basedAnnualSalary + bonusAmount;
    }

    public PayrollCalculation CalculatePayroll(DateTime payrollPeriodStart, DateTime payrollPeriodEnd)
    {
        if (!_isActive)
            throw new InvalidOperationException("Cannot calculate an inactive employee.");

        ValidatePayrollPeriod(payrollPeriodStart, payrollPeriodEnd);

        var totalDaysInPeriod = (payrollPeriodEnd - payrollPeriodStart).TotalDays + 1;
        var workingDaysInPeriod = CalculateWorkingDays(payrollPeriodStart, payrollPeriodEnd);
        var basePay = _salary / (decimal)totalDaysInPeriod * workingDaysInPeriod;
        var bonusPay = basePay * _bonusPercentage;
        var totalPay = basePay + bonusPay;

        var payroll = new PayrollCalculation
        {
            EmployeeId = _employeeId,
            PayPeriodStart = payrollPeriodStart,
            PayPeriodEnd = payrollPeriodEnd,
            BaseSalary = basePay,
            BonusPay = bonusPay,
            TotalPay = totalPay,
            WorkingDays = workingDaysInPeriod
        };

        _payrollHistory.Add(new PayrollRecord
        {
            Date = _timeProvider.UtcNow,
            PayPeriod = $"{payrollPeriodStart:yyyy-MM-dd} to {payrollPeriodEnd:yyyy-MM-dd}",
            Amount = totalPay,
            Type = PayrollChangeType.RegularPayroll,
            PreviousAmount = 0.0m,
            Reason = string.Empty
        });

        return payroll;
    }

    public override string ToString()
    {
        return $"Employee: {FullName} ({EmployeeId}) - {Department} - {(IsActive ? "Active" : "Inactive")}";
    }

    // Protected methods
    protected string GetUnmaskedSSN()
    {
        return _socialSecurityNumber ?? string.Empty;
    }

    // Private methods
    private int CalculateYearsOfService(DateTime currentDate)
    {
        var years = currentDate.Year - _hireDate.Year;
        if (currentDate < _hireDate.AddYears(years)) // Not yet had birthday this year
            years--;
        return years;
    }

    private void ValidatePayrollPeriod(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ArgumentException($"Payroll period end '{end.ToString("dd/MM/yyyy")}' cannot be before period start '{start.ToString("dd/MM/yyyy")}'");
        if ((end - start).Days > 31)
            throw new ArgumentException("Payroll period cannot exceed 31 days");
        if (start < _hireDate)
            throw new ArgumentException("Payroll period cannot start before hire date");
    }

    private static int CalculateWorkingDays(DateTime start, DateTime end)
    {
        var workingDays = 0;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }

        return workingDays;
    }

    private void LogPayrollChange(PayrollChangeType type, decimal oldSalary, decimal newSalary, string reason)
    {
        _payrollHistory.Add(new PayrollRecord
        {
            Date = _timeProvider.UtcNow,
            Type = type,
            Amount = newSalary,
            PreviousAmount = oldSalary,
            Reason = reason
        });
    }

    private static string MaskingSSN(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 4)
            return "***-**-****";

        return $"***-**-{value.Substring(value.Length - 4)}";
    }

    private static bool IsValidSSN(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        // Simple validation - in real app, use more robust validation
        var pattern = @"^\d{3}-\d{2}-\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
    }
}

public class PayrollCalculation
{
    public string? EmployeeId { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal BonusPay { get; set; }
    public decimal TotalPay { get; set; }
    public int WorkingDays { get; set; }
}

public class PayrollRecord
{
    public DateTime Date { get; set; }
    public PayrollChangeType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal PreviousAmount { get; set; }
    public string? PayPeriod { get; set; }
    public string? Reason { get; set; }
}

public enum PayrollChangeType
{
    RegularPayroll,
    Promotion,
    BonusAdjustment,
    Termination,
    Reactivation,
    SalaryAdjustment
}

public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

public class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}

public class EmployeeDetailBuilder
{
    private string? _employeeId;
    private string? _firstName;
    private string? _lastName;
    private string? _socialSecurityNumber;
    private decimal _initialSalary;
    private string? _department;
    private DateTime _hireDate;
    private ITimeProvider? _timeProvider;

    public string? EmployeeId => _employeeId;
    public string? FirstName => _firstName;
    public string? LastName => _lastName;
    public string? SocialSecurityNumber => _socialSecurityNumber;
    public decimal Salary => _initialSalary;
    public string? Department => _department;
    public DateTime HireDate => _hireDate;
    public ITimeProvider? TimeProvider => _timeProvider;

    public EmployeeDetailBuilder WithEmployeeId(string employeeId)
    {
        _employeeId = employeeId;
        return this;
    }

    public EmployeeDetailBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public EmployeeDetailBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public EmployeeDetailBuilder WithSocialSecurityNumber(string ssn)
    {
        _socialSecurityNumber = ssn;
        return this;
    }

    public EmployeeDetailBuilder WithInitialSalary(decimal salary)
    {
        _initialSalary = salary;
        return this;
    }

    public EmployeeDetailBuilder WithDepartment(string department)
    {
        _department = department;
        return this;
    }

    public EmployeeDetailBuilder WithHireDate(DateTime hireDate)
    {
        _hireDate = hireDate;
        return this;
    }

    public EmployeeDetailBuilder WithTimeProvider(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        return this;
    }

    public GoodEmployee Build()
    {
        if (string.IsNullOrWhiteSpace(_employeeId))
            throw new InvalidOperationException("EmployeeId is required");
        if (string.IsNullOrWhiteSpace(_firstName))
            throw new InvalidOperationException("FirstName is required");
        if (string.IsNullOrWhiteSpace(_lastName))
            throw new InvalidOperationException("LastName is required");
        if (string.IsNullOrWhiteSpace(_socialSecurityNumber))
            throw new InvalidOperationException("SocialSecurityNumber is required");
        if (string.IsNullOrWhiteSpace(_department))
            throw new InvalidOperationException("Department is required");
        if (_hireDate == default)
            throw new InvalidOperationException("HireDate is required");

        if (_timeProvider == null)
            _timeProvider = new SystemTimeProvider();

        return new GoodEmployee(this);
    }
}
