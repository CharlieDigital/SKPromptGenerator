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
          What is the capitol of {{$state}} {{$country}}
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
      namespace Unit.Test.Namespace;

      public static class Prompts
      {
        [PromptTemplate<CustomBase>(1000, 0.7)]
        public const string Template1 = """
          What is the capitol of {{$state}} {{$country}}
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
  public async Task SampleTmpl2_With_Generic_Type_Test()
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

  [Fact]
  public void SampleTmplJson_Test()
  {
    var prompt = new SampleTmplJsonPrompt("NJ", "USA");

    Assert.Equal(
      """
      What is the capitol of NJ USA
      Respond in JSON like this: { "capitol": "(answer here)" }
      Respond directly on a single line.
      """,
      prompt.Text
    );
  }

  [Fact]
  public void Typed_Parameters_Test()
  {
    var prompt = new CitiesTmplPrompt(4, "NJ", "USA");

    Assert.Equal(
      """
      Write the names of 4 cities in NJ, USA
      Write one on each line.
      """,
      prompt.Text
    );
  }

  [Fact]
  public void Namespace_Test()
  {
    var prompt = new This.Is.A.Namespace.TestNamespacePrompt("NJ", "USA");

    Assert.Equal("This.Is.A.Namespace", prompt.GetType().Namespace);
  }
}

public static class Prompts
{
  [PromptTemplate(1000, 0.7)]
  public const string SampleTmpl1 = """
    What is the capitol of {{$state}} {{$country}}
    Respond directly on a single line.
    """;

  [PromptTemplate<CustomBase>(1000, 0.7)]
  public const string SampleTmpl2 = """
    What is the capitol of {{$state}} {{$country}}
    Respond directly on a single line.
    """;

  [PromptTemplate(1000, 0.7)]
  public const string SampleTmplJson = """
    What is the capitol of {{$state}} {{$country}}
    Respond in JSON like this: { "capitol": "(answer here)" }
    Respond directly on a single line.
    """;

  [PromptTemplate]
  public const string CitiesTmpl = """
    Write the names of {{$count:int}} cities in {{$state}}, {{$country}}
    Write one on each line.
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
