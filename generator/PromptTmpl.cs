namespace SKPromptGenerator;

/// <summary>
/// Stores the encountered template configurations.
/// </summary>
public record struct PromptTmpl(string Namespace, string Name, string Tmpl, string[] ConfigValues)
{
  public readonly string MaxTokens
  {
    get { return ConfigValues.Length < 1 ? "500" : ConfigValues[0]; }
  }

  public readonly string Temperature
  {
    get { return ConfigValues.Length < 2 ? "0.5" : ConfigValues[1]; }
  }

  public readonly string TopP
  {
    get { return ConfigValues.Length < 3 ? "0" : ConfigValues[2]; }
  }
}
