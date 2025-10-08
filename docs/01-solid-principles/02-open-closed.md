# Open/Closed Principle (OCP)

> **"Software entities should be open for extension, but closed for modification."** - Bertrand Meyer

## 🎯 Definition

The Open/Closed Principle states that classes should be open for extension but closed for modification. You should be able to add new functionality without changing existing code.

## 🏠 Real-World Analogy

Think of **electrical outlets** in your home:
- ✅ **Open for extension**: You can plug in different devices (phone charger, laptop, lamp)
- ✅ **Closed for modification**: You don't need to rewire the outlet for each new device
- The outlet provides a stable interface, devices extend functionality

## 🚫 Violation Example

```csharp
// ❌ BAD: Must modify existing code to add new functionality
public enum EmployeeType
{
    Permanent,
    Contract,
    Intern
}

public class Employee
{
    public string Name { get; set; }
    public EmployeeType Type { get; set; }
    public decimal BaseSalary { get; set; }
}

public class SalaryCalculator
{
    public decimal CalculateSalary(Employee employee)
    {
        switch (employee.Type)
        {
            case EmployeeType.Permanent:
                return employee.BaseSalary + (employee.BaseSalary * 0.1m); // 10% bonus
            
            case EmployeeType.Contract:
                return employee.BaseSalary; // No bonus
            
            case EmployeeType.Intern:
                return employee.BaseSalary * 0.8m; // 80% of base salary
            
            default:
                throw new ArgumentException("Unknown employee type");
        }
    }

    public string GetBenefits(Employee employee)
    {
        switch (employee.Type)
        {
            case EmployeeType.Permanent:
                return "Health Insurance, Retirement Plan, Paid Leave";
            
            case EmployeeType.Contract:
                return "Limited Health Insurance";
            
            case EmployeeType.Intern:
                return "Learning Materials, Mentorship";
            
            default:
                throw new ArgumentException("Unknown employee type");
        }
    }
}
```

### Problems with this approach:
1. **Must modify existing code** to add new employee types
2. **Violates Single Responsibility**: Calculator knows about all employee types
3. **Risk of breaking existing functionality** when adding new types
4. **Switch statement duplication** across multiple methods

## ✅ Correct Implementation

```csharp
// ✅ GOOD: Open for extension, closed for modification

// Base abstraction
public abstract class Employee
{
    public string Name { get; set; }
    public decimal BaseSalary { get; set; }

    // Template method - defines the algorithm structure
    public abstract decimal CalculateSalary();
    public abstract string GetBenefits();
    public abstract int GetVacationDays();
}

// Concrete implementations - extend functionality
public class PermanentEmployee : Employee
{
    public override decimal CalculateSalary()
    {
        return BaseSalary + (BaseSalary * 0.1m); // 10% bonus
    }

    public override string GetBenefits()
    {
        return "Health Insurance, Retirement Plan, Paid Leave, Stock Options";
    }

    public override int GetVacationDays()
    {
        return 25;
    }
}

public class ContractEmployee : Employee
{
    public int ContractDurationMonths { get; set; }

    public override decimal CalculateSalary()
    {
        return BaseSalary; // No bonus for contract
    }

    public override string GetBenefits()
    {
        return "Limited Health Insurance";
    }

    public override int GetVacationDays()
    {
        return 0; // No paid vacation
    }
}

public class InternEmployee : Employee
{
    public string University { get; set; }

    public override decimal CalculateSalary()
    {
        return BaseSalary * 0.8m; // 80% of base salary
    }

    public override string GetBenefits()
    {
        return "Learning Materials, Mentorship, Certification Programs";
    }

    public override int GetVacationDays()
    {
        return 10;
    }
}

// Easy to add new employee types without modifying existing code
public class FreelancerEmployee : Employee
{
    public decimal HourlyRate { get; set; }
    public int HoursWorked { get; set; }

    public override decimal CalculateSalary()
    {
        return HourlyRate * HoursWorked;
    }

    public override string GetBenefits()
    {
        return "Flexible Schedule, Project-based Work";
    }

    public override int GetVacationDays()
    {
        return 0; // Unpaid time off
    }
}

// Calculator doesn't need to change when adding new employee types
public class SalaryCalculator
{
    public decimal CalculateTotalSalary(IEnumerable<Employee> employees)
    {
        return employees.Sum(emp => emp.CalculateSalary());
    }

    public decimal CalculateAverageSalary(IEnumerable<Employee> employees)
    {
        return employees.Average(emp => emp.CalculateSalary());
    }
}
```

