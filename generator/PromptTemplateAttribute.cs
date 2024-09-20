using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SKPromptGenerator;

/// <summary>
/// Generates the attribute that we used to decorate classes for the prompt template.
/// </summary>
[Generator]
public sealed class PromptTemplateAttributeGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context) =>
    context.RegisterPostInitializationOutput(context =>
      context.AddSource(
        $"{PromptTemplateAttributeSource.Name}.g.cs",
        SourceText.From(PromptTemplateAttributeSource.SourceCode, Encoding.UTF8)
      )
    );
}

/// <summary>
/// Source for the template attribute.
/// </summary>
internal static class PromptTemplateAttributeSource
{
  public const string Namespace = "SKPromptGenerator";
  public const string Name = "PromptTemplateAttribute";
  public const string FullyQualifiedName = $"{Namespace}.{Name}";

  public const string SourceCode = """
    using System;
    using System.Reflection;

    namespace SKPromptGenerator;

    /// <summary>
    /// Attribute applied to `const string` class fields to generate a prompt class.
    /// Use this when specifying a custom base class for executing the prompt.
    /// </summary>
    /// <param name="maxTokens">The maximum number of tokens; default is 500</param>
    /// <param name="temperature">The temperature; default is 0.5</param>
    /// <param name="topP">The Top P parameter; default is 0</param>
    /// <typeparam name="T">The base type for the template inheriting from `PromptTemplateBase`</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class PromptTemplateAttribute<T>(
      int maxTokens = 500,
      double temperature = 0.5,
      double topP = 0
    ) : Attribute where T : PromptTemplateBase {
      public int MaxTokens => maxTokens;
      public double Temperature => temperature;
      public double TopP => topP;
    }

    /// <summary>
    /// Attribute applied to `const string` class fields to generate a prompt class.
    /// </summary>
    /// <param name="maxTokens">The maximum number of tokens; default is 500</param>
    /// <param name="temperature">The temperature; default is 0.5</param>
    /// <param name="topP">The Top P parameter; default is 0</param>
    /// <typeparam name="T">The base type for the template inheriting from `PromptTemplateBase`</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class PromptTemplateAttribute(
      int maxTokens = 500,
      double temperature = 0.5,
      double topP = 0
    ) : PromptTemplateAttribute<PromptTemplateBase>(maxTokens, temperature, topP) {

    }
    """;
}
