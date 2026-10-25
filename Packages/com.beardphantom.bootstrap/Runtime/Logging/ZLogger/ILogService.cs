#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap.SourceGen;
using Microsoft.Extensions.Logging;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Provides access to <see cref="ILogger" /> instances by category.
    /// </summary>
    [GenerateSingleton(SingletonAccessors.OutMethod)]
    public partial interface ILogService
    {
        /// <summary>
        /// A list of all logger category exclusions.
        /// </summary>
        CategoryFilterCollection FilterCollection { get; }

        /// <summary>
        /// Attempts to get a logger for the given <paramref name="category" />.
        /// </summary>
        bool TryGetLogger(string category, out ILogger logger);
    }
}
#endif