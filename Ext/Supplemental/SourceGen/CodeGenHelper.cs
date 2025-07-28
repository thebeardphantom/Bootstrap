using Microsoft.CodeAnalysis;

namespace BeardPhantom.Bootstrap.SourceGen;

internal static class CodeGenHelper
{
    public static bool TryGetParentSyntax<T>(this SyntaxNode? syntaxNode, out T? result) where T : SyntaxNode
    {
        result = null;

        if (syntaxNode == null)
        {
            return false;
        }

        try
        {
            syntaxNode = syntaxNode.Parent;

            if (syntaxNode == null)
            {
                return false;
            }

            if (syntaxNode.GetType() == typeof(T))
            {
                result = syntaxNode as T;
                return true;
            }

            return syntaxNode.TryGetParentSyntax(out result);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsType<T>(this INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.Name == typeof(T).Name || namedTypeSymbol.ToDisplayString() == typeof(T).FullName;
    }
}