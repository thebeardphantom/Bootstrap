using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if DEBUG
using System.Diagnostics;
#endif

namespace BeardPhantom.Bootstrap.SourceGen
{
    /// <summary>
    /// Roslyn source generator that emits partial-class members for types decorated with a
    /// <see cref="BootstrapGeneratorAttribute"/>-derived attribute (e.g. <see cref="GenerateLoggerAttribute"/>,
    /// <see cref="GenerateSingletonAttribute"/>).
    /// </summary>
    [Generator]
    public class BootstrapSourceGenerator : ISourceGenerator
    {
        private const string Format =
            """
            {0}

            namespace {1}
            {{
                public partial {2} {3}
                {{
            {4}
                }}
            }}
            """;

        private const string FormatNoNamespace =
            """
            {0}

            public partial {1} {2}
            {{
            {3}
            }}
            """;

        private const string KeywordInterface = "interface";

        private const string KeywordClass = "class";

        private const string KeywordStruct = "struct";

        private const string KeywordRecord = "record";

        private static void GenerateExtensionClass(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax typeDeclarationSyntax,
            IReadOnlyCollection<BootstrapGeneratorAttribute> generatorAttributes)
        {
            if (generatorAttributes.Count == 0)
            {
                return;
            }

            StringBuilder featuresStringBuilder = new();

            string typeName = typeDeclarationSyntax.Identifier.Text;
            Console.WriteLine($"Generating extension classes for type {typeName}.");
            foreach (BootstrapGeneratorAttribute generatorAttribute in generatorAttributes)
            {
                Console.WriteLine($"Found attribute {generatorAttribute}.");
                featuresStringBuilder.Clear();
                generatorAttribute.Generate(featuresStringBuilder, typeName);

                string imports = string.Join(Environment.NewLine, generatorAttribute.Imports.Select(import => $"using {import};"));

                string featuresString = featuresStringBuilder.ToString().TrimEnd();
                string typeKeyword = GetKeywordForType(typeDeclarationSyntax);
                string contents = typeDeclarationSyntax.TryGetParentSyntax(out NamespaceDeclarationSyntax? namespaceDeclarationSyntax)
                    ? string.Format(Format, imports, namespaceDeclarationSyntax!.Name, typeKeyword, typeName, featuresString)
                    : string.Format(FormatNoNamespace, imports, typeKeyword, typeName, featuresString);

                var hintName = $"{typeDeclarationSyntax.Identifier}.g.{generatorAttribute.FilenameId}.cs";
                Console.WriteLine($"Generating class file '{hintName}'.");
                SourceText sourceText = SourceText.From(contents, Encoding.UTF8);
                context.AddSource(hintName, sourceText);
            }
        }

        private static void GenerateSource(GeneratorExecutionContext context)
        {
            foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(syntaxTree);

                IEnumerable<TypeDeclarationSyntax> classNodes = syntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<TypeDeclarationSyntax>();

                foreach (TypeDeclarationSyntax typeDeclarationSyntax in classNodes)
                {
                    if (model.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol symbol)
                    {
                        continue;
                    }

                    BootstrapGeneratorAttribute[] attributes = CodeGenHelper.GetAttributeInstances(symbol);
                    GenerateExtensionClass(context, typeDeclarationSyntax, attributes);
                }
            }
        }

        private static string GetKeywordForType(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax switch
            {
                InterfaceDeclarationSyntax => KeywordInterface,
                ClassDeclarationSyntax => KeywordClass,
                StructDeclarationSyntax => KeywordStruct,
                RecordDeclarationSyntax => KeywordRecord,
                _ => throw new ArgumentException($"Unknown type {typeDeclarationSyntax}."),
            };
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            Debugger.Launch();
#endif
            TextWriter consoleOut = Console.Out;
            TextWriter consoleErr = Console.Error;
            string? logFilePath = null;
            try
            {
                string tempPath = Path.GetTempPath();

                FileStream? fileStream = null;
                StreamWriter? streamWriter = null;
                for (var i = 0; i < 32; i++)
                {
                    string potentialLogFilePath = Path.Combine(tempPath, $"BootstrapSourceGenerator_{i}.log");
                    try
                    {
                        fileStream = File.Open(potentialLogFilePath, FileMode.Create, FileAccess.Write);
                        streamWriter = new StreamWriter(fileStream);
                        Console.SetOut(streamWriter);
                        Console.SetError(streamWriter);
                        logFilePath = potentialLogFilePath;
                        break;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        fileStream?.Dispose();
                        streamWriter?.Dispose();
                    }
                    catch (IOException)
                    {
                        fileStream?.Dispose();
                        streamWriter?.Dispose();
                    }
                }

                using (fileStream)
                {
                    using (streamWriter)
                    {
                        try
                        {
                            GenerateSource(context);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine(ex);
                        }
                    }
                }
            }
            finally
            {
                Console.SetOut(consoleOut);
                Console.SetError(consoleErr);
                if (logFilePath != null)
                {
                    var info = new FileInfo(logFilePath);
                    if (info.Length == 0)
                    {
                        info.Delete();
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context) { }
    }
}