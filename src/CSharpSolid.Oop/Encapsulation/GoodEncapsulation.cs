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
        SocialSecuriryNumber = socialScurityNumber;
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