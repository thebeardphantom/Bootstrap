using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    /// <summary>
    /// Marks a partial type to have a static logger field generated for it.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class GenerateLoggerAttribute : BootstrapGeneratorAttribute
    {
        internal override string[] Imports { get; } =
        {
            "BeardPhantom.Bootstrap.ZLogger",
        };

        internal override string FilenameId => "Logger";

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            var line =
                $"\t\tprivate static readonly Microsoft.Extensions.Logging.ILogger s_logger = LogUtility.GetStaticLogger(nameof({className}));";
            featuresStringBuilder.AppendLine(line);
        }
    }
}