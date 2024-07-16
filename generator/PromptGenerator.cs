using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace SKPromptGenerator;

/// <summary>
/// This is our SKPromptGenerator that actually outputs the source code.
/// </summary>
[Generator]
public partial class PromptGenerator : ISourceGenerator
{
  private static readonly Regex TokenPattern = new(@"\{\{\$([^\}]+)\}\}", RegexOptions.Multiline);

  /// <summary>
  /// We hook up our receiver here so that we can access it later.
  /// </summary>
  public void Initialize(GeneratorInitializationContext context)
  {
    context.RegisterForSyntaxNotifications(() => new PromptSyntaxReceiver());
  }

  /// <summary>
  /// And consume the receiver here.
  /// </summary>
  public void Execute(GeneratorExecutionContext context)
  {
    var receiver = context.SyntaxContextReceiver as PromptSyntaxReceiver;

    if (receiver == null)
    {
      return;
    }

    var prompts = receiver.Prompts;

    foreach (var prompt in prompts)
    {
      var parameters = string.Join(
        ", ",
        TokenPattern.Matches(prompt.Tmpl).Select(m => $"string {m.Groups.Values.Last()}").Distinct()
      );

      var tmpl = TokenPattern.Replace(prompt.Tmpl.Trim(), "{{$1}}");

      var src = $$""""
        using System;
        using Microsoft.SemanticKernel.Connectors.OpenAI;
        using SKPromptGenerator;

        namespace {{prompt.Namespace}};

        public partial class {{prompt.Name}}Prompt(
          {{parameters}}
        ) : {{prompt.BaseClass}}
        {
          public override string Text => $$"""
        {{tmpl}}
        """;

          public override OpenAIPromptExecutionSettings Settings => new OpenAIPromptExecutionSettings
          {
            MaxTokens = {{prompt.MaxTokens}},
            Temperature = {{prompt.Temperature}}d,
            TopP = {{prompt.TopP}}d,
          };
        }
        """";

      context.AddSource($"{prompt.Name}Prompt.g.cs", src);
    }
  }
}
