## Small demo of using ILogger for Azure.Core and System.ClientModel purposes

### Overview

- users are expected to provide `LoggerFactory` (either explicitly or with dependency injection), if they don't provide it, we can create a default one with `EventSourceProvider`
- Client libraries can use ILogger directly, but should make sure they do it in performant way
- HTTP Logging policy will use `{Assembly.Name}.Http` as a logger name for HTTP requests
- In common case, users would enable a specific verbosity for that logger, e.g. `Warning`. Additional configuration and filtering can be done, but it's not part of ILogger or `LoggingEventSource`

### Benchmark results for EventSource vs ILogger for HTTP request log

|                  Method |       Mean |     Error |     StdDev |   Gen0 | Allocated |
|------------------------ |-----------:|----------:|-----------:|-------:|----------:|
|  EnabledEventSourceWarn | 322.963 ns | 6.3481 ns | 12.0779 ns | 0.0982 |    1544 B |
| DisabledEventSourceInfo |   1.336 ns | 0.0134 ns |  0.0125 ns |      - |         - |
|  EnabledHighPerfLogWarn | 316.275 ns | 6.2164 ns |  7.1588 ns | 0.0587 |     920 B |
| DisabledHighPerfLogInfo |   1.984 ns | 0.0472 ns |  0.0441 ns |      - |         - |

### How logs look like

#### Classic console provider

```log
info: OpenAI.Http[1]
      HTTP Request clientRequestId=42, method=GET, uri=https://microsoft.com/, headers={"x-ms-client-request-id"="42","x-ms-return-client-request-id"="true","User-Agent"="azsdk-net-test/1.0.0 (.NET 8.0.7; Microsoft Windows 10.0.22631)"}
```

#### OTel (console exporter)

In practice people would send logs preserving the structure somewhere else where data would be queryable by individual properties

```log
LogRecord.Timestamp:               2024-07-24T03:20:14.5481609Z
LogRecord.CategoryName:            OpenAI.Http
LogRecord.Severity:                Info
LogRecord.SeverityText:            Information
LogRecord.FormattedMessage:        HTTP Request clientRequestId=42, method=GET, uri=https://microsoft.com/, headers={"x-ms-client-request-id"="42","x-ms-return-client-request-id"="true","User-Agent"="azsdk-net-test/1.0.0 (.NET 8.0.7; Microsoft Windows 10.0.22631)"}
LogRecord.Body:                    HTTP Request clientRequestId=42, method=GET, uri=https://microsoft.com/, headers={"x-ms-client-request-id"="42","x-ms-return-client-request-id"="true","User-Agent"="azsdk-net-test/1.0.0 (.NET 8.0.7; Microsoft Windows 10.0.22631)"}
LogRecord.Attributes (Key:Value):
    OriginalFormat (a.k.a Body): HTTP Request clientRequestId={clientRequestId}, method={method}, uri={uri}, headers={headers}
    headers: {"x-ms-client-request-id"="42","x-ms-return-client-request-id"="true","User-Agent"="azsdk-net-test/1.0.0 (.NET 8.0.7; Microsoft Windows 10.0.22631)"}
    uri: https://microsoft.com/
    method: GET
    clientRequestId: 42
LogRecord.EventId:                 1
LogRecord.EventName:               Request
```

#### PerfView
