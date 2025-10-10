namespace CSharpSolid.Oop.Encapsulation;

// GOOD EXAMPLE: Proper encapsulation, private fields with public properties,
// validation in setters, controlled access to internal state, and methods
// to manipulate the object's data safely.

public class GoodEmployee
{
    // Private fields
    private string _firstName;
    private string _lastName;
    private decimal _salary;
    private DateTime _hireDate;
    private string _department;
    private List<string> _projects;
    private bool _isActive;
    private string _socialSecurityNumber;
    private decimal _bonusPercentage;
    private readonly List<PayrollRecord> _payrollHistory;
    private readonly string _employeeId;

    // Constructor with validation
    public GoodEmployee(string employeeId, string firstname, string lastName,
    string socialSecurityNumber, decimal initialSalary, string department, DateTime hireDate)
    {
        _employeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));

        FirstName = firstname;
        LastName = lastName;
        SocialSecurityNumber = socialSecurityNumber;
        Salary = initialSalary;
        Department = department;
        HireDate = hireDate;

        _projects = new List<string>();
        _payrollHistory = new List<PayrollRecord>();
        _isActive = true;
        _bonusPercentage = 0.0m;
    }

    public string EmployeeId()
    {
        return _employeeId;
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("First name should be less than 50 characters.");

            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Last name should be less than 50 characters.");

            _lastName = value.Trim();
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public decimal Salary
    {
        get => _salary;
        set
        {
            if (value < 0)
                throw new ArgumentException("Salary cannot be negative", nameof(value));
            
            if (value > 1000000)
                throw new ArgumentException("Salary cannot exceed $.1000.000", nameof(value));

            _salary = value;
        }
    }

    public string Department
    {
        get => _department;
        set
        {
            if string.IsNullOrWhiteSpace(value)
                throw new ArgumentException("Department cannot be empty", nameof(value))
            
            var validDeparments = new[] {"IT", "Finance", "HR", "Marketing", "Operation"}
            if (!validDeparments.Contains(value))
                throw new InvalidOperationException ($"Invalid department. Must be one of: {string.Join("", validDeparments)}");
        
            _department = value;
        }
    }

    public DateTime HireDate
    {
        get => _hireDate;
        set
        {
            if (value > DateTime.Now)
                throw new ArgumentException("Hire date cannot be in the future", nameof(value));
            if (value < new DateTime(1900,1,1))
                throw new ArgumentException("Hire date cannot before 01-01-1900", nameof(value));
            
            HireDate = value;
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
            if (value < 0)
                throw new ArgumentException("Bonus percentage cannot be negative value", nameof(value));
            if (value > 1.0m)
                throw new ArgumentException("Bonus percentage cannot be exceed 100%", nameof(value));

            _bonusPercentage = value;
        }
    }

    public string SocialSecurityNumber
    {
        get => MaskingSSN(_socialSecurityNumber);
        set
        {
            if (IsValidSSN(value))
            {
                throw new ArgumentException("Invalid Social Security Number format", nameof(value));
            }

            _socialSecurityNumber = value;
        }
    }

    // Read only properties
    public IReadOnlyList<string> Projects => _projects.AsReadOnly();

    public IReadOnlyList<PayrollRecord> PayrollHistory => _payrollHistory.AsReadOnly();

    public int YearOfService => (DateTime.Now - _hireDate).Days / 365;

    // Operations methods
    public void AssignToProjects(string projectName)
    {
        if string.IsNullOrWhiteSpace(projectName)
            throw new ArgumentException("Project name cannot be empty", nameof(projectName));
        
        if (!_isActive)
            throw new InvalidOperationException("Cannot assign project to an inactive employee");

        if (_projects.Contains(projectName))
            throw new InvalidOperationException($"Employee already assigned to project: {projectName}");

        if (_projects.Count > 5)
            throw new InvalidOperationException("Employee cannot be assigned to more than 5 projects.")

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

    public void PromoteWithSalaryIncrease(decimal increasePercentage)
    {
        if(!_isActive)
            throw new InvalidOperationException("Cannot promote an inactive employee.");
        if (increasePercentage < 0.0m || increasePercentage > 0.5m)
            throw new ArgumentException("Salary increase must be between 0% and 50%", nameof(increasePercentage));
        
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
            throw new ArgumentException("Termination reason is require", nameof(reason));
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

    public PayrollCalculation CalculatePayroll(DateTime payrollPeriodStart, DateTime payrollPeriodEnd) {
        if (!_isActive)
            throw new InvalidOperationException("Cannot calculate an inactive employee.");

        ValidatePayrollPeriod(payrollPeriodStart, payrollPeriodEnd);

        var daysInPeriod = (payrollPeriodEnd - payrollPeriodStart).Days + 1;
        var workingDays = CalculateWorkingDays(payrollPeriodStart, payrollPeriodEnd);

        var basePay = (_salary / 30) * workingDays; // Assuming 30-day month
        var bonusPay = basePay * _bonusPercentage;
        var totalPay = basePay + bonusPay;

        var payroll = new PayrollCalculation
        {
            EmployeeId = _employeeId,
            PayPeriodStart = payrollPeriodStart,
            PayPeriodEnd = payrollPeriodEnd,
            BasePay = basePay,
            BonusPay = bonusPay,
            TotalPay = totalPay,
            WorkingDays = workingDays
        };

        _payrollHistory.Add(new PayrollRecord 
        {
            Date = DateTime.Now,
            PayPeriod = $"{payrollPeriodStart:yyyy-MM-dd} to {payrollPeriodEnd:yyyy-MM-dd}",
            Amount = totalPay,
            Type = PayrollChangeType.RegularPayroll
        });

        return payroll;
    }

    public override string ToString
    {
        return $"Employee: {FullName} ({EmployeeId}) - {Department} - {(IsActive ? "Active" : "Inactive")}";
    }

    // Protected methods
    protected string GetUnmaskedSSN()
    {
        return _socialSecurityNumber;
    }

    // Private methods
    private void ValidatePayrollPeriod(DateTime start, DateTime end)
    {
        if (end < date)
            throw new ArgumentException($"Payroll period end '{end.ToString("dd/MM/yyyy")}' cannot be before period start '{start.ToString("dd/MM/yyyy")}'");
        if ((end - start).Days > 31)
            throw new ArgumentException("Payroll period cannot exceed 31 days");
        if (start < _hireDate)
            throw new ArgumentException("Payroll period cannot start before hire date");
    }

    private int CalculateWorkingDays(DateTime start, DateTime end)
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
        _payrollHistory.Add(new PayrollRecord {
            Date = DateTime.Now,
            Type = type,
            Amount = newSalary,
            PreviousAmount = oldSalary,
            Reason = notes
        });
    }

    private string MaskingSSN(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 4)
            return "***-**-****";
        
        return $"***-**-{value.Substring(value.Length - 4)}";
    }

    private bool IsValidSSN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        // Simple validation - in real app, use more robust validation
        var pattern = @"^\d{3}-\d{2}-\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(ssn, pattern);
    }
}

public class PayrollCalculation
{
    public string EmployeeId { get; set; }
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
    public decimal PreviousAmount {get; set;}
    public string PayPeriod { get; set; }
    public string Reason { get;  set; }
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