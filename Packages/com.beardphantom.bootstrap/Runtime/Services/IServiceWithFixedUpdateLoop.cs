namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to receive a callback aligned with Unity's FixedUpdate loop.
    /// </summary>
    public interface IServiceWithFixedUpdateLoop : IService
    {
        /// <summary>
        /// Called on Unity's fixed update loop.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed timestep duration, in seconds.</param>
        void FixedUpdate(float fixedDeltaTime);
    }
}