## 🔧 Using Interfaces for Better Flexibility

```csharp
// Strategy pattern implementation for even more flexibility
public interface ISalaryCalculationStrategy
{
    decimal CalculateSalary(decimal baseSalary, Employee employee);
}

public interface IBenefitsProvider
{
    string GetBenefits(Employee employee);
}

public class PermanentSalaryStrategy : ISalaryCalculationStrategy
{
    public decimal CalculateSalary(decimal baseSalary, Employee employee)
    {
        return baseSalary + (baseSalary * 0.1m);
    }
}

public class ContractSalaryStrategy : ISalaryCalculationStrategy
{
    public decimal CalculateSalary(decimal baseSalary, Employee employee)
    {
        return baseSalary;
    }
}

public class Employee
{
    public string Name { get; set; }
    public decimal BaseSalary { get; set; }
    public ISalaryCalculationStrategy SalaryStrategy { get; set; }
    public IBenefitsProvider BenefitsProvider { get; set; }

    public decimal CalculateSalary()
    {
        return SalaryStrategy.CalculateSalary(BaseSalary, this);
    }

    public string GetBenefits()
    {
        return BenefitsProvider.GetBenefits(this);
    }
}
```

## 🏢 ERP Example: Discount Calculation

### ❌ Violation
```csharp
public class DiscountCalculator
{
    public decimal CalculateDiscount(Customer customer, decimal orderAmount)
    {
        switch (customer.Type)
        {
            case "Regular":
                return 0;
            case "Premium":
                return orderAmount * 0.05m;
            case "VIP":
                return orderAmount * 0.10m;
            case "Corporate":
                return orderAmount * 0.15m;
            // Adding new customer type requires modifying this method
            default:
                return 0;
        }
    }
}
```

### ✅ Correct Implementation
```csharp
public abstract class Customer
{
    public string Name { get; set; }
    public abstract decimal CalculateDiscount(decimal orderAmount);
}

public class RegularCustomer : Customer
{
    public override decimal CalculateDiscount(decimal orderAmount)
    {
        return 0; // No discount
    }
}

public class PremiumCustomer : Customer
{
    public override decimal CalculateDiscount(decimal orderAmount)
    {
        return orderAmount * 0.05m; // 5% discount
    }
}

public class VIPCustomer : Customer
{
    public override decimal CalculateDiscount(decimal orderAmount)
    {
        return orderAmount * 0.10m; // 10% discount
    }
}

public class CorporateCustomer : Customer
{
    public int EmployeeCount { get; set; }

    public override decimal CalculateDiscount(decimal orderAmount)
    {
        var baseDiscount = orderAmount * 0.15m; // 15% base discount
        var volumeDiscount = EmployeeCount > 100 ? orderAmount * 0.05m : 0;
        return baseDiscount + volumeDiscount;
    }
}

// Easy to add new customer types
public class GovernmentCustomer : Customer
{
    public override decimal CalculateDiscount(decimal orderAmount)
    {
        return orderAmount * 0.20m; // 20% government discount
    }
}

// Calculator doesn't need to change
public class OrderCalculator
{
    public decimal CalculateFinalAmount(Customer customer, decimal orderAmount)
    {
        var discount = customer.CalculateDiscount(orderAmount);
        return orderAmount - discount;
    }
}
```

## 📊 More Advanced Example: Report Generation

```csharp
// Base interface
public interface IReportGenerator
{
    void GenerateReport(ReportData data);
}

// Concrete implementations
public class PDFReportGenerator : IReportGenerator
{
    public void GenerateReport(ReportData data)
    {
        // Generate PDF report
        Console.WriteLine("Generating PDF report...");
    }
}

public class ExcelReportGenerator : IReportGenerator
{
    public void GenerateReport(ReportData data)
    {
        // Generate Excel report
        Console.WriteLine("Generating Excel report...");
    }
}

public class HTMLReportGenerator : IReportGenerator
{
    public void GenerateReport(ReportData data)
    {
        // Generate HTML report
        Console.WriteLine("Generating HTML report...");
    }
}

// Easy to add new report types without changing existing code
public class PowerBIReportGenerator : IReportGenerator
{
    public void GenerateReport(ReportData data)
    {
        // Generate Power BI report
        Console.WriteLine("Generating Power BI report...");
    }
}

// Report service doesn't need to change
public class ReportService
{
    private readonly IEnumerable<IReportGenerator> _generators;

    public ReportService(IEnumerable<IReportGenerator> generators)
    {
        _generators = generators;
    }

    public void GenerateAllReports(ReportData data)
    {
        foreach (var generator in _generators)
        {
            generator.GenerateReport(data);
        }
    }
}
```

