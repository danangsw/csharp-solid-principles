# Liskov Substitution Principle (LSP)

> **"Objects of a superclass should be replaceable with objects of a subclass without breaking the application."** - Barbara Liskov

## 🎯 Definition

The Liskov Substitution Principle states that objects of a superclass should be replaceable with objects of any of its subclasses without altering the correctness of the program. In other words, if S is a subtype of T, then objects of type T may be replaced with objects of type S.

## 🏠 Real-World Analogy

Think of **car controls**:

- ✅ **All cars have gas pedal, brake, steering wheel** - you can drive any car using the same interface
- ✅ **Sports car, SUV, truck** - all can be driven the same way despite internal differences
- ❌ **If a "car" required you to pedal like a bicycle** - it would violate the expectation

## 🚫 Violation Example

```csharp
// ❌ BAD: Violates LSP - Rectangle and Square don't behave consistently

public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int GetArea()
    {
        return Width * Height;
    }
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            base.Height = value; // Violates expected behavior
        }
    }

    public override int Height
    {
        get => base.Height;
        set
        {
            base.Width = value; // Violates expected behavior
            base.Height = value;
        }
    }
}

// This code breaks when Square is used instead of Rectangle
public class AreaCalculator
{
    public void TestRectangle(Rectangle rectangle)
    {
        rectangle.Width = 4;
        rectangle.Height = 5;
        
        // Expected: 20 (4 * 5)
        // Actual with Square: 25 (5 * 5) - BROKEN!
        Console.WriteLine($"Area: {rectangle.GetArea()}");
    }
}
```

### Problems with this approach:

1. **Square changes both dimensions** when setting one property
2. **Unexpected behavior** when Square is used in place of Rectangle
3. **Breaks client expectations** about how Width and Height should work
4. **Violates the contract** established by the base class

## ✅ Correct Implementation

```csharp
// ✅ GOOD: Proper abstraction that respects LSP

public abstract class Shape
{
    public abstract int GetArea();
    public abstract int GetPerimeter();
}

public class Rectangle : Shape
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Rectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override int GetArea()
    {
        return Width * Height;
    }

    public override int GetPerimeter()
    {
        return 2 * (Width + Height);
    }
}

public class Square : Shape
{
    public int Side { get; set; }

    public Square(int side)
    {
        Side = side;
    }

    public override int GetArea()
    {
        return Side * Side;
    }

    public override int GetPerimeter()
    {
        return 4 * Side;
    }
}

// Now both shapes can be used interchangeably
public class AreaCalculator
{
    public void CalculateArea(Shape shape)
    {
        Console.WriteLine($"Area: {shape.GetArea()}");
        Console.WriteLine($"Perimeter: {shape.GetPerimeter()}");
    }

    public void ProcessShapes(IEnumerable<Shape> shapes)
    {
        foreach (var shape in shapes)
        {
            CalculateArea(shape); // Works correctly for all shapes
        }
    }
}
```

## 🐦 Classic Bird Example

### ❌ Violation

```csharp
public class Bird
{
    public virtual void Fly()
    {
        Console.WriteLine("Flying...");
    }
}

public class Eagle : Bird
{
    public override void Fly()
    {
        Console.WriteLine("Eagle soaring high...");
    }
}

public class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException("Penguins can't fly!"); // LSP Violation!
    }
}

// This breaks when Penguin is used
public class BirdWatcher
{
    public void WatchBirds(IEnumerable<Bird> birds)
    {
        foreach (var bird in birds)
        {
            bird.Fly(); // Throws exception for Penguin!
        }
    }
}
```

### ✅ Correct Implementation

```csharp
public abstract class Bird
{
    public abstract void Move();
    public abstract void MakeSound();
}

public interface IFlyingBird
{
    void Fly();
}

public interface ISwimmingBird
{
    void Swim();
}

public class Eagle : Bird, IFlyingBird
{
    public override void Move()
    {
        Fly();
    }

    public override void MakeSound()
    {
        Console.WriteLine("Eagle screeches...");
    }

    public void Fly()
    {
        Console.WriteLine("Eagle soaring high...");
    }
}

public class Penguin : Bird, ISwimmingBird
{
    public override void Move()
    {
        Swim();
    }

    public override void MakeSound()
    {
        Console.WriteLine("Penguin makes penguin sounds...");
    }

    public void Swim()
    {
        Console.WriteLine("Penguin swimming gracefully...");
    }
}

// Now the code respects each bird's capabilities
public class BirdWatcher
{
    public void WatchBirds(IEnumerable<Bird> birds)
    {
        foreach (var bird in birds)
        {
            bird.Move(); // Always works!
            bird.MakeSound();
        }
    }

    public void WatchFlyingBirds(IEnumerable<IFlyingBird> flyingBirds)
    {
        foreach (var bird in flyingBirds)
        {
            bird.Fly(); // Only flying birds are passed here
        }
    }
}
```

