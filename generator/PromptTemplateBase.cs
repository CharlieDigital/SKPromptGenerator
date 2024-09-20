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
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;

    namespace SKPromptGenerator;

    /// <summary>
    /// Abstract base class for executing the prompt.  Override this class to
    /// provide custom execution of the prompt.
    /// </summary>
    public abstract class PromptTemplateBase
    {
      protected static readonly JsonSerializerOptions SerializerOptions = new() {
        PropertyNameCaseInsensitive = true
      };

      /// <summary>
      /// The execution settings for this prompt.
      /// </summary>
      public abstract OpenAIPromptExecutionSettings Settings { get; }

      /// <summary>
      /// The text of this prompt.
      /// </summary>
      public abstract string Text { get; }

      /// <summary>
      /// Executes the prompt using the default execution.  Override this method
      /// to provide custom execution logic (e.g. logging, telemetry, etc.)
      /// </summary>
      /// <param name="kernel">The Semantic Kernel instance.</param>
      /// <param name="serviceId">An optional service ID to specify for execution.</param>
      /// <param name="cancellation">An optional cancellation token.</param>
      /// <returns>A string with the results of execution.</returns>
      public virtual async Task<string> ExecuteAsync(
        Kernel kernel,
        #nullable enable
        string? serviceId = null,
        #nullable disable
        CancellationToken cancellation = default
      )
      {
        var chat = kernel.GetRequiredService<IChatCompletionService>(serviceId);

        var history = new ChatHistory();

        history.AddUserMessage(Text);

        var result = await chat.GetChatMessageContentAsync(history, Settings, kernel, cancellation);

        return result.ToString();
      }

      /// <summary>
      /// Executes the prompt and expects a JSON response that will be deserialized
      /// to the type `T`.
      /// </summary>
      /// <param name="kernel">The Semantic Kernel instance.</param>
      /// <param name="serviceId">An optional service ID to specify for execution.</param>
      /// <param name="cancellation">An optional cancellation token.</param>
      /// <typeparam name="T">The type `T` of the response object.</typeparam>
      /// <returns>An instance of type `T` deserialized from the JSON response.</returns>
      #nullable enable
      public virtual async Task<T?> ExecuteAsync<T>(
        Kernel kernel,
        string? serviceId = null,
        CancellationToken cancellation = default
      ) {
        var (result, _) = await ExecuteWithJsonAsync<T>(kernel, serviceId, cancellation);

        return result;
      }
      #nullable disable

      /// <summary>
      /// Executes the prompt and expects a JSON response that will be deserialized
      /// to the type `T`.  This call includes the JSON result as part of the tuple.
      /// This method call will perform trimming of JSON fences if present using
      /// regular string find/replace.
      /// </summary>
      /// <param name="kernel">The Semantic Kernel instance.</param>
      /// <param name="serviceId">An optional service ID to specify for execution.</param>
      /// <param name="cancellation">An optional cancellation token.</param>
      /// <typeparam name="T">The type `T` of the response object.</typeparam>
      /// <returns>An instance of type `T` deserialized from the JSON response in a tuple with the full JSON response as well..</returns>
      #nullable enable
      public virtual async Task<(T? Result, string Json)> ExecuteWithJsonAsync<T>(
        Kernel kernel,
        string? serviceId = null,
        CancellationToken cancellation = default
      ) {
        var json = await ExecuteAsync(kernel, serviceId, cancellation);

        json = json.Trim().Replace("```json", "").Replace("```", "");

        return (JsonSerializer.Deserialize<T>(json, SerializerOptions), json);
      }
      #nullable disable
    }
    """;
}
