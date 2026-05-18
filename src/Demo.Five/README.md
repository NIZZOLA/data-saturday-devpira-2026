# Demo 5 — Workflow Graph with Streaming Output

## What it demonstrates

- Declaring an **agent workflow as a directed graph** using `WorkflowBuilder`
- A **conditional edge** that routes output from the reviewer back to the writer only when the post `NeedsWork`
- Strongly-typed, structured review results as a `ReviewResult` JSON object
- **Executor** classes (`WriterExecutor`, `ReviewerExecutor`) that encapsulate agent logic and routing
- **Streaming output** via `InProcessExecution.RunStreamingAsync` and `WatchStreamAsync`
- Workflow **state management** to carry the last post between loop iterations

---

## How it works

```
  Workflow input (string prompt)
          │
          ▼
  ┌────────────────┐   AgentResponse   ┌─────────────────┐
  │ WriterExecutor │ ────────────────► │ ReviewerExecutor │
  │                │                  │                  │
  │                │ ◄──────────────── │  (NeedsWork only)│
  └────────────────┘   ReviewResult   └─────────────────┘
          │                                    │
    (output)                             (output)
```

### Edges

| From | To | Condition |
|---|---|---|
| `WriterExecutor` | `ReviewerExecutor` | always |
| `ReviewerExecutor` | `WriterExecutor` | `result.Classification == NeedsWork` |

1. The workflow starts by sending the user prompt to `WriterExecutor`.
2. `WriterExecutor.HandleInitialAsync` runs the writer agent and caches the post in workflow state.
3. The post is forwarded to `ReviewerExecutor`, which returns a strongly-typed `ReviewResult` JSON object.
4. If the classification is `NeedsWork`, the conditional edge sends the review back to `WriterExecutor.HandleFeedbackAsync`, which improves the post using the stored state and feedback.
5. The cycle continues until the classification is `Good` or `Excellent`, at which point the loop-back edge is not triggered and the workflow ends.

### Streaming output

`InProcessExecution.RunStreamingAsync` runs the workflow and emits events as each executor produces output.
`WatchStreamAsync` yields `WorkflowOutputEvent` instances that are pattern-matched to `AgentResponse` (writer) and `ReviewResult` (reviewer), printing each in a different color.

---

## Project files

| File | Purpose |
|---|---|
| `Program.cs` | Builds and runs the workflow |
| `WriterExecutor.cs` | Executor wrapping the writer agent; handles initial prompt and feedback loop-back |
| `ReviewerExecutor.cs` | Executor wrapping the reviewer agent; emits `ReviewResult` |
| `ReviewResult.cs` | Strongly-typed review DTO with `ReviewClassification` enum (`Good`, `Excellent`, `NeedsWork`) |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Azure OpenAI resource with a deployed chat model
- Azure CLI logged in (`az login`) for `DefaultAzureCredential`

---

## Configuration

Create a `.env` file in this directory:

```env
AZURE_OPENAI_ENDPOINT=https://<your-resource-name>.openai.azure.com/
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o-mini
```

---

## Running

```bash
cd src/Demo.Five
dotnet run
```

Expected output (colors vary by agent):

```
🚀 LinkedIn Post — Writer / Reviewer Workflow
────────────────────────────────────────────────────────────
[writer]
# Minimal APIs in .NET ...

[reviewer]  classification: NeedsWork
            feedback: The post lacks a call-to-action and code example.
↻  Sending back to writer...

[writer]
# Minimal APIs in .NET — A Game Changer ...

[reviewer]  classification: Excellent  ✅  Post approved!
```

---

## Key Concepts

| Concept | API |
|---|---|
| Declare workflow graph | `new WorkflowBuilder(startExecutor)` |
| Unconditional edge | `.AddEdge(from, to)` |
| Conditional edge | `.AddEdge<T>(from, to, predicate, label)` |
| Multiple output nodes | `.WithOutputFrom(executor1, executor2)` |
| Run with streaming | `InProcessExecution.RunStreamingAsync(workflow, input)` |
| Consume stream | `run.WatchStreamAsync()` → `WorkflowOutputEvent` |
| Workflow state | `context.QueueStateUpdateAsync` / `context.ReadStateAsync` |

