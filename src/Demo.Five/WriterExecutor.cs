using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace Demo.Five;

internal sealed class WriterExecutor(AIAgent agent) : Executor("writer")
{
    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        protocolBuilder.ConfigureRoutes(routes =>
        {
            routes.AddHandler<string, AgentResponse>(HandleInitialAsync);
            routes.AddHandler<ReviewResult, AgentResponse>(HandleFeedbackAsync);
        });
        protocolBuilder.SendsMessage<AgentResponse>();
        protocolBuilder.YieldsOutput<AgentResponse>();
        return protocolBuilder;
    }

    // Handles the initial user prompt (plain string from workflow input)
    private async ValueTask<AgentResponse> HandleInitialAsync(
        string message, IWorkflowContext context, CancellationToken ct)
    {
        var response = await agent.RunAsync(message, cancellationToken: ct);
        await context.QueueStateUpdateAsync("lastPost", response.Text, cancellationToken: ct);
        return response;
    }

    // Handles structured feedback from the reviewer (loop-back)
    private async ValueTask<AgentResponse> HandleFeedbackAsync(
        ReviewResult feedback, IWorkflowContext context, CancellationToken ct)
    {
        var lastPost = await context.ReadStateAsync<string>("lastPost", cancellationToken: ct) ?? string.Empty;

        var prompt =
            $"Improve the following LinkedIn post based on the feedback. " +
            $"Return ONLY the improved post in markdown — no suggestions, no commentary.\n\n" +
            $"Post:\n{lastPost}\n\n" +
            $"Feedback: {feedback.Feedback}";

        var response = await agent.RunAsync(prompt, cancellationToken: ct);
        await context.QueueStateUpdateAsync("lastPost", response.Text, cancellationToken: ct);
        return response;
    }
}