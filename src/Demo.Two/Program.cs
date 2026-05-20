using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Demo.Two;
using dotenv.net;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.4-mini";

AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a specialist in linkedin post writer that creates a post based on a link.", name: "writer",
        tools: new List<AITool>
        {
            new WebBrowsingTool(new WebBrowsingToolOptions() { AllowPublicNetworks = true})
        });
    
var session = await agent.CreateSessionAsync();

//TODO Create an sample webpage in github to access. 
Console.WriteLine(await agent.RunAsync("I need to write a post about minimal api .net using this link as sample https://www.treinaweb.com.br/blog/asp-net-conhecendo-as-minimal-api", session));