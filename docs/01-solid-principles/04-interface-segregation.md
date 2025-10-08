# Interface Segregation Principle (ISP)

> **"No client should be forced to depend on methods it does not use."** - Robert C. Martin

## 🎯 Definition

The Interface Segregation Principle states that clients should not be forced to depend upon interfaces that they do not use. It's better to have many specific interfaces than one general-purpose interface.

## 🏠 Real-World Analogy

Think of **TV remote controls**:

- ❌ **One massive remote** with every possible button for all devices (TV, DVD, Cable, Sound System) - confusing and overwhelming
- ✅ **Separate remotes** for each device - each remote only has the buttons you need for that specific device
- Each device gets exactly what it needs, nothing more

## 🚫 Violation Example

```csharp
// ❌ BAD: Fat interface that forces classes to implement methods they don't need

public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void TakeBreak();
    void AttendMeeting();
    void WriteCode();
    void TestSoftware();
    void ManageTeam();
    void ReviewCode();
    void CreateDocumentation();
}

// Robot worker doesn't need eat, sleep, take breaks
public class RobotWorker : IWorker
{
    public void Work()
    {
        Console.WriteLine("Robot working 24/7...");
    }

    public void WriteCode()
    {
        Console.WriteLine("Robot generating code...");
    }

    public void TestSoftware()
    {
        Console.WriteLine("Robot running automated tests...");
    }

    // Forced to implement methods it doesn't need
    public void Eat()
    {
        throw new NotSupportedException("Robots don't eat!"); // ISP Violation!
    }

    public void Sleep()
    {
        throw new NotSupportedException("Robots don't sleep!"); // ISP Violation!
    }

    public void TakeBreak()
    {
        throw new NotSupportedException("Robots don't take breaks!"); // ISP Violation!
    }

    public void AttendMeeting()
    {
        throw new NotSupportedException("Robots don't attend meetings!");
    }

    public void ManageTeam()
    {
        throw new NotSupportedException("Robots don't manage teams!");
    }

    public void ReviewCode()
    {
        throw new NotSupportedException("Robots don't review code!");
    }

    public void CreateDocumentation()
    {
        throw new NotSupportedException("Robots don't create documentation!");
    }
}

// Human developer doesn't need all management functions
public class DeveloperWorker : IWorker
{
    public void Work() { Console.WriteLine("Developer working..."); }
    public void Eat() { Console.WriteLine("Developer eating lunch..."); }
    public void Sleep() { Console.WriteLine("Developer sleeping..."); }
    public void TakeBreak() { Console.WriteLine("Developer taking a break..."); }
    public void AttendMeeting() { Console.WriteLine("Developer attending meeting..."); }
    public void WriteCode() { Console.WriteLine("Developer writing code..."); }
    public void TestSoftware() { Console.WriteLine("Developer testing software..."); }
    public void ReviewCode() { Console.WriteLine("Developer reviewing code..."); }
    public void CreateDocumentation() { Console.WriteLine("Developer creating docs..."); }

    // Forced to implement methods they shouldn't have
    public void ManageTeam()
    {
        throw new NotSupportedException("Regular developers don't manage teams!");
    }
}
```

### Problems with this approach:

1. **Classes implement methods they don't need** leading to empty implementations or exceptions
2. **Changes to the interface affect all implementers** even if they don't use changed methods
3. **Difficult to understand** what each class actually does
4. **Violation of Single Responsibility** - interface has too many responsibilities

## ✅ Correct Implementation

