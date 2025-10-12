using CSharpSolid.Oop.Encapsulation;

namespace CSharpSolid.ConsoleApp;

public class HRConsoleApp
{
    private readonly HRManagementService _hrService;

    public HRConsoleApp(HRManagementService hrService)
    {
        _hrService = hrService;
    }

    public void Run()
    {
        Console.WriteLine("=== HR Management System Console ===");
        Console.WriteLine("Commands:");
        Console.WriteLine("1. hire - Hire a new employee");
        Console.WriteLine("2. promote - Promote an employee");
        Console.WriteLine("3. terminate - Terminate an employee");
        Console.WriteLine("4. details - Get employee details");
        Console.WriteLine("5. list - List all employees (not implemented)");
        Console.WriteLine("6. exit - Exit the application");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter command: ");
            var command = Console.ReadLine()?.Trim().ToLower();

            try
            {
                switch (command)
                {
                    case "1":
                    case "hire":
                        HandleHireEmployee();
                        break;
                    case "2":
                    case "promote":
                        HandlePromoteEmployee();
                        break;
                    case "3":
                    case "terminate":
                        HandleTerminateEmployee();
                        break;
                    case "4":
                    case "details":
                        HandleGetEmployeeDetails();
                        break;
                    case "5":
                    case "list":
                        Console.WriteLine("List all employees feature not implemented yet.");
                        break;
                    case "6":
                    case "exit":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private void HandleHireEmployee()
    {
        Console.WriteLine("--- Hire New Employee ---");

        var model = new EmployeeDataModel();

        Console.Write("First Name: ");
        model.FirstName = Console.ReadLine();

        Console.Write("Last Name: ");
        model.LastName = Console.ReadLine();

        Console.Write("Social Security Number (XXX-XX-XXXX): ");
        model.SocialSecurityNumber = Console.ReadLine();

        Console.Write("Department (IT, Finance, HR, Marketing, Operation, Engineering): ");
        model.Department = Console.ReadLine();

        Console.Write("Salary: ");
        if (decimal.TryParse(Console.ReadLine(), out var salary))
        {
            model.Salary = salary;
        }
        else
        {
            Console.WriteLine("Invalid salary format.");
            return;
        }

        Console.Write("Hire Date (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var hireDate))
        {
            model.HireDate = hireDate;
        }
        else
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        try
        {
            var employeeId = _hrService.HireEmployee(model);
            Console.WriteLine($"Employee hired successfully! Employee ID: {employeeId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to hire employee: {ex.Message}");
        }
    }

    private void HandlePromoteEmployee()
    {
        Console.WriteLine("--- Promote Employee ---");

        var model = new EmployeeDataModel();

        Console.Write("Employee ID: ");
        model.EmployeeId = Console.ReadLine();

        Console.Write("New Title/Reason for promotion: ");
        var newTitle = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            Console.WriteLine("New title cannot be empty.");
            return;
        }

        Console.Write("Salary Increase Percentage (e.g., 0.10 for 10%): ");
        if (!decimal.TryParse(Console.ReadLine(), out var increase))
        {
            Console.WriteLine("Invalid percentage format.");
            return;
        }

        try
        {
            _hrService.PromoteEmployee(model, newTitle, increase);
            Console.WriteLine("Employee promoted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to promote employee: {ex.Message}");
        }
    }

    private void HandleTerminateEmployee()
    {
        Console.WriteLine("--- Terminate Employee ---");

        var model = new EmployeeDataModel();

        Console.Write("Employee ID: ");
        model.EmployeeId = Console.ReadLine();

        Console.Write("Termination Reason: ");
        var reason = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(reason))
        {
            Console.WriteLine("Termination reason cannot be empty.");
            return;
        }

        try
        {
            _hrService.TerminateEmployee(model, reason);
            Console.WriteLine("Employee terminated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to terminate employee: {ex.Message}");
        }
    }

    private void HandleGetEmployeeDetails()
    {
        Console.WriteLine("--- Get Employee Details ---");

        Console.Write("Employee ID: ");
        var employeeId = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            Console.WriteLine("Employee ID cannot be empty.");
            return;
        }

        try
        {
            var details = _hrService.GetEmployeeDetails(employeeId);
            Console.WriteLine("Employee Details:");
            Console.WriteLine($"ID: {details.EmployeeId}");
            Console.WriteLine($"Name: {details.FirstName} {details.LastName}");
            Console.WriteLine($"Department: {details.Department}");
            Console.WriteLine($"Salary: {details.Salary:C}");
            Console.WriteLine($"Hire Date: {details.HireDate:yyyy-MM-dd}");
            Console.WriteLine($"SSN: {details.SocialSecurityNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get employee details: {ex.Message}");
        }
    }
}