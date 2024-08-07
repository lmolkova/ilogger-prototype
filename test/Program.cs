﻿using Azure.Core;
using Azure.Core.Pipeline;
using test;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using Microsoft.Extensions.Logging.EventSource;

var factory = LoggerFactory.Create(builder => builder
.AddEventSourceLogger()
.AddOpenTelemetry(c => {
    c.AddConsoleExporter();
    c.IncludeFormattedMessage = false;
    c.ParseStateValues = true;
 })
//.AddConsole()
);

var loggingPolicy = new AzureLoggingPolicy(new AzureCoreHttpLogger("OpenAI"));
var pipeline = HttpPipelineBuilder.Build(new OpenAIClientOptions(), loggingPolicy);

var msg = pipeline.CreateMessage();
msg.Request.Method = RequestMethod.Get;
msg.Request.ClientRequestId = "42";
msg.Request.Uri = new RequestUriBuilder();
msg.Request.Uri.Reset(new Uri("https://microsoft.com"));

Console.WriteLine("Start collecting ETW traces if necessary and press any button to write some logs.");
Console.ReadLine();
await pipeline.SendAsync(msg, default);
Console.ReadLine();

class OpenAIClientOptions : ClientOptions
{

}