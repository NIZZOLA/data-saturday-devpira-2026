using Azure.AI.OpenAI;
using Azure.Identity;
using Demo.Five;
using dotenv.net;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using OpenAI.Chat;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

// ── Agents ─────────────────────────────────────────────────────────────────
AIAgent writerAgent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "You are a LinkedIn post specialist. Write posts in markdown. " +
                      "When improving, apply the feedback directly — no meta-commentary or suggestions.",
        name: "writer");

AIAgent reviewerAgent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: """
            You are a LinkedIn post reviewer.
            Always respond ONLY with a valid JSON object — no other text.
            Schema: { "classification": "Good" | "Excellent" | "NeedsWork", "feedback": "<concise feedback>" }
            """,
        name: "reviewer");

// ── Executors ──────────────────────────────────────────────────────────────
var writerExecutor   = new WriterExecutor(writerAgent);
var reviewerExecutor = new ReviewerExecutor(reviewerAgent);

// ── Workflow graph ─────────────────────────────────────────────────────────
//
//   ┌─────────┐   AgentResponse   ┌──────────┐
//   │  writer │ ────────────────► │ reviewer │
//   │         │                   │          │
//   │         │ ◄──────────────── │          │  (only when NeedsWork)
//   └─────────┘   ReviewResult    └──────────┘
//
var workflow = new WorkflowBuilder(writerExecutor)
    .AddEdge(writerExecutor, reviewerExecutor)
    .AddEdge<ReviewResult>(
        reviewerExecutor,
        writerExecutor,
        result => result?.Classification == ReviewClassification.NeedsWork,
        label: "needs-work")
    .WithOutputFrom(writerExecutor, reviewerExecutor)
    .Build();

// ── Run ────────────────────────────────────────────────────────────────────
Console.WriteLine("🚀 LinkedIn Post — Writer / Reviewer Workflow");
Console.WriteLine(new string('─', 60));

await using var run = await InProcessExecution.RunStreamingAsync(
    workflow,
    "Write an intentionally bad LinkedIn post about Minimal API in .NET. " +
    "Just return the post in markdown.");

await foreach (var evt in run.WatchStreamAsync())
{
    switch (evt)
    {
        case WorkflowOutputEvent { Data: AgentResponse post }:
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[writer]");
            Console.ResetColor();
            Console.WriteLine(post.Text);
            Console.WriteLine();
            break;

        case WorkflowOutputEvent { Data: ReviewResult review }:
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[reviewer]  classification: {review.Classification}");
            Console.ResetColor();

            if (review.Classification == ReviewClassification.NeedsWork)
            {
                Console.WriteLine($"\n           feedback: {review.Feedback}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("↻  Sending back to writer...");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ✅  Post approved!");
                Console.ResetColor();
            }

            Console.WriteLine();
            break;
    }
}

