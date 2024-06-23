using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
}

public static class Prompts
{
  [PromptTemplate(1000, 0.7)]
  public const string SampleTmpl1 = """
    What is the capitol of {state} {country}
    Respond directly on a single line.
    """;
}
