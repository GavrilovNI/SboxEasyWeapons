using EasyWeapons.Entities.Components;
using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Reload;

public partial class ReloadModule : WeaponModule, ICancelableComponent
{
    [Net]
    public bool IsReloading { get; protected set; }

    [Net]
    public float ReloadTime { get; set; } = 1f;

    [Net]
    public bool ShouldCancelOtherModules { get; set; } = false;

    [Net, Predicted]
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

        if(ShouldCancelOtherModules)
            CancelOtherModules();
    }

    public virtual void Cancel()
    {
        if(IsReloading == false)
            throw new InvalidOperationException($"Can't {nameof(Cancel)}. Test {nameof(IsReloading)} first.");

        IsReloading = false;
    }

    public override SimulationResult Simulate(SimulationType simulationType)
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

    protected void CancelOtherModules()
    {
        foreach(var cancelable in Weapon.Components.GetAll<ICancelableComponent>(true))
        {
            if(object.ReferenceEquals(cancelable, this))
                continue;

            cancelable.Cancel();
        }
    }

    protected virtual void HandleReload()
    {
        if(IsValidToContinueReload() == false)
        {
            Cancel();
            return;
        }

        if(TimeSinceReload >= ReloadTime)
        {
            IsReloading = false;
            OnFinishedReload();
        }
    }

    protected virtual void OnFinishedReload()
    {

    }

    protected virtual bool IsValidToContinueReload()
    {
        return true;
    }
}
