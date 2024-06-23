# Semantic Kernel (SK) Prompt Generator

This project is a proof-of-concept prompt class generator using C# source generators.

## Motivation

When working with prompts, you'll end up doing a lot of string templating and repetitive code.

Wouldn't it be nice if you could just have a strongly typed class for each prompt automatically created using the prompt?

This library does exactly that.

```csharp
public static class Prompts
{
  // Define a prompt
  [PromptTemplate]
  public const string Capitol = """
    What is the capitol of {state} {country}?
    Respond directly in a single line
    """;
}

// Execute the prompt passing in a Semantic Kernel instance.
var capitol = await new CapitolPrompt("NJ", "USA").ExecuteAsync(kernel);
```

The tokens in the prompt string become named parameters on the class constructor 🎉

## Limitations

1. Your prompt must be a `const string` because the generator needs to be able to read the string in the source.
2. Your prompts must live in some class in a namespace.  If you get the error `error CS1001: Identifier expected`, then you are probably missing a namespace around your prompt.
3. You must add a dependency to `Microsoft.SemanticKernel` since the `ExecuteAsync` method requires the `Kernel` instance.

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

In the project, create a class like so (you can call your class whatever you want):

```csharp
namespace SomeNamespace;

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

> 💡 Note the usage of a namespace for the class.

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

Note the two class parameters `state` and `country` which are extracted from the prompt template.

Now we can use the prompt like so:

```csharp
var capitol = await new CapitolPrompt("NJ", "USA").ExecuteAsync(kernel);

Console.WriteLine($"{capitol}");
// The capitol of New Jersey is: Trenton.

capitol = await new CapitolPrompt("NY", "USA").ExecuteAsync(kernel);

Console.WriteLine($"{capitol}");
// The capitol of New York is: Albany.
```

## Prompt Execution Settings

The `PromptTemplate` attribute also allows specification of the prompt execution settings.

The three parameters are:

|Parameter|Details|Default|
|--|--|--|
|`MaxTokens`|The maximum number of tokens in the response|`500`|
|`Temperature`|The temperature|`0.5`|
|`TopP`|The TopP|`0`|

For example:

```csharp
public static class Prompts
{
  [PromptTemplate(10, 0.1)]
  public const string SampleTmpl1 = """
    What is the capitol of {state} {country}
    Respond directly on a single line.
    """;
}
```

(See the `PromptTmpl` class for details)

## Using the Sample App

To use the sample app, you'll need to set up user secrets:

```shell
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAIKey" "YOUR_AZURE_OPEN_AI_KEY"
dotnet user-secrets set "AzureOpenAIEndpoint" "YOUR_AZURE_OPEN_AI_ENDPOINT"
```

If you are using OpenAI, feel free to fork this project and simply change the service type and configuration values.
