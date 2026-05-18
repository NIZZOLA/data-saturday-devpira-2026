using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace Demo.Five;

internal sealed class ReviewerExecutor(AIAgent agent) : Executor("reviewer")
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        protocolBuilder.ConfigureRoutes(routes =>
        {
            routes.AddHandler<AgentResponse, ReviewResult>(HandleAsync);
        });
        protocolBuilder.SendsMessage<ReviewResult>();
        protocolBuilder.YieldsOutput<ReviewResult>();
        return protocolBuilder;
    }

    // Reviews the writer's output and returns a structured ReviewResult
    private async ValueTask<ReviewResult> HandleAsync(
        AgentResponse writerOutput, IWorkflowContext context, CancellationToken ct)
    {
        var response = await agent.RunAsync(writerOutput.Text, cancellationToken: ct);

        try
        {
            return JsonSerializer.Deserialize<ReviewResult>(response.Text, JsonOptions)
                   ?? Fallback(response.Text);
        }
        catch
        {
            return Fallback(response.Text);
        }
    }

    private static ReviewResult Fallback(string rawText) =>
        new() { Classification = ReviewClassification.NeedsWork, Feedback = rawText };
}