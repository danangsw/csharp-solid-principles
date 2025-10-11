# Custom IoC Container Implementation

This document explains how to implement the **Inversion of Control (IoC)** pattern without using `Microsoft.Extensions.DependencyInjection`.

## 🎯 **What is IoC (Inversion of Control)?**

IoC is a design principle where the **control of object creation and dependency management is inverted**:

- **Traditional approach**: Objects create their own dependencies
- **IoC approach**: An external container manages object creation and dependency injection

## 🏗️ **Custom IoC Container Architecture**

### **Core Components**

#### **1. SimpleIoCContainer**
The main container that manages service registration and resolution.

```csharp
public class SimpleIoCContainer
{
    // Service registry
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _services = new();

    // Registration methods
    public void AddTransient<TService, TImplementation>()
    public void AddSingleton<TService, TImplementation>()
    public void AddSingleton<TService>(TService instance)

    // Resolution methods
    public TService GetService<TService>()
    public object GetService(Type serviceType)
}
```

#### **2. ServiceDescriptor**
Internal class that holds service metadata.

```csharp
private sealed class ServiceDescriptor
{
    public Type ServiceType { get; }        // The implementation type
    public ServiceLifetime Lifetime { get; } // Transient or Singleton
    public object? Instance { get; set; }   // Cached singleton instance
}
```

#### **3. ServiceLifetime Enum**
Defines how services are managed.

```csharp
private enum ServiceLifetime
{
    Transient,  // New instance each time
    Singleton   // Same instance always
}
```

## 🔧 **How It Works**

### **Service Registration**

```csharp
var container = new SimpleIoCContainer();

// Register services
container.AddSingleton<ISimpleLogger, ConsoleLogger>();
container.AddSingleton<ITimeProvider, SystemTimeProvider>();
container.AddSingleton<HRManagementService, HRManagementService>();
```

### **Dependency Resolution**

When `GetService<T>()` is called:

1. **Find the service descriptor** in the registry
2. **Check lifetime**:
   - **Transient**: Always create new instance
   - **Singleton**: Return cached instance or create if not exists
3. **Resolve dependencies** using reflection:
   - Find constructor with most parameters
   - Recursively resolve each parameter
   - Invoke constructor with resolved dependencies

### **Constructor Injection**

```csharp
// HRManagementService constructor
public HRManagementService(
    ILogger<HRManagementService> logger,  // Injected by container
    ITimeProvider timeProvider = null     // Injected by container
)
```

The container automatically:
1. Finds `ILogger<HRManagementService>` → resolves to `LoggerAdapter<HRManagementService>`
2. Finds `ITimeProvider` → resolves to `SystemTimeProvider`
3. Creates `HRManagementService` with these dependencies

## 🎨 **Key Design Patterns Used**

### **Factory Pattern**
The container acts as a factory for creating objects with their dependencies.

### **Singleton Pattern**
Ensures single instances for singleton services.

### **Adapter Pattern**
`LoggerAdapter<T>` bridges our simple logger with `Microsoft.Extensions.Logging`.

### **Registry Pattern**
`ConcurrentDictionary<Type, ServiceDescriptor>` serves as the service registry.

## 📊 **Comparison: Custom vs Microsoft.Extensions.DI**

| Feature | Custom IoC | Microsoft.Extensions.DI |
|---------|------------|-------------------------|
| **Registration** | `AddTransient<T, U>()` | `services.AddTransient<T, U>()` |
| **Resolution** | `GetService<T>()` | `serviceProvider.GetService<T>()` |
| **Lifetimes** | Transient, Singleton | Transient, Scoped, Singleton |
| **Size** | ~150 lines | Thousands of lines |
| **Features** | Basic DI | Advanced (validation, logging, etc.) |
| **Performance** | Very fast | Optimized for ASP.NET Core |

## 🚀 **Usage in Console App**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        // Setup custom IoC container
        var container = ConfigureServices();

        // Resolve and use services
        var hrService = container.GetService<HRManagementService>();
        var consoleApp = new HRConsoleApp(hrService);

        consoleApp.Run();
    }

    private static SimpleIoCContainer ConfigureServices()
    {
        var container = new SimpleIoCContainer();

        // Register dependencies
        container.AddSingleton<ISimpleLogger, ConsoleLogger>();
        container.AddTransient(typeof(ILogger<HRManagementService>),
            typeof(LoggerAdapter<HRManagementService>));
        container.AddSingleton<ITimeProvider, SystemTimeProvider>();
        container.AddSingleton<HRManagementService, HRManagementService>();

        return container;
    }
}
```

## 🎯 **Benefits of Custom Implementation**

### **Educational Value**
- Understands core DI principles
- Learns reflection and metadata programming
- Grasps lifetime management concepts

### **Control & Simplicity**
- No external dependencies for basic DI
- Complete control over behavior
- Easy to extend and modify

### **Performance**
- Minimal overhead
- Fast resolution for small applications
- No complex framework initialization

## 🔮 **Limitations & Extensions**

### **Current Limitations**
- No scoped lifetime (per-request in web apps)
- No service validation
- No automatic disposal management
- No named services

### **Possible Extensions**
- Add `AddScoped<T>()` with scope context
- Add service disposal with `IDisposable`
- Add named registrations: `AddTransient<T>("name")`
- Add service validation and diagnostics

## 🎓 **Learning Outcomes**

This custom IoC implementation demonstrates:

1. **SOLID Principles**: Dependency Inversion through constructor injection
2. **Reflection**: Runtime type inspection and instantiation
3. **Thread Safety**: Using `ConcurrentDictionary` for multi-threading
4. **Design Patterns**: Factory, Singleton, Adapter, Registry patterns
5. **Clean Architecture**: Separation of concerns between business logic and infrastructure

## 🏃 **Running the Example**

```bash
cd /path/to/csharp-solid-principles
dotnet run --project src/CSharpSolid.ConsoleApp
```

The console app now uses our custom IoC container instead of Microsoft.Extensions.DependencyInjection, proving that IoC can be implemented with just ~150 lines of code!