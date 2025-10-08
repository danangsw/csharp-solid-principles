# Dependency Inversion Principle (DIP)

> **"High-level modules should not depend on low-level modules. Both should depend on abstractions. Abstractions should not depend on details. Details should depend on abstractions."** - Robert C. Martin

## 🎯 Definition

The Dependency Inversion Principle states that:
1. **High-level modules** should not depend on **low-level modules**. Both should depend on **abstractions**.
2. **Abstractions** should not depend on **details**. **Details** should depend on **abstractions**.

## 🏠 Real-World Analogy

Think of **electrical appliances and wall outlets**:

- ✅ **Appliances** (high-level) don't depend on specific power generation (low-level)
- ✅ Both depend on **electrical standards** (abstraction) - voltage, frequency, plug shape
- ✅ Power plants (details) conform to the standard, not the other way around
- You can plug any compliant device into any compliant outlet

## 🚫 Violation Example

```csharp
// ❌ BAD: High-level class directly depends on low-level implementations

// Low-level modules (concrete implementations)
public class EmailSender
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"Sending email to {to}: {subject}");
        // SMTP email sending logic
    }
}

public class SMSSender
{
    public void SendSMS(string phoneNumber, string message)
    {
        Console.WriteLine($"Sending SMS to {phoneNumber}: {message}");
        // SMS API calls
    }
}

public class SqlDatabase
{
    public void SaveUser(string name, string email)
    {
        Console.WriteLine($"Saving user {name} to SQL database");
        // SQL Server specific code
    }

    public string GetUser(int id)
    {
        Console.WriteLine($"Getting user {id} from SQL database");
        return "John Doe"; // Simplified
    }
}

// High-level module directly depends on low-level modules
public class UserService
{
    private readonly EmailSender _emailSender;
    private readonly SqlDatabase _database;

    public UserService()
    {
        _emailSender = new EmailSender(); // Tight coupling!
        _database = new SqlDatabase();     // Tight coupling!
    }

    public void CreateUser(string name, string email)
    {
        // Business logic
        _database.SaveUser(name, email);
        _emailSender.SendEmail(email, "Welcome", $"Welcome {name}!");
    }

    public void NotifyUser(int userId, string message)
    {
        var user = _database.GetUser(userId);
        _emailSender.SendEmail("user@email.com", "Notification", message);
    }
}
```

### Problems with this approach:

1. **UserService is tightly coupled** to specific implementations
2. **Hard to test** - can't mock EmailSender or SqlDatabase
3. **Hard to change** - switching from SQL to NoSQL requires changing UserService
4. **Violates Open/Closed** - must modify UserService to add new notification methods
5. **No flexibility** - can't configure different implementations for different environments

## ✅ Correct Implementation

