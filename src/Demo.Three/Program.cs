using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using dotenv.net;
using Microsoft.Agents.AI;
using OpenAI.Chat;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.4-mini";

AIAgent writer = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a specialist in linkedin post writer", name: "writer");
    
var session = await writer.CreateSessionAsync();

var post = await writer.RunAsync("I need to write a post about minimal api .net just return the post in markdown do not add suggestions", session);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(post);
Console.ResetColor();
Console.WriteLine();

AIAgent reviewer = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a specialist in review linkedin post and suggest changes, dont add suggestions after the review", name: "reviewer");

var reviewSession = await reviewer.CreateSessionAsync();

var reviewPost = await reviewer.RunAsync(post.Text, reviewSession);

Console.WriteLine(reviewPost);