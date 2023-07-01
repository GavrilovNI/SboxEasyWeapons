using Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace EasyWeapons.Sounds;

public abstract class PlayableDelayedSound : BaseNetworkable
{
    public virtual async Task PlayOnEntity(Entity entity, CancellationToken? cancellationToken = null, float delayRatio = 1, string? attachment = null)
    {
        return;
    }
}