namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to receive a callback aligned with Unity's Update loop.
    /// </summary>
    public interface IServiceWithUpdateLoop : IService
    {
        /// <summary>
        /// Called on Unity's update loop.
        /// </summary>
        void Update(float deltaTime);
    }
}