# Demo 4 — Writer / Reviewer Feedback Loop

## What it demonstrates

- A **manual feedback loop** between two agents that iterates until quality is achieved
- The writer starts by intentionally producing a bad post
- The reviewer classifies the post as `GOOD`, `EXCELLENT`, or `NEEDS_WORK`
- If the post needs work, the writer receives the original post **plus** the reviewer's feedback and improves it
- The loop continues until the reviewer approves the post

---

## How it works

```
  writer.RunAsync("Write a bad post...")
           │
           ▼
  ┌──────────────────────────────────────────────┐
  │  reviewer.RunAsync(post)                      │
  │                                               │
  │  classification == GOOD / EXCELLENT?          │
  │        YES ──► "Post approved!" ──► exit      │
  │        NO  ──► writer.RunAsync(post + review) │
  │                      │                        │
  │                      └─────────────────────►  │  (loop)
  └──────────────────────────────────────────────┘
```

1. The **writer** agent generates an intentionally poor post.
2. The **reviewer** agent evaluates it. Its response must include `GOOD`, `EXCELLENT`, or `NEEDS_WORK`.
3. If the classification is `GOOD` or `EXCELLENT`, the program prints "Post approved!" and exits.
4. Otherwise, the writer is called again with the previous post and the reviewer's critique, and the cycle repeats.

> **Note:** The reviewer and writer in this demo run **stateless** (no persistent session per call inside the loop).
> Compare with [Demo.Five](../Demo.Five/README.md) where the same pattern is expressed as a declarative workflow graph.

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
cd src/Demo.Four
dotnet run
```

---

## Key Concepts

| Concept | Notes |
|---|---|
| Quality gate loop | `while(true)` exits only when reviewer returns `GOOD` / `EXCELLENT` |
| Classification via text search | `reviewPost.Text.Contains("GOOD")` |
| Stateless agent calls in loop | `reviewer.RunAsync(post.Text)` — no session passed |
| Iterative improvement | Writer receives original post + review as combined prompt |

