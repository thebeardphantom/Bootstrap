using System;
using System.Reflection;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Applied to an assembly to register an <see cref="IAppExtension"/> type for automatic instantiation and
    /// registration during <see cref="App"/> startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AppExtensionTypeAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="IAppExtension"/> implementation to register.
        /// </summary>
        public readonly Type ExtensionType;

        /// <summary>
        /// Creates a new app extension type attribute.
        /// </summary>
        /// <param name="extensionType">
        /// The <see cref="IAppExtension"/> implementation to register. Must have a public parameterless constructor.
        /// </param>
        public AppExtensionTypeAttribute(Type extensionType)
        {
            if (extensionType.IsNull())
            {
                throw new ArgumentNullException(nameof(extensionType));
            }

            if (!typeof(IAppExtension).IsAssignableFrom(extensionType))
            {
                throw new ArgumentException($"{extensionType} must implement {nameof(IAppExtension)}.");
            }

            ConstructorInfo parameterlessCtor = extensionType.GetConstructor(Type.EmptyTypes);

            if (parameterlessCtor.IsNull() || !parameterlessCtor.IsPublic)
            {
                throw new ArgumentException("The extension type must have a public parameterless constructor.");
            }

            ExtensionType = extensionType;
        }
    }
}