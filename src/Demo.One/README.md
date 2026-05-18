# Demo 1 — Single Agent with Session Management

## What it demonstrates

- Creating a single `AIAgent` (LinkedIn post writer) backed by **Azure OpenAI**
- Running the agent with a user prompt
- **Serializing** the conversation session to JSON
- **Deserializing** and resuming the session to continue the conversation in a new turn

This is the "Hello, Agent!" starting point for the series.

---

## How it works

```
User prompt ──► AIAgent (writer) ──► Response
                       │
               SerializeSessionAsync
                       │
               DeserializeSessionAsync
                       │
User prompt ──► Resumed session ──► Response
```

1. An `AIAgent` is created from an `AzureOpenAIClient` chat client with a system instruction.
2. A session is created with `CreateSessionAsync`.
3. The agent writes a LinkedIn post about .NET Minimal APIs.
4. The session is serialized to JSON and printed to the console.
5. The session is deserialized and the conversation continues with a follow-up prompt comparing Minimal APIs with Controllers.

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
cd src/Demo.One
dotnet run
```

---

## Key Concepts

| Concept | API |
|---|---|
| Create agent | `chatClient.AsAIAgent(instructions, name)` |
| Start a session | `agent.CreateSessionAsync()` |
| Run the agent | `agent.RunAsync(prompt, session)` |
| Serialize session | `agent.SerializeSessionAsync(session)` |
| Deserialize session | `agent.DeserializeSessionAsync(data)` |
