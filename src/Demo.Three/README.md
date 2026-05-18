# Demo 3 — Two Agents: Writer + Reviewer (Single Pass)

## What it demonstrates

- Orchestrating **two independent agents** that collaborate in a pipeline
- The **writer** agent creates a LinkedIn post
- The **reviewer** agent critiques the post and suggests improvements
- Each agent has its own session and focused system instructions
- Colored console output to visually distinguish each agent's contribution

---

## How it works

```
  ┌──────────────────────────────────────────────────────┐
  │  1.  writer.RunAsync("Write a post about Minimal API") │
  │              ──► post (Markdown, green output)         │
  │                                                        │
  │  2.  reviewer.RunAsync(post)                           │
  │              ──► review with improvement suggestions   │
  └──────────────────────────────────────────────────────┘
```

1. A **writer** agent is created with instructions to produce a clean Markdown post (no meta-commentary).
2. The agent generates the post.  Output is printed in **green**.
3. A separate **reviewer** agent receives the post text as its prompt.
4. The reviewer returns a critique with suggested changes.

This demo is intentionally simple — there is **no loop**; the reviewer's feedback is printed but
the post is not automatically revised. See [Demo.Four](../Demo.Four/README.md) for the iterative version.

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
cd src/Demo.Three
dotnet run
```

---

## Key Concepts

| Concept | Notes |
|---|---|
| Multiple independent agents | Each agent has its own `AzureOpenAIClient` and session |
| Focused system prompts | Writer: produce Markdown only. Reviewer: critique only |
| Sequential orchestration | Output of agent A becomes the input of agent B |
| Console color coding | `ConsoleColor.Green` for writer output |

