namespace BeardPhantom.Bootstrap
{
    public class EditorAppStateContainer : IAppStateContainer
    {
        private volatile bool _isPlaying;

        private volatile bool _isQuitting;

        private volatile bool _isRunningTests;

        private volatile bool _isResetting;

        bool IAppStateContainer.IsPlaying
        {
            get => _isPlaying;
            set => _isPlaying = value;
        }

        bool IAppStateContainer.IsQuitting
        {
            get => _isQuitting;
            set => _isQuitting = value;
        }

        bool IAppStateContainer.IsRunningTests
        {
            get => _isRunningTests;
            set => _isRunningTests = value;
        }

        bool IAppStateContainer.IsResetting
        {
            get => _isResetting;
            set => _isResetting = value;
        }
    }
}