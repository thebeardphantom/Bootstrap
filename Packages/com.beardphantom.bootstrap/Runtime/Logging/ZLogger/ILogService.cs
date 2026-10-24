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
        /// Attempts to get a logger for the given <paramref name="category" />.
        /// </summary>
        bool TryGetLogger(string category, out ILogger logger);

        /// <summary>
        /// Sets a given log group to be enabled or disabled.
        /// </summary>
        void SetGroupEnabled(string group, bool enabled);

        /// <summary>
        /// Returns true if the given logger group is enabled, otherwise returns false.
        /// </summary>
        bool IsGroupEnabled(string group);
    }
}
#endif