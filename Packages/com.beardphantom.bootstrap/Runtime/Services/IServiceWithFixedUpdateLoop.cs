namespace BeardPhantom.Bootstrap
{
    public interface IServiceWithFixedUpdateLoop : IService
    {
        void FixedUpdate(float fixedDeltaTime);
    }
}