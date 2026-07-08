using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public partial class GenerateSingletonAttribute : BootstrapGeneratorAttribute
    {
        public readonly SingletonAccess Access;

        internal override string[] Imports { get; } =
        {
            "UnityEngine",
            "BeardPhantom.Bootstrap",
            "System.Runtime.CompilerServices",
        };

        internal override string FilenameId => "Singleton";

        public GenerateSingletonAttribute() : this(SingletonAccess.Property) { }

        public GenerateSingletonAttribute(SingletonAccess access)
        {
            Access = access;
        }

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            string accessFormatStr = Access == SingletonAccess.Property ? PropertyFormatStr : OutMethodFormatStr;
            featuresStringBuilder.AppendFormat(accessFormatStr, className);
        }
    }
}