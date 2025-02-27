using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IBootstrapService
    {
        void InitService(BootstrapContext context);
    }
}