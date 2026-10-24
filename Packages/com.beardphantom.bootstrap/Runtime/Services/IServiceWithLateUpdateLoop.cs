namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to receive a callback aligned with Unity's LateUpdate loop.
    /// </summary>
    public interface IServiceWithLateUpdateLoop : IService
    {
        /// <summary>
        /// Called on Unity's late update loop.
        /// </summary>
        void LateUpdate(float deltaTime);
    }
}