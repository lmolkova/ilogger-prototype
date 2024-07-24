using Azure.Core.Diagnostics;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Tracing;

namespace logging_benchmark;

[MemoryDiagnoser]
public partial class LoggingBenchmarks
{
    private static readonly Dictionary<string, string> Headers = new() { ["User-Agent"] = "foo", ["Date"] = "bar" };

    private static readonly TestEventSource Source = new ();
    private static readonly AzureEventSourceListener Listener = new AzureEventSourceListener((a, s) => { }, EventLevel.Warning);

    private static readonly ILogger<LoggingBenchmarks> Logger = TestProvider.CreateLogger<LoggingBenchmarks>(LogLevel.Warning);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "HTTP Request clientRequestId={clientRequestId}, method={method}, uri={uri}, headers={headers}")]
    public static partial void LogRequestAsInfo(ILogger logger, string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "HTTP Request clientRequestId={clientRequestId}, method={method}, uri={uri}, headers={headers}")]
    private static partial void LogRequestAsWarn(ILogger logger, string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers);

    [Benchmark]
    public void EnabledEventSourceWarn()
    {
        Source.Warn("clientRequest42", "GET", "https://microsoft.com", Headers);
    }

    [Benchmark]
    public void DisabledEventSourceInfo()
    {
        Source.Info("clientRequest42", "GET", "https://microsoft.com", Headers);
    }

    [Benchmark]
    public void EnabledHighPerfLogWarn()
    {
        LogRequestAsWarn(Logger, "clientRequest42", "GET", "https://microsoft.com", Headers);
    }

    [Benchmark]
    public void DisabledHighPerfLogInfo()
    {
        LogRequestAsInfo(Logger, "clientRequest42", "GET", "https://microsoft.com", Headers);
    }

    private class TestEventSource : AzureEventSource
    {
        public TestEventSource() : base("Azure-Core") { }

        [NonEvent]
        public void Warn(string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (IsEnabled(EventLevel.Warning, EventKeywords.None))
            {
                LogRequestAsWarn(clientRequestId, method, uri, headers);
            }
        }

        [NonEvent]
        public void Info(string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (IsEnabled(EventLevel.Informational, EventKeywords.None))
            {
                LogRequestAsInfo(clientRequestId, method, uri, headers);
            }
        }

        [Event(1, Level = EventLevel.Warning)]
        public void LogRequestAsWarn(string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            WriteEvent(1, clientRequestId, method, uri, headers);
        }

        [Event(2, Level = EventLevel.Informational)]
        public void LogRequestAsInfo(string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            WriteEvent(2, clientRequestId, method, uri, headers);
        }
    }
}