```csharp
// ✅ GOOD: Segregated interfaces - each interface has a specific purpose

// Core work interface - everyone can work
public interface IWorkable
{
    void Work();
}

// Human-specific needs
public interface IHumanNeeds
{
    void Eat();
    void Sleep();
    void TakeBreak();
}

// Meeting participant
public interface IMeetingParticipant
{
    void AttendMeeting();
}

// Development skills
public interface IDeveloper
{
    void WriteCode();
    void TestSoftware();
    void ReviewCode();
    void CreateDocumentation();
}

// Management skills
public interface IManager
{
    void ManageTeam();
    void PlanProject();
    void ConductPerformanceReview();
}

// Automated worker capabilities
public interface IAutomatedWorker
{
    void RunContinuously();
    void GenerateReports();
}

// Now implementations only implement what they need
public class RobotWorker : IWorkable, IDeveloper, IAutomatedWorker
{
    public void Work()
    {
        Console.WriteLine("Robot working 24/7...");
    }

    public void WriteCode()
    {
        Console.WriteLine("Robot generating code...");
    }

    public void TestSoftware()
    {
        Console.WriteLine("Robot running automated tests...");
    }

    public void ReviewCode()
    {
        Console.WriteLine("Robot performing static code analysis...");
    }

    public void CreateDocumentation()
    {
        Console.WriteLine("Robot generating API documentation...");
    }

    public void RunContinuously()
    {
        Console.WriteLine("Robot running without breaks...");
    }

    public void GenerateReports()
    {
        Console.WriteLine("Robot generating automated reports...");
    }
}

public class DeveloperWorker : IWorkable, IHumanNeeds, IMeetingParticipant, IDeveloper
{
    public void Work()
    {
        Console.WriteLine("Developer working on tasks...");
    }

    public void Eat()
    {
        Console.WriteLine("Developer eating lunch...");
    }

    public void Sleep()
    {
        Console.WriteLine("Developer sleeping at night...");
    }

    public void TakeBreak()
    {
        Console.WriteLine("Developer taking a coffee break...");
    }

    public void AttendMeeting()
    {
        Console.WriteLine("Developer attending team meeting...");
    }

    public void WriteCode()
    {
        Console.WriteLine("Developer writing clean code...");
    }

    public void TestSoftware()
    {
        Console.WriteLine("Developer writing unit tests...");
    }

    public void ReviewCode()
    {
        Console.WriteLine("Developer reviewing pull requests...");
    }

    public void CreateDocumentation()
    {
        Console.WriteLine("Developer writing technical docs...");
    }
}

public class TeamLead : IWorkable, IHumanNeeds, IMeetingParticipant, IDeveloper, IManager
{
    public void Work() { Console.WriteLine("Team lead coordinating work..."); }
    public void Eat() { Console.WriteLine("Team lead eating..."); }
    public void Sleep() { Console.WriteLine("Team lead sleeping..."); }
    public void TakeBreak() { Console.WriteLine("Team lead taking break..."); }
    public void AttendMeeting() { Console.WriteLine("Team lead attending meetings..."); }
    public void WriteCode() { Console.WriteLine("Team lead coding when needed..."); }
    public void TestSoftware() { Console.WriteLine("Team lead testing..."); }
    public void ReviewCode() { Console.WriteLine("Team lead reviewing code..."); }
    public void CreateDocumentation() { Console.WriteLine("Team lead creating docs..."); }
    public void ManageTeam() { Console.WriteLine("Team lead managing team..."); }
    public void PlanProject() { Console.WriteLine("Team lead planning projects..."); }
    public void ConductPerformanceReview() { Console.WriteLine("Team lead conducting reviews..."); }
}
```

## 🏢 ERP Example: Order Management

### ❌ Violation

