using SKPromptGenerator;

namespace This.Is.A.Namespace;

public static class Prompts
{
  [PromptTemplate]
  public const string TestNamespace = """
    What is the capitol of {{$state}} {{$country}}
    Respond directly on a single line.
    """;
}
