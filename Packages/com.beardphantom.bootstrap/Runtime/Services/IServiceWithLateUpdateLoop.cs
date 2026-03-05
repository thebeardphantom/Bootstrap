namespace BeardPhantom.Bootstrap
{
    public interface IServiceWithLateUpdateLoop : IService
    {
        void LateUpdate(float deltaTime);
    }
}