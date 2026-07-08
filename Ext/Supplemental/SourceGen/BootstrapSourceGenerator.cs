using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
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

        private static void GenerateExtensionClass(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax clss,
            IReadOnlyCollection<BootstrapGeneratorAttribute> generatorAttributes)
        {
            if (generatorAttributes.Count == 0)
            {
                return;
            }

            StringBuilder featuresStringBuilder = new();

            string className = clss.Identifier.Text;
            foreach (BootstrapGeneratorAttribute generatorAttribute in generatorAttributes)
            {
                featuresStringBuilder.Clear();
                generatorAttribute.Generate(featuresStringBuilder, className);

                string imports = string.Join(Environment.NewLine, generatorAttribute.Imports.Select(import => $"using {import};"));

                string featuresString = featuresStringBuilder.ToString().TrimEnd();
                string contents = clss.TryGetParentSyntax(out NamespaceDeclarationSyntax? namespaceDeclarationSyntax)
                    ? string.Format(Format, imports, namespaceDeclarationSyntax!.Name, className, featuresString)
                    : string.Format(FormatNoNamespace, imports, className, featuresString);

                var hintName = $"{clss.Identifier}.g.{generatorAttribute.FilenameId}.cs";
                SourceText sourceText = SourceText.From(contents, Encoding.UTF8);
                context.AddSource(hintName, sourceText);
            }
        }

        private static void GenerateSource(GeneratorExecutionContext context)
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

                    BootstrapGeneratorAttribute[] attributes = CodeGenHelper.FindAttributes(symbol);
                    GenerateExtensionClass(context, clss, attributes);
                }
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                GenerateSource(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                string logFile = Path.Combine(Path.GetTempPath(), "BootstrapSourceGenerator.log");
                File.WriteAllText(logFile, ex.ToString());
                throw;
            }
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}