# Semantic Kernel (SK) Prompt Generator

This project is a proof-of-concept prompt class generator using C# source generators.

It simplifies interacting with Semantic Kernel by automatically generating strongly typed classes for every prompt string that you decorate with an attribute.

## Installing

This generator is built for .NET 8.

To install:

```shell
dotnet add package SKPromptGenerator
```

## Using

This repository includes a sample project under the `/app` directory.

To use, create a new console app:

```shell
mkdir sk-prompt-gen-test
cd sk-prompt-gen-test
dotnet new console
dotnet add package SKPromptGenerator
dotnet add package Microsoft.SemanticKernel
```

In the project, create a class like so:

```csharp
public static class Prompts
{
  [PromptTemplate]
  public const string Capitol = """
    What is the capitol of {state} {country}?
    Respond directly in a single line
    When writing the state, always write it as the full name
    Write your output in the format: The capitol of <STATE> is: <CAPITOL>.
    For example: The capitol of California is: Sacramento.
    """;
}
```

In the code above, we've created a prompt with two tokens: `{state}` and `{country}`.

The `[PromptTemplate]` attribute instructs the generator to create a class like so:

```csharp
using System;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKPromptGenerator;

namespace App;

public partial class CapitolPrompt(
  string state, string country
) : PromptTemplateBase
{
  public override string Text => $"""
What is the capitol of {state} {country}?
Respond directly in a single line
When writing the state, always write it as the full name
Write your output in the format: The capitol of <STATE> is: <CAPITOL>.
For example: The capitol of California is: Sacramento.
""";

  public override OpenAIPromptExecutionSettings Settings => new OpenAIPromptExecutionSettings
  {
    MaxTokens = 500,
    Temperature = 0.5d,
    TopP = 0d,
  };
}
```

Now we can use the prompt like so:

```csharp
var capitol = await new CapitolPrompt("NJ", "USA").ExecuteAsync(kernel);

Console.WriteLine($"{capitol}");
// The capitol of New Jersey is: Trenton.

capitol = await new CapitolPrompt("NY", "USA").ExecuteAsync(kernel);

Console.WriteLine($"{capitol}");
// The capitol of New York is: Albany.
```