## 🏢 ERP Example: Employee Payroll

### ❌ Violation

```csharp
public class Employee
{
    public string Name { get; set; }
    public decimal BaseSalary { get; set; }

    public virtual decimal CalculatePayroll()
    {
        return BaseSalary;
    }

    public virtual void ProcessPayment()
    {
        Console.WriteLine($"Paying {Name}: ${CalculatePayroll()}");
    }
}

public class ContractEmployee : Employee
{
    public override void ProcessPayment()
    {
        throw new InvalidOperationException("Contract employees are paid differently!"); // LSP Violation!
    }
}

public class VolunteerEmployee : Employee
{
    public override decimal CalculatePayroll()
    {
        throw new InvalidOperationException("Volunteers don't get paid!"); // LSP Violation!
    }
}
```

### ✅ Correct Implementation

```csharp
public abstract class Person
{
    public string Name { get; set; }
    public abstract string GetRole();
}

public interface IPaidEmployee
{
    decimal CalculatePayroll();
    void ProcessPayment();
}

public interface IContractWorker
{
    decimal CalculateContractPayment();
    void ProcessContractPayment();
}

public class PermanentEmployee : Person, IPaidEmployee
{
    public decimal BaseSalary { get; set; }

    public override string GetRole()
    {
        return "Permanent Employee";
    }

    public decimal CalculatePayroll()
    {
        return BaseSalary + (BaseSalary * 0.1m); // 10% bonus
    }

    public void ProcessPayment()
    {
        Console.WriteLine($"Paying {Name}: ${CalculatePayroll()}");
    }
}

public class ContractEmployee : Person, IContractWorker
{
    public decimal HourlyRate { get; set; }
    public int HoursWorked { get; set; }

    public override string GetRole()
    {
        return "Contract Employee";
    }

    public decimal CalculateContractPayment()
    {
        return HourlyRate * HoursWorked;
    }

    public void ProcessContractPayment()
    {
        Console.WriteLine($"Paying contractor {Name}: ${CalculateContractPayment()}");
    }
}

public class Volunteer : Person
{
    public override string GetRole()
    {
        return "Volunteer";
    }

    public void RecognizeContribution()
    {
        Console.WriteLine($"Thank you {Name} for your valuable contribution!");
    }
}

// Payroll service works correctly with appropriate types
public class PayrollService
{
    public void ProcessEmployeePayments(IEnumerable<IPaidEmployee> employees)
    {
        foreach (var employee in employees)
        {
            employee.ProcessPayment(); // Always works!
        }
    }

    public void ProcessContractPayments(IEnumerable<IContractWorker> contractors)
    {
        foreach (var contractor in contractors)
        {
            contractor.ProcessContractPayment(); // Always works!
        }
    }
}
```

## 💰 Financial Transaction Example

### ❌ Violation

```csharp
public class Transaction
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public virtual void Process()
    {
        Console.WriteLine($"Processing transaction: ${Amount}");
    }

    public virtual void Reverse()
    {
        Console.WriteLine($"Reversing transaction: ${Amount}");
    }
}

public class CashTransaction : Transaction
{
    public override void Reverse()
    {
        throw new InvalidOperationException("Cash transactions cannot be reversed!"); // LSP Violation!
    }
}
```

### ✅ Correct Implementation

```csharp
public abstract class Transaction
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string TransactionId { get; set; }

    public abstract void Process();
    public abstract bool CanBeReversed();
}

public interface IReversibleTransaction
{
    void Reverse();
}

public class CreditCardTransaction : Transaction, IReversibleTransaction
{
    public string CardNumber { get; set; }

    public override void Process()
    {
        Console.WriteLine($"Processing credit card transaction: ${Amount}");
    }

    public override bool CanBeReversed()
    {
        return true;
    }

    public void Reverse()
    {
        Console.WriteLine($"Reversing credit card transaction: ${Amount}");
    }
}

public class CashTransaction : Transaction
{
    public override void Process()
    {
        Console.WriteLine($"Processing cash transaction: ${Amount}");
    }

    public override bool CanBeReversed()
    {
        return false;
    }
}

public class TransactionProcessor
{
    public void ProcessTransactions(IEnumerable<Transaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            transaction.Process(); // Always works!
        }
    }

    public void ReverseTransactions(IEnumerable<IReversibleTransaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            transaction.Reverse(); // Only reversible transactions are passed
        }
    }
}
```

## ✅ Benefits

