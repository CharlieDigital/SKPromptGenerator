using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SKPromptGenerator;

/// <summary>
/// Generates the base class for executing the prompt.
/// </summary>
[Generator]
public sealed class PromptTemplateBaseGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context) =>
    context.RegisterPostInitializationOutput(context =>
      context.AddSource(
        $"{PromptTemplateBaseSource.Name}.g.cs",
        SourceText.From(PromptTemplateBaseSource.SourceCode, Encoding.UTF8)
      )
    );
}

/// <summary>
/// Source code for the base class.
/// </summary>
internal static class PromptTemplateBaseSource
{
  public const string Namespace = "SKPromptGenerator";
  public const string Name = "PromptTemplateBase";
  public const string FullyQualifiedName = $"{Namespace}.{Name}";

  public const string SourceCode = """
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;

    namespace SKPromptGenerator;

    public abstract class PromptTemplateBase
    {
      public abstract OpenAIPromptExecutionSettings Settings { get; }

      public abstract string Text { get; }

      public async Task<string> ExecuteAsync(
        Kernel kernel,
        string serviceId = null,
        CancellationToken cancellation = default
      )
      {
        var chat = kernel.GetRequiredService<IChatCompletionService>(serviceId);

        var history = new ChatHistory();

        history.AddUserMessage(Text);

        var result = await chat.GetChatMessageContentAsync(history, Settings, kernel, cancellation);

        return result.ToString();
      }
    }
    """;
}
