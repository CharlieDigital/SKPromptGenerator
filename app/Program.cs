using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SKPromptGenerator.App;

Console.WriteLine("Running test app...");
Console.WriteLine($"Using key: {AppConfig.AzureOpenAIKey[..8]}****");
Console.WriteLine($"Using endpoint: {AppConfig.AzureOpenAIEndpoint[..24]}****");
Console.WriteLine($"Using deployment: {AppConfig.AzureOpenAIDeployment}");

var kernel = Kernel
  .CreateBuilder()
  .AddAzureOpenAIChatCompletion(
    deploymentName: AppConfig.AzureOpenAIDeployment,
    endpoint: AppConfig.AzureOpenAIEndpoint,
    apiKey: AppConfig.AzureOpenAIKey
  )
  .Build();

var trenton = await new CapitolPrompt("NJ", "USA").ExecuteAsync(kernel);

Console.WriteLine(trenton);

var albany = await new CapitolPrompt("NY", "USA").ExecuteAsync(kernel);

Console.WriteLine(albany);

// Here, we are using a custom prompt template base class.
var response = await new CapitolCustomPrompt("DOESN'T", "MATTER").ExecuteAsync(kernel);

Console.WriteLine($"{response}");

var (sacramento, json) = await new CapitolJsonPrompt(
  "CA",
  "US"
).ExecuteWithJsonAsync<CapitolResponse>(kernel);

Console.WriteLine($"The capitol of {sacramento?.State} is {sacramento?.Capitol}");

var njCities = await new CitiesPrompt(4, "NJ", "USA").ExecuteAsync(kernel);

Console.WriteLine(njCities);

var njCitiesCaps = await new CitiesWithCapitalizationPrompt(
  4,
  "NJ",
  "USA",
  CapitalizationOptions.AllCaps
).ExecuteAsync(kernel);

Console.WriteLine(njCitiesCaps);

public abstract class CustomBase : SKPromptGenerator.PromptTemplateBase
{
  public override async Task<string> ExecuteAsync(
    Kernel kernel,
    string? serviceId = null,
    Action<ChatHistory>? historyBuilder = null,
    CancellationToken cancellation = default
  )
  {
    return await Task.FromResult("A fake response");
  }
}

public record CapitolResponse(string Country, string State, string Capitol);
