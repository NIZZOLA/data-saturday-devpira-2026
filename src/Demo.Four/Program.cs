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
    .AsAIAgent(instructions: "You are a specialist in linkeind post writer you can get an initial post or just the topic suggestion and create post in markdown. dont add suggestions", name: "writer");
    
var session = await writer.CreateSessionAsync();



AIAgent reviewer = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a specialist in review linkeind post and suggest changes and give an classification GOOD, EXCELLENT, NEEDS_WORK, dont add suggestions after the review", name: "reviewer");

var post = await writer.RunAsync("I need to write a bad post about minimal api .net just return the post in markdown do not add suggestions", session);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(post);
Console.ResetColor();
Console.WriteLine();

while (true)
{
    var reviewPost = await reviewer.RunAsync(post.Text);
    
    if(reviewPost.Text.Contains("GOOD") || reviewPost.Text.Contains("EXCELLENT"))
    {
        Console.WriteLine("Post approved!");
        break;
    }

    Console.WriteLine("Post needs work, regenerating...");
    post = await writer.RunAsync("original post:" + post.Text + "/n analyse this review and make the original post better" + reviewPost.Text);
        
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(post);
    Console.ResetColor();
    Console.WriteLine();

}