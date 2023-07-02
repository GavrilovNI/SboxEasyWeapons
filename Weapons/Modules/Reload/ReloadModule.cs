using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Reload;

public partial class ReloadModule : WeaponModule
{
    [Net]
    public bool IsReloading { get; protected set; }

    [Net]
    public float ReloadTime { get; set; } = 1f;

    [Net]
    public TimeSince TimeSinceReload { get; protected set; }


    public virtual bool CanStartReload()
    {
        return IsReloading == false;
    }

    public virtual void Reload()
    {
        if(CanStartReload() == false)
            throw new InvalidOperationException($"Can't {nameof(Reload)}. Test {nameof(CanStartReload)} first.");

        TimeSinceReload = 0;
        IsReloading = true;
    }

    public virtual void Cancel()
    {
        if(IsReloading == false)
            throw new InvalidOperationException($"Can't {nameof(Cancel)}. Test {nameof(IsReloading)} first.");

        IsReloading = false;
    }

    public override SimulationResult Simulate()
    {
        if(CanStartReload())
            Reload();

        if(IsReloading)
        {
            HandleReload();
            return SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    protected virtual void HandleReload()
    {
        if(TimeSinceReload >= ReloadTime)
        {
            IsReloading = false;
            OnFinishedReload();
        }
    }

    protected virtual void OnFinishedReload()
    {

    }
}
