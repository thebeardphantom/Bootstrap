#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap.SourceGen;
using Microsoft.Extensions.Logging;

namespace BeardPhantom.Bootstrap.ZLogger
{
    [GenerateSingleton(SingletonAccessors.OutMethod)]
    public partial interface ILogService
    {
        bool TryGetLogger(string category, out ILogger logger);
    }
}
#endif