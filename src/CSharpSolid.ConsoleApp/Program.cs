using CSharpSolid.Oop.Encapsulation;
using CSharpSolid.Core.Library;
using CSharpSolid.Oop;

namespace CSharpSolid.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        // Run OOP Principles Demo
        OOPPrinciplesDemo.RunAllDemos();

        // Run Dependency Injection Demo
        DependencyInjectionDemo.RunAllDemos();

        // Run Composition Demo
        // CompositionAppDemo();

        // Run HR Management Demo
        // HRAppDemo();
    }

    private static void CompositionAppDemo()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("Press any key to continue to Composition demo...");
        Console.ReadKey();

        CompositionBasicDemo.Run();
    }

    private static void HRAppDemo()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("Press any key to continue to HR Management demo...");
        Console.ReadKey();

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
        container.AddSingleton<ISimpleLogger, CSharpSolid.Core.Library.ConsoleLogger>();

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