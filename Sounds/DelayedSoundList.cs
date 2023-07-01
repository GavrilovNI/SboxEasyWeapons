using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyWeapons.Sounds;

public partial class DelayedSoundList : PlayableDelayedSound
{
    [Net]
    public float TotalTime { get; private set; } = 0;

    [Net]
    private IList<DelayedSound> Sounds { get; set; } = new List<DelayedSound>();

    public DelayedSoundList()
    {
    }

    public static DelayedSoundList One(DelayedSound sound)
    {
        var list = new DelayedSoundList();
        list.AddFromStart(sound);
        return list;
    }

    public static DelayedSoundList EachAfterAnother(params DelayedSound[] sounds)
    {
        var list = new DelayedSoundList();
        foreach(var sound in sounds)
            list.AddAfterLast(sound);
        return list;
    }

    public static DelayedSoundList AllFromStart(params DelayedSound[] sounds)
    {
        var list = new DelayedSoundList();
        foreach(var sound in sounds)
            list.AddFromStart(sound);
        return list;
    }

    public void AddAfterLast(DelayedSound sound)
    {
        TotalTime += sound.Delay;
        Sounds.Add(new DelayedSound(sound.SoundName, TotalTime));
    }

    public void AddFromStart(DelayedSound sound)
    {
        for(int i = 0; i < Sounds.Count; ++i)
        {
            if(Sounds[i].Delay > sound.Delay)
            {
                Sounds.Insert(i, sound);
                return;
            }
        }

        TotalTime = sound.Delay;
        Sounds.Add(sound);
    }

    public override async Task PlayOnEntity(Entity entity, CancellationToken? cancellationToken, float delayRatio = 1, string? attachment = null)
    {
        if(delayRatio < 0)
            throw new ArgumentOutOfRangeException(nameof(delayRatio));

        cancellationToken?.ThrowIfCancellationRequested();

        float totalDelay = 0;
        foreach(var sound in Sounds)
        {
            float currentDelay = (sound.Delay - totalDelay) * delayRatio;
            totalDelay += currentDelay;

            if(currentDelay > 0)
                await Task.Delay(TimeSpan.FromSeconds(currentDelay));

            cancellationToken?.ThrowIfCancellationRequested();

            if(entity.IsValid())
            {
                if(attachment == null)
                    entity.PlaySound(sound.SoundName);
                else
                    entity.PlaySound(sound.SoundName, attachment);
            }
            else
            {
                return;
            }
        }
    }

}
