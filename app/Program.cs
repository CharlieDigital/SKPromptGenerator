using App;
using Microsoft.SemanticKernel;

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

Console.WriteLine($"{trenton}");

var albany = await new CapitolPrompt("NY", "USA").ExecuteAsync(kernel);

Console.WriteLine($"{albany}");