```csharp
// ✅ GOOD: Depend on abstractions, not concretions

// Abstractions (interfaces)
public interface INotificationService
{
    void SendNotification(string recipient, string message);
}

public interface IUserRepository
{
    void SaveUser(User user);
    User GetUser(int id);
    User GetUserByEmail(string email);
    void UpdateUser(User user);
    void DeleteUser(int id);
}

public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message, Exception exception = null);
}

// Domain entity
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedDate { get; set; }
}

// Low-level modules (implementations) depend on abstractions
public class EmailNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public EmailNotificationService(ILogger logger)
    {
        _logger = logger;
    }

    public void SendNotification(string recipient, string message)
    {
        try
        {
            Console.WriteLine($"Sending email to {recipient}: {message}");
            // SMTP email sending logic
            _logger.LogInfo($"Email sent successfully to {recipient}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send email to {recipient}", ex);
            throw;
        }
    }
}

public class SMSNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public SMSNotificationService(ILogger logger)
    {
        _logger = logger;
    }

    public void SendNotification(string recipient, string message)
    {
        try
        {
            Console.WriteLine($"Sending SMS to {recipient}: {message}");
            // SMS API calls
            _logger.LogInfo($"SMS sent successfully to {recipient}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send SMS to {recipient}", ex);
            throw;
        }
    }
}

public class SlackNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public SlackNotificationService(ILogger logger)
    {
        _logger = logger;
    }

    public void SendNotification(string recipient, string message)
    {
        try
        {
            Console.WriteLine($"Sending Slack message to {recipient}: {message}");
            // Slack API calls
            _logger.LogInfo($"Slack message sent successfully to {recipient}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send Slack message to {recipient}", ex);
            throw;
        }
    }
}

public class SqlUserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public SqlUserRepository(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void SaveUser(User user)
    {
        Console.WriteLine($"Saving user {user.Name} to SQL database");
        _logger.LogInfo($"User {user.Name} saved successfully");
    }

    public User GetUser(int id)
    {
        Console.WriteLine($"Getting user {id} from SQL database");
        return new User { Id = id, Name = "John Doe", Email = "john@example.com" };
    }

    public User GetUserByEmail(string email)
    {
        Console.WriteLine($"Getting user by email {email} from SQL database");
        return new User { Id = 1, Name = "John Doe", Email = email };
    }

    public void UpdateUser(User user)
    {
        Console.WriteLine($"Updating user {user.Id} in SQL database");
    }

    public void DeleteUser(int id)
    {
        Console.WriteLine($"Deleting user {id} from SQL database");
    }
}

public class NoSqlUserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public NoSqlUserRepository(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void SaveUser(User user)
    {
        Console.WriteLine($"Saving user {user.Name} to NoSQL database");
        _logger.LogInfo($"User {user.Name} saved to NoSQL successfully");
    }

    public User GetUser(int id)
    {
        Console.WriteLine($"Getting user {id} from NoSQL database");
        return new User { Id = id, Name = "Jane Doe", Email = "jane@example.com" };
    }

    public User GetUserByEmail(string email)
    {
        Console.WriteLine($"Getting user by email {email} from NoSQL database");
        return new User { Id = 2, Name = "Jane Doe", Email = email };
    }

    public void UpdateUser(User user)
    {
        Console.WriteLine($"Updating user {user.Id} in NoSQL database");
    }

    public void DeleteUser(int id)
    {
        Console.WriteLine($"Deleting user {id} from NoSQL database");
    }
}

public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
    }

    public void LogError(string message, Exception exception = null)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
        }
    }
}

// High-level module depends only on abstractions
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    // Dependency injection through constructor
    public UserService(
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger logger)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public void CreateUser(string name, string email)
    {
        try
        {
            _logger.LogInfo($"Creating user: {name}");
            
            var user = new User
            {
                Name = name,
                Email = email,
                CreatedDate = DateTime.Now
            };

            _userRepository.SaveUser(user);
            _notificationService.SendNotification(email, $"Welcome {name}! Your account has been created.");
            
            _logger.LogInfo($"User {name} created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create user {name}", ex);
            throw;
        }
    }

    public void NotifyUser(int userId, string message)
    {
        try
        {
            var user = _userRepository.GetUser(userId);
            _notificationService.SendNotification(user.Email, message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to notify user {userId}", ex);
            throw;
        }
    }

    public User GetUser(int id)
    {
        return _userRepository.GetUser(id);
    }
}
```

## 🏗️ Dependency Injection Configuration

```csharp
// Composition root - where dependencies are wired up
public class ServiceRegistry
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register dependencies
        services.AddSingleton<ILogger, ConsoleLogger>();
        
        // Can easily switch implementations
        services.AddScoped<IUserRepository, SqlUserRepository>(provider =>
            new SqlUserRepository("Server=localhost;Database=Users", provider.GetService<ILogger>()));
        
        // Or use NoSQL instead
        // services.AddScoped<IUserRepository, NoSqlUserRepository>(provider =>
        //     new NoSqlUserRepository("mongodb://localhost:27017", provider.GetService<ILogger>()));

        services.AddScoped<INotificationService, EmailNotificationService>();
        
        // Register the high-level service
        services.AddScoped<UserService>();

        return services.BuildServiceProvider();
    }
}

// Usage
public class Program
{
    public static void Main()
    {
        var serviceProvider = ServiceRegistry.ConfigureServices();
        var userService = serviceProvider.GetService<UserService>();

        userService.CreateUser("John Doe", "john@example.com");
        userService.NotifyUser(1, "Your profile has been updated.");
    }
}
```

## 🏢 ERP Example: Order Processing

### ❌ Violation

