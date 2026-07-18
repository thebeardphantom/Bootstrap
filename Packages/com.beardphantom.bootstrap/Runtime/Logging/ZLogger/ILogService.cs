#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap.SourceGen;
using Microsoft.Extensions.Logging;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Provides access to <see cref="ILogger"/> instances by category.
    /// </summary>
    [GenerateSingleton(SingletonAccessors.OutMethod)]
    public partial interface ILogService
    {
        /// <summary>
        /// Attempts to get a logger for the given <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The logger category name.</param>
        /// <param name="logger">The resulting logger, if one could be obtained.</param>
        /// <returns>True if a logger was obtained.</returns>
        bool TryGetLogger(string category, out ILogger logger);
    }
}
#endif