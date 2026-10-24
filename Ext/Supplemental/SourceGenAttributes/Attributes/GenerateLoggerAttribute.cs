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
        private readonly string _group;

        internal override string[] Imports { get; } =
        {
            "BeardPhantom.Bootstrap.ZLogger",
        };

        internal override string FilenameId => "Logger";

        public GenerateLoggerAttribute(string group = null)
        {
            _group = group;
        }

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            featuresStringBuilder.AppendLine(
                string.IsNullOrWhiteSpace(_group)
                    ? "\t\tpublic static readonly string LoggerGroup = null;"
                    : $"\t\tpublic static readonly string LoggerGroup = \"{_group}\";");

            featuresStringBuilder.AppendLine(
                $"\t\tprivate static readonly Microsoft.Extensions.Logging.ILogger s_logger = LogUtility.GetStaticLogger(nameof({className}));");
        }
    }
}