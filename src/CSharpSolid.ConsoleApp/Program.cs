using CSharpSolid.Oop.Encapsulation;
using CSharpSolid.Core.Library;

namespace CSharpSolid.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        // Setup our custom IoC container
        var container = ConfigureServices();

        var hrService = container.GetService<HRManagementService>();
        var consoleApp = new HRConsoleApp(hrService);

        consoleApp.Run();
    }

    private static SimpleIoCContainer ConfigureServices()
    {
        var container = new SimpleIoCContainer();

        // Register our simple logger
        container.AddSingleton<ISimpleLogger, ConsoleLogger>();

        // Register logger adapter for HRManagementService
        container.AddTransient(typeof(Microsoft.Extensions.Logging.ILogger<HRManagementService>),
            typeof(LoggerAdapter<HRManagementService>));

        // Register time provider
        container.AddSingleton<CSharpSolid.Oop.Encapsulation.ITimeProvider,
            CSharpSolid.Oop.Encapsulation.SystemTimeProvider>();

        // Register HR Management Service
        container.AddSingleton<HRManagementService, HRManagementService>();

        return container;
    }
}