using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.SemanticKernel;
using SKPromptGenerator;

namespace tests;

public class BasicTests
{
  [Fact]
  public void Debugger_Test()
  {
    var source = """"
      namespace test;

      public static class Prompts
      {
        [PromptTemplate(1000, 0.7)]
        public const string Template1 = """
          What is the capitol of {state} {country}
          Respond directly on a single line.
          """;
      }
      """";

    var generator = new PromptGenerator();

    var compilation = CSharpCompilation
      .Create("CSharpCodeGen.GenerateAssembly")
      .AddSyntaxTrees(CSharpSyntaxTree.ParseText(source))
      .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
      .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    var driver = CSharpGeneratorDriver
      .Create(generator)
      .RunGeneratorsAndUpdateCompilation(compilation, out _, out var _);

    // Verify the generated code
    driver.GetRunResult();
  }

  [Fact]
  public void Debugger_Test_With_Generic()
  {
    var source = """"
      namespace test;

      public static class Prompts
      {
        [PromptTemplate<CustomBase>(1000, 0.7)]
        public const string Template1 = """
          What is the capitol of {state} {country}
          Respond directly on a single line.
          """;
      }

      public abstract class CustomBase : PromptTemplateBase
      {
        public override async Task<string> ExecuteAsync(
          Kernel kernel,
          string? serviceId = null,
          CancellationToken cancellation = default
        )
        {
          return await Task.FromResult("response");
        }
      }
      """";

    var generator = new PromptGenerator();

    var compilation = CSharpCompilation
      .Create("CSharpCodeGen.GenerateAssembly")
      .AddSyntaxTrees(CSharpSyntaxTree.ParseText(source))
      .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
      .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    var driver = CSharpGeneratorDriver
      .Create(generator)
      .RunGeneratorsAndUpdateCompilation(compilation, out _, out var _);

    // Verify the generated code
    driver.GetRunResult();
  }

  [Fact]
  public void SampleTmpl1_Test()
  {
    var prompt = new SampleTmpl1Prompt("NJ", "USA");

    Assert.Equal(
      """
      What is the capitol of NJ USA
      Respond directly on a single line.
      """,
      prompt.Text
    );
  }

  [Fact]
  public async void SampleTmpl2_With_Generic_Type_Test()
  {
    var prompt = new SampleTmpl2Prompt("NJ", "USA");

    Assert.Equal(
      """
      What is the capitol of NJ USA
      Respond directly on a single line.
      """,
      prompt.Text
    );

    var response = await prompt.ExecuteAsync(new Kernel());

    Assert.Equal("response", response);
  }
}

public static class Prompts
{
  [PromptTemplate(1000, 0.7)]
  public const string SampleTmpl1 = """
    What is the capitol of {state} {country}
    Respond directly on a single line.
    """;

  [PromptTemplate<CustomBase>(1000, 0.7)]
  public const string SampleTmpl2 = """
    What is the capitol of {state} {country}
    Respond directly on a single line.
    """;
}

public abstract class CustomBase : PromptTemplateBase
{
  public override async Task<string> ExecuteAsync(
    Kernel kernel,
    string? serviceId = null,
    CancellationToken cancellation = default
  )
  {
    return await Task.FromResult("response");
  }
}
