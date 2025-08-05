using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class GenerateLoggerAttribute : BootstrapGeneratorAttribute
    {
        internal override void Generate(StringBuilder importsStringBuilder, StringBuilder featuresStringBuilder, string className)
        {
            importsStringBuilder.AppendLine("using BeardPhantom.Bootstrap.ZLogger;");
            featuresStringBuilder.AppendLine(
                $"\t\tprivate static readonly Microsoft.Extensions.Logging.ILogger s_logger = LogUtility.GetStaticLogger(nameof({className}));");
        }
    }
}