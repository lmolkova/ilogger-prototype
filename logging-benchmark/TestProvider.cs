using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace logging_benchmark;

class TestProvider : ILoggerProvider
{
    public static ILogger<T> CreateLogger<T>(LogLevel minLevel)
    {
        var filters = new LoggerFilterOptions();
        filters.AddFilter(typeof(T).FullName, minLevel);

        var factory = new LoggerFactory([new TestProvider()], filters);

        return factory.CreateLogger<T>();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger();
    }

    public void Dispose()
    {
    }

    private class TestLogger : ILogger
    {
        internal IExternalScopeProvider? ScopeProvider { get; set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state) ?? NoopScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var formattedString = formatter(state, exception);
            if (formattedString.Length > 1)
            {
                Debug.WriteLine(formattedString);
            }
        }
    }

    private class NoopScope : IDisposable
    {
        public static NoopScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}