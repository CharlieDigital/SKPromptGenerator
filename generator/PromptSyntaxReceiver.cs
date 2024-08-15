using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SKPromptGenerator;

/// <summary>
/// Captures the fields and stores them as prompt templates.
/// </summary>
public class PromptSyntaxReceiver : ISyntaxContextReceiver
{
  public List<PromptTmpl> Prompts = [];

  public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
  {
    if (context.Node is not FieldDeclarationSyntax fieldDec)
    {
      return; // ! EXIT: Not a field, skip
    }

    if ((fieldDec.Declaration.Type as PredefinedTypeSyntax)?.Keyword.ValueText != "string")
    {
      return; // ! EXIT: Not a string, skip
    }

    if (
      (
        context.SemanticModel.GetDeclaredSymbol(fieldDec.Declaration.Variables.First())
        as IFieldSymbol
      )?.IsConst != true
    )
    {
      return; // ! EXIT: Not a const, skip
    }

    // Find the fields decorated with the attributes.
    var attributeList = fieldDec.AttributeLists.FirstOrDefault();

    var attribute = attributeList?.Attributes.First();

    var attributeName = attribute?.Name.GetFirstToken();

    if (attributeName == null || attributeName.Value.Text != "PromptTemplate")
    {
      return; // ! EXIT: Not a PromptTemplate
    }

    // Find the value declaration.
    foreach (var statement in fieldDec.Declaration.Variables)
    {
      var declared = context.SemanticModel.GetDeclaredSymbol(statement);

      if (declared == null)
      {
        continue;
      }

      var ns = string.IsNullOrWhiteSpace(declared?.ContainingNamespace.Name)
        ? "SkPromptGenerator"
        : declared.ContainingNamespace.ToString();

      var fieldName = declared?.Name;

      var fieldValue = (declared as IFieldSymbol)?.ConstantValue as string;

      var args = attribute
        ?.ArgumentList?.Arguments.Select(a => a.Expression.GetFirstToken().Text)
        .ToArray();

      if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldValue))
      {
        continue;
      }

      var baseClass = "PromptTemplateBase";

      // Check if it's a generic attribute
      if (attribute?.Name is GenericNameSyntax generic)
      {
        baseClass =
          (generic.TypeArgumentList.Arguments[0] as IdentifierNameSyntax)?.Identifier.Text
          ?? "PromptTemplateBase";
      }

      Prompts.Add(new(ns ?? "", fieldName, fieldValue, baseClass, args ?? []));
    }
  }
}