```csharp
// ❌ BAD: Order service tightly coupled to implementations
public class OrderProcessingService
{
    private readonly SqlOrderRepository _orderRepository;
    private readonly EmailService _emailService;
    private readonly StripePaymentProcessor _paymentProcessor;
    private readonly FedExShippingService _shippingService;

    public OrderProcessingService()
    {
        _orderRepository = new SqlOrderRepository();
        _emailService = new EmailService();
        _paymentProcessor = new StripePaymentProcessor();
        _shippingService = new FedExShippingService();
    }

    public void ProcessOrder(Order order)
    {
        _orderRepository.SaveOrder(order);
        _paymentProcessor.ProcessPayment(order.PaymentInfo);
        _shippingService.ArrangeShipping(order);
        _emailService.SendOrderConfirmation(order.CustomerEmail);
    }
}
```

### ✅ Correct Implementation

```csharp
// ✅ GOOD: Depend on abstractions
public interface IOrderRepository
{
    void SaveOrder(Order order);
    Order GetOrder(int orderId);
    void UpdateOrderStatus(int orderId, OrderStatus status);
}

public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(PaymentInfo paymentInfo);
    void RefundPayment(string transactionId);
}

public interface IShippingService
{
    string ArrangeShipping(Order order);
    decimal CalculateShipping(Order order);
    TrackingInfo GetTrackingInfo(string shipmentId);
}

public interface INotificationService
{
    void SendOrderConfirmation(string email, Order order);
    void SendShippingNotification(string email, string trackingNumber);
}

public class OrderProcessingService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IShippingService _shippingService;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public OrderProcessingService(
        IOrderRepository orderRepository,
        IPaymentProcessor paymentProcessor,
        IShippingService shippingService,
        INotificationService notificationService,
        ILogger logger)
    {
        _orderRepository = orderRepository;
        _paymentProcessor = paymentProcessor;
        _shippingService = shippingService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> ProcessOrderAsync(Order order)
    {
        try
        {
            _logger.LogInfo($"Processing order {order.Id}");

            // Save order
            _orderRepository.SaveOrder(order);
            _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Processing);

            // Process payment
            var paymentResult = _paymentProcessor.ProcessPayment(order.PaymentInfo);
            if (!paymentResult.IsSuccessful)
            {
                _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.PaymentFailed);
                return false;
            }

            // Arrange shipping
            var shipmentId = _shippingService.ArrangeShipping(order);
            _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Shipped);

            // Send notifications
            _notificationService.SendOrderConfirmation(order.CustomerEmail, order);
            _notificationService.SendShippingNotification(order.CustomerEmail, shipmentId);

            _logger.LogInfo($"Order {order.Id} processed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to process order {order.Id}", ex);
            _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Failed);
            return false;
        }
    }
}

// Easy to swap implementations
public class StripePaymentProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(PaymentInfo paymentInfo)
    {
        // Stripe-specific implementation
        return new PaymentResult { IsSuccessful = true, TransactionId = "stripe_123" };
    }

    public void RefundPayment(string transactionId)
    {
        // Stripe refund logic
    }
}

public class PayPalPaymentProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(PaymentInfo paymentInfo)
    {
        // PayPal-specific implementation
        return new PaymentResult { IsSuccessful = true, TransactionId = "paypal_456" };
    }

    public void RefundPayment(string transactionId)
    {
        // PayPal refund logic
    }
}
```

## 🧪 Unit Testing with DIP

```csharp
// Testing is much easier with dependency injection
[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockRepository;
    private Mock<INotificationService> _mockNotificationService;
    private Mock<ILogger> _mockLogger;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockLogger = new Mock<ILogger>();
        
        _userService = new UserService(
            _mockRepository.Object,
            _mockNotificationService.Object,
            _mockLogger.Object);
    }

    [Test]
    public void CreateUser_Should_Save_User_And_Send_Notification()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";

        // Act
        _userService.CreateUser(name, email);

        // Assert
        _mockRepository.Verify(r => r.SaveUser(It.Is<User>(u => u.Name == name && u.Email == email)), Times.Once);
        _mockNotificationService.Verify(n => n.SendNotification(email, It.IsAny<string>()), Times.Once);
        _mockLogger.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Test]
    public void CreateUser_Should_Log_Error_When_Repository_Fails()
    {
        // Arrange
        _mockRepository.Setup(r => r.SaveUser(It.IsAny<User>())).Throws(new Exception("Database error"));

        // Act & Assert
        Assert.Throws<Exception>(() => _userService.CreateUser("John", "john@example.com"));
        _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }
}
```

