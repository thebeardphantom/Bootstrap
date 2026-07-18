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
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        void Update(float deltaTime);
    }
}