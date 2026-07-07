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
            string accessStr = string.Format(accessFormatStr, className);
            featuresStringBuilder.AppendFormat(Format, className, accessStr);
        }
    }
}