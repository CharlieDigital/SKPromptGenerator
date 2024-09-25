namespace SKPromptGenerator.App;

public static class Prompts
{
  [PromptTemplate]
  public const string Capitol = """
    What is the capitol of {{$state}} {{$country}}?
    Respond directly in a single line
    When writing the state, always write it as the full name
    Write your output in the format: The capitol of <STATE> is: <CAPITOL>.
    For example: The capitol of California is: Sacramento.
    """;

  [PromptTemplate<CustomBase>]
  public const string CapitolCustom = """
    What is the capitol of {{$state}} {{$country}}?
    Respond directly in a single line
    When writing the state, always write it as the full name
    Write your output in the format: The capitol of <STATE> is: <CAPITOL>.
    For example: The capitol of California is: Sacramento.
    """;

  [PromptTemplate]
  public const string CapitolJson = """
    What is the capitol of {{$state}} {{$country}}?
    Respond directly in a single line
    When writing the state, always write it as the full name
    Write your output in the JSON format: { "country": "", "state": "", "capitol": "" }
    For example: { "country": "US", "state": "CA", "capitol": "Sacramento" }
    ONLY OUTPUT THE VALID JSON IN A SINGLE LINE; DON'T WASTE TOKENS ON WHITESPACE AND NEWLINES
    DO NOT OUTPUT MARKDOWN OR CODE FENCES
    """;

  [PromptTemplate]
  public const string Cities = """
    Write a list of {{$count:int}} cities in {{$region}}, {{$country}}
    Write each city on a separate line
    Start you response with: Sure, here are {{$count:int}} cities in {{$region}}, {{$country}}
    """;

  [PromptTemplate]
  public const string CitiesWithCapitalization = """
    Write a list of {{$count:int}} cities in {{$region}}, {{$country}}
    Write each city on a separate line
    Write each city with the following capitalization: {{$capitalization:CapitalizationOptions}}
    Start you response with: Sure, here are {{$count:int}} cities in {{$region}}, {{$country}}
    """;
}
