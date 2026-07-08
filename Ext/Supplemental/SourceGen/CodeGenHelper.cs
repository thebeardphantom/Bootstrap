using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BeardPhantom.Bootstrap.SourceGen
{
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

        public static bool IsType<T>(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.Name == typeof(T).Name || namedTypeSymbol.ToDisplayString() == typeof(T).FullName;
        }

        public static BootstrapGeneratorAttribute[] FindAttributes(INamedTypeSymbol symbol)
        {
            return symbol
                .GetAttributes()
                .Where(attribute => attribute.AttributeClass is { BaseType: not null, } attrClass
                                    && attrClass.BaseType.IsType<BootstrapGeneratorAttribute>())
                .Select(CreateAttributeInstance)
                .ToArray();
        }

        private static BootstrapGeneratorAttribute CreateAttributeInstance(AttributeData attributeData)
        {
            INamedTypeSymbol attrClass = attributeData.AttributeClass!;
            var type = Type.GetType(attrClass.ToDisplayString());
            if (type == null)
            {
                throw new Exception($"Unable to find type {attrClass.ToDisplayString()}");
            }

            // Convert constructor arguments (in declared order) to real CLR values.
            object?[] ctorArgs = attributeData.ConstructorArguments
                .Select(TypedConstantToObject)
                .ToArray();

            var instance = (BootstrapGeneratorAttribute)Activator.CreateInstance(type, ctorArgs)!;

            // Apply any named arguments (e.g. [MyAttr(Bar = 5)]) to fields/properties.
            foreach (KeyValuePair<string, TypedConstant> named in attributeData.NamedArguments)
            {
                object? value = TypedConstantToObject(named.Value);
                FieldInfo? field = type.GetField(named.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(instance, value);
                    continue;
                }

                PropertyInfo? prop = type.GetProperty(named.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                prop?.SetValue(instance, value);
            }

            return instance;
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
                    Type elementType = Type.GetType(((IArrayTypeSymbol)constant.Type!).ElementType.ToDisplayString())
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