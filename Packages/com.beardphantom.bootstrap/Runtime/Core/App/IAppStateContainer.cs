namespace BeardPhantom.Bootstrap
{
    public interface IAppStateContainer
    {
        bool IsPlaying { get; internal set; }

        bool IsQuitting { get; internal set; }

        bool IsRunningTests { get; internal set; }

        bool IsResetting { get; internal set; }
    }
}