## 🔧 DIP Implementation Techniques

### 1. Constructor Injection (Recommended)

```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPaymentService _paymentService;

    public OrderService(IOrderRepository repository, IPaymentService paymentService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }
}
```

### 2. Property Injection

```csharp
public class OrderService
{
    public IOrderRepository Repository { get; set; }
    public IPaymentService PaymentService { get; set; }

    public void ProcessOrder(Order order)
    {
        if (Repository == null) throw new InvalidOperationException("Repository not set");
        if (PaymentService == null) throw new InvalidOperationException("PaymentService not set");
        
        // Process order
    }
}
```

### 3. Method Injection

```csharp
public class OrderService
{
    public void ProcessOrder(Order order, IPaymentService paymentService, IOrderRepository repository)
    {
        // Dependencies passed as method parameters
        repository.SaveOrder(order);
        paymentService.ProcessPayment(order.PaymentInfo);
    }
}
```

### 4. Service Locator (Anti-pattern - avoid)

```csharp
// ❌ BAD: Service Locator pattern (violates DIP)
public class OrderService
{
    public void ProcessOrder(Order order)
    {
        var repository = ServiceLocator.GetService<IOrderRepository>(); // Hidden dependency
        var paymentService = ServiceLocator.GetService<IPaymentService>(); // Not testable
        
        repository.SaveOrder(order);
        paymentService.ProcessPayment(order.PaymentInfo);
    }
}
```

## ✅ Benefits

1. **Testability**: Easy to mock dependencies for unit testing
2. **Flexibility**: Easy to swap implementations without changing high-level code
3. **Maintainability**: Changes to low-level modules don't affect high-level modules
4. **Loose coupling**: High-level and low-level modules are independent
5. **Configuration**: Can configure different implementations for different environments
6. **Single Responsibility**: Each class focuses on its own responsibility

## 🚨 Common DIP Violations

1. **Direct instantiation**: `new EmailService()` in business logic
2. **Static dependencies**: Calling static methods from other classes
3. **Concrete type parameters**: Method parameters that are concrete classes
4. **Framework coupling**: Business logic directly dependent on specific frameworks
5. **Configuration coupling**: Hard-coded connection strings or API keys

## 🎯 Interview Questions

**Q: What's the difference between Dependency Injection and Dependency Inversion?**
**A:** Dependency Inversion is the principle (depend on abstractions), while Dependency Injection is a technique to implement the principle (injecting dependencies rather than creating them).

**Q: When should you use DIP?**
**A:** Use DIP when:
- You need to support multiple implementations
- You want to improve testability
- You're dealing with external dependencies (databases, web services, file systems)
- You want to make your code more flexible and maintainable

**Q: What are the drawbacks of DIP?**
**A:** 
- Increased complexity (more interfaces and abstractions)
- Runtime cost of dependency resolution
- Can be over-engineered for simple applications
- Learning curve for teams unfamiliar with DI containers

**Q: How does DIP relate to IoC containers?**
**A:** IoC (Inversion of Control) containers are tools that help implement DIP by automatically resolving and injecting dependencies. They manage object lifecycles and dependency graphs.

**Q: Can you give an ERP example where DIP is crucial?**
**A:** In a multi-tenant ERP system, different customers might use different payment processors, shipping providers, or tax calculation services. With DIP, you can inject the appropriate implementations based on the customer's configuration without changing the core business logic.

## 📝 Checklist

- [ ] High-level modules don't instantiate low-level modules directly
- [ ] Dependencies are injected through constructor, properties, or methods
- [ ] Business logic depends on interfaces, not concrete classes
- [ ] External dependencies (database, web services) are abstracted
- [ ] Easy to swap implementations for testing or different environments
- [ ] No static calls to other classes from business logic
- [ ] Configuration is externalized from business logic

---

**Previous**: [← Interface Segregation Principle](./04-interface-segregation.md) | **Next**: [Repository Pattern →](../02-enterprise-patterns/01-repository-pattern.md)