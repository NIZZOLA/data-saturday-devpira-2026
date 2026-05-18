# Data Saturday DevPira 2026 — Demos

A collection of progressive demos showcasing how to build **AI Agents** with the
[Microsoft Agents SDK for .NET](https://github.com/microsoft/agents) (`Microsoft.Agents.AI`) backed by **Azure OpenAI**.

The demos walk through a concrete scenario — generating and reviewing a LinkedIn post about .NET Minimal APIs — and
gradually introduce more sophisticated multi-agent patterns with each step.

---

## Prerequisites

| Requirement | Version / Notes |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | **10.0** or later |
| [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) | For `DefaultAzureCredential` authentication |
| Azure OpenAI resource | A deployed chat model (e.g. `gpt-4o-mini`) |

### Azure Authentication

The demos use [`DefaultAzureCredential`](https://learn.microsoft.com/dotnet/azure/sdk/authentication/credential-chains#defaultazurecredential-overview).
The easiest way to authenticate locally is via the Azure CLI:

```bash
az login
```

---

## Environment Setup

Each project reads its configuration from a **`.env`** file placed in the project folder.
Create a `.env` file inside each `src/Demo.*/` directory (or copy the template below):

```env
AZURE_OPENAI_ENDPOINT=https://<your-resource-name>.openai.azure.com/
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o-mini
```

| Variable | Description |
|---|---|
| `AZURE_OPENAI_ENDPOINT` | Full URL of your Azure OpenAI resource |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | Name of the chat model deployment to use |

> **Tip:** You can create a single `.env` file at the solution root and symlink / copy it into each project folder, or set the variables as shell environment variables instead.

---

## Running a Demo

```bash
cd src/Demo.<Number>
dotnet run
```

---

## Demo Overview

| # | Project | What it demonstrates |
|---|---|---|
| 1 | [Demo.One](src/Demo.One/README.md) | Single agent with **session management** — serialize and resume a conversation |
| 2 | [Demo.Two](src/Demo.Two/README.md) | Agent augmented with a **web browsing tool** — fetches a URL and uses its content |
| 3 | [Demo.Three](src/Demo.Three/README.md) | **Two agents** (writer + reviewer) working in sequence — single-pass pipeline |
| 4 | [Demo.Four](src/Demo.Four/README.md) | Writer / reviewer **feedback loop** — iterates until the post is approved |
| 5 | [Demo.Five](src/Demo.Five/README.md) | Full **workflow graph** with streaming output using `WorkflowBuilder` |

---

## Solution Structure

```
data-saturday-devpira-2026.sln
└── src/
    ├── Demo.One/     # Single agent + session serialization
    ├── Demo.Two/     # Agent + WebBrowsingTool
    ├── Demo.Three/   # Writer agent → Reviewer agent (single pass)
    ├── Demo.Four/    # Writer ↔ Reviewer loop (manual)
    └── Demo.Five/    # WorkflowBuilder graph with streaming
```

---

## Key NuGet Packages

| Package | Purpose |
|---|---|
| `Microsoft.Agents.AI` | Core agent abstractions (`AIAgent`, sessions, tools) |
| `Microsoft.Agents.AI.OpenAI` | Azure OpenAI / OpenAI backend for `AIAgent` |
| `Microsoft.Agents.AI.Workflows` | `WorkflowBuilder` graph engine (Demo.Five) |
| `Azure.AI.OpenAI` | Azure OpenAI client |
| `Azure.Identity` | `DefaultAzureCredential` |
| `dotenv.net` | Loads `.env` files at startup |

---

## License

[MIT](LICENSE)
