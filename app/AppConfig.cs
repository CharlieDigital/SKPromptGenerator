using System.Reflection;
using Microsoft.Extensions.Configuration;

internal static class AppConfig
{
  public const string SupportedGpt35TurboModel = "gpt-3.5-turbo-1106";

  public static IConfiguration Configuration { get; } = CreateConfiguration();

  public static string AzureOpenAIKey => Configuration["AzureOpenAIKey"] ?? "MISSING_KEY";

  public static string AzureOpenAIEndpoint =>
    Configuration["AzureOpenAIEndpoint"] ?? "MISSING_ENDPOINT";

  public static string AzureOpenAIDeployment =>
    Configuration["AzureOpenAIDeployment"] ?? "MISSING_DEPLOYMENT";

  private static IConfiguration CreateConfiguration()
  {
    return new ConfigurationBuilder()
      .AddJsonFile("appconfig.json")
      .AddUserSecrets(Assembly.GetExecutingAssembly())
      .Build();
  }
}
