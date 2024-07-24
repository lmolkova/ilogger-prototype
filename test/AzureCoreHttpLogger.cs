using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventSource;
using System.Reflection;

namespace test;

internal partial class AzureCoreHttpLogger
{
    // TODO this is not so simple - EventSourceLogger is defined in Microsoft.Extensions.Logging.EventSource
    private static readonly ILoggerFactory DefaultFactory = LoggerFactory.Create(b => b.AddEventSourceLogger());
    private const int RequestEvent = 1;

    private readonly ILogger _logger;

    public AzureCoreHttpLogger(string assemblyName) : this(DefaultFactory, assemblyName)
    {

    }

    public AzureCoreHttpLogger(ILoggerFactory loggerFactory, string assemblyName)
    {
        _logger = loggerFactory.CreateLogger(assemblyName + ".Http");
    }

    public void LogRequest(Request request)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            Request(_logger, request.ClientRequestId, request.Method.ToString(), request.Uri.ToString(), request.Headers.Select(x => new KeyValuePair<string, string>(x.Name, x.Value)).ToArray());
        }
    }

    [LoggerMessage(EventId = RequestEvent, Level = LogLevel.Information, Message = "HTTP Request clientRequestId={clientRequestId}, method={method}, uri={uri}, headers={headers}")]
    private static partial void Request(ILogger logger, string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers);
}