```csharp
// ❌ BAD: Fat interface for all order operations
public interface IOrderService
{
    // Order creation
    void CreateOrder(Order order);
    void ValidateOrder(Order order);
    
    // Payment processing
    void ProcessPayment(Order order, PaymentInfo payment);
    void RefundPayment(Order order);
    
    // Inventory management
    void ReserveInventory(Order order);
    void ReleaseInventory(Order order);
    
    // Shipping
    void CalculateShipping(Order order);
    void ArrangeShipping(Order order);
    void TrackShipment(Order order);
    
    // Reporting
    void GenerateInvoice(Order order);
    void GenerateShippingLabel(Order order);
    void GenerateOrderReport(Order order);
    
    // Customer service
    void SendOrderConfirmation(Order order);
    void SendShippingNotification(Order order);
    void HandleCustomerInquiry(Order order, string inquiry);
}

// Order validator only needs validation methods but forced to implement everything
public class OrderValidator : IOrderService
{
    public void ValidateOrder(Order order)
    {
        // Actual validation logic
    }

    public void CreateOrder(Order order) => throw new NotSupportedException();
    public void ProcessPayment(Order order, PaymentInfo payment) => throw new NotSupportedException();
    public void RefundPayment(Order order) => throw new NotSupportedException();
    public void ReserveInventory(Order order) => throw new NotSupportedException();
    public void ReleaseInventory(Order order) => throw new NotSupportedException();
    public void CalculateShipping(Order order) => throw new NotSupportedException();
    public void ArrangeShipping(Order order) => throw new NotSupportedException();
    public void TrackShipment(Order order) => throw new NotSupportedException();
    public void GenerateInvoice(Order order) => throw new NotSupportedException();
    public void GenerateShippingLabel(Order order) => throw new NotSupportedException();
    public void GenerateOrderReport(Order order) => throw new NotSupportedException();
    public void SendOrderConfirmation(Order order) => throw new NotSupportedException();
    public void SendShippingNotification(Order order) => throw new NotSupportedException();
    public void HandleCustomerInquiry(Order order, string inquiry) => throw new NotSupportedException();
}
```

### ✅ Correct Implementation

```csharp
// ✅ GOOD: Segregated interfaces for different responsibilities

public interface IOrderCreator
{
    void CreateOrder(Order order);
}

public interface IOrderValidator
{
    bool ValidateOrder(Order order);
    IEnumerable<string> GetValidationErrors(Order order);
}

public interface IPaymentProcessor
{
    bool ProcessPayment(Order order, PaymentInfo payment);
    void RefundPayment(Order order);
}

public interface IInventoryManager
{
    bool ReserveInventory(Order order);
    void ReleaseInventory(Order order);
    bool IsInventoryAvailable(Order order);
}

public interface IShippingService
{
    decimal CalculateShipping(Order order);
    void ArrangeShipping(Order order);
    string TrackShipment(Order order);
}

public interface IOrderReportGenerator
{
    Invoice GenerateInvoice(Order order);
    ShippingLabel GenerateShippingLabel(Order order);
    OrderReport GenerateOrderReport(Order order);
}

public interface IOrderNotificationService
{
    void SendOrderConfirmation(Order order);
    void SendShippingNotification(Order order);
    void SendDeliveryConfirmation(Order order);
}

public interface ICustomerService
{
    void HandleCustomerInquiry(Order order, string inquiry);
    void ProcessReturn(Order order);
}

// Now each implementation only implements what it needs
public class OrderValidator : IOrderValidator
{
    public bool ValidateOrder(Order order)
    {
        return GetValidationErrors(order).Any() == false;
    }

    public IEnumerable<string> GetValidationErrors(Order order)
    {
        var errors = new List<string>();
        
        if (order.Items.Count == 0)
            errors.Add("Order must have at least one item");
            
        if (order.Total <= 0)
            errors.Add("Order total must be greater than zero");
            
        return errors;
    }
}

public class PaymentProcessor : IPaymentProcessor
{
    public bool ProcessPayment(Order order, PaymentInfo payment)
    {
        // Payment processing logic
        Console.WriteLine($"Processing payment of ${order.Total}");
        return true;
    }

    public void RefundPayment(Order order)
    {
        Console.WriteLine($"Refunding ${order.Total}");
    }
}

public class InventoryManager : IInventoryManager
{
    public bool ReserveInventory(Order order)
    {
        Console.WriteLine("Reserving inventory for order");
        return true;
    }

    public void ReleaseInventory(Order order)
    {
        Console.WriteLine("Releasing reserved inventory");
    }

    public bool IsInventoryAvailable(Order order)
    {
        return true; // Simplified
    }
}

// Orchestrator that uses multiple segregated interfaces
public class OrderProcessingService
{
    private readonly IOrderValidator _validator;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IInventoryManager _inventoryManager;
    private readonly IOrderNotificationService _notificationService;

    public OrderProcessingService(
        IOrderValidator validator,
        IPaymentProcessor paymentProcessor,
        IInventoryManager inventoryManager,
        IOrderNotificationService notificationService)
    {
        _validator = validator;
        _paymentProcessor = paymentProcessor;
        _inventoryManager = inventoryManager;
        _notificationService = notificationService;
    }

    public bool ProcessOrder(Order order, PaymentInfo payment)
    {
        if (!_validator.ValidateOrder(order))
            return false;

        if (!_inventoryManager.ReserveInventory(order))
            return false;

        if (!_paymentProcessor.ProcessPayment(order, payment))
        {
            _inventoryManager.ReleaseInventory(order);
            return false;
        }

        _notificationService.SendOrderConfirmation(order);
        return true;
    }
}
```

