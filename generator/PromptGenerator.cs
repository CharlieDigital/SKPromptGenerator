using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace SKPromptGenerator;

/// <summary>
/// This is our SKPromptGenerator that actually outputs the source code.
/// </summary>
[Generator]
public partial class PromptGenerator : ISourceGenerator
{
  private static readonly Regex TokenPattern =
    new(@"\{\{\$(?'parameter'[^\}\:]+)\:?(?'type'[^\}]+)?\}\}", RegexOptions.Multiline);

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
      // We want to cache the tokens we've processed so we don't duplicate.
      var encountered = new List<string>();

      var parameters = string.Join(
        ", ",
        TokenPattern
          .Matches(prompt.Tmpl)
          .Select(m =>
          {
            var parameter = m.Groups["parameter"].Value.ToString().Trim();
            var type = "string";

            if (encountered.Contains(parameter))
            {
              return "";
            }

            if (m.Groups.ContainsKey("type"))
            {
              type = m.Groups["type"].Value.ToString();

              if (string.IsNullOrWhiteSpace(type))
              {
                type = "string";
              }
            }

            encountered.Add(parameter);

            return $"{type} {parameter}";
          })
          .Where(p => !string.IsNullOrWhiteSpace(p))
          .Distinct()
      );

      var tmpl = TokenPattern.Replace(prompt.Tmpl.Trim(), "{{${parameter}}}");

      var src = $$""""
        using System;
        using Microsoft.SemanticKernel.Connectors.OpenAI;
        using SKPromptGenerator;

        namespace {{prompt.Namespace}};

        /// <summary>
        /// Generated prompt for `{{prompt.Name}}`
        /// </summary>
        public partial class {{prompt.Name}}Prompt(
          {{parameters}}
        ) : {{prompt.BaseClass}}
        {
          /// <summary>
          /// The base prompt template string for `{{prompt.Name}}`
          /// </summary>
          public override string Text => $$"""
        {{tmpl}}
        """;

          /// <summary>
          /// Settings for the prompt `{{prompt.Name}}`:
          ///   MaxTokens = {{prompt.MaxTokens}}
          ///   Temperature = {{prompt.Temperature}}d
          ///   TopP = {{prompt.TopP}}d
          /// </summary>
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
