using Sandbox;

namespace EasyWeapons.Events;

public class CustomGameEvent
{
    public class Client
    {
        public class BuildInput
        {
            public const string Post = "buildinput.post";

            public class PostAttribute : EventAttribute
            {
                public PostAttribute() : base(Post) { }
            }
        }
    }
}