## 📧 Email Service Example

### ❌ Violation

```csharp
public interface IEmailService
{
    // Basic email
    void SendEmail(string to, string subject, string body);
    
    // HTML email
    void SendHtmlEmail(string to, string subject, string htmlBody);
    
    // With attachments
    void SendEmailWithAttachments(string to, string subject, string body, List<string> attachments);
    
    // Bulk email
    void SendBulkEmail(List<string> recipients, string subject, string body);
    
    // Template email
    void SendTemplateEmail(string to, string template, Dictionary<string, string> parameters);
    
    // Marketing email
    void SendMarketingEmail(string to, string campaignId, string content);
    void TrackEmailOpen(string emailId);
    void TrackEmailClick(string emailId, string linkId);
    
    // SMS (why is this in email service?)
    void SendSMS(string phoneNumber, string message);
}
```

### ✅ Correct Implementation

```csharp
// Basic email sending
public interface IEmailSender
{
    void SendEmail(string to, string subject, string body);
    void SendHtmlEmail(string to, string subject, string htmlBody);
}

// Email with attachments
public interface IAttachmentEmailSender : IEmailSender
{
    void SendEmailWithAttachments(string to, string subject, string body, IEnumerable<string> attachments);
}

// Bulk email capability
public interface IBulkEmailSender
{
    void SendBulkEmail(IEnumerable<string> recipients, string subject, string body);
}

// Template-based email
public interface ITemplateEmailSender
{
    void SendTemplateEmail(string to, string template, Dictionary<string, string> parameters);
}

// Marketing-specific features
public interface IMarketingEmailSender
{
    void SendMarketingEmail(string to, string campaignId, string content);
}

public interface IEmailTracker
{
    void TrackEmailOpen(string emailId);
    void TrackEmailClick(string emailId, string linkId);
}

// SMS is separate service
public interface ISMSService
{
    void SendSMS(string phoneNumber, string message);
}

// Implementations
public class BasicEmailService : IEmailSender
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"Sending email to {to}: {subject}");
    }

    public void SendHtmlEmail(string to, string subject, string htmlBody)
    {
        Console.WriteLine($"Sending HTML email to {to}: {subject}");
    }
}

public class MarketingEmailService : IMarketingEmailSender, IEmailTracker, IBulkEmailSender
{
    public void SendMarketingEmail(string to, string campaignId, string content)
    {
        Console.WriteLine($"Sending marketing email for campaign {campaignId} to {to}");
    }

    public void TrackEmailOpen(string emailId)
    {
        Console.WriteLine($"Email {emailId} was opened");
    }

    public void TrackEmailClick(string emailId, string linkId)
    {
        Console.WriteLine($"Link {linkId} clicked in email {emailId}");
    }

    public void SendBulkEmail(IEnumerable<string> recipients, string subject, string body)
    {
        Console.WriteLine($"Sending bulk email to {recipients.Count()} recipients");
    }
}
```

## ✅ Benefits

1. **Focused interfaces**: Each interface has a clear, single purpose
2. **Reduced coupling**: Classes only depend on methods they actually use
3. **Easier testing**: Mock only the interfaces you need
4. **Better maintainability**: Changes to one interface don't affect unrelated implementations
5. **Clearer design**: Interface names clearly indicate their purpose

## 🧪 Unit Testing

