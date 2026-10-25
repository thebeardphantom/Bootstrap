using Microsoft.CodeAnalysis.CSharp;
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
        private readonly string _categoryPrefix;

        private readonly string _categoryOverride;

        internal override string[] Imports { get; } =
        {
            "BeardPhantom.Bootstrap.ZLogger",
        };

        internal override string FilenameId => "Logger";

        /// <summary>
        /// Creates a static ILogger field used for logging.
        /// </summary>
        /// <param name="categoryPrefix">A prefix prepended to the category.</param>
        /// <param name="categoryOverride">A category to override the default (the typename that the attribute is applied to).</param>
        public GenerateLoggerAttribute(string categoryPrefix = null, string categoryOverride = null)
        {
            _categoryPrefix = categoryPrefix;
            _categoryOverride = categoryOverride;
        }

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            var category = GetEffectiveCategory(className);
            var categoryLiteral = SymbolDisplay.FormatLiteral(category, true);
            featuresStringBuilder.AppendLine(
                $"\t\tprivate static readonly Microsoft.Extensions.Logging.ILogger s_logger = LogUtility.GetStaticLogger({categoryLiteral});");
        }

        private string GetEffectiveCategory(string className)
        {
            var effectiveCategory = _categoryOverride ?? className;
            if (_categoryPrefix != null)
            {
                effectiveCategory = _categoryPrefix + effectiveCategory;
            }

            return effectiveCategory;
        }
    }
}