using Microsoft.Extensions.Logging;

namespace CSharpSolid.Core.Library;

/// <summary>
/// Adapter to bridge our simple logger with Microsoft.Extensions.Logging
/// This allows us to use the existing HRManagementService without modifications
/// </summary>
public class LoggerAdapter<T> : ILogger<T>
{
    private readonly ISimpleLogger _simpleLogger;

    public LoggerAdapter(ISimpleLogger simpleLogger)
    {
        _simpleLogger = simpleLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        switch (logLevel)
        {
            case LogLevel.Error:
            case LogLevel.Critical:
                if (exception != null)
                    _simpleLogger.LogError(exception, message);
                else
                    _simpleLogger.LogError(message);
                break;
            case LogLevel.Information:
            case LogLevel.Warning:
            case LogLevel.Debug:
            case LogLevel.Trace:
                _simpleLogger.LogInformation(message);
                break;
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}