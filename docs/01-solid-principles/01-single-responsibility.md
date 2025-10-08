# Single Responsibility Principle (SRP)

> **"A class should have only one reason to change."** - Robert C. Martin

## 🎯 Definition

The Single Responsibility Principle states that every class should have responsibility over a single part of the functionality provided by the software, and that responsibility should be entirely encapsulated by the class.

## 🏠 Real-World Analogy

Think of a **Swiss Army knife** vs **specialized tools**:
- ❌ Swiss Army knife: One tool, many functions (harder to maintain, replace, or improve)
- ✅ Specialized tools: Each tool has one specific purpose (easier to maintain, replace, upgrade)

## 🚫 Violation Example

```csharp
// ❌ BAD: This class has multiple responsibilities
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }

    // Responsibility 1: Employee data management
    public void UpdateEmployee(Employee employee)
    {
        // Update employee logic
    }

    // Responsibility 2: Salary calculation
    public decimal CalculateBonus()
    {
        return Salary * 0.1m;
    }

    // Responsibility 3: Email notification
    public void SendWelcomeEmail()
    {
        var emailService = new EmailService();
        emailService.Send(Email, "Welcome", "Welcome to the company!");
    }

    // Responsibility 4: Database persistence
    public void SaveToDatabase()
    {
        var connection = new SqlConnection("connectionString");
        // Save employee to database
    }

    // Responsibility 5: Report generation
    public string GenerateEmployeeReport()
    {
        return $"Employee Report: {Name}, Salary: {Salary}";
    }
}
```

### Problems with this approach:
1. **Multiple reasons to change**: Email format, database schema, bonus calculation, reporting format
2. **Hard to test**: Must mock database, email service, etc.
3. **High coupling**: Changes in one area affect others
4. **Difficult to maintain**: Large, complex class

## ✅ Correct Implementation

```csharp
// ✅ GOOD: Each class has a single responsibility

// 1. Employee Entity - Only holds employee data
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}

// 2. Employee Repository - Only handles data persistence
public interface IEmployeeRepository
{
    void Save(Employee employee);
    Employee GetById(int id);
    void Update(Employee employee);
    void Delete(int id);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connectionString;

    public EmployeeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Save(Employee employee)
    {
        using var connection = new SqlConnection(_connectionString);
        // Save employee to database
    }

    public Employee GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        // Retrieve employee from database
        return new Employee(); // Simplified
    }

    public void Update(Employee employee)
    {
        using var connection = new SqlConnection(_connectionString);
        // Update employee in database
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        // Delete employee from database
    }
}

// 3. Salary Calculator - Only handles salary calculations
public interface ISalaryCalculator
{
    decimal CalculateBonus(Employee employee);
    decimal CalculateAnnualSalary(Employee employee);
}

public class SalaryCalculator : ISalaryCalculator
{
    public decimal CalculateBonus(Employee employee)
    {
        return employee.Salary * 0.1m;
    }

    public decimal CalculateAnnualSalary(Employee employee)
    {
        return employee.Salary * 12;
    }
}

// 4. Email Service - Only handles email notifications
public interface IEmailService
{
    void SendWelcomeEmail(string email, string name);
    void SendEmail(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    public void SendWelcomeEmail(string email, string name)
    {
        var subject = "Welcome to the Company";
        var body = $"Dear {name}, welcome to our company!";
        SendEmail(email, subject, body);
    }

    public void SendEmail(string to, string subject, string body)
    {
        // Email sending logic using SMTP, SendGrid, etc.
        Console.WriteLine($"Sending email to {to}: {subject}");
    }
}

// 5. Report Generator - Only handles report generation
public interface IReportGenerator
{
    string GenerateEmployeeReport(Employee employee);
    string GenerateEmployeeListReport(IEnumerable<Employee> employees);
}

public class EmployeeReportGenerator : IReportGenerator
{
    public string GenerateEmployeeReport(Employee employee)
    {
        return $"Employee Report:\\nName: {employee.Name}\\nSalary: {employee.Salary:C}";
    }

    public string GenerateEmployeeListReport(IEnumerable<Employee> employees)
    {
        var report = "Employee List Report:\\n";
        foreach (var emp in employees)
        {
            report += $"- {emp.Name}: {emp.Salary:C}\\n";
        }
        return report;
    }
}

// 6. Employee Service - Orchestrates operations (Application Service)
public class EmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ISalaryCalculator _salaryCalculator;
    private readonly IEmailService _emailService;
    private readonly IReportGenerator _reportGenerator;

    public EmployeeService(
        IEmployeeRepository repository,
        ISalaryCalculator salaryCalculator,
        IEmailService emailService,
        IReportGenerator reportGenerator)
    {
        _repository = repository;
        _salaryCalculator = salaryCalculator;
        _emailService = emailService;
        _reportGenerator = reportGenerator;
    }

    public void HireEmployee(Employee employee)
    {
        _repository.Save(employee);
        _emailService.SendWelcomeEmail(employee.Email, employee.Name);
    }

    public decimal GetEmployeeBonus(int employeeId)
    {
        var employee = _repository.GetById(employeeId);
        return _salaryCalculator.CalculateBonus(employee);
    }

    public string GetEmployeeReport(int employeeId)
    {
        var employee = _repository.GetById(employeeId);
        return _reportGenerator.GenerateEmployeeReport(employee);
    }
}
```

