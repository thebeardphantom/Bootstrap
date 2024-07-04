using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPostBootstrapHandler
    {
        UniTask OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper);
    }
}