// OOP Principles - Basic Examples with Validation and Exception Handling
// This file demonstrates all major OOP principles with simple, easy-to-understand examples
// Each principle builds on the previous one to show how they work together
// Includes comprehensive validation and exception handling for edge cases

using System;
using System.Collections.Generic;

namespace CSharpSolid.Oop
{
    #region CUSTOM EXCEPTIONS

    /// <summary>
    /// Custom exceptions for better error handling
    /// </summary>
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException(string message) : base(message) { }
    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message) : base(message) { }
    }

    public class InvalidShapeException : Exception
    {
        public InvalidShapeException(string message) : base(message) { }
    }

    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(string message) : base(message) { }
    }

    #endregion

    #region 1. ENCAPSULATION - Hiding internal details and exposing only necessary information

    /// <summary>
    /// Encapsulation: Bank Account hides balance and provides controlled access
    /// </summary>
    public class BankAccount
    {
        // Private fields - internal details hidden from outside
        private decimal _balance;
        private string _accountNumber;

        // Constructor with validation
        public BankAccount(string accountNumber, decimal initialBalance = 0)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty", nameof(accountNumber));

            if (initialBalance < 0)
                throw new InvalidAmountException("Initial balance cannot be negative");

            _accountNumber = accountNumber;
            _balance = initialBalance;
        }

        // Public properties - controlled access to private data
        public string AccountNumber => _accountNumber;

        public decimal Balance => _balance; // Read-only from outside

        // Public methods - controlled ways to modify data with validation
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException("Deposit amount must be positive");

            if (amount > 1000000) // Reasonable upper limit
                throw new InvalidAmountException("Deposit amount cannot exceed $1,000,000");

            _balance += amount;
            Console.WriteLine($"Deposited ${amount}. New balance: ${_balance}");
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException("Withdrawal amount must be positive");

            if (amount > _balance)
                throw new InsufficientFundsException($"Insufficient funds. Available balance: ${_balance}");

            _balance -= amount;
            Console.WriteLine($"Withdrew ${amount}. New balance: ${_balance}");
            return true;
        }

        // Additional validation method
        public bool CanWithdraw(decimal amount)
        {
            return amount > 0 && amount <= _balance;
        }
    }

    #endregion

    #region 2. ABSTRACTION - Focusing on essential features while hiding complexity

    /// <summary>
    /// Abstraction: Coffee Machine hides brewing complexity, shows simple interface
    /// </summary>
    public abstract class CoffeeMachine
    {
        protected bool _isOn = false;
        protected int _waterLevel = 0; // ml

        // Abstract method - must be implemented by concrete classes
        public abstract void BrewCoffee();

        // Concrete method - common behavior with validation
        public void TurnOn()
        {
            if (_isOn)
                throw new InvalidOperationException("Coffee machine is already on");

            _isOn = true;
            Console.WriteLine("Coffee machine is now ON");
        }

        public void TurnOff()
        {
            if (!_isOn)
                throw new InvalidOperationException("Coffee machine is already off");

            _isOn = false;
            Console.WriteLine("Coffee machine is now OFF");
        }

        public void AddWater(int milliliters)
        {
            if (!_isOn)
                throw new InvalidOperationException("Cannot add water while machine is off");

            if (milliliters <= 0 || milliliters > 2000)
                throw new ArgumentException("Water amount must be between 1-2000ml", nameof(milliliters));

            _waterLevel += milliliters;
            Console.WriteLine($"Added {milliliters}ml water. Total: {_waterLevel}ml");
        }

        protected void ValidateBrewingConditions()
        {
            if (!_isOn)
                throw new InvalidOperationException("Machine must be on to brew coffee");

            if (_waterLevel < 100)
                throw new InvalidOperationException("Not enough water. Minimum 100ml required");
        }
    }

    public class BasicCoffeeMachine : CoffeeMachine
    {
        public override void BrewCoffee()
        {
            ValidateBrewingConditions();

            Console.WriteLine("Brewing basic coffee... Ready!");
            _waterLevel -= 100; // Consume water
        }
    }

    public class EspressoMachine : CoffeeMachine
    {
        public override void BrewCoffee()
        {
            ValidateBrewingConditions();

            if (_waterLevel < 50)
                throw new InvalidOperationException("Espresso requires at least 50ml water");

            Console.WriteLine("Brewing espresso with high pressure... Ready!");
            _waterLevel -= 50; // Consume water
        }
    }

    #endregion

    #region 3. INHERITANCE - Creating new classes based on existing ones

    /// <summary>
    /// Inheritance: Animal base class with common behavior
    /// </summary>
    public class Animal
    {
        private string _name;
        private int _age;

        public Animal(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Animal name cannot be null or empty", nameof(name));

            if (age < 0 || age > 150)
                throw new ArgumentException("Age must be between 0-150 years", nameof(age));

            _name = name;
            _age = age;
        }

        public string Name => _name;
        public int Age => _age;

        public virtual void MakeSound()
        {
            Console.WriteLine("Some animal sound");
        }

        public void Eat()
        {
            if (string.IsNullOrWhiteSpace(_name))
                throw new InvalidOperationException("Cannot feed animal without a name");

            Console.WriteLine($"{_name} is eating");
        }

        public void Sleep()
        {
            Console.WriteLine($"{_name} is sleeping");
        }

        // Protected method for subclasses to validate
        protected void ValidateActivity()
        {
            if (Age > 100)
                throw new InvalidOperationException("Animal is too old for activities");
        }
    }

    /// <summary>
    /// Dog inherits from Animal and adds specific behavior
    /// </summary>
    public class Dog : Animal
    {
        private string _breed;

        public Dog(string name, int age, string breed) : base(name, age)
        {
            if (string.IsNullOrWhiteSpace(breed))
                throw new ArgumentException("Dog breed cannot be null or empty", nameof(breed));

            _breed = breed;
        }

        public string Breed => _breed;

        public override void MakeSound()
        {
            ValidateActivity();
            Console.WriteLine($"{Name} says: Woof!");
        }

        public void Fetch()
        {
            ValidateActivity();
            if (Age < 1)
                throw new InvalidOperationException("Puppy is too young to fetch");

            Console.WriteLine($"{Name} is fetching the ball");
        }
    }

    /// <summary>
    /// Cat inherits from Animal and adds specific behavior
    /// </summary>
    public class Cat : Animal
    {
        public Cat(string name, int age) : base(name, age)
        {
        }

        public override void MakeSound()
        {
            ValidateActivity();
            Console.WriteLine($"{Name} says: Meow!");
        }

        public void Purr()
        {
            ValidateActivity();
            Console.WriteLine($"{Name} is purring");
        }
    }

    #endregion

    #region 4. POLYMORPHISM - Same interface, different implementations

    /// <summary>
    /// Polymorphism: Shape interface with different implementations
    /// </summary>
    public interface IShape
    {
        double CalculateArea();
        void Draw();
        bool IsValid();
    }

    public class Circle : IShape
    {
        private double _radius;

        public Circle(double radius)
        {
            if (radius <= 0 || radius > 10000)
                throw new InvalidShapeException("Circle radius must be between 0-10000");

            _radius = radius;
        }

        public double Radius => _radius;

        public double CalculateArea()
        {
            return Math.PI * _radius * _radius;
        }

        public void Draw()
        {
            Console.WriteLine($"Drawing a circle with radius {_radius}");
        }

        public bool IsValid()
        {
            return _radius > 0;
        }
    }

    public class Rectangle : IShape
    {
        private double _width;
        private double _height;

        public Rectangle(double width, double height)
        {
            if (width <= 0 || height <= 0 || width > 10000 || height > 10000)
                throw new InvalidShapeException("Rectangle dimensions must be positive and less than 10000");

            _width = width;
            _height = height;
        }

        public double Width => _width;
        public double Height => _height;

        public double CalculateArea()
        {
            return _width * _height;
        }

        public void Draw()
        {
            Console.WriteLine($"Drawing a rectangle {_width}x{_height}");
        }

        public bool IsValid()
        {
            return _width > 0 && _height > 0;
        }
    }

    public class Triangle : IShape
    {
        private double _base;
        private double _height;

        public Triangle(double @base, double height)
        {
            if (@base <= 0 || height <= 0 || @base > 10000 || height > 10000)
                throw new InvalidShapeException("Triangle dimensions must be positive and less than 10000");

            _base = @base;
            _height = height;
        }

        public double Base => _base;
        public double Height => _height;

        public double CalculateArea()
        {
            return (_base * _height) / 2;
        }

        public void Draw()
        {
            Console.WriteLine($"Drawing a triangle with base {_base} and height {_height}");
        }

        public bool IsValid()
        {
            return _base > 0 && _height > 0;
        }
    }

    #endregion

    #region 5. COUPLING - How tightly classes are connected

    /// <summary>
    /// Low Coupling: Email service that can work with different notification systems
    /// </summary>
    public interface INotificationService
    {
        void SendNotification(string message);
        bool IsAvailable();
    }

    public class EmailNotification : INotificationService
    {
        private bool _isConfigured = true; // Simulate configuration

        public void SendNotification(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            if (!IsAvailable())
                throw new InvalidOperationException("Email service is not available");

            Console.WriteLine($"Sending email: {message}");
        }

        public bool IsAvailable()
        {
            return _isConfigured;
        }
    }

    public class SMSNotification : INotificationService
    {
        private bool _hasCredits = true; // Simulate SMS credits

        public void SendNotification(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            if (message.Length > 160)
                throw new ArgumentException("SMS message cannot exceed 160 characters", nameof(message));

            if (!IsAvailable())
                throw new InvalidOperationException("SMS service is not available");

            Console.WriteLine($"Sending SMS: {message}");
        }

        public bool IsAvailable()
        {
            return _hasCredits;
        }
    }

    /// <summary>
    /// UserService depends on interface, not concrete class (loose coupling)
    /// </summary>
    public class UserService
    {
        private readonly INotificationService _notificationService;

        public UserService(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void RegisterUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            if (username.Length < 3 || username.Length > 50)
                throw new ArgumentException("Username must be 3-50 characters", nameof(username));

            // Simulate registration logic
            Console.WriteLine($"User {username} registered");

            // Send notification (loosely coupled)
            try
            {
                _notificationService.SendNotification($"Welcome {username}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification: {ex.Message}");
                // Continue with registration even if notification fails
            }
        }
    }

    #endregion

    #region 6. COMPOSITION - Building complex objects from simpler ones

    /// <summary>
    /// Composition: Computer built from components (Has-A relationship)
    /// </summary>
    public class Processor
    {
        private string _model;
        private int _cores;

        public Processor(string model, int cores)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Processor model cannot be null or empty", nameof(model));

            if (cores <= 0 || cores > 128)
                throw new ArgumentException("Processor cores must be between 1-128", nameof(cores));

            _model = model;
            _cores = cores;
        }

        public string Model => _model;
        public int Cores => _cores;

        public void Process()
        {
            Console.WriteLine($"Processor {_model} with {_cores} cores is processing");
        }
    }

    public class Memory
    {
        private int _sizeGB;

        public Memory(int sizeGB)
        {
            if (sizeGB <= 0 || sizeGB > 1024)
                throw new ArgumentException("Memory size must be between 1-1024 GB", nameof(sizeGB));

            _sizeGB = sizeGB;
        }

        public int SizeGB => _sizeGB;

        public void LoadData()
        {
            Console.WriteLine($"Loading data into {_sizeGB}GB memory");
        }
    }

    public class HardDrive
    {
        private int _sizeGB;

        public HardDrive(int sizeGB)
        {
            if (sizeGB <= 0 || sizeGB > 100000)
                throw new ArgumentException("Hard drive size must be between 1-100000 GB", nameof(sizeGB));

            _sizeGB = sizeGB;
        }

        public int SizeGB => _sizeGB;

        public void ReadData()
        {
            Console.WriteLine($"Reading data from {_sizeGB}GB hard drive");
        }
    }

    /// <summary>
    /// Computer composed of Processor, Memory, and HardDrive
    /// </summary>
    public class Computer
    {
        // Composition: Computer HAS-A Processor, Memory, and HardDrive
        private readonly Processor _processor;
        private readonly Memory _memory;
        private readonly HardDrive _hardDrive;
        private readonly string _brand;
        private bool _isRunning = false;

        public Computer(string brand, Processor processor, Memory memory, HardDrive hardDrive)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Computer brand cannot be null or empty", nameof(brand));

            _brand = brand;
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _hardDrive = hardDrive ?? throw new ArgumentNullException(nameof(hardDrive));
        }

        public string Brand => _brand;
        public bool IsRunning => _isRunning;

        public void Start()
        {
            if (_isRunning)
                throw new InvalidOperationException("Computer is already running");

            Console.WriteLine($"Starting {_brand} computer...");
            _memory.LoadData();
            _processor.Process();
            _hardDrive.ReadData();
            _isRunning = true;
            Console.WriteLine("Computer is ready!");
        }

        public void Shutdown()
        {
            if (!_isRunning)
                throw new InvalidOperationException("Computer is not running");

            Console.WriteLine($"Shutting down {_brand} computer...");
            _isRunning = false;
        }

        // Additional validation methods
        public void ValidateComponents()
        {
            if (_processor == null)
                throw new ComponentNotFoundException("Processor component is missing");

            if (_memory == null)
                throw new ComponentNotFoundException("Memory component is missing");

            if (_hardDrive == null)
                throw new ComponentNotFoundException("Hard drive component is missing");
        }
    }

    #endregion

    #region DEMO CLASS - Shows all principles working together with error handling

    public class OOPPrinciplesDemo
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("🎯 OOP PRINCIPLES DEMO WITH VALIDATION & ERROR HANDLING");
            Console.WriteLine("======================================================\n");

            DemonstrateEncapsulation();
            DemonstrateAbstraction();
            DemonstrateInheritance();
            DemonstratePolymorphism();
            DemonstrateCoupling();
            DemonstrateComposition();

            Console.WriteLine("\n✅ All OOP principles demonstrated with proper validation!");
        }

        private static void DemonstrateEncapsulation()
        {
            Console.WriteLine("1️⃣ ENCAPSULATION");
            Console.WriteLine("----------------");

            try
            {
                var account = new BankAccount("123456", 1000);
                Console.WriteLine($"Account: {account.AccountNumber}, Balance: ${account.Balance}");

                account.Deposit(500);
                account.Withdraw(200);

                // Test edge cases
                try { account.Deposit(-100); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { account.Withdraw(2000); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { account.Deposit(2000000); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateAbstraction()
        {
            Console.WriteLine("2️⃣ ABSTRACTION");
            Console.WriteLine("--------------");

            try
            {
                var basicMachine = new BasicCoffeeMachine();
                var espressoMachine = new EspressoMachine();

                basicMachine.TurnOn();
                basicMachine.AddWater(500);
                basicMachine.BrewCoffee();
                basicMachine.TurnOff();

                Console.WriteLine();

                espressoMachine.TurnOn();
                espressoMachine.AddWater(200);
                espressoMachine.BrewCoffee();
                espressoMachine.TurnOff();

                // Test edge cases
                try { basicMachine.BrewCoffee(); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { espressoMachine.AddWater(3000); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateInheritance()
        {
            Console.WriteLine("3️⃣ INHERITANCE");
            Console.WriteLine("--------------");

            try
            {
                var dog = new Dog("Buddy", 3, "Golden Retriever");
                var cat = new Cat("Whiskers", 2);

                Console.WriteLine($"{dog.Name} is a {dog.Breed} dog, age {dog.Age}");
                dog.MakeSound();
                dog.Eat();
                dog.Fetch();

                Console.WriteLine();

                Console.WriteLine($"{cat.Name} is a cat, age {cat.Age}");
                cat.MakeSound();
                cat.Eat();
                cat.Purr();

                // Test edge cases
                try { new Dog("", 5, "Labrador"); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new Dog("Max", -1, "Poodle"); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstratePolymorphism()
        {
            Console.WriteLine("4️⃣ POLYMORPHISM");
            Console.WriteLine("---------------");

            try
            {
                var shapes = new List<IShape>
                {
                    new Circle(5),
                    new Rectangle(4, 6),
                    new Triangle(3, 4)
                };

                foreach (var shape in shapes)
                {
                    if (shape.IsValid())
                    {
                        shape.Draw();
                        Console.WriteLine($"Area: {shape.CalculateArea():F2}");
                    }
                }

                // Test edge cases
                try { new Circle(-5); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new Rectangle(0, 10); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new Triangle(5, -2); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateCoupling()
        {
            Console.WriteLine("5️⃣ COUPLING");
            Console.WriteLine("-----------");

            try
            {
                // Loose coupling - can easily switch notification methods
                var emailService = new UserService(new EmailNotification());
                var smsService = new UserService(new SMSNotification());

                emailService.RegisterUser("john@example.com");
                smsService.RegisterUser("jane@example.com");

                // Test edge cases
                try { emailService.RegisterUser(""); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { smsService.RegisterUser("ThisUsernameIsWayTooLongForSMS"); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateComposition()
        {
            Console.WriteLine("6️⃣ COMPOSITION");
            Console.WriteLine("--------------");

            try
            {
                // Build computer from components
                var processor = new Processor("Intel i7", 8);
                var memory = new Memory(16);
                var hardDrive = new HardDrive(512);

                var computer = new Computer("Dell", processor, memory, hardDrive);
                computer.Start();
                computer.Shutdown();

                // Test edge cases
                try { new Processor("", 4); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new Memory(-8); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new HardDrive(200000); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                try { new Computer("", processor, memory, hardDrive); } catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    #endregion

    #region 7. INTERFACE DEPENDENCY INJECTION - Advanced SOLID Implementation

    /// <summary>
    /// Interface Dependency Injection: Demonstrates how to inject dependencies through interfaces
    /// This follows the Dependency Inversion Principle (DIP) - depend on abstractions, not concretions
    /// </summary>

    #region DEPENDENCY INJECTION INTERFACES

    /// <summary>
    /// Logger interface - abstraction for logging functionality
    /// </summary>
    public interface ILogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
        bool IsEnabled(LogLevel level);
    }

    /// <summary>
    /// Data access interface - abstraction for data operations
    /// </summary>
    public interface IUserRepository
    {
        User? GetUserById(int id);
        IEnumerable<User> GetAllUsers();
        void SaveUser(User user);
        bool DeleteUser(int id);
    }

    /// <summary>
    /// Email service interface - abstraction for email functionality
    /// </summary>
    public interface IEmailService
    {
        void SendWelcomeEmail(string email, string username);
        void SendPasswordResetEmail(string email, string resetToken);
        bool IsValidEmail(string email);
    }

    /// <summary>
    /// Payment processor interface - abstraction for payment processing
    /// </summary>
    public interface IPaymentProcessor
    {
        bool ProcessPayment(decimal amount, string cardNumber, string expiryDate);
        bool RefundPayment(string transactionId, decimal amount);
        PaymentResult GetPaymentStatus(string transactionId);
    }

    #endregion

    #region DEPENDENCY INJECTION IMPLEMENTATIONS

    /// <summary>
    /// User entity for data operations
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public User(int id, string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            Id = id;
            Username = username;
            Email = email;
            IsActive = true;
            CreatedDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Log levels for logging abstraction
    /// </summary>
    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Payment result for payment operations
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }

        public PaymentResult(bool success, string transactionId, string message)
        {
            Success = success;
            TransactionId = transactionId;
            Message = message;
        }
    }

    /// <summary>
    /// Console Logger implementation
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minimumLevel;

        public ConsoleLogger() : this(LogLevel.Information) { }

        public ConsoleLogger(LogLevel minimumLevel = LogLevel.Information)
        {
            _minimumLevel = minimumLevel;
        }

        public void LogInformation(string message)
        {
            if (IsEnabled(LogLevel.Information))
                Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogWarning(string message)
        {
            if (IsEnabled(LogLevel.Warning))
                Console.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (IsEnabled(LogLevel.Error))
            {
                Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                if (exception != null)
                    Console.WriteLine($"Exception: {exception.Message}");
            }
        }

        public bool IsEnabled(LogLevel level)
        {
            return (int)level >= (int)_minimumLevel;
        }
    }

    /// <summary>
    /// In-memory user repository implementation
    /// </summary>
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<int, User> _users = new();
        private int _nextId = 1;

        public User? GetUserById(int id)
        {
            return _users.TryGetValue(id, out var user) ? user : null;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users.Values;
        }

        public void SaveUser(User user)
        {
            if (user.Id == 0)
            {
                user.Id = _nextId++;
            }
            _users[user.Id] = user;
        }

        public bool DeleteUser(int id)
        {
            return _users.Remove(id);
        }
    }

    /// <summary>
    /// SMTP Email service implementation
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger _logger;

        // Constructor injection - dependencies are injected
        public SmtpEmailService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SendWelcomeEmail(string email, string username)
        {
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email address", nameof(email));

            // Simulate sending email
            _logger.LogInformation($"Sending welcome email to {email} for user {username}");
            Console.WriteLine($"📧 Welcome email sent to {email}");
        }

        public void SendPasswordResetEmail(string email, string resetToken)
        {
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email address", nameof(email));

            _logger.LogInformation($"Sending password reset email to {email}");
            Console.WriteLine($"🔑 Password reset email sent to {email} with token: {resetToken}");
        }

        public bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }
    }

    /// <summary>
    /// Mock payment processor for demonstration
    /// </summary>
    public class MockPaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, PaymentResult> _transactions = new();
        private int _transactionCounter = 1;

        public MockPaymentProcessor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool ProcessPayment(decimal amount, string cardNumber, string expiryDate)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            var transactionId = $"TXN{_transactionCounter++}";
            var success = amount < 10000; // Simulate some payments failing for large amounts

            var result = new PaymentResult(success, transactionId,
                success ? "Payment processed successfully" : "Payment declined: Amount too large");

            _transactions[transactionId] = result;

            _logger.LogInformation($"Payment processed: {transactionId}, Amount: ${amount}, Success: {success}");

            return success;
        }

        public bool RefundPayment(string transactionId, decimal amount)
        {
            if (!_transactions.ContainsKey(transactionId))
            {
                _logger.LogWarning($"Refund failed: Transaction {transactionId} not found");
                return false;
            }

            _logger.LogInformation($"Refund processed: {transactionId}, Amount: ${amount}");
            return true;
        }

        public PaymentResult GetPaymentStatus(string transactionId)
        {
            return _transactions.TryGetValue(transactionId, out var result)
                ? result
                : new PaymentResult(false, transactionId, "Transaction not found");
        }
    }

    #endregion

    #region DEPENDENCY INJECTION PATTERNS

    /// <summary>
    /// User Service with Constructor Injection
    /// Dependencies are injected through the constructor
    /// </summary>
    public class UserServiceWithDI
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        // Constructor Injection - All dependencies provided at creation time
        public UserServiceWithDI(
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public User RegisterUser(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            _logger.LogInformation($"Registering new user: {username}");

            var user = new User(0, username, email);
            _userRepository.SaveUser(user);

            try
            {
                _emailService.SendWelcomeEmail(email, username);
                _logger.LogInformation($"User {username} registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send welcome email to {email}", ex);
                // Continue with registration even if email fails
            }

            return user;
        }

        public User? GetUser(int id)
        {
            _logger.LogInformation($"Retrieving user with ID: {id}");
            return _userRepository.GetUserById(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            _logger.LogInformation("Retrieving all users");
            return _userRepository.GetAllUsers();
        }
    }

    /// <summary>
    /// Order Service demonstrating Property Injection
    /// Dependencies can be set after object creation
    /// </summary>
    public class OrderService
    {
        private readonly ILogger _logger;

        // Property Injection - Dependencies can be set later
        public IPaymentProcessor? PaymentProcessor { get; set; }
        public IUserRepository? UserRepository { get; set; }

        public OrderService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool ProcessOrder(int userId, decimal amount, string cardNumber, string expiryDate)
        {
            if (PaymentProcessor == null)
                throw new InvalidOperationException("PaymentProcessor not set");
            if (UserRepository == null)
                throw new InvalidOperationException("UserRepository not set");

            var user = UserRepository.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning($"Order failed: User {userId} not found");
                return false;
            }

            _logger.LogInformation($"Processing order for user {user.Username}, amount: ${amount}");

            var paymentSuccess = PaymentProcessor.ProcessPayment(amount, cardNumber, expiryDate);

            if (paymentSuccess)
            {
                _logger.LogInformation($"Order completed successfully for user {user.Username}");
            }
            else
            {
                _logger.LogError($"Order failed: Payment processing failed for user {user.Username}");
            }

            return paymentSuccess;
        }
    }

    /// <summary>
    /// Notification Service demonstrating Method Injection
    /// Dependencies are injected when needed for specific operations
    /// </summary>
    public class NotificationService
    {
        private readonly ILogger _logger;

        public NotificationService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Method Injection - Email service injected only for email operations
        public void SendNotification(string message, IEmailService emailService, string email)
        {
            if (emailService == null)
                throw new ArgumentNullException(nameof(emailService));

            try
            {
                // For demo purposes, we'll simulate sending a notification email
                _logger.LogInformation($"Sending notification to {email}: {message}");
                Console.WriteLine($"📢 Notification sent to {email}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification to {email}", ex);
            }
        }

        // Method Injection - Payment processor injected only for payment notifications
        public void SendPaymentConfirmation(string transactionId, IPaymentProcessor paymentProcessor, IEmailService emailService, string email)
        {
            if (paymentProcessor == null)
                throw new ArgumentNullException(nameof(paymentProcessor));
            if (emailService == null)
                throw new ArgumentNullException(nameof(emailService));

            var paymentResult = paymentProcessor.GetPaymentStatus(transactionId);

            if (paymentResult.Success)
            {
                SendNotification($"Payment confirmed! Transaction ID: {transactionId}",
                               emailService, email);
            }
            else
            {
                SendNotification($"Payment failed: {paymentResult.Message}",
                               emailService, email);
            }
        }
    }

    #endregion

    #region DEPENDENCY INJECTION CONTAINER (Simple IoC Container)

    /// <summary>
    /// Simple Dependency Injection Container
    /// Demonstrates how to manage dependencies centrally
    /// </summary>
    public class SimpleDIContainer
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new();

        public void Register<TInterface, TImplementation>()
            where TImplementation : TInterface
        {
            _registrations[typeof(TInterface)] = () => Activator.CreateInstance(typeof(TImplementation))!;
        }

        public void Register<TInterface>(Func<object> factory)
        {
            _registrations[typeof(TInterface)] = factory;
        }

        public TInterface Resolve<TInterface>()
        {
            if (_registrations.TryGetValue(typeof(TInterface), out var factory))
            {
                return (TInterface)factory();
            }
            throw new InvalidOperationException($"Type {typeof(TInterface).Name} not registered");
        }
    }

    #endregion

    #region DEPENDENCY INJECTION DEMO

    public class DependencyInjectionDemo
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("🔄 INTERFACE DEPENDENCY INJECTION DEMO");
            Console.WriteLine("=====================================\n");

            DemonstrateConstructorInjection();
            DemonstratePropertyInjection();
            DemonstrateMethodInjection();
            DemonstrateDIContainer();

            Console.WriteLine("\n✅ Interface Dependency Injection patterns demonstrated!");
        }

        private static void DemonstrateConstructorInjection()
        {
            Console.WriteLine("1️⃣ CONSTRUCTOR INJECTION");
            Console.WriteLine("-----------------------");

            // Create dependencies
            var logger = new ConsoleLogger();
            var userRepository = new InMemoryUserRepository();
            var emailService = new SmtpEmailService(logger);

            // Inject dependencies through constructor
            var userService = new UserServiceWithDI(userRepository, emailService, logger);

            // Use the service
            var user1 = userService.RegisterUser("john_doe", "john@example.com");
            var user2 = userService.RegisterUser("jane_smith", "jane@example.com");

            Console.WriteLine($"Registered users: {user1.Username}, {user2.Username}");

            // Test error handling
            try
            {
                userService.RegisterUser("", "invalid@example.com");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstratePropertyInjection()
        {
            Console.WriteLine("2️⃣ PROPERTY INJECTION");
            Console.WriteLine("--------------------");

            var logger = new ConsoleLogger();
            var paymentProcessor = new MockPaymentProcessor(logger);
            var userRepository = new InMemoryUserRepository();

            // Create service without all dependencies
            var orderService = new OrderService(logger);

            // Inject dependencies through properties
            orderService.PaymentProcessor = paymentProcessor;
            orderService.UserRepository = userRepository;

            // Register a user first
            var user = new User(1, "order_user", "orders@example.com");
            userRepository.SaveUser(user);

            // Process an order
            var orderSuccess = orderService.ProcessOrder(1, 99.99m, "4111111111111111", "12/25");

            Console.WriteLine($"Order processing result: {(orderSuccess ? "SUCCESS" : "FAILED")}");

            // Test with missing dependency
            orderService.PaymentProcessor = null;
            try
            {
                orderService.ProcessOrder(1, 50.00m, "4111111111111111", "12/25");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateMethodInjection()
        {
            Console.WriteLine("3️⃣ METHOD INJECTION");
            Console.WriteLine("------------------");

            var logger = new ConsoleLogger();
            var emailService = new SmtpEmailService(logger);
            var paymentProcessor = new MockPaymentProcessor(logger);

            var notificationService = new NotificationService(logger);

            // Process a payment first to get a transaction ID
            var paymentSuccess = paymentProcessor.ProcessPayment(150.00m, "4111111111111111", "12/25");

            if (paymentSuccess)
            {
                var transactionId = "TXN1"; // We know this from our mock processor
                notificationService.SendPaymentConfirmation(transactionId, paymentProcessor, emailService, "customer@example.com");
            }

            // Send a general notification
            notificationService.SendNotification("Your order has been shipped!", emailService, "customer@example.com");

            // Test with null dependency
            try
            {
                notificationService.SendNotification("Test", null!, "test@example.com");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateDIContainer()
        {
            Console.WriteLine("4️⃣ DEPENDENCY INJECTION CONTAINER");
            Console.WriteLine("--------------------------------");

            var container = new SimpleDIContainer();

            // Register dependencies
            container.Register<ILogger, ConsoleLogger>();
            container.Register<IUserRepository, InMemoryUserRepository>();
            container.Register<IEmailService>(() => new SmtpEmailService(container.Resolve<ILogger>()));
            container.Register<IPaymentProcessor>(() => new MockPaymentProcessor(container.Resolve<ILogger>()));

            // Resolve and use services
            var logger = container.Resolve<ILogger>();
            var userRepo = container.Resolve<IUserRepository>();
            var emailSvc = container.Resolve<IEmailService>();
            var paymentProc = container.Resolve<IPaymentProcessor>();

            logger.LogInformation("All services resolved from container");

            // Create user service with resolved dependencies
            var userService = new UserServiceWithDI(userRepo, emailSvc, logger);
            var user = userService.RegisterUser("container_user", "container@example.com");

            Console.WriteLine($"User registered via container: {user.Username}");

            // Test payment processing
            var paymentResult = paymentProc.ProcessPayment(75.00m, "4111111111111111", "12/25");
            Console.WriteLine($"Payment via container: {(paymentResult ? "SUCCESS" : "FAILED")}");

            Console.WriteLine();
        }
    }

    #endregion

    #endregion
}