## ✅ Benefits

1. **Maintainability**: Existing code remains untouched when adding features
2. **Stability**: Less risk of introducing bugs in working code
3. **Testability**: New functionality can be tested independently
4. **Flexibility**: Easy to add new behaviors without affecting existing ones
5. **Reusability**: Extensions can be used in different contexts

## 🧪 Unit Testing

```csharp
[Test]
public void PermanentEmployee_Should_Calculate_Salary_With_Bonus()
{
    // Arrange
    var employee = new PermanentEmployee { BaseSalary = 1000m };

    // Act
    var salary = employee.CalculateSalary();

    // Assert
    Assert.AreEqual(1100m, salary); // 1000 + 10% bonus
}

[Test]
public void FreelancerEmployee_Should_Calculate_Salary_Based_On_Hours()
{
    // Arrange
    var employee = new FreelancerEmployee 
    { 
        HourlyRate = 50m, 
        HoursWorked = 160 
    };

    // Act
    var salary = employee.CalculateSalary();

    // Assert
    Assert.AreEqual(8000m, salary); // 50 * 160
}
```

## 🎯 Common Implementation Techniques

### 1. Inheritance (Template Method Pattern)
```csharp
public abstract class PaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        ValidateAmount(amount);
        var success = ExecutePayment(amount);
        LogTransaction(amount, success);
    }

    protected abstract bool ExecutePayment(decimal amount);
    private void ValidateAmount(decimal amount) { /* common logic */ }
    private void LogTransaction(decimal amount, bool success) { /* common logic */ }
}
```

### 2. Composition (Strategy Pattern)
```csharp
public class ShippingCalculator
{
    private readonly IShippingStrategy _strategy;

    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal CalculateShipping(Package package)
    {
        return _strategy.CalculateCost(package);
    }
}
```

### 3. Plugins/Extensions
```csharp
public interface IOrderProcessor
{
    void ProcessOrder(Order order);
}

public class OrderProcessingService
{
    private readonly IEnumerable<IOrderProcessor> _processors;

    public OrderProcessingService(IEnumerable<IOrderProcessor> processors)
    {
        _processors = processors;
    }

    public void ProcessOrder(Order order)
    {
        foreach (var processor in _processors)
        {
            processor.ProcessOrder(order);
        }
    }
}
```

## 🚨 Common Mistakes

1. **Overengineering**: Creating abstractions for things that won't change
2. **Premature abstraction**: Adding extension points before they're needed
3. **Wrong abstraction**: Creating abstractions that don't match future requirements
4. **Interface pollution**: Too many small interfaces

## 🎯 Interview Questions

**Q: When should you apply the Open/Closed Principle?**
**A:** When you anticipate that a class will need to be extended with new behaviors, especially when those behaviors follow a similar pattern (like different calculation methods, file formats, or processing strategies).

**Q: What's the difference between OCP and adding new methods to existing classes?**
**A:** OCP focuses on extending behavior through inheritance or composition without modifying existing code. Adding new methods to existing classes can break existing clients and violates the principle.

**Q: How does OCP relate to polymorphism?**
**A:** Polymorphism is the key mechanism that enables OCP. It allows different implementations to be used interchangeably through a common interface or base class.

**Q: Can you give an ERP example where OCP is critical?**
**A:** In a tax calculation system, different regions have different tax rules. Instead of modifying the tax calculator for each region, create ITaxCalculator implementations for each region (USTaxCalculator, EUTaxCalculator, etc.).

## 📝 Checklist

- [ ] New functionality can be added without modifying existing code
- [ ] Use abstraction (interfaces/abstract classes) to define contracts
- [ ] Implementations extend behavior through inheritance or composition
- [ ] Existing code remains stable when adding new features
- [ ] Changes are additive, not modifications
- [ ] Plugin-like architecture where appropriate

---

**Previous**: [← Single Responsibility Principle](./01-single-responsibility.md) | **Next**: [Liskov Substitution Principle →](./03-liskov-substitution.md)