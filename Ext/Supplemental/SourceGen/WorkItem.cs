using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BeardPhantom.Bootstrap.SourceGen
{
    internal sealed class WorkItem : IEquatable<WorkItem>
    {
        private const string Format =
            """
            {0}

            namespace {1}
            {{
                partial {2} {3}
                {{
            {4}
                }}
            }}
            """;

        private const string FormatNoNamespace =
            """
            {0}

            partial {1} {2}
            {{
            {3}
            }}
            """;

        public readonly string? Contents;

        public readonly string? HintName;

        public readonly string? ErrorMessage;

        private WorkItem(string? contents, string? hintName, string? errorMessage = null)
        {
            Contents = contents;
            HintName = hintName;
            ErrorMessage = errorMessage;
        }

        public static WorkItem? Create(GeneratorAttributeSyntaxContext ctx)
        {
            if (ctx.TargetSymbol is not INamedTypeSymbol namedTypeSymbol
                || ctx.TargetNode is not TypeDeclarationSyntax typeDeclarationSyntax)
            {
                return null;
            }

            var symbolFullName = Regex.Replace(namedTypeSymbol.ToDisplayString(), "[<>]", "");
            if (namedTypeSymbol.ContainingType != null)
            {
                var errorMessage = $"'{symbolFullName}' is a nested type, which are not currently supported.";
                return new WorkItem(null, null, errorMessage);
            }

            try
            {
                var typeKeyword = CodeGenHelper.GetKeywordForType(typeDeclarationSyntax);
                var typeNamespace = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                    ? null
                    : namedTypeSymbol.ContainingNamespace.ToDisplayString();
                var attributeInstance = CodeGenHelper.CreateAttributeInstance(ctx.Attributes[0]);
                var hintName = $"{symbolFullName}.g.{attributeInstance.FilenameId}.cs";

                StringBuilder featuresStringBuilder = new();
                attributeInstance.Generate(featuresStringBuilder, namedTypeSymbol.Name);

                var attributeFeatures = featuresStringBuilder.ToString().TrimEnd();
                var attributeImports = string.Join(
                    Environment.NewLine,
                    attributeInstance.Imports.Select(import => $"using {import};"));
                var contents = typeNamespace == null
                    ? string.Format(
                        FormatNoNamespace,
                        attributeImports,
                        typeKeyword,
                        namedTypeSymbol.Name,
                        attributeFeatures)
                    : string.Format(
                        Format,
                        attributeImports,
                        typeNamespace,
                        typeKeyword,
                        namedTypeSymbol.Name,
                        attributeFeatures);

                return new WorkItem(contents, hintName);
            }
            catch (Exception e)
            {
                var attributeName = ctx.Attributes[0].AttributeClass?.ToDisplayString();
                var errorMessage = $"Exception thrown while generating attribute '{attributeName}'"
                                   + $" for type '{symbolFullName}':\n{e}";
                return new WorkItem(null, null, errorMessage);
            }
        }

        public bool Equals(WorkItem? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Contents == other.Contents
                   && HintName == other.HintName
                   && ErrorMessage == other.ErrorMessage;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Contents != null ? Contents.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (HintName != null ? HintName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ErrorMessage != null ? ErrorMessage.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals((WorkItem)obj);
        }

        public static bool operator ==(WorkItem? left, WorkItem? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WorkItem? left, WorkItem? right)
        {
            return !Equals(left, right);
        }
    }
}