1. **Reliability**: Code works correctly regardless of which subtype is used
2. **Maintainability**: Adding new subtypes doesn't break existing code
3. **Testability**: Can test with any valid subtype
4. **Polymorphism**: True polymorphic behavior without surprises
5. **Design clarity**: Clear contracts and expectations

## 🧪 Unit Testing

```csharp
[Test]
public void AllShapes_Should_Calculate_Area_Correctly()
{
    // Arrange
    var shapes = new List<Shape>
    {
        new Rectangle(4, 5),
        new Square(4),
        new Circle(3) // Another shape implementation
    };

    // Act & Assert
    foreach (var shape in shapes)
    {
        var area = shape.GetArea();
        Assert.IsTrue(area > 0, "All shapes should have positive area");
        // Each shape calculates area correctly according to its contract
    }
}

[Test]
public void AllPaidEmployees_Should_Process_Payment_Successfully()
{
    // Arrange
    var employees = new List<IPaidEmployee>
    {
        new PermanentEmployee { Name = "John", BaseSalary = 5000 },
        new PartTimeEmployee { Name = "Jane", HourlyRate = 25, HoursWorked = 80 }
    };

    // Act & Assert
    foreach (var employee in employees)
    {
        Assert.DoesNotThrow(() => employee.ProcessPayment());
    }
}
```

## 🎯 LSP Design Guidelines

### 1. Preconditions cannot be strengthened

```csharp
// ❌ BAD: Subclass adds stronger precondition
public class FileReader
{
    public virtual string ReadFile(string filename)
    {
        // Base: accepts any filename
        return File.ReadAllText(filename);
    }
}

public class XmlFileReader : FileReader
{
    public override string ReadFile(string filename)
    {
        if (!filename.EndsWith(".xml")) // Stronger precondition!
            throw new ArgumentException("Must be XML file");
        
        return base.ReadFile(filename);
    }
}

// ✅ GOOD: Same or weaker preconditions
public class XmlFileReader : FileReader
{
    public override string ReadFile(string filename)
    {
        // Accept any file, but handle XML specially
        var content = base.ReadFile(filename);
        return filename.EndsWith(".xml") ? ParseXml(content) : content;
    }
}
```

### 2. Postconditions cannot be weakened

```csharp
// ❌ BAD: Subclass provides weaker postcondition
public class Calculator
{
    public virtual int Add(int a, int b)
    {
        return a + b; // Always returns correct sum
    }
}

public class ApproximateCalculator : Calculator
{
    public override int Add(int a, int b)
    {
        return (int)Math.Round((a + b) * 0.9); // Weaker postcondition!
    }
}

// ✅ GOOD: Same or stronger postconditions
public class PreciseCalculator : Calculator
{
    public override int Add(int a, int b)
    {
        var result = a + b;
        LogOperation($"Added {a} + {b} = {result}"); // Additional behavior
        return result; // Same postcondition
    }
}
```

## 🚨 Common LSP Violations

1. **Throwing exceptions in subclasses** for operations the base class supports
2. **Strengthening preconditions** (requiring more than the base class)
3. **Weakening postconditions** (guaranteeing less than the base class)
4. **Changing expected behavior** in ways that break client expectations
5. **Empty implementations** that don't fulfill the contract

## 🎯 Interview Questions

**Q: What's the key difference between LSP and simple inheritance?**
**A:** Inheritance allows code reuse, but LSP ensures that subclasses can be used anywhere the base class is expected without breaking functionality. It's about behavioral compatibility, not just structural inheritance.

**Q: How do you fix LSP violations?**
**A:** 
1. Use composition instead of inheritance
2. Create more specific interfaces
3. Move common behavior to abstract base classes
4. Use strategy pattern for varying behaviors

**Q: Can you give an ERP example where LSP is critical?**
**A:** In a payment processing system, all payment methods (CreditCard, BankTransfer, PayPal) should work interchangeably with the same payment interface. If one method throws exceptions or behaves differently than expected, it violates LSP and breaks the payment flow.

**Q: What's the relationship between LSP and the Open/Closed Principle?**
**A:** LSP ensures that extensions (new subclasses) don't break existing functionality, which supports OCP by making it safe to extend behavior through inheritance.

## 📝 Checklist

- [ ] Subclasses can be used anywhere the base class is expected
- [ ] No exceptions thrown for operations the base class supports
- [ ] Preconditions are not strengthened in subclasses
- [ ] Postconditions are not weakened in subclasses
- [ ] Subclass behavior is consistent with base class contracts
- [ ] Client code works correctly regardless of which subclass is used
- [ ] Unit tests pass when using any valid subclass

---

**Previous**: [← Open/Closed Principle](./02-open-closed.md) | **Next**: [Interface Segregation Principle →](./04-interface-segregation.md)