using Azure.Core;
using Azure.Core.Pipeline;
using test;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

var factory = LoggerFactory.Create(builder => builder
.AddEventSourceLogger()
.AddOpenTelemetry(c => c.AddConsoleExporter())
//.AddConsole()
);

var loggingPolicy = new AzureLoggingPolicy(new AzureCoreHttpLogger(factory, "OpenAI"));
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