## 🏢 ERP Example: Order Processing

### ❌ Violation
```csharp
public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }

    // Multiple responsibilities in one class
    public void CalculateTotal() { /* calculation logic */ }
    public void SaveToDatabase() { /* persistence logic */ }
    public void SendConfirmationEmail() { /* email logic */ }
    public void UpdateInventory() { /* inventory logic */ }
    public void ProcessPayment() { /* payment logic */ }
    public void GenerateInvoice() { /* reporting logic */ }
}
```

### ✅ Correct Implementation
```csharp
// Order Entity - Only order data
public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

// Each service has single responsibility
public interface IOrderCalculator
{
    decimal CalculateTotal(Order order);
}

public interface IOrderRepository
{
    void Save(Order order);
}

public interface IInventoryService
{
    void UpdateStock(Order order);
}

public interface IPaymentProcessor
{
    bool ProcessPayment(Order order, PaymentInfo payment);
}

public interface IInvoiceGenerator
{
    Invoice GenerateInvoice(Order order);
}

public interface INotificationService
{
    void SendOrderConfirmation(Order order);
}
```

## ✅ Benefits

1. **Easier to understand**: Each class has a clear, single purpose
2. **Easier to test**: Smaller, focused classes are simpler to unit test
3. **Easier to maintain**: Changes are isolated to specific responsibilities
4. **Better reusability**: Single-purpose classes can be reused in different contexts
5. **Reduced coupling**: Classes depend on fewer things

## 🧪 Unit Testing

```csharp
[Test]
public void CalculateBonus_Should_Return_Ten_Percent_Of_Salary()
{
    // Arrange
    var calculator = new SalaryCalculator();
    var employee = new Employee { Salary = 1000m };

    // Act
    var bonus = calculator.CalculateBonus(employee);

    // Assert
    Assert.AreEqual(100m, bonus);
}

[Test]
public void SendWelcomeEmail_Should_Call_SendEmail_With_Correct_Parameters()
{
    // Arrange
    var mockEmailService = new Mock<IEmailService>();
    var emailService = mockEmailService.Object;

    // Act
    emailService.SendWelcomeEmail("john@example.com", "John Doe");

    // Assert
    mockEmailService.Verify(x => x.SendEmail(
        "john@example.com", 
        "Welcome to the Company", 
        It.IsAny<string>()), Times.Once);
}
```

## 🚨 Common Mistakes

1. **God Classes**: Classes that do everything
2. **Mixed Concerns**: Business logic mixed with infrastructure
3. **Large Methods**: Methods that do multiple things
4. **Static Utilities**: Static classes with unrelated methods

## 🎯 Interview Questions

**Q: How do you identify SRP violations?**
**A:** Look for:
- Classes with many methods doing different things
- Classes that change for multiple reasons
- Method names with "And" or "Or" (CalculateAndSave)
- Classes with many dependencies

**Q: What's the difference between SRP and separation of concerns?**
**A:** SRP is specifically about classes having one responsibility, while separation of concerns is a broader principle about separating different aspects of a program (like business logic from data access).

**Q: Can you give an ERP example where SRP is crucial?**
**A:** In order processing: separate classes for OrderValidator, OrderCalculator, PaymentProcessor, InventoryUpdater, and OrderNotifier instead of one OrderProcessor doing everything.

## 📝 Checklist

- [ ] Each class has only one reason to change
- [ ] Class names clearly indicate their single responsibility  
- [ ] Methods within a class are cohesive (related to the same responsibility)
- [ ] No mixed concerns (business logic separate from infrastructure)
- [ ] Easy to write focused unit tests
- [ ] Changes to one responsibility don't affect others

---

**Next**: [Open/Closed Principle →](./02-open-closed.md)