```csharp
[Test]
public void OrderValidator_Should_Only_Need_Validation_Interface()
{
    // Arrange - Only need to mock what the validator actually uses
    var validator = new OrderValidator();
    var order = new Order
    {
        Items = new List<OrderItem> { new OrderItem { Name = "Test", Price = 10 } },
        Total = 10
    };

    // Act
    var isValid = validator.ValidateOrder(order);

    // Assert
    Assert.IsTrue(isValid);
    // No need to mock payment, shipping, etc. - validator doesn't depend on them
}

[Test]
public void PaymentProcessor_Should_Only_Need_Payment_Interface()
{
    // Arrange
    var processor = new PaymentProcessor();
    var order = new Order { Total = 100 };
    var payment = new PaymentInfo { CardNumber = "1234", Amount = 100 };

    // Act
    var result = processor.ProcessPayment(order, payment);

    // Assert
    Assert.IsTrue(result);
    // PaymentProcessor doesn't depend on validation, shipping, etc.
}
```

## 🎯 ISP Design Guidelines

### 1. Keep interfaces small and focused

```csharp
// ✅ GOOD: Small, focused interfaces
public interface IReadable
{
    string Read();
}

public interface IWritable
{
    void Write(string content);
}

// Combine when you need both
public interface IReadWritable : IReadable, IWritable
{
}
```

### 2. Use composition over large interfaces

```csharp
// ✅ GOOD: Compose multiple small interfaces
public class FileManager
{
    private readonly IReadable _reader;
    private readonly IWritable _writer;

    public FileManager(IReadable reader, IWritable writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public void CopyContent()
    {
        var content = _reader.Read();
        _writer.Write(content);
    }
}
```

### 3. Role-based interfaces

```csharp
// ✅ GOOD: Interfaces based on roles/capabilities
public interface ICanApproveOrders
{
    bool ApproveOrder(Order order);
}

public interface ICanCancelOrders
{
    bool CancelOrder(Order order);
}

public interface ICanViewOrders
{
    IEnumerable<Order> GetOrders();
}

// Different roles implement different capabilities
public class Manager : ICanApproveOrders, ICanCancelOrders, ICanViewOrders
{
    // Manager can do everything
}

public class Employee : ICanViewOrders
{
    // Employee can only view
}
```

## 🚨 Common ISP Violations

1. **God interfaces** with too many methods
2. **Mixed concerns** in a single interface
3. **Optional methods** that throw NotSupportedException
4. **All-in-one interfaces** that try to cover every scenario
5. **Parameter interfaces** with many optional parameters

## 🎯 Interview Questions

**Q: How do you identify ISP violations?**
**A:** Look for:
- Interfaces with many methods (more than 5-7 is often too many)
- Classes implementing interfaces but throwing NotSupportedException
- Interface names that contain "And" or "Or"
- Classes that only use a subset of interface methods

**Q: What's the relationship between ISP and Single Responsibility Principle?**
**A:** ISP applies SRP to interfaces - each interface should have a single responsibility. If an interface has multiple responsibilities, it should be split into smaller, focused interfaces.

**Q: How does ISP help with testing?**
**A:** With segregated interfaces, you only need to mock the specific interfaces your class depends on, making tests simpler and more focused.

**Q: Can you give an ERP example where ISP is crucial?**
**A:** In a user management system, instead of one IUser interface with login, profile, permissions, and billing methods, create separate interfaces: IAuthenticatable, IProfileManageable, IPermissionHolder, and IBillable. Different parts of the system only depend on what they need.

## 📝 Checklist

- [ ] Interfaces are small and focused on a single concern
- [ ] No class is forced to implement methods it doesn't use
- [ ] Interface names clearly indicate their specific purpose
- [ ] Clients only depend on methods they actually call
- [ ] No methods throw NotSupportedException in implementations
- [ ] Related interfaces can be composed when needed
- [ ] Changes to one interface don't affect unrelated implementations

---

**Previous**: [← Liskov Substitution Principle](./03-liskov-substitution.md) | **Next**: [Dependency Inversion Principle →](./05-dependency-inversion.md)