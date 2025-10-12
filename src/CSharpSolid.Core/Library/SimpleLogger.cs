namespace CSharpSolid.Core.Library;

/// <summary>
/// Simple logger interface and implementation for demonstration
/// </summary>
public interface ISimpleLogger
{
    void LogError(string message);
    void LogError(Exception ex, string message);
    void LogInformation(string message);
}

public class ConsoleLogger : ISimpleLogger
{
    public void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    public void LogError(Exception ex, string message)
    {
        Console.WriteLine($"[ERROR] {message}: {ex.Message}");
    }

    public void LogInformation(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }
}