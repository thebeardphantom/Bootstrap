using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;
#if DEBUG
using System.Diagnostics;
#endif

namespace BeardPhantom.Bootstrap.SourceGen
{
    /// <summary>
    /// Roslyn source generator that emits partial-class members for types decorated with a
    /// <see cref="BootstrapGeneratorAttribute" />-derived attribute (e.g. <see cref="GenerateLoggerAttribute" />,
    /// <see cref="GenerateSingletonAttribute" />).
    /// </summary>
    [Generator]
    public class BootstrapSourceGenerator : IIncrementalGenerator
    {
        private static readonly DiagnosticDescriptor s_generationFailed = new(
            "BSG001",
            "Source generation failed",
            "{0}",
            "BeardPhantom.Bootstrap.SourceGen",
            DiagnosticSeverity.Error,
            true);

        private static void GenerateForAttribute(IncrementalGeneratorInitializationContext initCtx, Type attributeType)
        {
            if (attributeType.FullName == null)
            {
                return;
            }

            var workItemProvider = initCtx.SyntaxProvider
                .ForAttributeWithMetadataName(
                    attributeType.FullName,
                    static (syntaxNode, _) => syntaxNode is TypeDeclarationSyntax,
                    static (ctx, cancellationToken) =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        return WorkItem.Create(ctx);
                    })
                .Where(static workItem => workItem != null)
                .Select(static (workItem, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return workItem!;
                });

            initCtx.RegisterSourceOutput(
                workItemProvider,
                static (sourceProductionCtx, workItem) =>
                {
                    if (workItem.ErrorMessage != null)
                    {
                        var diagnostic = Diagnostic.Create(s_generationFailed, Location.None, workItem.ErrorMessage);
                        sourceProductionCtx.ReportDiagnostic(diagnostic);
                        return;
                    }

                    var sourceText = SourceText.From(workItem.Contents!, Encoding.UTF8);
                    sourceProductionCtx.AddSource(workItem.HintName!, sourceText);
                });
        }

        public void Initialize(IncrementalGeneratorInitializationContext initCtx)
        {
#if DEBUG
            Debugger.Launch();
#endif

            var attributeTypes = CodeGenHelper.GetAttributeTypes();
            foreach (var attributeType in attributeTypes)
            {
                GenerateForAttribute(initCtx, attributeType);
            }
        }
    }
}