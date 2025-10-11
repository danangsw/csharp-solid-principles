# HR Management System Console Application

This console application demonstrates the usage of the HRManagementService for managing employees with proper encapsulation principles.

## Features

- **Hire Employee**: Add a new employee to the system
- **Promote Employee**: Promote an existing employee with salary increase
- **Terminate Employee**: Terminate an employee with reason
- **Get Employee Details**: Retrieve detailed information about an employee

## How to Run

```bash
cd /path/to/csharp-solid-principles
dotnet run --project src/CSharpSolid.ConsoleApp
```

## Usage Examples

### Hiring an Employee
```
Enter command: 1
--- Hire New Employee ---
First Name: John
Last Name: Doe
Social Security Number (XXX-XX-XXXX): 123-45-6789
Department (IT, Finance, HR, Marketing, Operation): IT
Salary: 50000
Hire Date (YYYY-MM-DD): 2024-01-15
Employee hired successfully! Employee ID: EMP-2024-01-15-1
```

### Getting Employee Details
```
Enter command: 4
--- Get Employee Details ---
Employee ID: EMP-2024-01-15-1
Employee Details:
ID: EMP-2024-01-15-1
Name: John Doe
Department: IT
Salary: $50,000.00
Hire Date: 2024-01-15
SSN: ***-**-6789
```

### Promoting an Employee
```
Enter command: 2
--- Promote Employee ---
Employee ID: EMP-2024-01-15-1
New Title/Reason for promotion: Senior Developer
Salary Increase Percentage (e.g., 0.10 for 10%): 0.15
Employee promoted successfully!
```

### Terminating an Employee
```
Enter command: 3
--- Terminate Employee ---
Employee ID: EMP-2024-01-15-1
Termination Reason: End of contract
Employee terminated successfully!
```

## Commands

- `1` or `hire` - Hire a new employee
- `2` or `promote` - Promote an employee
- `3` or `terminate` - Terminate an employee
- `4` or `details` - Get employee details
- `5` or `list` - List all employees (not implemented)
- `6` or `exit` - Exit the application

## Architecture

The console application uses:
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection for service registration
- **Logging**: Console logging for error tracking
- **SOLID Principles**: Demonstrates proper encapsulation and dependency inversion
- **Builder Pattern**: For creating complex employee objects
- **Validation**: Comprehensive input validation and error handling

## Project Structure

```
src/
├── CSharpSolid.ConsoleApp/
│   ├── Program.cs              # Application entry point with DI setup
│   ├── HRConsoleApp.cs         # Main console interface logic
│   └── CSharpSolid.ConsoleApp.csproj
└── CSharpSolid.Oop/
    └── Encapsulation/
        ├── HRManagementService.cs    # Core business logic
        ├── GoodEncapsulation.cs      # Employee entity with proper encapsulation
        └── EmployeeDetailBuilder.cs  # Builder for employee creation
```