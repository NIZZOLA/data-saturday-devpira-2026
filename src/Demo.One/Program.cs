
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using dotenv.net;
using Microsoft.Agents.AI;
using OpenAI.Chat;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.4-mini";

AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a specialist in linkedin post writer", name: "writer");
    
var session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("I need to write a post about minimal api .net", session));

var serializedSession = await agent.SerializeSessionAsync(session);

Console.WriteLine("\n--- Serialized session ---\n");
Console.WriteLine(JsonSerializer.Serialize(serializedSession, new JsonSerializerOptions { WriteIndented = true }) + "\n");

var resumedSession = await agent.DeserializeSessionAsync(serializedSession);

Console.WriteLine(await agent.RunAsync("Now I need to compare with Controllers", resumedSession));
