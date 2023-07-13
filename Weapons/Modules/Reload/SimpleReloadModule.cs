using EasyWeapons.Effects;
using EasyWeapons.Extensions;
using EasyWeapons.Inventories;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EasyWeapons.Weapons.Modules.Reload;

public partial class SimpleReloadModule : ReloadModule
{
    [Net]
    public AmmoInventory WeaponClip { get; set; }

    public IAmmoInventory? AmmoInventory
    {
        get
        {
            if(Weapon.Owner is null)
                return null;

            return Weapon.Owner.GetOrCreateAmmoInventoryComponent()?.AmmoInventory;
        }
    }

    [Net]
    public string AmmoId { get; set; }

    [Net]
    public string ReloadAction { get; set; }

    [Net]
    public IList<WeaponEffect> ReloadEffects { get; set; } = null!;

    [Net]
    public IList<WeaponEffect> ReloadFailEffects { get; set; } = null!;

    [Net]
    private OneTypeAmmoInventory? ReloadingSet { get; set; }

    private CancellationTokenSource? ReloadEffectCancellationTokenSource { get; set; }


    public SimpleReloadModule()
    {
        Game.AssertClient();
        WeaponClip = null!;
        AmmoId = null!;
        ReloadAction = null!;
    }

    public SimpleReloadModule(OneTypeAmmoInventory weaponClip, string reloadAction = "reload") :
        this(weaponClip, weaponClip.AmmoId, reloadAction)
    {
    }

    public SimpleReloadModule(AmmoInventory weaponClip, string ammoTypeId, string reloadAction = "reload")
    {
        WeaponClip = weaponClip;
        AmmoId = ammoTypeId;
        ReloadAction = reloadAction;
    }

    protected override void OnDeactivate()
    {
        if(IsReloading)
            Cancel();

        base.OnDeactivate();
    }

    public override SimulationResult Simulate(SimulationType simulationType)
    {
        if(simulationType == SimulationType.Simulating && Input.Pressed(ReloadAction))
        {
            if(CanStartReload())
                Reload();
            else if(WeaponClip.GetMaxAmountCanAdd(AmmoId) > 0)
                ReloadFailEffects.Play(Weapon);
        }

        if(IsReloading)
        {
            HandleReload();
            return SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    public override bool CanStartReload()
    {
        if(base.CanStartReload() == false)
            return false;

        var ammoInventory = AmmoInventory;
        if(ammoInventory is null)
            return false;

        var maxAmmoNeeded = WeaponClip.GetMaxAmountCanAdd(AmmoId);
        if(maxAmmoNeeded == 0)
            return false;

        return ammoInventory.Contains(AmmoId);
    }

    public override void Reload()
    {
        base.Reload();
        var maxAmmoNeeded = WeaponClip.GetMaxAmountCanAdd(AmmoId);
        ReloadingSet = AmmoInventory!.TakeSome(AmmoId, maxAmmoNeeded);

        ReloadEffectCancellationTokenSource = new CancellationTokenSource();
        ReloadEffects.Play(Weapon, ReloadEffectCancellationTokenSource.Token);
    }

    public override void Cancel()
    {
        base.Cancel();

        ReloadEffectCancellationTokenSource?.Cancel();
        ReloadEffectCancellationTokenSource = null;

        ReturnReloadingAmmo();
        if(ReloadingSet!.AmmoCount > 0)
            DropReloadingAmmo();

        ReloadingSet = null;
    }

    protected virtual void ReturnReloadingAmmo()
    {
        if(AmmoInventory is not null)
            ReloadingSet = AmmoInventory.AddAsMaxAsCan(ReloadingSet!);
    }

    protected virtual void DropReloadingAmmo()
    {
        //TODO: drop ammo
    }

    protected override void OnFinishedReload()
    {
        ReloadingSet = WeaponClip.AddAsMaxAsCan(ReloadingSet!);
        ReturnReloadingAmmo();
        if(ReloadingSet.AmmoCount > 0)
            DropReloadingAmmo();

        ReloadEffectCancellationTokenSource = null;
    }

    protected override bool IsValidToContinueReload()
    {
        return AmmoInventory is not null;
    }
}