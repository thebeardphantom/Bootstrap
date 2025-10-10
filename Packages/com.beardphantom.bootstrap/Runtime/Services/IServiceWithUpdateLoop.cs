namespace BeardPhantom.Bootstrap
{
    public interface IServiceWithUpdateLoop : IService
    {
        void Update(float deltaTime);
    }
}