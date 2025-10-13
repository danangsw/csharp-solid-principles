# C# SOLID Principles & Enterprise Patterns

A comprehensive learning resource for SOLID principles and Enterprise design patterns in C# for senior software developer technical interviews (ERP focus).

## 🎯 Learning Objectives

This project provides practical examples and in-depth understanding of:

### ✅ SOLID Principles
- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Extensible through interfaces and inheritance
- **Liskov Substitution**: Implementations can be swapped
- **Interface Segregation**: Focused, minimal interfaces
- **Dependency Inversion**: Depends on abstractions, not concretions

### ✅ Enterprise Patterns
- **Repository Pattern**: Data access abstraction
- **Strategy Pattern**: Pluggable storage and validation
- **Decorator Pattern**: Caching and logging layers
- **Factory Pattern**: Object creation management
- **Observer Pattern**: Event notifications

## 📁 Project Structure

```
├── 01-SOLID-Principles/
│   ├── 01-SingleResponsibility/
│   ├── 02-OpenClosed/
│   ├── 03-LiskovSubstitution/
│   ├── 04-InterfaceSegregation/
│   └── 05-DependencyInversion/
├── 02-Enterprise-Patterns/
│   ├── 01-Repository/
│   ├── 02-Strategy/
│   ├── 03-Decorator/
│   ├── 04-Factory/
│   └── 05-Observer/
├── 03-ERP-Examples/
│   ├── OrderManagement/
│   ├── InventorySystem/
│   └── FinancialModule/
└── 04-Tests/
    ├── SOLID-Tests/
    └── Pattern-Tests/
```

## 🚀 Getting Started

1. Clone this repository
2. Open in Visual Studio or VS Code
3. Build the solution: `dotnet build`
4. Run tests: `dotnet test`
5. Explore examples in each folder

## 📚 Study Guide

### For Technical Interviews

Each principle and pattern includes:
- ✅ Clear explanation with real-world analogies
- ✅ Code examples with violations and corrections
- ✅ ERP-specific scenarios
- ✅ Unit tests demonstrating testability
- ✅ Interview questions and answers

### Key ERP Scenarios Covered
- Order processing workflows
- Inventory management systems
- Financial transaction handling
- User permission systems
- Audit logging mechanisms

## 🎓 Resources

- [Microsoft Learn - SOLID Principles](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [C# Corner - SOLID Principles](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/)
- [FreeCodeCamp - SOLID in C#](https://www.freecodecamp.org/news/what-are-the-solid-principles-in-csharp)
- [🤖 GitHub Copilot Study Instructions](.github/copilot-instructions.md)

---

**Note**: This is a learning project designed for technical interview preparation. Focus on understanding the principles rather than memorizing code.
