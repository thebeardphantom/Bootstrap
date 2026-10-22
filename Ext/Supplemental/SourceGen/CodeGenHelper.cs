using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Reflection;

namespace BeardPhantom.Bootstrap.SourceGen
{
    internal static class CodeGenHelper
    {
        private const string KeywordInterface = "interface";

        private const string KeywordClass = "class";

        private const string KeywordStruct = "struct";

        private const string KeywordRecord = "record";

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

                if (syntaxNode is T typedSyntaxNode)
                {
                    result = typedSyntaxNode;
                    return true;
                }

                return syntaxNode.TryGetParentSyntax(out result);
            }
            catch
            {
                return false;
            }
        }

        public static Type[] GetAttributeTypes()
        {
            return
            [
                .. typeof(BootstrapGeneratorAttribute)
                    .Assembly
                    .GetTypes()
                    .Where(t => !t.IsAbstract && typeof(BootstrapGeneratorAttribute).IsAssignableFrom(t)),
            ];
        }

        public static BootstrapGeneratorAttribute CreateAttributeInstance(AttributeData attributeData)
        {
            var attrClass = attributeData.AttributeClass!;
            var type = Type.GetType(attrClass.ToDisplayString());
            if (type == null)
            {
                throw new Exception($"Unable to find type {attrClass.ToDisplayString()}");
            }

            // Convert constructor arguments (in declared order) to real CLR values.
            var ctorArgs = attributeData.ConstructorArguments
                .Select(TypedConstantToObject)
                .ToArray();

            var instance = (BootstrapGeneratorAttribute)Activator.CreateInstance(type, ctorArgs)!;

            // Apply any named arguments (e.g. [MyAttr(Bar = 5)]) to fields/properties.
            foreach (var named in attributeData.NamedArguments)
            {
                var value = TypedConstantToObject(named.Value);
                var field = type.GetField(named.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(instance, value);
                    continue;
                }

                var prop = type.GetProperty(named.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                prop?.SetValue(instance, value);
            }

            return instance;
        }

        public static string GetKeywordForType(TypeDeclarationSyntax typeDeclarationSyntax)
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

        private static object? TypedConstantToObject(TypedConstant constant)
        {
            if (constant.IsNull)
            {
                return null;
            }

            switch (constant.Kind)
            {
                case TypedConstantKind.Array:
                {
                    var elementType = Type.GetType(((IArrayTypeSymbol)constant.Type!).ElementType.ToDisplayString())
                                      ?? typeof(object);
                    var array = Array.CreateInstance(elementType, constant.Values.Length);
                    for (var i = 0; i < constant.Values.Length; i++)
                    {
                        array.SetValue(TypedConstantToObject(constant.Values[i]), i);
                    }

                    return array;
                }
                case TypedConstantKind.Enum:
                {
                    var enumType = Type.GetType(constant.Type!.ToDisplayString())!;
                    return Enum.ToObject(enumType, constant.Value!);
                }
                case TypedConstantKind.Type:
                {
                    // Handles typeof(X) arguments — resolve to a runtime Type if needed.
                    return Type.GetType(((ITypeSymbol)constant.Value!).ToDisplayString());
                }
                case TypedConstantKind.Primitive:
                {
                    // Primitive: string, int, bool, etc.
                    return constant.Value;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}