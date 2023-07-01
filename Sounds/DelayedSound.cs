using Sandbox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyWeapons.Sounds;

public partial class DelayedSound : PlayableDelayedSound
{
    [Net]
    public string SoundName { get; private set; }

    [Net]
    public float Delay { get; private set; }

    public DelayedSound()
    {
        Game.AssertClient();
        SoundName = null!;
        Delay = 0;
    }

    public DelayedSound(string soundName, float delay = 0)
    {
        SoundName = soundName;
        Delay = delay;
    }

    public override async Task PlayOnEntity(Entity entity, CancellationToken? cancellationToken, float delayRatio = 1, string? attachment = null)
    {
        if(delayRatio < 0)
            throw new ArgumentOutOfRangeException(nameof(delayRatio));

        cancellationToken?.ThrowIfCancellationRequested();

        float delay = Delay * delayRatio;
        if(delay > 0)
            await Task.Delay(TimeSpan.FromSeconds(delay));

        cancellationToken?.ThrowIfCancellationRequested();

        if(entity.IsValid())
        {
            if(attachment == null)
                entity.PlaySound(SoundName);
            else
                entity.PlaySound(SoundName, attachment);
        }
    }
}
