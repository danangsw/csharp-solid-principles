using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CSharpSolid.Oop.Encapsulation;

namespace CSharpSolid.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        // Setup dependency injection
        var serviceProvider = ConfigureServices();

        var hrService = serviceProvider.GetRequiredService<HRManagementService>();
        var consoleApp = new HRConsoleApp(hrService);

        consoleApp.Run();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(configure => configure.AddConsole());

        // Add HR Management Service
        services.AddSingleton<HRManagementService>();

        return services.BuildServiceProvider();
    }
}