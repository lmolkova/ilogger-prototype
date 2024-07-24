﻿using Azure.Core;
using Microsoft.Extensions.Logging;

namespace test;

internal partial class AzureCoreHttpLogger
{
    private const int RequestEvent = 1;

    private readonly ILogger _logger;
    public AzureCoreHttpLogger(ILoggerFactory loggerFactory, string assemblyName)
    {
        _logger = loggerFactory.CreateLogger(assemblyName + ".Http");
    }

    public void LogRequest(Request request)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            Request(_logger, request.ClientRequestId, request.Method.ToString(), request.Uri.ToString(), request.Headers.Select(x => new KeyValuePair<string, string>(x.Name, x.Value)).ToList());
            //Request(_logger, new RequestLoggingProperties(request));
        }
    }

    [LoggerMessage(EventId = RequestEvent, Level = LogLevel.Information, Message = "HTTP Request clientRequestId={clientRequestId}, method={method}, uri={uri}, headers={headers}")]
    private static partial void Request(ILogger logger, string clientRequestId, string method, string uri, IEnumerable<KeyValuePair<string, string>> headers);

    /*[LoggerMessage(EventId = RequestEvent, Level = LogLevel.Information, Message = "HTTP Request")]
    private static partial void Request(ILogger logger, [LogProperties] RequestLoggingProperties props);

    [EventData]
    readonly struct RequestLoggingProperties
    {
        public string ClientRequestId { get; }
        public string RequestMethod { get; }
        public string Uri { get; }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; }
        public RequestLoggingProperties(Request request)
        {
            ClientRequestId = request.ClientRequestId;
            RequestMethod = request.Method.ToString();
            Uri = request.Uri.ToString(); // sanitize
            Headers = request.Headers.Select(x => new KeyValuePair<string, string>(x.Name, x.Value)).ToList(); //sanitize
        }
    }*/
}