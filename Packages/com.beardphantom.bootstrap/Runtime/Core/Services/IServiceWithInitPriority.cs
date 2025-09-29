namespace BeardPhantom.Bootstrap
{
    public interface IServiceWithInitPriority : IService
    {
        int InitPriority { get; }
    }
}