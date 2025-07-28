using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen;

[Generator]
public class BootstrapSourceGenerator : ISourceGenerator
{
    private const string Format =
        """
        {0}

        namespace {1}
        {{
            public partial class {2}
            {{
        {3}
            }}
        }}
        """;

    private const string FormatNoNamespace =
        """
        {0}

        public partial class {1}
        {{
        {2}
        }}
        """;

    private static readonly StringBuilder s_importsStringBuilder = new();

    private static readonly StringBuilder s_featuresStringBuilder = new();

    private static BootstrapGeneratorAttribute[] FindAttributes(INamedTypeSymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Select(attribute => attribute.AttributeClass)
            .OfType<INamedTypeSymbol>()
            .Where(attrClass => attrClass is { BaseType: not null, } && attrClass.BaseType.IsType<BootstrapGeneratorAttribute>())
            .Select(attrClas =>
            {
                var type = Type.GetType(attrClas.ToDisplayString());
                if (type == null)
                {
                    throw new Exception($"Unable to find type {attrClas.ToDisplayString()}");
                }

                return type;
            })
            .Select(Activator.CreateInstance)
            .Cast<BootstrapGeneratorAttribute>()
            .ToArray();
    }

    private static void GenerateExtensionClass(
        GeneratorExecutionContext context,
        ClassDeclarationSyntax clss,
        IReadOnlyCollection<BootstrapGeneratorAttribute> generatorAttributes)
    {
        if (generatorAttributes.Count == 0)
        {
            return;
        }

        s_featuresStringBuilder.Clear();
        s_importsStringBuilder.Clear();
        string className = clss.Identifier.Text;

        foreach (BootstrapGeneratorAttribute generatorAttribute in generatorAttributes)
        {
            generatorAttribute.Generate(s_importsStringBuilder, s_featuresStringBuilder, className);
        }

        string featuresString = s_featuresStringBuilder.ToString().TrimEnd();
        string contents = clss.TryGetParentSyntax(out NamespaceDeclarationSyntax? namespaceDeclarationSyntax)
            ? string.Format(Format, s_importsStringBuilder, namespaceDeclarationSyntax!.Name, className, featuresString)
            : string.Format(FormatNoNamespace, s_importsStringBuilder, className, featuresString);

        context.AddSource(
            $"{clss.Identifier}.g.cs",
            SourceText.From(contents, Encoding.UTF8));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(syntaxTree);

            IEnumerable<ClassDeclarationSyntax> classNodes = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            foreach (ClassDeclarationSyntax? clss in classNodes)
            {
                if (model.GetDeclaredSymbol(clss) is not INamedTypeSymbol symbol)
                {
                    continue;
                }

                BootstrapGeneratorAttribute[] attributes = FindAttributes(symbol);
                GenerateExtensionClass(context, clss, attributes);
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }

    [Flags]
    private enum GenerationFlags
    {
        GenerateSingleton,
        GenerateLogger,
    }
}