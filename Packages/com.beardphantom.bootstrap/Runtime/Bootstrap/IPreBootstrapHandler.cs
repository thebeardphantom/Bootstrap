using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPreBootstrapHandler
    {
        UniTask OnPreBootstrapAsync(in BootstrapContext context);
    }
}