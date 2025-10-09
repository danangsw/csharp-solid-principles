namespace CSharpSolid.Oop.Encapsulation;

// BAD EXAMPLE: No encapsulation, all fields are public, 
// no validation or control over data access, no protection of internal state. 

public class BadEmployee
{
    // Public fields - no protection or validation!
    public string FirstName;
    public string LastName;
    public decimal Salary;
    public DateTime HireDate;
    public string Department;
    public List<string> Projects;
    public bool IsActive;
    public string SocialSecurityNumber;
    public decimal BonusPercentage;
}

public class PayrollSystem
{
    public void ProcessPayroll(List<BadEmployee> employees)
    {
        foreach (var employee in employees)
        {
            // Direct field access - dangerous!
            if (employee.IsActive)
            {
                // No validation - can set negative salary!
                employee.Salary = -1000;

                // Can modify sensitive data directly
                employee.SocialSecurityNumber = "123-45-6789";

                // Can corrupt data
                employee.Projects = null;
                employee.Department = "";

                // Logic scattered everywhere
                var bonus = employee.Salary * employee.BonusPercentage;
                var totalPay = employee.Salary + bonus;
            }
        }
    }
}

// Usage shows the problems
public class HRManager
{
    public void HireEmployee()
    {
        var employee = new BadEmployee();

        // No validation or business rules!
        employee.Salary = -50000; // Negative salary!
        employee.HireDate = DateTime.Now.AddYears(1); // Future hire date!
        employee.BonusPercentage = 500; // 50000% bonus!
        employee.SocialSecurityNumber = "invalid"; // Invalid format!
        employee.IsActive = true;
        employee.Projects = null; // Null reference waiting to happen!
    }
}
