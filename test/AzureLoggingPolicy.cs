using Azure.Core;
using Azure.Core.Pipeline;

namespace test;

internal class AzureLoggingPolicy : HttpPipelinePolicy
{
    private AzureCoreHttpLogger _logger;
    public AzureLoggingPolicy(AzureCoreHttpLogger logger)
    {
        _logger = logger;
    }

    public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        _logger.LogRequest(message.Request);
        ProcessNext(message, pipeline);

    }

    public override ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        _logger.LogRequest(message.Request);
        return ProcessNextAsync(message, pipeline);
    }
}
