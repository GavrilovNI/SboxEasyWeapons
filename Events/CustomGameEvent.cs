using Sandbox;

namespace EasyWeapons.Events;

public static class CustomGameEvent
{
    public const string PreSimulate = "game.presimulate";
    public const string PostSimulate = "game.postsimulate";

    [MethodArguments(typeof(IClient))]
    public class PreSimulateAttribute : EventAttribute
    {
        public PreSimulateAttribute() : base(PreSimulate) { }
    }

    [MethodArguments(typeof(IClient))]
    public class PostSimulateAttribute : EventAttribute
    {
        public PostSimulateAttribute() : base(PostSimulate) { }
    }
}
