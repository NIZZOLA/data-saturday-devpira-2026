# Demo 2 — Agent with Web Browsing Tool

## What it demonstrates

- Extending an `AIAgent` with a custom **`AITool`** (`WebBrowsingTool`)
- The agent autonomously decides when to call the tool during a run
- The tool fetches a public URL, converts the HTML to Markdown, and returns it to the model
- The model uses the page content to write a grounded LinkedIn post

---

## How it works

```
User prompt (URL included)
        │
        ▼
  AIAgent (writer)
        │   decides to call tool
        ▼
  WebBrowsingTool.DownloadUriAsync(url)
        │   returns HTML → Markdown
        ▼
  AIAgent ──► Response (post grounded in the page)
```

1. An `AIAgent` is created with a `WebBrowsingTool` in its tool list.
2. The user prompt asks for a LinkedIn post and supplies a URL as source material.
3. The agent calls `WebBrowsingTool` to fetch and convert the page.
4. The model generates the post using the retrieved content.

### WebBrowsingTool

`WebBrowsingTool` (defined in `WebBrowsingTool.cs`) is a custom `AIFunction` that:

- Validates the URL scheme (`http` / `https` only)
- Enforces an access policy via `WebBrowsingToolOptions`:
  - `AllowPublicNetworks` — allow any public internet host
  - `AllowPrivateNetworks` — allow private/intranet hosts
  - `AllowedHosts` — explicit allow-list with wildcard support (`*.example.com`)
- Downloads the HTML with `HttpClient`
- Converts HTML to Markdown using a built-in regex-based converter

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Azure OpenAI resource with a deployed chat model
- Azure CLI logged in (`az login`) for `DefaultAzureCredential`
- Internet access (the demo fetches a public URL)

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
cd src/Demo.Two
dotnet run
```

---

## Key Concepts

| Concept | API / Class |
|---|---|
| Attach tools to an agent | `AsAIAgent(..., tools: new List<AITool> { ... })` |
| Custom tool | Extend `AIFunction` / use `AIFunctionFactory.Create` |
| Web access policy | `WebBrowsingToolOptions.AllowPublicNetworks = true` |
