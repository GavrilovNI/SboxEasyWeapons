using Sandbox;
using System;

namespace EasyWeapons.Entities.Components;

public partial class TimedDestroyer : EntityComponent
{
    [Net, Local]
    public TimeSpan TimeToDestroy { get; set; } = TimeSpan.Zero;

    protected override void OnActivate()
    {
        base.OnActivate();
        Event.Register(this);
    }


    protected override void OnDeactivate()
    {
        Event.Unregister(this);
        base.OnDeactivate();
    }

    [GameEvent.Tick]
    protected virtual void Tick()
    {
        TimeToDestroy = TimeToDestroy.Subtract(TimeSpan.FromSeconds(Time.Delta));

        if(TimeToDestroy.TotalSeconds <= 0)
            DeleteEntity();
    }

    protected virtual void DeleteEntity()
    {
        Entity.Delete();
    }
}
